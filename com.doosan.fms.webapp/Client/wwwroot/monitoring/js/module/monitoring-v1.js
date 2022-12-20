
// #region html data
let renderBody;
// #endregino

// #region razor pass data
let razor;
let prefabDataContext;
let defaultPrefabDataContext;
// #endregion

// #region drawmode
let drawMode = false;
let prevCursor = null;
let plane, pointer, raycaster = false;
let isCtrlDown, isShiftDown, isAltDown = false;
const planeName = "ray_plane";
let raycasterObjects = [];
// #endregion

// #region three data
let camera, scene, renderer;
let gridHelper;
let ambientLight, directionalLight;
let orbit;

let testObject, testGeometry;

let uiOverlay = false;

let stop = false;
let frameCount = 0;
let fps, fpsInterval, startTime, now, then, elapsed;
let startTime2, now2, then2, elapsed2;
// #endregion

let worker;




// #region class
class PrefabContext {
    constructor() {
        this.dicPrefabs = {};
    }

    addPrefab(prefab) {
        this.dicPrefabs[prefab.prefabKey] = prefab;
    }

    getPrefab(key) {
        return this.dicPrefabs[key];
    }
}
class Prefab {
    constructor(type, key, useBox, path) {
        this.prefabType = type;
        this.prefabKey = key;
        this.prefabUseBox = useBox;
        this.prefabLoadPath = path;
        this.prefabModel = {};
    }
}

class RazorPrefabInterface {
    constructor(type, key, defaultPosition, position, rotate, bindKey) {
        this.RazorPrefabType = type;
        this.RazorKey = key;
        this.RazorDefaultPosition = defaultPosition;
        this.RazorPosition = position;
        this.RazorRotate = rotate;
        this.RazorBindKey = bindKey;
    }
}
// #endregion

// #region 3d func
function CreatePrefabFbx(eType, key, useBox, loadPath) {
    var prefab = new Prefab(eType, key, useBox, loadPath);
    var fbxLoader = new THREE.FBXLoader();
    fbxLoader.load(prefab.prefabLoadPath, function (fbx) {
        prefab.prefabModel = fbx;
        prefab.prefabModel.name = prefab.prefabKey;

        fbx.traverse(function (child) {
            if (child.isMesh) {
                child.castShadow = true;
                child.receiveShadow = true;
                if (child.name == 'box') {
                    fbx.boxMesh = child;
                }
            }
        });

    });

    return prefab;
}
// #endregion

// #region interop
export function disposeInterop() {
    stop = false;
    window.removeEventListener('resize', onWindowResize);
    document.removeEventListener('pointermove', onPointerMove);
    document.removeEventListener('pointerdown', onPointerDown);
    document.removeEventListener('keydown', onDocumentKeyDown);
    document.removeEventListener('keyup', onDocumentKeyUp);
    worker.terminate();

    worker = undefined;
    renderBody = undefined;
    razor = undefined;
    prefabDataContext = undefined;
    defaultPrefabDataContext = undefined;
    drawMode = undefined;
    prevCursor = undefined;
    plane = undefined;
    pointer = undefined;
    raycaster = undefined;
    isCtrlDown = undefined;
    isShiftDown = undefined;
    isAltDown = undefined;
    planeName = undefined;
    raycasterObjects = undefined;
    camera = undefined;
    scene = undefined;
    renderer = undefined;
    gridHelper = undefined;
    ambientLight = undefined;
    directionalLight = undefined;
    orbit = undefined;
    testObject = undefined;
    testGeometry = undefined;
    uiOverlay = undefined;
    stop = undefined;
    frameCount = undefined;
    fps = undefined;
    fpsInterval = undefined;
    startTime = undefined;
    now = undefined;
    then = undefined;
    elapsed = undefined;
    startTime2 = undefined;
    now2 = undefined;
    then2 = undefined;
    elapsed2 = undefined;

}


export function initInterop(dotNetObject) {
    if (dotNetObject == null || dotNetObject == undefined) {
        return false;
    }
    razor = dotNetObject;
    return true;
}


export function callAnimateInterop() {
    fps = 60;
    fpsInterval = 1000 / fps;
    then = Date.now();
    then2 = then;
    startTime = then;   
    startTime2 = then;
    animate();
    worker = new Worker('/monitoring/js/module/monitoring-v1-worker.js');
    worker.onmessage = (e) => {
        //razor.invokeMethodAsync('ReadDatasInvokable', e.data);
    }

    dataCollectWorker();
}


export function changeDrawModeInterop(isDraw) {
    drawMode = isDraw[0];
    gridHelper.visible = drawMode;

    if (prevCursor == null) {
        return true;
    }

    var loadedCursor = scene.getObjectByName(prevCursor.prefabKey, true);
    if (loadedCursor != null) {
        scene.remove(loadedCursor);
    }
    prevCursor = null;
    return true;
}

export function changeUIOverayInterop(isUi) {
    uiOverlay = isUi[0];
    return true;
}

export function drawModeCasterChangeInterop(castedItem) {
    var casted = JSON.parse(castedItem);
    if (drawMode == true) {
        var castedPrefab = defaultPrefabDataContext.dicPrefabs[casted.Key];
        if (castedPrefab == null) return;
        if (prevCursor == null) {
            prevCursor = castedPrefab;
            scene.add(castedPrefab.prefabModel);
            return;
        }

        if (prevCursor.prefabKey == castedPrefab.prefabKey) {
            return;
        }
        else {
            var loadedCursor = scene.getObjectByName(prevCursor.prefabKey, false);
            if (loadedCursor != null) {
                scene.remove(loadedCursor);
            }
            prevCursor = castedPrefab;
            scene.add(castedPrefab.prefabModel);
        }
    }
}

export function getDrawModeObjectsInterop() {
    var prefabArray = new Array();
    for (var ite = 0; ite < raycasterObjects.length; ite++) {
        var item = raycasterObjects[ite];
        if (item.name == planeName) continue;
        var bindkey = item.eBindKey;
        if (bindkey == undefined) bindkey = '';
        var radRotation = {};
        radRotation.x = THREE.Math.radToDeg(item.rotation.x);
        radRotation.y = THREE.Math.radToDeg(item.rotation.y);
        radRotation.z = THREE.Math.radToDeg(item.rotation.z);
        prefabArray.push(new RazorPrefabInterface(item.eType, item.name, item.defaultPosition, item.position, radRotation, item.eBindKey));
    }

    return JSON.stringify(prefabArray);
}

export function initMonitoringDataInterop(json) {
    try {
        var razorPrefabInterface = JSON.parse(json);
        for (var ite = 0; ite < razorPrefabInterface.length; ite++) {
            var iteItem = razorPrefabInterface[ite];
            var drawIteItem = defaultPrefabDataContext.dicPrefabs[iteItem.RazorPrefabType];
            if (iteItem.RazorPrefabType == 2) {
                if (iteItem.RazorBindKey == '01001') {
                    var aaaa = 0;
                }
            }
            if (drawIteItem == null) continue;
            var drawIteItemModel = drawIteItem.prefabModel.clone();
            drawIteItemModel.animMixer = new THREE.AnimationMixer(drawIteItemModel);
            drawIteItemModel.prevAction = undefined;
            drawIteItemModel.animations = drawIteItem.prefabModel.animations;
            drawIteItemModel.name = iteItem.RazorKey;
            drawIteItemModel.defaultPosition = {};
            drawIteItemModel.rotation.x = THREE.Math.degToRad(iteItem.RazorRotate.x);
            drawIteItemModel.rotation.y = THREE.Math.degToRad(iteItem.RazorRotate.y);
            drawIteItemModel.rotation.z = THREE.Math.degToRad(iteItem.RazorRotate.z);
            drawIteItemModel.defaultPosition.x = iteItem.RazorDefaultPosition.x;
            drawIteItemModel.defaultPosition.y = iteItem.RazorDefaultPosition.y;
            drawIteItemModel.defaultPosition.z = iteItem.RazorDefaultPosition.z;
            drawIteItemModel.position.x = iteItem.RazorPosition.x;
            drawIteItemModel.position.y = iteItem.RazorPosition.y;
            drawIteItemModel.position.z = iteItem.RazorPosition.z;
            drawIteItemModel.eType = iteItem.RazorPrefabType;
            drawIteItemModel.eBindKey = iteItem.RazorBindKey;
            scene.add(drawIteItemModel);
            raycasterObjects.push(drawIteItemModel);

            //if (drawIteItem.prefabUseBox == 'Y') {
            //    var boxItem = defaultPrefabDataContext.dicPrefabs[0];
            //    var drawBoxItem = boxItem.prefabModel.clone();
            //    drawBoxItem.name = (iteItem.RazorKey + 'BOX');
            //    drawBoxItem.position.copy(drawIteItemModel.position);
            //    drawBoxItem.position.divideScalar(10).floor().multiplyScalar(10)
            //    drawBoxItem.position.y = (drawBoxItem.position.y + 10);
            //    drawBoxItem.visible = false;
            //    scene.add(drawBoxItem);
            //    controlObjects.push(drawBoxItem);
            //}
        }
        return true;
    } catch (e) {
        console.log(e);
        return false;
    }
}

export function saveDrawModeSelectedItemPropInterop(json) {
    try {
        var rqItem = JSON.parse(json);
        var origin = rqItem[0];
        var change = rqItem[1];
        var loadedItem = scene.getObjectByName(origin.RazorKey);
        if (loadedItem == undefined) return false;
        loadedItem.name = change.RazorKey;
        loadedItem.eBindKey = change.RazorBindKey;
        loadedItem.rotation.x = THREE.Math.degToRad(change.RazorRotate.x);
        loadedItem.rotation.y = THREE.Math.degToRad(change.RazorRotate.y);
        loadedItem.rotation.z = THREE.Math.degToRad(change.RazorRotate.z);
        render();
        return true;
    }
    catch (e) {
        console.log(e);
        return false;
    }

}
// #endregion

// #region handler code
function onWindowResize() {
    camera.aspect = renderBody.offsetWidth / renderBody.offsetHeight;
    camera.updateProjectionMatrix();
    renderer.setSize(renderBody.offsetWidth, renderBody.offsetHeight);

    render();
}
// #endregion

// #region handler logic code
function onPointerMove(event) {
    pointer.set((event.offsetX / renderBody.offsetWidth) * 2 - 1, - (event.offsetY / renderBody.offsetHeight) * 2 + 1);
    raycaster.setFromCamera(pointer, camera);
    const intersects = raycaster.intersectObjects(raycasterObjects, true);
    if (intersects.length < 1) return;

    // #region drawmode
    if (drawMode == true && isCtrlDown == true) {
        // 선택된 객체가 있어야만 그릴수 있음
        if (prevCursor == null) return;

        //plane 위에만 그릴수 있음 (보류)
        const intersect = intersects[0];
        //if (intersect != plane) return;

        // prevCursor 포지션 변경
        var loadedCursor = scene.getObjectByName(prevCursor.prefabKey, false);
        if (loadedCursor != null) {
            loadedCursor.position.copy(intersect.point).add(intersect.face.normal);
            loadedCursor.position.divideScalar(10).floor().multiplyScalar(10).addScalar(0);
        }
    }
    // #endregion
    // #region drawmode == false
    else {

    }
    //#endregion
}

function onPointerDown(event) {
    pointer.set((event.offsetX / renderBody.offsetWidth) * 2 - 1, - (event.offsetY / renderBody.offsetHeight) * 2 + 1);
    raycaster.setFromCamera(pointer, camera);
    const intersects = raycaster.intersectObjects(raycasterObjects, true);
    if (intersects.length > 0) {
        const intersect = intersects[0];

        //#region drawmode
        if (drawMode == true) {
            //삭제
            if (isShiftDown) {
                if (intersect.object == plane) return;
                var intersectParent = intersect.object.parent;
                if (intersectParent == null || intersectParent == undefined) {
                    scene.remove(intersect.object);
                    raycasterObjects.splice(raycasterObjects.indexOf(intersect.object), 1);
                }
                else {
                    scene.remove(intersectParent);
                    raycasterObjects.splice(raycasterObjects.indexOf(intersectParent), 1);
                }
                render();
            }
            //추가
            else if (isCtrlDown) {
                if (prevCursor == null) return;
                var drawPrefab = defaultPrefabDataContext.dicPrefabs[prevCursor.prefabKey];
                var clonePrefabModel = drawPrefab.prefabModel.clone();
                clonePrefabModel.name = uuidv4();
                clonePrefabModel.eType = drawPrefab.prefabType;
                clonePrefabModel.eBindKey = '';
                scene.add(clonePrefabModel);
                raycasterObjects.push(clonePrefabModel);
                clonePrefabModel.position.copy(intersect.point).add(intersect.face.normal);
                clonePrefabModel.position.divideScalar(10).floor().multiplyScalar(10).addScalar(0);
            }
            //orbit
            else if (isAltDown) {
                return;
            }
            //상태변경
            else {
                if (event.button == 0 && isAltDown == false) {
                    if (intersect.object == plane) return;
                    var intersectParent = intersect.object.parent;
                    if (intersectParent == null || intersectParent == undefined) return;
                    var item = new RazorPrefabInterface(intersectParent.eType, intersectParent.name, intersectParent.position, intersectParent.position, intersectParent.rotation, intersectParent.eBindKey);
                    razor.invokeMethodAsync('ChangePropDrawModePrefabInvokable', JSON.stringify(item));
                }
            }
        }
        //#endregion
        //#region drawmode == false && uioverlay == true
        else if (uiOverlay == true) {
            if (isShiftDown) {
                return;
            }
            else if (isCtrlDown) {
                return;
            }
            else if (isAltDown) {
                return;
            }
            else {
                if (event.button == 0 && isAltDown == false) {
                    if (intersect.object == plane) return;
                    var intersectParent = intersect.object.parent;
                    if (intersectParent == null || intersectParent == undefined) return;
                    var item = new RazorPrefabInterface(intersectParent.eType, intersectParent.name, intersectParent.position, intersectParent.position, intersectParent.rotation, intersectParent.eBindKey);
                    razor.invokeMethodAsync('ChnagePropUIOverlayPrefabInvokable', JSON.stringify(item));
                }
            }
        }
        //#endregion
    }
}

function uuidv4() {
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    );
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

                if (prevCursor != null) {
                    var loadedCursor = scene.getObjectByName(prevCursor.prefabKey, false);
                    if (loadedCursor != null) {
                        loadedCursor.visible = true;
                    }
                }
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

                if (prevCursor != null) {
                    var loadedCursor = scene.getObjectByName(prevCursor.prefabKey, false);
                    if (loadedCursor != null) {
                        loadedCursor.visible = false;
                    }
                }

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

// #region initalize code
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
    if (initTestObject() == false) {
        console.log("Error :: initTestObject");
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
        console.log("Error :: AddDrawModePointer")
    }
    return true;
}
// #endregion asdf

// #region intialize item code
function initCamera() {
    camera = new THREE.PerspectiveCamera(45, renderBody.offsetWidth / renderBody.offsetHeight, 1, 10000);
    camera.position.set(500, 800, 1300);
    camera.lookAt(0, 0, 0);
}
function initScene() {
    scene = new THREE.Scene();
    scene.background = new THREE.Color(0xffffff);
}
function initGridHelper() {
    gridHelper = new THREE.GridHelper(1000, 100);
    gridHelper.position.x = -5;
    gridHelper.position.z = -5;
    var v = 0;
}
function initLight() {
    ambientLight = new THREE.AmbientLight(0x606060);
}
function initDirectionLight() {
    directionalLight = new THREE.DirectionalLight(0xffffff);
    directionalLight.position.set(1, 0.75, 0.75).normalize();
}
function initTestObject() {

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

    orbit.enableDamping = true; // an animation loop is required when either damping or auto-rotation are enabled
    orbit.dampingFactor = 0.05;

    orbit.screenSpacePanning = false;

    orbit.minDistance = 100;
    orbit.maxDistance = 500;

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
    if (testObject != undefined) {
        scene.add(testObject);
    }
}
function initListener() {
    window.addEventListener('resize', onWindowResize);
    document.addEventListener('pointermove', onPointerMove);
    document.addEventListener('pointerdown', onPointerDown);
    document.addEventListener('keydown', onDocumentKeyDown);
    document.addEventListener('keyup', onDocumentKeyUp);
}
function addDrawModePointer() {
    raycaster = new THREE.Raycaster();
    pointer = new THREE.Vector2();
    const geometry = new THREE.PlaneGeometry(1000, 1000);
    geometry.rotateX(- Math.PI / 2);
    plane = new THREE.Mesh(geometry, new THREE.MeshBasicMaterial({ visible: false, }));
    plane.name = planeName;
    scene.add(plane);
    var loadedPlane = scene.getObjectByName(planeName);
    loadedPlane.position.x = -5;
    loadedPlane.position.z = -5;
    raycasterObjects.push(plane);
}
// #endregion intialize item code



// #region rendering code
function animate() {
    if (stop == false) {
        requestAnimationFrame(animate);
    }
    now = Date.now();
    elapsed = now - then;
    if (elapsed > fpsInterval) {
        then = now - (elapsed % fpsInterval);

        //render code
        renderControl();
        renderMap();
        renderObject(elapsed);

        render();
    }
}

function dataCollectWorker() {

    if (stop == false) {
        worker.postMessage("https://localhost:44355/eqp-data/cv-data/get/ALL&ALL");
        setTimeout(dataCollectWorker, 1000);
    }
}

function renderControl() {
    if (orbit.enabled == true) {
        orbit.update();
    }
}
function renderMap() {

}
function renderObject(elapsed) {
    if (drawMode == false) {
        razor.invokeMethodAsync('CallRenderObjectInvokable', elapsed).then(data => {
            // #region statrt loop
            if (data == '') return;
            var razorData = JSON.parse(data)
            for (var ite = 0; ite < razorData.items.length; ite++) {
                var item = razorData.items[ite];
                var itemData = JSON.parse(item);
                var found = raycasterObjects.find(x => x.name == itemData.key);
                if (found == undefined) continue;
                var loadedItem = scene.getObjectByName(found.name)
                if (loadedItem != undefined) {
                    // #region 무브포지션
                    loadedItem.position.x = itemData.pos[0];
                    loadedItem.position.y = itemData.pos[1];
                    loadedItem.position.z = itemData.pos[2];
                    // #endregion

                    if (loadedItem.eBindKey == '01001' && loadedItem.eType == 1) {
                        var a = 0;
                    }

                    // #region anim
                    if (loadedItem.animMixer != undefined) {
                        if (loadedItem.prevAction == undefined) {
                            var foundAnim = loadedItem.animations.find(x => x.name == itemData.anim);
                            if (foundAnim != undefined) {
                                if (itemData.anim == 'default') {
                                    if (loadedItem.boxMesh != undefined) {
                                        loadedItem.boxMesh.visible = false;
                                    }
                                }
                                else {
                                    if (loadedItem.boxMesh != undefined) {
                                        loadedItem.boxMesh.visible = true;
                                    }
                                    var action = loadedItem.animMixer.clipAction(foundAnim, loadedItem);
                                    loadedItem.prevAction = action;
                                    loadedItem.prevAction.play();
                                    loadedItem.prevAction.AnimCode = itemData.anim;
                                }
                            }
                        }
                        else {
                            if (loadedItem.prevAction.AnimCode != itemData.anim) {
                                var foundAnim = loadedItem.animations.find(x => x.name == itemData.anim);
                                if (foundAnim != undefined) {
                                    if (itemData.anim == 'default') {
                                        if (loadedItem.boxMesh != undefined) {
                                            loadedItem.boxMesh.visible = false;
                                        }
                                        if (loadedItem.prevAction.isRunning()) {
                                            loadedItem.prevAction.stop();
                                        }
                                    }
                                    else {
                                        if (loadedItem.boxMesh != undefined) {
                                            loadedItem.boxMesh.visible = true;
                                        }
                                        var action = loadedItem.animMixer.clipAction(foundAnim, loadedItem);
                                        if (loadedItem.prevAction.isRunning()) {
                                            loadedItem.prevAction.stop();
                                        }
                                        loadedItem.prevAction = action;
                                        loadedItem.prevAction.play();
                                        loadedItem.prevAction.AnimCode = item.RazorMonitoringData.AnimCode;
                                    }
                                }
                            }
                        }

                        if (loadedItem.prevAction != undefined) {
                            loadedItem.animMixer.update((elapsed / 1000));
                        }
                    }
                }
                // #endregion
            }
            //#endregion
        });
    }
}
function render() {
    renderer.render(scene, camera);
}
// #endregion

