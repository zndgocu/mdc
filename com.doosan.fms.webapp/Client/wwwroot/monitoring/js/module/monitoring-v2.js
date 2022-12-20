

// #region const
const modelScale = 2.5;
const prefabTypes = {
    Cv: 0,
    Sc: 1,
    Rtv: 2,
    Rack: 3,
    Bcr: 4,
    CvLeft: 10,
    CvRight: 20
};
Object.freeze(prefabTypes);

const prefabColors = {
    Normal: 0x00000,
    Gray: 0xa2a2a2,
    Red: 0xff0000,
    Green: 0x07e705,
    Blue: 0x0d0fe7,
    Purple: 0xe70cc9,
    Skyblue: 0x31e7e4,
}
Object.freeze(prefabColors);

const wrkTyps = {
    Normal: 0,
    Sto: 1,
    Ret: 2,
    Move: 3,
}
Object.freeze(wrkTyps);

// #endregion


//#region var
const ORIGIN_MODEL_NAME = 'ORIGIN_MODEL';
//#endergion

// #region html data
let renderBody = undefined;
let razor = undefined;
let drawMode = false;
let uiOverlay = false;
let playbackMode = false;
// #endregion

// #region defaultDatas
let defaultPrefabDataContext = undefined;
let defaultRequestUrlContext = undefined;
let defaultPrefabFbxModelPathContext = undefined;
let prefabDataContext = undefined;
// #endregion

// #region raycast
let rayCaster = undefined;
let rayCasterObjects = [];
let rayCasterObjectsDrawMode = [];
let mousePointer = undefined;
// #endregion

// #region listener Items
let cursorPrefab = undefined;
let cursorKey = "cursor_prefab";
let isCtrlDown, isShiftDown, isAltDown = false;
let isPointerDown = false;
// #endregion

// #region default objects
const planeName = "ray_plane";
let plane = undefined;
let camera, scene, renderer;
let ambientLight, directionalLight;
let gridHelper;
let orbit;
// #endregion

// #region rendering item
let stop = false;
let frameCount = 0;
let fps, fpsInterval;
let startTime;
let now, then, elapsed;
// #endregion

// #region thread worker
let dicMonitoringDatas = {};
let worker;
let dicWorkerMutex = {};
// #endregion

// #region playback datas
let dicPlaybackDefaultDatas = {};
let dicPlaybackHistoryDatas = {};
let playbackRun = false;
// #endregion


export function disposeInterop(aaa) {
    worker.terminate();
    return true;
}
export function changeUIOverayInterop(isUi) {
    uiOverlay = isUi;
    return true;
}

export function changePlaybackModeInterop(isPlayback) {
    playbackMode = isPlayback;
    return true;
}

export async function historyDatasPassInterop(defaultDatas, historyDatas) {
    for (var ite = 0; ite < defaultDatas.length; ite++) {
        var jsonData = JSON.parse(defaultDatas[ite]);
        dicPlaybackDefaultDatas[jsonData.PrefabType] = jsonData.Data;
    }

    for (var ite = 0; ite < historyDatas.length; ite++) {
        var jsonData = JSON.parse(historyDatas[ite]);
        dicPlaybackHistoryDatas[jsonData.PrefabType] = jsonData.Data;
    }
    await defaultDataSet();
    return true;
}

async function defaultDataSet() {
    await clearMonitoringData();
    for (const [key, value] of Object.entries(defaultDatas)) {
        var dataStack = [];
        for (var iteDatas = 0; iteDatas < value.length; iteDatas++) {
            var data = value[iteDatas];
            var defaultT = Date.parse(tm);
            var dataT = Date.parse(data.Dt);
            var timeSpan = dataT - defaultT;
            if (timeSpan > 0 && timeSpan <= (gap)) {
                dataStack.push(value[iteDatas].Data);
            }
        }
        if (dataStack.length > 0) {
            await setMonitoringData(key, dataStack);
        }
    }
    await renderObject(0);
    render();
}


//여기 3개
export function playbackRunInterop(run) {
    playbackRun = run;

    return true;
}

export async function playbackChangeTimerCallback(tm, gap) {
    if (playbackRun == true) {
        await logicWorkerPlayback(tm, gap);
    }
    return true;
}

async function logicWorkerPlayback(tm, gap) {
    await clearMonitoringData();
    for (const [key, value] of Object.entries(dicPlaybackHistoryDatas)) {
        var dataStack = [];
        for (var iteDatas = 0; iteDatas < value.length; iteDatas++) {
            var data = value[iteDatas];
            var defaultT = Date.parse(tm);
            var dataT = Date.parse(data.Dt);
            var timeSpan = dataT - defaultT;
            if (timeSpan > 0 && timeSpan <= (gap)) {
                dataStack.push(value[iteDatas].Data);
            }
        }
        if (dataStack.length > 0) {
            await setMonitoringData(key, dataStack);
        }
   }
}


// #region default Object
function init() {
    renderBody = document.getElementById('render-container');
    if (initScene() == false) {
        console.log("Error :: initScene");
        return false;
    }
    if (initCamera() == false) {
        console.log("Error :: initCamera");
        return false;
    }
    if (initGridHelper() == false) {
        console.log("Error :: initGridHelper");
        return false;
    }
    if (initLight() == false) {
        console.log("Error :: initLight");
        return false;
    }
    if (initDirectionLight() == false) {
        console.log("Error :: initDirectionLight");
        return false;
    }
    if (initRenderer() == false) {
        console.log("Error :: initRenderer");
        return false;
    }
    if (initListener() == false) {
        console.log("Error :: initListener");
        return false;
    }
    if (addObjects() == false) {
        console.log("Error :: AddObjects");
        return false;
    }
    if (addOrbit() == false) {
        console.log("Error :: AddOrbit");
        return false;
    }
    if (addDrawModePointer() == false) {
        console.log("Error :: addDrawModePointer");
        return false;
    }
    return true;
}



function dataCollectWorker() {
    if (stop == false && playbackRun == false) {
        var qrys = [];
        for (const [key, value] of Object.entries(defaultPrefabDataContext.dicPrefab)) {
            if (key > 10) continue;
            if (value.isAnim == undefined) continue;
            if (value.isAnim == false) continue;
            qrys.push(('https://localhost:44355/wecs/monitoring/v2/data/get/').concat(key));
        }
        worker.postMessage(qrys);
    }
}


async function logicWorker(datas) {
    if (datas.count == undefined || datas.count < 1) throw 'worker.onmessage return:jsonData.content.Zero';
    if (datas.statCode == undefined || datas.statCode != 2) throw 'worker.onmessage return:jsonData.content.Zero';
    if (datas.content == undefined) return 'logicWork loop:fail, content:undefined';
    const jsonData = JSON.parse(datas.content);
    var prefabType = jsonData.PrefabType;
    //에러메시지
    await clearMonitoringData();
    await setMonitoringData(prefabType, jsonData.Data);
    return 'logicWorker loop:cmplt';
}
async function clearMonitoringData() {
    for (var dicElem in dicWorkerMutex) {
        await dicWorkerMutex[Number(dicElem)].acquire();
        if (dicMonitoringDatas.hasOwnProperty(dicElem)) {
            delete dicMonitoringDatas[Number(dicElem)];
        }
        dicWorkerMutex[Number(dicElem)].release();
    }
    //await dicWorkerMutex[prefabType].acquire();
    //dicMonitoringDatas[prefabType] = datas;
    //dicWorkerMutex[prefabType].release();
}
async function setMonitoringData(prefabType, datas) {
    await dicWorkerMutex[prefabType].acquire();
    dicMonitoringDatas[prefabType] = datas;
    dicWorkerMutex[prefabType].release();
}
async function getMonitoringData(prefabType) {
    var rtDatas = [];
    if (dicMonitoringDatas[prefabType] == undefined) return undefined;
    await dicWorkerMutex[prefabType].acquire();
    dicMonitoringDatas[prefabType].forEach(function (monitoringData) {
        rtDatas.push(deepCopy(monitoringData));
    });
    dicWorkerMutex[prefabType].release();
    return rtDatas;
}

function deepCopy(o) {
    var result = {};
    if (typeof o === "object" && o !== null)
        for (var i in o) result[i] = deepCopy(o[i]);
    else result = o;
    return result;
}

function initCamera() {
    camera = new THREE.PerspectiveCamera(45, renderBody.offsetWidth / renderBody.offsetHeight, 1, 10000);
    camera.position.set(0, 50, 0);
    camera.lookAt(0, 0, 0);
}
function initScene() {
    scene = new THREE.Scene();
    scene.background = new THREE.Color(0xffffff);
}
function initGridHelper() {
    gridHelper = new THREE.GridHelper(1000, 400);
    var v = 0;
}
function initLight() {
    ambientLight = new THREE.AmbientLight(prefabColors.Gray);
}
function initDirectionLight() {
    directionalLight = new THREE.DirectionalLight(0xffffff);
    directionalLight.position.set(1, 0.75, 0.75).normalize();
}
function initRenderer() {
    renderer = new THREE.WebGLRenderer({ antialias: true });
    renderer.setPixelRatio(window.devicePixelRatio);
    renderer.setSize(renderBody.offsetWidth, renderBody.offsetHeight);
    renderBody.appendChild(renderer.domElement);
}
function addOrbit() {
    orbit = new THREE.OrbitControls(camera, renderer.domElement);
    //controls.listenToKeyEvents(window); // optional
    //controls.addEventListener( 'change', render ); // call this only in static scenes (i.e., if there is no animation loop)

    //orbit.enableDamping = true; // an animation loop is required when either damping or auto-rotation are enabled
    //orbit.dampingFactor = 0.05;

    orbit.screenSpacePanning = false;

    orbit.minDistance = 30;
    orbit.maxDistance = 1000;

    orbit.maxPolarAngle = Math.PI / 2;

    orbit.panSpeed = 0.5;
    orbit.rotateSpeed = 0.5;

    orbit.enabled = false;
    orbit.enablePan = false;
    orbit.enableRotate = false;
    orbit.enableZoom = false;
}
function addObjects() {
    if (gridHelper != undefined) {
        scene.add(gridHelper);
    }
    if (ambientLight != undefined) {
        scene.add(ambientLight);
    }
    if (directionalLight != undefined) {
        scene.add(directionalLight);
    }
}
function addDrawModePointer() {
    rayCaster = new THREE.Raycaster();
    mousePointer = new THREE.Vector2();
    const geometry = new THREE.PlaneGeometry(1000, 1000);
    geometry.rotateX(- Math.PI / 2);
    plane = new THREE.Mesh(geometry, new THREE.MeshBasicMaterial({ visible: false, }));
    plane.name = planeName;
    scene.add(plane);
    var loadedPlane = scene.getObjectByName(planeName);
    rayCasterObjects.push(loadedPlane);
    rayCasterObjectsDrawMode.push(loadedPlane);
}
function initListener() {
    window.addEventListener('resize', onWindowResize);
    document.addEventListener('pointermove', onPointerMove);
    document.addEventListener('pointerdown', onPointerDown);
    document.addEventListener('pointerup', onPointerUp);
    document.addEventListener('keydown', onDocumentKeyDown);
    document.addEventListener('keyup', onDocumentKeyUp);
}
// #endregion

// #region handler code
function onWindowResize() {
    camera.aspect = renderBody.offsetWidth / renderBody.offsetHeight;
    camera.updateProjectionMatrix();
    renderer.setSize(renderBody.offsetWidth, renderBody.offsetHeight);

    render();
}
function onPointerMove(event) {
    if (isAltDown == true) return;
    mousePointer.set((event.offsetX / renderBody.offsetWidth) * 2 - 1, - (event.offsetY / renderBody.offsetHeight) * 2 + 1);
    rayCaster.setFromCamera(mousePointer, camera);

    if (drawMode == true) {
        const intersects = rayCaster.intersectObjects(rayCasterObjectsDrawMode, true);
        if (intersects.length < 1) return;
        var intersect = intersects[0];

        if (isCtrlDown == true) {
            if (isPointerDown == true) {
                if (intersect.object.name != planeName) return;
                if (cursorPrefab == undefined) return;
                var cloneModel = cursorPrefab.getModelClone();
                cloneModel.name = uuidv4();
                cloneModel.prefabType = cursorPrefab.prefabType;
                cloneModel.bindKey = '';
                cloneModel.position.copy(intersect.point).add(intersect.face.normal);
                cloneModel.position.divideScalar(modelScale).floor().multiplyScalar(modelScale).addScalar(0);
                var cloningPrefab = new Prefab(cursorPrefab.prefabType, cloneModel.name, '', cloneModel.position.x, cloneModel.position.y, cloneModel.position.z, cursorPrefab.rotation.x, cursorPrefab.rotation.y, cursorPrefab.rotation.z, cursorPrefab.isCasting, cursorPrefab.isAnim, cursorPrefab.isPosition);
                cloningPrefab.setModel(cloneModel);
                cloningPrefab.complete();
                prefabDataContext.addPrefabWithPrefabKey(cloningPrefab);
                scene.add(cloneModel);
                if (cloningPrefab.isCasting == true) {
                    rayCasterObjects.push(cloneModel);
                }
                rayCasterObjectsDrawMode.push(cloneModel);
            }
            //origin
            if (cursorPrefab != undefined) {
                if (cursorPrefab.fbxComponent.model.visible == false) {
                    cursorPrefab.fbxComponent.model.visible = true;
                }
                cursorPrefab.fbxComponent.model.position.copy(intersect.point).add(intersect.face.normal);
                cursorPrefab.fbxComponent.model.position.divideScalar(modelScale).floor().multiplyScalar(modelScale).addScalar(0);
            }
        }
        else {
            if (cursorPrefab != undefined) {
                if (cursorPrefab.fbxComponent.model.visible == true) {
                    cursorPrefab.fbxComponent.model.visible = false;
                }
            }
        }
    }
}
function onPointerUp(event) {
    isPointerDown = false;
}
function onPointerDown(event) {
    isPointerDown = true;
    if (isAltDown == true) return;

    mousePointer.set((event.offsetX / renderBody.offsetWidth) * 2 - 1, - (event.offsetY / renderBody.offsetHeight) * 2 + 1);
    rayCaster.setFromCamera(mousePointer, camera);

    if (drawMode == true) {
        const intersects = rayCaster.intersectObjects(rayCasterObjectsDrawMode, true);
        if (intersects.length < 1) return;
        const intersect = intersects[0];
        //추가
        if (isCtrlDown == true) {
            if (cursorPrefab == undefined) return;
            var cloneModel = cursorPrefab.getModelClone();
            cloneModel.name = uuidv4();
            cloneModel.prefabType = cursorPrefab.prefabType;
            cloneModel.bindKey = '';
            cloneModel.position.copy(intersect.point).add(intersect.face.normal);
            cloneModel.position.divideScalar(modelScale).floor().multiplyScalar(modelScale).addScalar(0);
            var cloningPrefab = new Prefab(cursorPrefab.prefabType, cloneModel.name, '', cloneModel.position.x, cloneModel.position.y, cloneModel.position.z, cursorPrefab.rotation.x, cursorPrefab.rotation.y, cursorPrefab.rotation.z, cursorPrefab.isCasting, cursorPrefab.isAnim, cursorPrefab.isPosition);
            cloningPrefab.setModel(cloneModel);
            cloningPrefab.complete();
            prefabDataContext.addPrefabWithPrefabKey(cloningPrefab);
            scene.add(cloneModel);
            if (cloningPrefab.isCasting == true) {
                rayCasterObjects.push(cloneModel);
            }
            rayCasterObjectsDrawMode.push(cloneModel);
        }
        //삭제
        else if (isShiftDown == true) {
            if (intersect.object.name == planeName) return;
            var intersectParent = intersect.object.parent;
            if (intersectParent == null || intersectParent == undefined) {
                scene.remove(intersect.object);
                rayCasterObjects.splice(rayCasterObjects.indexOf(intersect.object), 1);
                rayCasterObjectsDrawMode.splice(rayCasterObjectsDrawMode.indexOf(intersect.object), 1);
                prefabDataContext.removePrefabWithPrefabKey(intersect.object.name);
            }
            else {
                scene.remove(intersectParent);
                rayCasterObjects.splice(rayCasterObjects.indexOf(intersectParent), 1);
                rayCasterObjectsDrawMode.splice(rayCasterObjectsDrawMode.indexOf(intersectParent), 1);
                prefabDataContext.removePrefabWithPrefabKey(intersectParent.name);
            }
            render();
        }
        else {
            if (event.button == 0) {
                if (intersect.object.name == planeName) return;
                var intersectParent = intersect.object.parent;
                if (intersectParent == null || intersectParent == undefined) return;
                var prefab = prefabDataContext.getPrefab(intersectParent.name);
                if (prefab == undefined || prefab == null) return;
                var item = new PrefabInterfaceItem(prefab.prefabType, prefab.prefabKey, prefab.bindingKey, prefab.position, prefab.rotation, prefab.isCasting, prefab.isAnim, prefab.isPosition);
                razor.invokeMethodAsync('ChangePropDrawModePrefabInvokable', JSON.stringify(item));
            }
        }
    }
    else if (uiOverlay == true) {

        const intersects = rayCaster.intersectObjects(rayCasterObjectsDrawMode, true);
        if (intersects.length < 1) return;
        const intersect = intersects[0];
        //추가
        if (isCtrlDown == true) {
            return;
        }
        else if (isAltDown == true) {
            return;
        }
        else {
            if (event.button == 0) {
                if (intersect.object.name == planeName) return;
                var intersectParent = intersect.object.parent;
                if (intersectParent == null || intersectParent == undefined) return;
                var prefab = prefabDataContext.getPrefab(intersectParent.name);
                if (prefab == undefined || prefab == null) return;
                var item = new PrefabInterfaceItem(prefab.prefabType, prefab.prefabKey, prefab.bindingKey, prefab.position, prefab.rotation, prefab.isCasting, prefab.isAnim, prefab.isPosition);
                razor.invokeMethodAsync('ClickUiOverlayPrefabInvokable', JSON.stringify(item));
            }
        }
    }
}
function onDocumentKeyDown(event) {
    switch (event.keyCode) {
        case 16:
            {
                if (drawMode == true) {
                    renderBody.style.cursor = 'no-drop';
                }
                isShiftDown = true;
                break;
            }
        case 17:
            {
                if (drawMode == true) {
                    renderBody.style.cursor = 'crosshair';
                }
                isCtrlDown = true;
                break;
            }
        case 18:
            {
                isAltDown = true;
                renderBody.style.cursor = 'move';
                orbit.enabled = true;
                orbit.enablePan = true;
                orbit.enableRotate = true;
                orbit.enableZoom = true;
                break;
            }
    }
}
function onDocumentKeyUp(event) {
    switch (event.keyCode) {
        case 16:
            {
                renderBody.style.cursor = 'default';
                isShiftDown = false;
                break;
            }
        case 17:
            {
                renderBody.style.cursor = 'default';
                isCtrlDown = false;

                break;
            }
        case 18: {
            isAltDown = false;
            renderBody.style.cursor = 'default';
            orbit.enabled = false;
            orbit.enablePan = false;
            orbit.enableRotate = false;
            orbit.enableZoom = false;
        }
    }
}
// #endregion


// #region razor interface function : js <- razor

//savejson
export function getDrawModeObjectsInterop() {
    return prefabDataContext.stringfy();
}


export function saveDrawModeSelectedItemPropInterop(json) {
    try {
        var interfaceItem = JSON.parse(json);
        var position = new THREE.Vector3(interfaceItem.Position.X, interfaceItem.Position.Y, interfaceItem.Position.Z);
        var rotation = new THREE.Vector3(interfaceItem.Rotation.X, interfaceItem.Rotation.Y, interfaceItem.Rotation.Z);
        var interfaceItemCast = new PrefabInterfaceItem(interfaceItem.PrefabType, interfaceItem.PrefabKey, interfaceItem.BindingKey, position, rotation, interfaceItem.IsCasting, interfaceItem.IsAnim, interfaceItem.IsPosition);
        var loadedPrefab = prefabDataContext.getPrefab(interfaceItemCast.prefabKey);
        if (loadedPrefab == undefined || loadedPrefab == null) return false;
        loadedPrefab.bindingKey = interfaceItemCast.bindingKey;
        loadedPrefab.position = interfaceItemCast.position;
        loadedPrefab.rotation = interfaceItemCast.rotation;
        loadedPrefab.isCasting = interfaceItemCast.isCasting;
        loadedPrefab.isAnim = interfaceItemCast.isAnim;
        loadedPrefab.isPosition = interfaceItemCast.isPosition;
        loadedPrefab.fbxSetPosition();
        loadedPrefab.fbxSetRotation();
        render();
        return true;
    }
    catch (e) {
        console.log(e);
        return false;
    }

}


export function initInterop(dotNetObject) {
    if (dotNetObject == null || dotNetObject == undefined) {
        return false;
    }
    razor = dotNetObject;
    init();

    return true;
}

export function initMonitoringDataInterop(jsonObjects) {
    var objects = JSON.parse(jsonObjects);
    //url
    defaultRequestUrlContext = new PrefabUrlContext();
    objects.UrlInit.forEach(
        function (item) {
            var url = item.Url;
            item.PrefabTypes.forEach(
                function (typeItem) {
                    defaultRequestUrlContext.addUrl(typeItem, url);
                }
            );
        }
    );

    //fbxpath
    defaultPrefabFbxModelPathContext = new PrefabFbxModelPathContext();
    objects.FbxModelInit.forEach(
        function (item) {
            defaultPrefabFbxModelPathContext.addPath(item.PrefabType, item.Path);
        }
    );

    //default model - step 1 : context stack
    defaultPrefabDataContext = new PrefabContext();
    objects.PrefabDefaultInit.forEach(
        function (item) {
            defaultPrefabDataContext.addPrefabWithType(new Prefab(item.PrefabType, item.PrefabKey, item.BindingKey, item.Position.X, item.Position.Y, item.Position.Z, item.Rotation.X, item.Rotation.Y, item.Rotation.Z, item.IsCasting, item.IsAnim, item.IsPosition), item.PrefabType);
        }
    );

    //default model - step 2 : context model load
    //cloning - loaded rq model
    prefabDataContext = new PrefabContext();
    var loadRequestObjects;
    if (objects.PrefabJsonData != undefined && objects.PrefabJsonData != null && objects.PrefabJsonData != '') {
        loadRequestObjects = JSON.parse(objects.PrefabJsonData);
    }
    for (const [key, value] of Object.entries(defaultPrefabDataContext.dicPrefab)) {
        var fbxPath = defaultPrefabFbxModelPathContext.getPath(key);
        loadFbx(fbxPath, key).then((fbx) => {
            value.setModel(fbx);
            value.complete();
            if (loadRequestObjects != undefined) {
                loadRequestObjects.forEach(function (item) {
                    if (fbx.prefabType != item.prefabType) return;
                    var loadRequestPrefab = new Prefab(item.prefabType, item.prefabKey, item.bindingKey, item.position.x, item.position.y, item.position.z, item.rotation.x, item.rotation.y, item.rotation.z, item.isCasting, item.isAnim, item.isPosition);
                    prefabDataContext.addPrefabWithPrefabKey(loadRequestPrefab);
                    var cloneModel = value.fbxComponent.getModelClone();
                    cloneModel.name = loadRequestPrefab.prefabKey;
                    cloneModel.prefabType = loadRequestPrefab.prefabType;
                    cloneModel.bindKey = loadRequestPrefab.bindingKey;
                    cloneModel.position.x = loadRequestPrefab.position.x;
                    cloneModel.position.y = loadRequestPrefab.position.y;
                    cloneModel.position.z = loadRequestPrefab.position.z;
                    cloneModel.rotation.x = THREE.Math.degToRad(loadRequestPrefab.rotation.x);
                    cloneModel.rotation.y = THREE.Math.degToRad(loadRequestPrefab.rotation.y);
                    cloneModel.rotation.z = THREE.Math.degToRad(loadRequestPrefab.rotation.z);
                    scene.add(cloneModel);
                    loadRequestPrefab.setModel(cloneModel);
                    loadRequestPrefab.complete();
                    //raycast objects
                    if (loadRequestPrefab.isCasting == true) {
                        rayCasterObjects.push(loadRequestPrefab.fbxComponent.model);
                    }
                    rayCasterObjectsDrawMode.push(loadRequestPrefab.fbxComponent.model);
                });
            }
        });
    }




    then = Date.now();
    fps = 60;
    fpsInterval = 1000 / fps;
    startTime = Date.now();
    animate().then(() => {
        console.log('animate loop 1');
    });


    worker = new Worker('/monitoring/js/module/monitoring-v2-worker.js');
    for (const [key, value] of Object.entries(defaultPrefabDataContext.dicPrefab)) {
        dicWorkerMutex[key] = new Mutex();
        dicMonitoringDatas[key] = [];
    }
    worker.onmessage = (jsonData) => {
        try {
            if (jsonData == undefined) throw 'worker.onmessage return:jsonData null';
            if (jsonData.data[0] == undefined) throw 'worker.onmessage return:jsonData.data null';
            jsonData.data[0].forEach((datas) => {
                logicWorker(datas)
            });
        } catch (e) {
            console.log(e);
        }
        setTimeout(dataCollectWorker, 500);
    }
    dataCollectWorker();

    return true;
}


export function changeDrawModeInterop(isDraw) {
    drawMode = isDraw;
    gridHelper.visible = drawMode;

    if (isDraw == false) {
        var loadedCursor = scene.getObjectByName(cursorKey, true);
        if (loadedCursor != null) {
            scene.remove(loadedCursor);
        }
    }
    cursorPrefab = undefined;
    return true;
}
export function drawModeCasterChangeInterop(castedType) {
    if (drawMode == true) {
        if (cursorPrefab == undefined) {
            var defaultPrefab = defaultPrefabDataContext.getPrefab(castedType);
            cursorPrefab = new Prefab(defaultPrefab.prefabType, defaultPrefab.prefabKey, defaultPrefab.bindingKey, defaultPrefab.position.x, defaultPrefab.position.y, defaultPrefab.position.z, defaultPrefab.rotation.x, defaultPrefab.rotation.y, defaultPrefab.rotation.z, defaultPrefab.isCasting, defaultPrefab.isAnim, defaultPrefab.isPosition);
            cursorPrefab.setModel(defaultPrefab.fbxComponent.getModelClone());
            cursorPrefab.complete();
            var model = cursorPrefab.getModel();
            model.name = cursorKey;
            scene.add(model);
        }
        else if (cursorPrefab.prefabType != castedType) {
            var loadedCursor = scene.getObjectByName(cursorPrefab.getModel().name, false);
            if (loadedCursor != null) {
                scene.remove(loadedCursor);
            }
            var defaultPrefab = defaultPrefabDataContext.getPrefab(castedType);
            cursorPrefab = new Prefab(defaultPrefab.prefabType, defaultPrefab.prefabKey, defaultPrefab.bindingKey, defaultPrefab.position.x, defaultPrefab.position.y, defaultPrefab.position.z, defaultPrefab.rotation.x, defaultPrefab.rotation.y, defaultPrefab.rotation.z, defaultPrefab.isCasting, defaultPrefab.isAnim, defaultPrefab.isPosition);
            cursorPrefab.setModel(defaultPrefab.fbxComponent.getModelClone());
            cursorPrefab.complete();
            var model = cursorPrefab.getModel();
            model.name = cursorKey;
            scene.add(model);
        }
    }
}
// #endregion

//#region render
async function animate() {
    now = Date.now();
    elapsed = now - then;
    if (elapsed > fpsInterval) {
        then = now - (elapsed % fpsInterval);

        //render code
        renderControl();
        await renderObject(elapsed);
        render();
        frameCount++;
        // console.log(frameCount / (startTime - elapsed));
    }
    if (stop == false || playbackRun == true) {
        requestAnimationFrame(animate);
    }
}

async function renderObject(elapsed) {
    var renderDatas = [];
    for (const [key, value] of Object.entries(defaultPrefabDataContext.dicPrefab)) {
        var data = await getMonitoringData(value.prefabType);
        if (data == undefined) continue;
        renderDatas.push(data);
    }

    if (renderDatas == undefined) return;
    let parallelDatas = [];
    renderDatas.forEach(
        function (renderData) {
            for (var ite = 0; ite < renderData.length; ite++) {
                var prefab = prefabDataContext.getPrefab(renderData[ite].PrefabKey);
                if (prefab == undefined) return;
                parallelDatas.push({
                    'prefab': prefab,
                    'renderData': renderData[ite],
                    'elapsed': elapsed,
                });
            }
        }
    );
    let requests = parallelDatas.map((pf, rd, elap) => renderLogic(pf, rd, elap));
    await Promise.all(requests)
        .then((responses) => {
            // console.log(responses);
        })
        .catch(error => {
            //  console.log(error.message)
        });
}
// #region return : 'success' or 'fail'
function renderLogic(renderDataRepo) {
    // CV = 0
    // SC, 1
    // RTV, 2 
    // RACK, 3 
    // BCR, 4
    // CV_LEFT = 10
    // CV_RIGHT = 20
    var prefabType = (renderDataRepo.prefab.prefabType == 0) ? 0 : (renderDataRepo.prefab.prefabType % 10);
    if (prefabType == prefabTypes.Cv) return renderLogicCv(renderDataRepo.prefab, renderDataRepo.renderData, renderDataRepo.elapsed);
    else if (prefabType == prefabTypes.Sc) return renderLogicSc(renderDataRepo.prefab, renderDataRepo.renderData, renderDataRepo.elapsed);
    else if (prefabType == prefabTypes.Rtv) return renderLogicRtv(renderDataRepo.prefab, renderDataRepo.renderData, renderDataRepo.elapsed);
    else if (prefabType == prefabTypes.Rack) return renderLogicRack(renderDataRepo.prefab, renderDataRepo.renderData, renderDataRepo.elapsed);
    else if (prefabType == prefabTypes.Bcr) return renderLogicBcr(renderDataRepo.prefab, renderDataRepo.renderData, renderDataRepo.elapsed);
    return 'fail';
}
// #endregion
function renderLogicCv(prefab, renderData, elapsed) {
    try {
        var foundFbxComponent = prefab.getFbxComponent();
        if (foundFbxComponent == undefined) return 'fail';

        // #region anim
        if (prefab.isAnim == true) {
            if (foundFbxComponent.dicAnimations != undefined) {
                var runAnim = undefined;
                if (renderData.Err != '0') {
                    runAnim = (renderData.Prod == '1') ? foundFbxComponent.getAnimation('boxon') : undefined;
                    var mesh = foundFbxComponent.getMesh(foundFbxComponent.getMaskMeshName());
                    if (mesh != undefined) {
                        mesh.material.emissive.setHex(prefabColors.Red);
                    }
                }
                else {
                    var mesh = foundFbxComponent.getMesh(foundFbxComponent.getMaskMeshName());
                    if (mesh != undefined) {
                        mesh.material.emissive.setHex(foundFbxComponent.getWrkTypColor(renderData.WrkTyp));
                    }
                    if (prefab.bindingKey == renderData.Dest || renderData.Dest == '0') {
                        runAnim = (renderData.Prod == '1') ? foundFbxComponent.getAnimation('boxon') : undefined;
                    }
                    else {
                        runAnim = (renderData.Prod == '1') ? foundFbxComponent.getAnimation('boxgo') : undefined;
                    }
                }

                //run routine
                if (runAnim == undefined) {
                    if (foundFbxComponent.prevAnim != undefined) {
                        if (foundFbxComponent.prevAnim.isRunning()) {
                            foundFbxComponent.prevAnim.stop();
                        }
                    }
                }
                else {
                    if (foundFbxComponent.prevAnim == undefined) {
                        foundFbxComponent.prevAnim = foundFbxComponent.animMixer.clipAction(runAnim, foundFbxComponent.model);
                        foundFbxComponent.prevAnim.play();
                    }
                    else {
                        if (foundFbxComponent.prevAnim._clip.name != runAnim.name) {
                            if (foundFbxComponent.prevAnim.isRunning()) {
                                foundFbxComponent.prevAnim.stop();
                            }
                            foundFbxComponent.prevAnim = foundFbxComponent.animMixer.clipAction(runAnim, foundFbxComponent.model);
                            foundFbxComponent.prevAnim.play();

                        }
                    }
                }

                var mesh = foundFbxComponent.getMesh('Box');
                if (mesh != undefined) {
                    mesh.visible = !(runAnim == undefined);
                }

                if (foundFbxComponent.prevAnim != undefined) {
                    if (foundFbxComponent.prevAnim._clip.name != 'boxon') {
                        foundFbxComponent.animMixer.update((elapsed / 3000));
                    }
                }
            }
        }
        // #endregion
        // #region position
        if (prefab.isPosition == true) {
            //somethign routine
        }
        // #endregion
        return 'success';
    } catch (e) {
        return 'fail';
    }
}
function renderLogicSc(prefab, renderData, elapsed) {
    try {
        var foundFbxComponent = prefab.getFbxComponent();
        if (foundFbxComponent == undefined) return 'fail';
        // #region anim
        if (prefab.isAnim == true) {
            var boxMesh = foundFbxComponent.getMesh('Box');
            if (foundFbxComponent.dicAnimations != undefined) {
                var runAnim = undefined;
                if (renderData.Err != '0') {
                    runAnim = (renderData.Prod == '1') ? foundFbxComponent.getAnimation('boxon') : undefined;
                    //mesh change
                    var mesh = foundFbxComponent.getMesh(foundFbxComponent.getMaskMeshName());
                    if (mesh != undefined) {
                        mesh.material.emissive.setHex(prefabColors.Red);
                    }

                } else {
                    var mesh = foundFbxComponent.getMesh(foundFbxComponent.getMaskMeshName());
                    if (mesh != undefined) {
                        mesh.material.emissive.setHex(prefabColors.Normal);
                    }
                    if (prefab.bindingKey == renderData.Dest || renderData.Dest == '0') {
                        runAnim = (renderData.Prod == '1') ? foundFbxComponent.getAnimation('boxon') : undefined;
                    }
                    else {
                        runAnim = (renderData.Prod == '1') ? foundFbxComponent.getAnimation('leftboxin') : undefined;
                    }
                }

                //run routine
                if (runAnim == undefined) {
                    if (foundFbxComponent.prevAnim != undefined) {
                        if (foundFbxComponent.prevAnim.isRunning()) {
                            foundFbxComponent.prevAnim.stop();
                        }
                    }
                }
                else {
                    if (foundFbxComponent.prevAnim == undefined) {
                        foundFbxComponent.prevAnim = foundFbxComponent.animMixer.clipAction(runAnim, foundFbxComponent.model);
                        foundFbxComponent.prevAnim.play();
                    }
                    else {
                        if (foundFbxComponent.prevAnim._clip.name != runAnim.name) {
                            if (foundFbxComponent.prevAnim.isRunning()) {
                                foundFbxComponent.prevAnim.stop();
                            }
                            foundFbxComponent.prevAnim = foundFbxComponent.animMixer.clipAction(runAnim, foundFbxComponent.model);
                            foundFbxComponent.prevAnim.play();

                        }
                    }
                }

                if (boxMesh != undefined) {
                    boxMesh.visible = !(runAnim == undefined);
                }

                if (foundFbxComponent.prevAnim != undefined) {
                    if (foundFbxComponent.prevAnim._clip.name != 'boxon') {
                        foundFbxComponent.animMixer.update((elapsed / 3000));
                    }
                }
            }
        }
        // #endregion

        // #region position
        if (prefab.isPosition == true) {
            var rotAxis = (prefab.rotation.y == 0 || prefab.rotation.y == 180) ? 'z' : 'x';
            var rotAxisMultiply = (prefab.rotation.y >= 180) ? -1 : 1;
            var prevPosHorizon = undefined;
            var originPosHorizon = undefined;
            var updatePosHorizon = undefined;
            var normalPosDistanceHorizon = undefined;
            var distancePosHorizon = undefined;
            var targetPosHorizon = undefined;
            var changePosHorizon = undefined;
            var prevPosVertical = undefined;
            var originPosVertical = undefined;
            var updatePosVertical = undefined;
            var normalPosDistanceVertical = undefined;
            var distancePosVertical = undefined;
            var targetPosVertical = undefined;
            var changePosVertical = undefined;
            var vertical = renderData.PosY;
            var horizon = renderData.PosZ;
            var carriageMesh = foundFbxComponent.getMesh('Carriage');
            var isVerticalRun = (carriageMesh == undefined) ? false : true;
            var speed = elapsed / 3000;

            if (rotAxis == 'x') {
                prevPosHorizon = foundFbxComponent.model.position.x;
                originPosHorizon = prefab.position.x;
                updatePosHorizon = prevPosHorizon;
            }
            else {
                prevPosHorizon = foundFbxComponent.model.position.z;
                originPosHorizon = prefab.position.z;
                updatePosHorizon = prevPosHorizon;
            }

            prevPosVertical = (isVerticalRun == true) ? carriageMesh.position.y : 0;
            originPosVertical = (isVerticalRun == true) ? foundFbxComponent.getOriginPos('Carriage').y : 0;
            updatePosVertical = prevPosVertical;

            if (rotAxisMultiply < 0) //음수벡터의 방향일 경우
            {
                normalPosDistanceHorizon = originPosHorizon - (horizon * modelScale);
                distancePosHorizon = prevPosHorizon - normalPosDistanceHorizon;
                targetPosHorizon = prevPosHorizon + (distancePosHorizon * rotAxisMultiply);
                changePosHorizon = prevPosHorizon + ((distancePosHorizon * speed) * rotAxisMultiply);
                updatePosHorizon = changePosHorizon;

                if (distancePosHorizon <= 0) //음수벡터의 이동일 경우
                {
                    if (targetPosHorizon <= changePosHorizon) {
                        updatePosHorizon = targetPosHorizon;
                    }
                }
                else {
                    if (targetPosHorizon >= changePosHorizon) {
                        updatePosHorizon = targetPosHorizon;
                    }
                }

                normalPosDistanceVertical = originPosVertical - (vertical * modelScale);
                distancePosVertical = prevPosVertical - normalPosDistanceVertical;
                targetPosVertical = prevPosVertical + (distancePosVertical * rotAxisMultiply);
                changePosVertical = prevPosVertical + ((distancePosVertical * speed) * rotAxisMultiply);
                updatePosVertical = changePosVertical;
                if (distancePosVertical <= 0) //음수벡터의 이동일 경우
                {
                    if (targetPosVertical <= changePosVertical) {
                        updatePosVertical = targetPosVertical;
                    }
                }
                else {
                    if (targetPosVertical >= changePosVertical) {
                        updatePosVertical = targetPosVertical;
                    }
                }
            }
            else// if(axisVerticalMultiply > 0) //양수벡터의 방향일 경우
            {
                normalPosDistanceHorizon = originPosHorizon + (horizon * modelScale);
                distancePosHorizon = prevPosHorizon - normalPosDistanceHorizon;
                targetPosHorizon = prevPosHorizon - (distancePosHorizon * rotAxisMultiply);
                changePosHorizon = prevPosHorizon - ((distancePosHorizon * speed) * rotAxisMultiply);
                updatePosHorizon = changePosHorizon

                if (distancePosHorizon >= 0) //양수벡터의 이동일 경우
                {
                    if (targetPosHorizon >= changePosHorizon) {
                        updatePosHorizon = targetPosHorizon;
                    }
                }
                else {
                    if (targetPosHorizon <= changePosHorizon) {
                        updatePosHorizon = targetPosHorizon;
                    }
                }

                normalPosDistanceVertical = originPosVertical + (vertical * modelScale);
                distancePosVertical = prevPosVertical - normalPosDistanceVertical;
                targetPosVertical = prevPosVertical - (distancePosVertical * rotAxisMultiply);
                changePosVertical = prevPosVertical - ((distancePosVertical * speed) * rotAxisMultiply);
                updatePosVertical = changePosVertical

                if (distancePosVertical >= 0) //양수벡터의 이동일 경우
                {
                    if (targetPosVertical >= changePosVertical) {
                        updatePosVertical = targetPosVertical;
                    }
                }
                else {
                    if (targetPosVertical <= changePosVertical) {
                        updatePosVertical = targetPosVertical;
                    }
                }
            }

            if (isVerticalRun == true) {
                carriageMesh.position.y = updatePosVertical;
                if (boxMesh != undefined) {
                    var normalizeBox = undefined;
                    normalizeBox = originPosVertical - foundFbxComponent.getOriginPos('Box').y;
                    boxMesh.position.y = updatePosVertical - normalizeBox;
                }
            }
            var updateVector3 = new THREE.Vector3((rotAxis == 'x') ? updatePosHorizon : foundFbxComponent.model.position.x,
                foundFbxComponent.model.position.y,
                (rotAxis == 'z') ? updatePosHorizon : foundFbxComponent.model.position.z);
            foundFbxComponent.fbxSetPosition(updateVector3);
        }
        // #endregion
        return 'success';
    } catch (e) {
        return 'fail';
    }
}
function renderLogicRtv(prefab, renderData, elapsed) {
    try {
        var foundFbxComponent = prefab.getFbxComponent();
        if (foundFbxComponent == undefined) return 'fail';
        // #region anim
        if (prefab.isAnim == true) {
            if (foundFbxComponent.dicAnimations != undefined) {
                var runAnim = undefined;
                if (renderData.Err != '0') {
                    runAnim = (renderData.Prod == '1') ? foundFbxComponent.getAnimation('boxon') : undefined;
                    //mesh change
                    var mesh = foundFbxComponent.getMesh(foundFbxComponent.getMaskMeshName());
                    if (mesh != undefined) {
                        mesh.material.emissive.setHex(prefabColors.Red);
                    }

                } else {
                    var mesh = foundFbxComponent.getMesh(foundFbxComponent.getMaskMeshName());
                    if (mesh != undefined) {
                        mesh.material.emissive.setHex(prefabColors.Normal);
                    }
                    if (prefab.bindingKey == renderData.Dest || renderData.Dest == '0') {
                        runAnim = (renderData.Prod == '1') ? foundFbxComponent.getAnimation('boxon') : undefined;
                    }
                    else {
                        runAnim = (renderData.Prod == '1') ? foundFbxComponent.getAnimation('leftboxin') : undefined;
                    }
                }


                //run routine
                if (runAnim == undefined) {
                    if (foundFbxComponent.prevAnim != undefined) {
                        if (foundFbxComponent.prevAnim.isRunning()) {
                            foundFbxComponent.prevAnim.stop();
                        }
                    }
                }
                else {
                    if (foundFbxComponent.prevAnim == undefined) {
                        foundFbxComponent.prevAnim = foundFbxComponent.animMixer.clipAction(runAnim, foundFbxComponent.model);
                        foundFbxComponent.prevAnim.play();
                    }
                    else {
                        if (foundFbxComponent.prevAnim._clip.name != runAnim.name) {
                            if (foundFbxComponent.prevAnim.isRunning()) {
                                foundFbxComponent.prevAnim.stop();
                            }
                            foundFbxComponent.prevAnim = foundFbxComponent.animMixer.clipAction(runAnim, foundFbxComponent.model);
                            foundFbxComponent.prevAnim.play();

                        }
                    }
                }

                var mesh = foundFbxComponent.getMesh('Box');
                if (mesh != undefined) {
                    mesh.visible = !(runAnim == undefined);
                }

                if (foundFbxComponent.prevAnim != undefined) {
                    if (foundFbxComponent.prevAnim._clip.name != 'boxon') {
                        foundFbxComponent.animMixer.update((elapsed / 3000));
                    }
                }
            }
        }
        // #endregion
        // #region position
        if (prefab.isPosition == true) {
            var rotAxis = (prefab.rotation.y == 0 || prefab.rotation.y == 180) ? 'x' : 'z';
            var rotAxisMultiply = (prefab.rotation.y >= 180) ? -1 : 1;
            var prevPosHorizon = undefined;
            var originPosHorizon = undefined;
            var updatePosHorizon = undefined;
            var normalPosDistanceHorizon = undefined;
            var distancePosHorizon = undefined;
            var targetPosHorizon = undefined;
            var changePosHorizon = undefined;
            var vertical = renderData.PosY;
            var horizon = renderData.PosZ;
            var speed = elapsed / 3000;
            if (rotAxis == 'x') {
                prevPosHorizon = foundFbxComponent.model.position.x;
                originPosHorizon = prefab.position.x;
                updatePosHorizon = foundFbxComponent.model.position.x;
            }
            else {
                prevPosHorizon = foundFbxComponent.model.position.z;
                originPosHorizon = prefab.position.z;
                updatePosHorizon = foundFbxComponent.model.position.z;
            }

            if (rotAxisMultiply < 0) //음수벡터의 방향일 경우
            {
                normalPosDistanceHorizon = originPosHorizon - (horizon * modelScale);
                distancePosHorizon = prevPosHorizon - normalPosDistanceHorizon;
                targetPosHorizon = prevPosHorizon + (distancePosHorizon * rotAxisMultiply);
                changePosHorizon = prevPosHorizon + ((distancePosHorizon * speed) * rotAxisMultiply);
                updatePosHorizon = changePosHorizon;

                if (distancePosHorizon <= 0) //음수벡터의 이동일 경우
                {
                    if (targetPosHorizon <= changePosHorizon) {
                        updatePosHorizon = targetPosHorizon;
                    }
                }
                else {
                    if (targetPosHorizon >= changePosHorizon) {
                        updatePosHorizon = targetPosHorizon;
                    }
                }
            }
            else// if(axisVerticalMultiply > 0) //양수벡터의 방향일 경우
            {
                normalPosDistanceHorizon = originPosHorizon + (horizon * modelScale);
                distancePosHorizon = prevPosHorizon - normalPosDistanceHorizon;
                targetPosHorizon = prevPosHorizon - (distancePosHorizon * rotAxisMultiply);
                changePosHorizon = prevPosHorizon - ((distancePosHorizon * speed) * rotAxisMultiply);
                updatePosHorizon = changePosHorizon

                if (distancePosHorizon >= 0) //양수벡터의 이동일 경우
                {
                    if (targetPosHorizon >= changePosHorizon) {
                        updatePosHorizon = targetPosHorizon;
                    }
                }
                else {
                    if (targetPosHorizon <= changePosHorizon) {
                        updatePosHorizon = targetPosHorizon;
                    }
                }
            }
            var updateVector3 = new THREE.Vector3((rotAxis == 'x') ? updatePosHorizon : foundFbxComponent.model.position.x,
                foundFbxComponent.model.position.y,
                (rotAxis == 'z') ? updatePosHorizon : foundFbxComponent.model.position.z);
            foundFbxComponent.fbxSetPosition(updateVector3);
        }
        // #endregion
        return 'success';
    } catch (e) {
        return 'fail';
    }
}
function renderLogicRack(prefab, renderData, elapsed) {
    try {

        return 'success';
    } catch (e) {
        return 'fail';
    }
}
function renderLogicBcr(prefab, renderData, elapsed) {
    try {

        return 'success';
    } catch (e) {
        return 'fail';
    }
}

//#region orbit
function renderControl() {
    if (orbit.enabled == true) {
        orbit.update();
    }
}
//#endregion

//#region  render
function render() {
    renderer.render(scene, camera);
}
//#endregion

// #endregion

// #region class
class PrefabFbxModelPathContext {
    constructor() {
        this.dicFbxPath = {};
    }
    addPath(type, path) {
        this.dicFbxPath[type] = path;
    }
    getPath(type) {
        return this.dicFbxPath[type];
    }
}
class PrefabUrlContext {
    constructor() {
        this.dicRequestUrls = {};
    }
    addUrl(type, url) {
        this.dicRequestUrls[type] = url;
    }
    getUrl(type) {
        return this.dicRequestUrls[type];
    }
}

class PrefabContext {
    constructor() {
        this.dicPrefab = {};
    }
    addPrefabWithPrefabKey(prefab) {
        this.dicPrefab[prefab.prefabKey] = prefab;
    }
    removePrefabWithPrefabKey(prefabKey) {
        if (this.dicPrefab.hasOwnProperty(prefabKey)) {
            delete this.dicPrefab[prefabKey];
        }

    }
    addPrefabWithType(prefab) {
        this.dicPrefab[prefab.prefabType] = prefab;
    }
    getPrefab(prefabKey) {
        return this.dicPrefab[prefabKey];
    }
    stringfy() {
        var rtArray = [];
        for (const [key, value] of Object.entries(this.dicPrefab)) {
            rtArray.push(value.getInterfaceItem());
        }
        //var prefabArray = new Prefab();
        //for (var ite = 0; ite < raycasterObjects.length; ite++) {
        //    var item = raycasterObjects[ite];
        //    if (item.name == planeName) continue;
        //    var bindkey = item.eBindKey;
        //    if (bindkey == undefined) bindkey = '';
        //    var radRotation = {};
        //    radRotation.x = THREE.Math.radToDeg(item.rotation.x);
        //    radRotation.y = THREE.Math.radToDeg(item.rotation.y);
        //    radRotation.z = THREE.Math.radToDeg(item.rotation.z);
        //    prefabArray.push(new RazorPrefabInterface(item.eType, item.name, item.defaultPosition, item.position, radRotation, item.eBindKey));
        //}

        return JSON.stringify(rtArray);
    }
}

class Prefab {
    constructor(pPrefabType, pPrefabKey, pBindingKey, posX, posY, posZ, rotX, rotY, rotZ, pIsCasting, pIsAnim, pIsPosition) {
        //#region interface object
        this.prefabType = pPrefabType;
        this.prefabKey = pPrefabKey;
        this.bindingKey = pBindingKey;
        this.position = new THREE.Vector3();
        this.rotation = new THREE.Vector3();
        this.position.x = posX;
        this.position.y = posY;
        this.position.z = posZ;
        this.rotation.x = rotX;
        this.rotation.y = rotY;
        this.rotation.z = rotZ;
        this.isCasting = pIsCasting;
        this.isAnim = pIsAnim;
        this.isPosition = pIsPosition;
        //#endregion

        //#region inner items
        this.fbxComponent = new FbxModelComponent();
        //#endregion
    }
    complete() {
        this.fbxComponent.initDictionary();
        this.fbxComponent.copyMaterial();
        this.fbxComponent.initOriginPos();
    }
    getFbxComponent() {
        return this.fbxComponent;
    }
    setModel(model) {
        this.fbxComponent.setModel(model);
    }
    setModelName(name) {
        this.fbxComponent.setModelName(name);
    }
    getModel() {
        return this.fbxComponent.getModel();
    }
    getModelClone() {
        return this.fbxComponent.getModelClone();
    }
    getInterfaceItem() {
        return new PrefabInterfaceItem(this.prefabType, this.prefabKey, this.bindingKey, this.position, this.rotation, this.isCasting, this.isAnim, this.isPosition);
    }
    fbxSetPosition() {
        return this.fbxComponent.fbxSetPosition(this.position);
    }
    fbxSetRotation() {
        return this.fbxComponent.fbxSetRotation(this.rotation);
    }
}

class PrefabInterfaceItem {
    constructor(pPrefabType, pPrefabKey, pBindingKey, pPosition, pRotation, pIsCasting, pIsAnim, pIsPosition) {
        //#region interface object
        this.prefabType = pPrefabType;
        this.prefabKey = pPrefabKey;
        this.bindingKey = pBindingKey;
        this.position = pPosition;
        this.rotation = pRotation;
        this.isCasting = pIsCasting;
        this.isAnim = pIsAnim;
        this.isPosition = pIsPosition;
        //#endregion
    }
}


class FbxModelComponent {
    constructor() {
        this.model = undefined;
        this.animMixer = undefined;
        this.prevAnim = undefined;
        this.dicAnimations = {};
        this.dicMeshs = {};
        this.dicOriginPos = {};
    }
    getWrkTypColor(wrkTyp) {
        if (this.model == undefined) return prefabColors.Normal;
        if (this.model.prefabType == undefined) return prefabColors.Normal;
        if (this.model.prefabType == prefabTypes.Cv ||
            this.model.prefabType == prefabTypes.CvLeft ||
            this.model.prefabType == prefabTypes.CvRight) {
            if (wrkTyp == wrkTyps.Normal) return prefabColors.Normal;
            if (wrkTyp == wrkTyps.Sto) return prefabColors.Green;
            if (wrkTyp == wrkTyps.Ret) return prefabColors.Blue;
            if (wrkTyp == wrkTyps.Move) return prefabColors.Skyblue;
            return prefabColors.Normal;
        }
        if (this.model.prefabType == prefabTypes.Sc) {
            if (wrkTyp == wrkTyps.Normal) return prefabColors.Normal;
            if (wrkTyp == wrkTyps.Sto) return prefabColors.Green;
            if (wrkTyp == wrkTyps.Ret) return prefabColors.Blue;
            if (wrkTyp == wrkTyps.Move) return prefabColors.Skyblue;
            return prefabColors.Normal;
        }
        if (this.model.prefabType == prefabTypes.Rtv) {
            if (wrkTyp == wrkTyps.Normal) return prefabColors.Normal;
            if (wrkTyp == wrkTyps.Sto) return prefabColors.Green;
            if (wrkTyp == wrkTyps.Ret) return prefabColors.Blue;
            if (wrkTyp == wrkTyps.Move) return prefabColors.Skyblue;
            return prefabColors.Normal;
        }

        return prefabColors.Normal;
    }
    getMaskMeshName() {
        if (this.model != undefined) {
            if (this.model.prefabType == undefined) return undefined;
            if (this.model.prefabType == prefabTypes.Cv) return 'Cv';
            if (this.model.prefabType == prefabTypes.Sc) return 'Carriage';
            if (this.model.prefabType == prefabTypes.Rtv) return 'Rtv';
            if (this.model.prefabType == prefabTypes.Rack) return 'Rack';
            if (this.model.prefabType == prefabTypes.Bcr) return 'Bcr';
            if (this.model.prefabType == prefabTypes.CvLeft) return 'CvLeft';
            if (this.model.prefabType == prefabTypes.CvRight) return 'CvRight';
        }
        return undefined;
    }
    getOriginPos(name) {
        if (this.dicOriginPos == undefined) return undefined;
        if (this.dicOriginPos.hasOwnProperty(name) == false) return undefined;
        return this.dicOriginPos[name];
    }
    initOriginPos() {
        this.dicOriginPos[ORIGIN_MODEL_NAME] = this.model.position;
        if (this.dicMeshs != undefined) {
            for (const [key, value] of Object.entries(this.dicMeshs)) {
                this.dicOriginPos[key] = new THREE.Vector3(value.position.x, value.position.y, value.position.z);
            }
        }
    }
    copyMaterial() {
        var mesh = this.getMesh(this.getMaskMeshName());
        if (mesh != undefined) {
            mesh.material = mesh.material.clone();
        }
    }
    initDictionary() {
        if (this.model != undefined) {
            //dicAnimations
            if (this.model.animations != undefined) {
                this.model.animations.forEach(
                    (animation) => {
                        this.dicAnimations[animation.name] = animation;
                    }
                );
            }
            //dicMeshs
            if (this.model.children != undefined) {
                this.model.children.forEach(
                    (item) => {
                        if (item.type == 'Mesh') {
                            this.dicMeshs[item.name] = item;
                        }
                    }
                );
            }
        }
    }
    getMesh(name) {
        if (this.dicMeshs == undefined) return undefined;
        if (this.dicMeshs.hasOwnProperty(name) == false) return undefined;
        return this.dicMeshs[name];
    }
    getAnimation(name) {
        if (this.dicAnimations == undefined) return undefined;
        if (this.dicAnimations.hasOwnProperty(name) == false) return undefined;
        return this.dicAnimations[name];
    }
    setModelName(nm) {
        this.model.name = nm;
    }
    setModel(model) {
        this.model = model;
        if (this.model != undefined) {
            this.animMixer = new THREE.AnimationMixer(this.model);
        }
    }
    getModel() {
        return this.model;
    }
    getModelClone() {
        var clone = this.model.clone();
        clone.animations = this.model.animations;
        return clone;
    }
    fbxSetPosition(position) {
        this.model.position.x = position.x;
        this.model.position.y = position.y;
        this.model.position.z = position.z;
    }
    fbxSetRotation(rotation) {
        this.model.rotation.x = THREE.Math.degToRad(rotation.x);
        this.model.rotation.y = THREE.Math.degToRad(rotation.y);
        this.model.rotation.z = THREE.Math.degToRad(rotation.z);
    }
}
class Mutex {
    constructor() {
        this.lock = false;
    }
    async acquire() {
        while (true) {
            if (this.lock === false) { break; }
            await sleep(3);
        }
        this.lock = true;
    }

    release() {
        this.lock = false;
    }
}

function sleep(ms) {
    return new Promise((r) => setTimeout(r, ms));
}

function loadFbx(path, prefabType) {
    return new Promise((resolve, reject) => {
        const loader = new THREE.FBXLoader();
        loader.load(path, function (object) {
            object.traverse(function (child) {
                if (child.isMesh) {
                    child.castShadow = false;
                    child.receiveShadow = false;
                }
            });
            object.prefabType = prefabType;
            resolve(object);
        });
    });
}
// #endregion

// #region extern func
function uuidv4() {
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    );
}
// #endregion