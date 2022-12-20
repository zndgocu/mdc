//#region variable
const __testMode = true;
const __entriesName = 0;
const __entriesValue = 1;
const __apiResultCode = {
    OK: 2
};

const __layerZMapBackground = parseFloat(0.1);
const __layerZMapNode = parseFloat(0.2);
const __layerZPrefab = parseFloat(0.3);
const __layerZPrefabNode = parseFloat(0.4);


//const
//origin = [16065.055, 32876.998] # Origin point(bottom - left corner)
//resolution = 0.026737.# Image resolution
//scale = 0.05 # Resize scale
//margin = 5
//const __mps = parseFloat(0.0266925);
//const __resolution = parseFloat(0.01);
const __mps = parseFloat(0.018384);
const __resolution = parseFloat(0.05);

//dom
const __canvasName = 'render-body';

const __keyResultCode = { ERR: 0, NONE: 1, OK: 2 };
const __colors = {
    Normal: 0x333333,
    Gray: 0xa2a2a2,
    Red: 0xff0000,
    Green: 0x07e705,
    Blue: 0x0d0fe7,
    Purple: 0xe70cc9,
    Skyblue: 0x31e7e4,
    Black: 0x000000,
    White: 0xffffff,
};

const __keyFont = { KR: 0, EN: 1 };
const __pathFont = ["./font/font-factory/kr/NotoSansKR.json", "./font/font-factory/kr/NotoSansKR.json"];

const __keyMapObject = { VERTEX: 0, LANE: 1 };
const __pathMapObject = "./monitoring/mon_data/yaml/topological_map.yaml";
const __invokeMapObject = "GetYamlToString";

const __pathBackground = "./monitoring/mon_data/png/topological_map.png";

const __keyPrefab = { ROBOT: 0 };
const __pathPrefab = ["./monitoring/model/fbx/robot.fbx"];
const __urlPrefab = ["https://localhost:44384/api/RobotState/robot-state/get"];

const __keyWorker = { ROBOT: 0 };
const __pathWorker = ["./js/fms_mon/worker/robot-state.js"];

const __canvasColor = 0x000000;
const __mapItemColor = 0xff0000;
const __mapBackgroundColor = 0xffffff;


let manager = undefined;
//#endregion --variable


//#region export func
export async function CallLoad(dotNetRef) {
    Object.freeze(__apiResultCode);

    Object.freeze(__keyResultCode);
    Object.freeze(__keyMapObject);
    Object.freeze(__keyFont);
    Object.freeze(__keyPrefab);
    Object.freeze(__keyWorker);
    Object.freeze(__colors);

    manager = new Manager();

    //#region dotNet
    manager.setDotNet(dotNetRef);
    //#endregion

    //#region dom
    var renderBody = getRenderBody(__canvasName);
    if (renderBody == undefined) {
        LogForce('*1');
        return;
    }
    manager.setRenderBody(getRenderBody(__canvasName));
    //#endregion

    //#region three renderObject
    var threeRenderObject = new ThreeRenderObject(__testMode, renderBody);
    manager.setThreeRenderObject(threeRenderObject);
    if (manager.initializeThreeRenderObject() == false) {
        LogForce('*99999');
        return;
    }
    //#endregion

    //#region const
    manager.setMps(__mps);
    manager.setResolution(__resolution);
    manager.setKeyResultCode(__keyResultCode);
    manager.setColors(__colors);
    //#endregion


    //#region reposConfig
    var repoConfigFont = new RepositoryConfig();
    repoConfigFont.setKeys(__keyFont);
    repoConfigFont.setPaths(__pathFont);
    manager.setRepoConfigFont(repoConfigFont);

    var repoConfigMapObject = new RepositoryConfig();
    repoConfigMapObject.setKeys(__keyMapObject);
    repoConfigMapObject.setPaths(__pathMapObject);
    repoConfigMapObject.setInvoke(__invokeMapObject);
    manager.setRepoConfigMapObject(repoConfigMapObject);

    var repoConfigBackground = new RepositoryConfig();
    repoConfigBackground.setPaths(__pathBackground);
    manager.setRepoConfigMapBackground(repoConfigBackground);

    var repoConfigPrefab = new RepositoryConfig();
    repoConfigPrefab.setKeys(__keyPrefab);
    repoConfigPrefab.setPaths(__pathPrefab);
    repoConfigPrefab.setUrls(__urlPrefab);
    manager.setRepoConfigPrefab(repoConfigPrefab);

    var repoConfigWorker = new RepositoryConfig();
    repoConfigWorker.setKeys(__keyWorker);
    repoConfigWorker.setPaths(__pathWorker);
    manager.setRepoConfigWorker(repoConfigWorker);
    //#endregion

    //#region repo
    manager.setRepoFont(new FontRepository());
    if (await manager.initializeFontRepository() == false) {
        LogForce('*2');
        return;
    }

    manager.setRepoMapObject(new MapObjectRepository());
    if (await manager.initializeMapObjectRepository() == false) {
        LogForce('*3');
        return;
    }

    manager.setRepoMapBackground(new MapBackgroundRepository());
    if (await manager.initializeBackgroundRepository() == false) {
        LogForce('*4');
        return;
    }

    manager.setRepoPrefab(new PrefabRepository());
    if (await manager.initializePrefabRepository() == false) {
        LogForce('*5');
        return;
    }

    manager.setRepoWorker(new WorkerRepository());
    if (await manager.initializeWorkerRepository() == false) {
        LogForce('*6');
        return;
    }

    if (await manager.generateFontRepo() == false) {
        LogForce('*7');
        return;
    }
    if (await manager.generateMapObjectRepo() == false) {
        LogForce('*9');
        return;
    }
    if (await manager.generateMapBackgroundRepo() == false) {
        LogForce('*10');
        return;
    }
    if (await manager.generatePrefabRepo() == false) {
        LogForce('*11');
        return;
    }
    //if (await manager.generateWokrerRepo() == false) {
    //    LogForce('*12');
    //    return;
    //}
    if (await manager.firstLoad() == false) {
        LogForce('*13');
        return;
    }
    if (await manager.firstDraw() == false) {
        LogForce('*14');
        return;
    }

    //#region

    //#region anim start
    var fpsCounter = new FpsCounter(60);
    manager.setFpsCounter(fpsCounter);

    if (await manager.firstWorker() == false) {
        LogForce('*15');
        return;
    }

    //#endregion
    await animate();
}

async function animate() {
    requestAnimationFrame(animate);
    manager.fpsCounter.now = Date.now();
    manager.fpsCounter.elapsed = manager.fpsCounter.now - manager.fpsCounter.then;
    if (manager.fpsCounter.elapsed > manager.fpsCounter.fpsInterval) {
        manager.fpsCounter.then = manager.fpsCounter.now - (manager.fpsCounter.elapsed % manager.fpsCounter.fpsInterval);
        var changedData = await manager.getCloneMutexDataAll();
        manager.render();
        manager.prefabRender(changedData);
        manager.requestWorker(changedData);
    }
}


//#region static Func
function getRenderBody() {
    return document.getElementById('render-body');
}

function LogForce(data) {
    console.log(data);
}

async function asyncFetch(requestUrl) {
    var result = {
        success: false,
        data: undefined
    }
    try {
        var response = await fetch(requestUrl, {
            method: "GET",
        });
        var res = await response.json();
        if (res.statCode != __apiResultCode.OK) {
            throw 'response statecode not ok';
        }
        result.success = true;
        result.data = res.content;
        return result;
    } catch (e) {
        result.success = false;
        result.data = e.message;
        return result;
    }
}
//#endregion



//#region testCode
class FontRepository {
    constructor() {
        this.items = {};
        this.config = undefined;
        this.prevItemKey = undefined;
    }
    getPrevItem() {
        return this.items[this.prevItemKey];
    }
    getItem(key) {
        return this.items[key];
    }
    async initializeFontRepository(config) {
        if (config == undefined) {
            return false;
        }
        this.config = config;
        return true;
    }
    async getFont(loader, path) {
        return new Promise((resolve, reject) => {
            loader.load(path, function (font) {
                resolve(font);
            });
        });
    }
    async generateFonts() {
        //#region getFonts
        var loader = new THREE.FontLoader();
        var first = true;
        for (var ite = 0; ite < Object.entries(this.config.keys).length; ite++) {
            var font = await this.getFont(loader, this.config.paths[Object.values(this.config.keys)[ite]]);
            if (font == undefined) continue;
            if (first == true) {
                this.prevItemKey = Object.values(this.config.keys)[ite];
                first = false;
            }
            this.items[Object.values(this.config.keys)[ite]] = font;
        }
        //#endregion

        return (Object.entries(this.items).length > 0);
    }
}

class PanelLabel {
    constructor() {
        this.id = undefined;
        this.floor = undefined;

        this.layerZ = parseFloat(0);
        this.modelComponent = undefined;
    }
    setModelComponent(modelComponent) {
        this.modelComponent = modelComponent;
    }
    getModelComponent() {
        return this.modelComponent;
    }
    setLayerZ(z) {
        this.layerZ = z;
    }

    async generate(filePath) {
        this.floor = await this.loadSVG(filePath);
        this.id = uuidv4();
    }
    loadSVG(filePath) {
        return new Promise((resolve, reject) => {
            const loader = new THREE.SVGLoader();
            loader.load(filePath, function (data) {
                var paths = data.paths;
                var group = new THREE.Group();
                for (let i = 0; i < paths.length; i++) {
                    var path = paths[i];
                    var material = new THREE.MeshBasicMaterial({
                        color: path.color,
                        side: THREE.DoubleSide,
                        depthWrite: false
                    });
                    var shapes = THREE.SVGLoader.createShapes(path);
                    for (let j = 0; j < shapes.length; j++) {
                        const shape = shapes[j];
                        const geometry = new THREE.ShapeGeometry(shape);
                        const mesh = new THREE.Mesh(geometry, material);
                        group.add(mesh);
                    }
                }
                resolve(group);
            });
        });
    }
}

class Prefab {
    firstRender(data, resolution, mps, multiplyValue, keys) {
        if (this.modelComponent == undefined) return;
        if (this.objectType == keys.ROBOT) {
            this.modelComponent.setScale(new THREE.Vector3(1 * mps * multiplyValue, 1 * mps * multiplyValue, 1 * mps * multiplyValue));
            this.modelComponent.setPosition
                (
                    new THREE.Vector3(
                        parseFloat(data.poseX)
                        , parseFloat(data.poseY)
                        , parseFloat(data.poseZ))
            );
            this.modelComponentLabel.setScale(new THREE.Vector3(1 * mps * multiplyValue, 1 * mps * multiplyValue, 1 * mps * multiplyValue));
            this.modelComponentLabel.setPosition
                (
                    new THREE.Vector3(
                        parseFloat(data.poseX)
                        , parseFloat(data.poseY)
                        , parseFloat(data.poseZ + 1))
                );
        }
    }
    clone() {
        var clonable = new Prefab();
        if (this.name != undefined) {
            clonable.name = this.name;
        }
        if (this.objectType != undefined) {
            clonable.objectType = this.objectType;
        }
        if (this.startPoint != undefined) {
            clonable.startPoint = new THREE.Vector3(this.startPoint.x, this.startPoint.y, this.startPoint.z);
        }
        if (this.endPoint != undefined) {
            clonable.endPoint = new THREE.Vector3(this.endPoint.x, this.endPoint.y, this.endPoint.z);
        }
        if (this.path != undefined) {
            clonable.path = this.path;
        }
        if (this.fbx != undefined) {
            clonable.fbx = this.fbx; //포인터연결 리포지토리 데이터는 지워지지 않음 괜춘
        }
        return clonable;
    }
    constructor() {
        this.name = undefined;
        this.objectType = undefined;
        this.startPoint = undefined;
        this.endPoint = undefined;
        this.path = undefined;
        this.fbx = undefined;
        this.layerZ = parseFloat(0);
        this.label = undefined;

        //non clone object
        this.modelComponent = undefined;
    }
    setLabel(label) {
        this.label = label;
    }
    setLayerZ(z) {
        this.layerZ = z;
    }
    setName(name) {
        this.name = name;
    }
    setObjectType(typ) {
        this.objectType = typ;
    }
    setStartPoint(sPoint) {
        this.startPoint = sPoint;
    }
    setEndPoint(ePoint) {
        this.endPoint = ePoint;
    }
    setFbx(fbx) {
        this.fbx = fbx;
    }
    setPath(path) {
        this.path = path;
    }
    async generateModelComonent(keys) {
        try {
            if (keys == undefined || this.objectType == undefined) return undefined;
            var component = new ModelComponent();
            component.setId(this.name);
            component.setItem(this.fbx.clone());
        } catch (e) {
            console.log(e);
            return undefined;
        }

        this.setModelComponent(component);
        return this.modelComponent;
    }

    setModelComponent(component) {
        this.modelComponent = component;
    }
    getModelComponent() {
        return this.modelComponent;
    }

    async generateFbx() {
        var fbx = await this.loadFbx(this.path);
        if (fbx == undefined) return false;
        this.setFbx(fbx);
        return true;
    }
    


    getFbx() {
        return this.fbx;
    }
    loadFbx(path) {
        return new Promise((resolve, reject) => {
            const loader = new THREE.FBXLoader();
            loader.load(path, function (object) {
                object.traverse(function (child) {
                    if (child.isMesh) {
                        child.castShadow = true;
                        child.receiveShadow = true;
                    }
                });
                resolve(object);
            });
        });
    }
    render(data, keys, elapsed, speed, cameraRotate) {

        var modelComponent = this.getModelComponent();
        if (modelComponent == undefined) return;

        if (this.objectType == keys.ROBOT) {
            //#region pose
            var prevPosition = modelComponent.item.position;
            if (data.poseX == 90) {
                var testPosition = 0;
            }

            var directionX = (prevPosition.x > data.poseX) ? -1 : 1;
            var directionY = (prevPosition.y > data.poseY) ? -1 : 1;
            var directionZ = (prevPosition.z > data.poseZ) ? -1 : 1;

            var minimizeDistance = 10;
            var distanceX = Math.abs(prevPosition.x - data.poseX);
            if (distanceX < minimizeDistance) distanceX = minimizeDistance;
            var distanceY = Math.abs(prevPosition.y - data.poseY);
            if (distanceY < minimizeDistance) distanceY = minimizeDistance;
            var distanceZ = Math.abs(prevPosition.z - data.poseZ);
            if (distanceZ < minimizeDistance) distanceZ = minimizeDistance;

            var fpsDriveSpeed = ((speed / 60 / 360) * elapsed) * __mps * __resolution;

            var updateX = prevPosition.x + ((distanceX * fpsDriveSpeed) * directionX);
            if (directionX > 0) {
                if (updateX > data.poseX) {
                    updateX = data.poseX;
                }
            } else {
                if (updateX < data.poseX) {
                    updateX = data.poseX;
                }
            }
            var updateY = prevPosition.y + ((distanceY * fpsDriveSpeed) * directionY);
            if (directionY > 0) {
                if (updateY > data.poseY) {
                    updateY = data.poseY;
                }
            } else {
                if (updateY < data.poseY) {
                    updateY = data.poseY;
                }
            }
            var updateZ = prevPosition.z + ((distanceZ * fpsDriveSpeed) * directionZ);
            if (directionZ > 0) {
                if (updateZ > data.poseZ) {
                    updateZ = data.poseZ;
                }
            } else {
                if (updateZ < data.poseZ) {
                    updateZ = data.poseZ;
                }
            }
            modelComponent.setPosition(new THREE.Vector3(updateX, updateY, this.layerZ));

            //label
            if (this.label != undefined) {
                var labelComponent = this.label.getModelComponent();
                if (labelComponent != undefined) {
                    labelComponent.setPosition(new THREE.Vector3(updateX, updateY, this.label.layerZ));
                    labelComponent.item.rotation.x = cameraRotate.x;
                    labelComponent.item.rotation.y = cameraRotate.y;
                    labelComponent.item.rotation.z = cameraRotate.z;
                }
            }
            //#endregion


            //#region rot
            //euler 반시계 방향
            elapsed = 43.56982421875;
            var prevRotationValue = THREE.Math.radToDeg(modelComponent.item.rotation.z);
            var dataRotationValue = data.pose_theta;
            if (dataRotationValue == 1) {
                var test = 0;
            }
            var distanceTemp = 360 - (prevRotationValue - dataRotationValue);
            var distanceClockWise = (distanceTemp == 0) ? 0 : (distanceTemp % 360);
            var distanceTemp = 360 - (dataRotationValue - prevRotationValue);
            var distanceCounterClockWise = (distanceTemp == 0) ? 0 : (distanceTemp % 360);
            if (distanceClockWise > 0 || distanceCounterClockWise > 0) {
                var rotationDirection = (distanceCounterClockWise > distanceClockWise) ? 1 : -1;
                var selectRotationValue = (rotationDirection < 0) ? distanceCounterClockWise : distanceClockWise;

                var fpsAngleSpeed = ((speed / 60 / 360) * elapsed) * __mps * __resolution;
                var addRotationValue = (selectRotationValue * fpsAngleSpeed) * rotationDirection;
                var updateRotationValue = (prevRotationValue + addRotationValue) % 360;
                //각도 목표치 근사 시 제자리값 변환할것
                this.modelComponent.item.rotation.z = THREE.Math.degToRad(updateRotationValue);
                //this.modelComponent.setRotateZ(THREE.Math.degToRad(updateRotationValue));
            }
            //#endregion



        }
    }
}



class PrefabRepository {
    render(changedData, elapsed, speed, cameraRotate) {
        for (const [itemKey, items] of Object.entries(changedData)) {
            var prefabs = this.loadedPrefab[itemKey];
            if (prefabs == undefined) continue;
            if (items.Data == undefined) continue;
            for (var ite = 0; ite < items.Data.length; ite++) {
                var prefab = prefabs[items.Data[ite].robotId];
                if (prefab == undefined) continue;
                prefab.render(items.Data[ite], this.config.keys, elapsed, speed, cameraRotate);
            }
        }
    }
    getModelComponentItem() {
        var resultItems = [];
        for (const [itemKey, items] of Object.entries(this.loadedPrefab)) {
            for (const [itemName, item] of Object.entries(items)) {
                resultItems.push(item.modelComponent.item);
            }
        }
        return resultItems;
    }
    async firstLoad(dotNet, scene) {
        //prefab gen
        for (const [itemKey, items] of Object.entries(this.config.keys)) {
            try {
                var prefab = this.items[this.config.keys[itemKey]];
                if (prefab == undefined) throw 'undefined prefab';
                if (await prefab.generateFbx() == false) {
                    throw 'load fail prefab : '.concat(itemKey);
                }

                var callUrl = this.config.urls[items];
                if (callUrl == undefined) continue;
                var datas = await asyncFetch(callUrl);
                if (datas == undefined) throw 'fetch fail';
                if (datas.success == false) throw datas.data;
                var prefabData = datas.data;
                this.firstData[prefab.objectType] = prefabData;

                if (prefab.objectType == this.config.keys.ROBOT) {
                    this.loadedPrefab[prefab.objectType] = {};
                    for (var ite = 0; ite < prefabData.length; ite++) {
                        var clonePrefab = prefab.clone();
                        clonePrefab.setName(prefabData[ite].robotId);
                        var component = await clonePrefab.generateModelComonent(this.config.keys);
                        if (component == undefined) return false;
                        scene.add(component.item);
                        this.loadedPrefab[prefab.objectType][clonePrefab.name] = clonePrefab;
                        prefab.setLayerZ(__layerZPrefab);

                        //label code
                        var label = new PanelLabel();
                        await label.generate('./monitoring/model/svg/label.svg');
                        var labelComponent = new ModelComponent();
                        labelComponent.setId(label.id);
                        labelComponent.setItem(label.floor);
                        label.setLayerZ(__layerZPrefabNode);
                        label.setModelComponent(labelComponent);
                        this.loadedPrefab[prefab.objectType][clonePrefab.name].modelComponentLabel = labelComponent;
                        this.loadedPrefab[prefab.objectType][clonePrefab.name].label = label;
                        scene.add(this.loadedPrefab[prefab.objectType][clonePrefab.name].modelComponentLabel.item);
                    }
                }
                else {
                    throw 'undefined prefab';
                }
            } catch (e) {
                console.log(e);
                continue;
            }
        }
    }
    async firstDraw(mps, resolution) {
        for (const [itemKey, items] of Object.entries(this.config.keys)) {
            var datas = this.firstData[items];
            if (datas == undefined) continue;
            var joinedPrefab = this.loadedPrefab[items];
            if (joinedPrefab == undefined) continue;
            for (var ite = 0; ite < datas.length; ite++) {
                var prefab = joinedPrefab[datas[ite].robotId];
                if (prefab == undefined) continue;
                prefab.firstRender(datas[ite], mps, resolution, parseFloat(0.5), this.config.keys);
            }
        }
    }
    constructor() {
        this.items = {};
        this.loadedPrefab = {};
        this.firstData = {};
        this.config = undefined;
    }
    async initializePrefabRepository(config) {
        this.config = config;
        return true;
    }
    async generatePrefab() {
        try {
            for (const [itemKey, items] of Object.entries(this.config.keys)) {
                var prefab = new Prefab();
                prefab.setName(itemKey);
                prefab.setObjectType(this.config.keys[itemKey]);
                prefab.setStartPoint(new THREE.Vector3(0, 0, 0));
                prefab.setPath(this.config.paths[this.config.keys[itemKey]]);
                this.items[this.config.keys[itemKey]] = prefab;
            }
        } catch (e) {
            console.log(e);
            return false;
        }
    }
}

class MapObject {
    constructor() {
        this.name = undefined;
        this.objectType = undefined;
        this.startPoint = undefined;
        this.endPoint = undefined;
        this.modelComponent = undefined;
        this.texTile = undefined;

        this.layerZ = parseFloat(0);
    }
    setLayerZ(z) {
        this.layerZ = z;
    }
    setTextile(textile) {
        this.texTile = undefined;
    }
    setName(name) {
        this.name = name;
    }
    setObjectType(typ) {
        this.objectType = typ;
    }
    setStartPoint(sPoint) {
        this.startPoint = sPoint;
    }
    setEndPoint(ePoint) {
        this.endPoint = ePoint;
    }
    setModelComponent(modelComponent) {
        this.modelComponent = modelComponent;
    }
    generateModelComonent(keys, layerZ) {
        if (keys == undefined || this.objectType == undefined) return undefined;
        var component = new ModelComponent();
        if (this.objectType == keys.VERTEX) {
            var radius = parseFloat(10),
                segments = 32,
                mat = new THREE.LineBasicMaterial({ color: 0xff0000 }),
                geo = new THREE.CircleGeometry(radius, segments);
            var circle = new THREE.Line(geo, mat);

            component.setId(this.name);
            component.setItem(circle);
        }
        else if (this.objectType == keys.LANE) {
            const mat = new THREE.LineBasicMaterial({ color: 0xff0000 });
            var drawVector3Arr = [];
            drawVector3Arr.push(
                new THREE.Vector3(this.startPoint.x, this.startPoint.y, layerZ)
                , new THREE.Vector3(this.endPoint.x, this.endPoint.y, layerZ
                )
            );
            var geo = new THREE.BufferGeometry().setFromPoints(drawVector3Arr);
            var lane = new THREE.Line(geo, mat);

            component.setId(this.name);
            component.setItem(lane);
        }
        else {
            return undefined;
        }
        this.setModelComponent(component);
        return this.modelComponent;
    }
    getModelComponent() {
        return this.modelComponent;
    }
}


class TexTile {
    constructor() {
        this.name = undefined;
        this.modelComponent = undefined;
        this.layerZ = parseFloat(0);
    }
    setLayerZ(z) {
        this.layerZ = parseFloat(z);
    }
    setName(name) {
        this.name = name;
    }
    setModelComponent(modelComponent) {
        this.modelComponent = modelComponent;
    }
    getModelComponent() {
        return this.modelComponent;
    }
    genName() {
        return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
            (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
        );
    }
    generateTextile(font, fontSize, text) {
        var defaultPos = {
            x: 1,
            y: 1,
            z: this.layerZ
        };
        var fixedFontHeight = 0.3;
        var texts = text.split('\r\n');
        var maxWidth = 1;
        var maxHeight = ((texts.length - 1) < 1) ? 1 : (texts.length - 1);
        for (var ite = 0; ite < maxHeight; ite++) {
            if (texts[ite].length > maxWidth) {
                maxWidth = texts[ite].length;
            }
        }
        var textGeo = new THREE.TextGeometry(text, {
            font: font,
            size: fontSize,
            height: fixedFontHeight,
        });
        var textMat = new THREE.MeshBasicMaterial({ color: 0x1b1e23 });
        var textMesh = new THREE.Mesh(textGeo, textMat);
        textMesh.position.set(defaultPos.x - (maxWidth * 0.5)
            , defaultPos.y + (maxHeight * 0.5)
            , defaultPos.z);

        var planeGeo = new THREE.BoxGeometry(maxWidth, (maxHeight * 2) + 1, fixedFontHeight);
        var planeMat = new THREE.MeshBasicMaterial({ color: 0xfffdfa, transparent: true });
        //planeMat.opacity = opacity;
        var planeMesh = new THREE.Mesh(planeGeo, planeMat);

        var edgeGeo = new THREE.EdgesGeometry(planeGeo);
        var edgeMesh = new THREE.LineSegments(edgeGeo, new THREE.LineBasicMaterial({ color: 0xff0000 }));

        var groupMesh = new THREE.Group();
        groupMesh.add(textMesh);
        groupMesh.add(planeMesh);
        groupMesh.add(edgeMesh);

        this.setName(this.genName());

        var modelComponent = new ModelComponent();
        modelComponent.setId(this.name);
        modelComponent.setItem(groupMesh);
        this.setModelComponent(modelComponent);
        return this.modelComponent;
    }
}

class MapObjectRepository {
    getModelComponentItem() {
        var modelComponentObjects = [];
        for (const [itemKey, items] of Object.entries(this.config.keys)) {
            for (var ite = 0; ite < Object.entries(this.items[items]).length; ite++) {
                var mapObject = this.items[items][ite];
                if (mapObject.modelComponent == undefined) continue;
                var item = mapObject.modelComponent.getItem();
                if (item == undefined) continue;
                modelComponentObjects.push(item);
            }
        }
        return modelComponentObjects;
    }

    async firstLoad(scene, font) {
        for (const [itemKey, items] of Object.entries(this.config.keys)) {
            //if for loop cost 감소 일부러 씀
            if (this.config.keys[itemKey] == this.config.keys.VERTEX) {
                for (var ite = 0; ite < Object.entries(this.items[items]).length; ite++) {
                    var mapObject = this.items[items][ite];
                    if (mapObject == undefined) continue;
                    var mapObjectComponent = mapObject.generateModelComonent(this.config.keys, this.layerZ);
                    scene.add(mapObjectComponent.item);

                    var texTile = new TexTile();
                    var fontSize = 10;
                    var text = 'node:'.concat(ite.toString()).concat('\r\n')
                        .concat('x:').concat(mapObject.startPoint.x.toString().substr(0, 4)).concat('\r\n')
                        .concat('y:').concat(mapObject.startPoint.y.toString().substr(0, 4)).concat('\r\n');
                    var texTileComponent = texTile.generateTextile(font, fontSize, text);
                    var item = texTileComponent.getItem();
                    mapObject.texTile = texTile;
                    mapObject.setLayerZ(__layerZMapNode);
                    scene.add(item);
                }
            }
            else if (this.config.keys[itemKey] == this.config.keys.LANE) {
                for (var ite = 0; ite < Object.entries(this.items[items]).length; ite++) {
                    var mapObject = this.items[items][ite];
                    if (mapObject == undefined) continue;
                    var mapObjectComponent = mapObject.generateModelComonent(this.config.keys, this.layerZ);
                    mapObject.setLayerZ(__layerZMapNode);
                    scene.add(mapObjectComponent.item);
                }
            }
            else {
                console.log('unknown type : '.concat(this.config.keys[itemKey]));
                continue;
            }
        }

        return true;
    }
    async firstDraw(mps, resolution, font) {
        for (const [itemKey, items] of Object.entries(this.config.keys)) {
            if (this.config.keys[itemKey] == this.config.keys.VERTEX) {
                for (var ite = 0; ite < Object.entries(this.items[items]).length; ite++) {
                    var mapObject = this.items[items][ite];
                    var mapObjectComponent = mapObject.getModelComponent();
                    mapObjectComponent.setScale(new THREE.Vector3(resolution, resolution, this.layerZ * resolution));
                    mapObjectComponent.setPosition(
                        new THREE.Vector3(
                            mapObject.startPoint.x
                            , mapObject.startPoint.y
                            , this.layerZ)
                    );

                    var texTileObject = mapObject.texTile;
                    if (texTileObject == undefined) continue;
                    var texTileComponentT = texTileObject.getModelComponent();
                    if (texTileComponentT == undefined) continue;
                    texTileComponentT.setScale(new THREE.Vector3(resolution, resolution, this.layerZ * resolution));
                    texTileComponentT.setPosition(
                        new THREE.Vector3(
                            mapObject.startPoint.x
                            , mapObject.startPoint.y
                            , this.layerZ)
                    );
                    //여기까지 컴포넌트 위치이동 스케일
                }
            }
            else if (this.config.keys[itemKey] == this.config.keys.LANE) {
                var mapObject = this.items[items][ite];
                var mapObjectComponent = mapObject.getModelComponent();
                mapObjectComponent.setScale(new THREE.Vector3(resolution, resolution, this.layerZ * resolution));
                mapObjectComponent.setPosition(
                    new THREE.Vector3(
                        mapObject.startPoint.x
                        , mapObject.startPoint.y
                        , this.layerZ)
                );
            }
            else {
                console.log('unknown type : '.concat(this.config.keys[itemKey]));
                continue;
            }
        }
        return true;
    }
    constructor() {
        this.items = {};
        this.config = undefined;
        this.layerZ = parseFloat(__layerZMapNode);
    }
    async initializeMapObjectRepository(config) {
        this.config = config;
        return true;
    }
    async generateMapObjects(dotNet) {
        try {
            var yamlString = await dotNet.invokeMethodAsync(this.config.invokes, this.config.paths);
            var yamlJson = JSON.parse(yamlString);

            var vertexAttrAxisX = 0;
            var vertexAttrAxisY = 1;
            var vertexAttrAttrExt = 2;

            for (const [itemKey, items] of Object.entries(this.config.keys)) {
                this.items[this.config.keys[itemKey]] = {};
                if (this.config.keys[itemKey] == this.config.keys.VERTEX) {
                    for (var ite = 0; ite < yamlJson.levels.L1.vertices.length; ite++) {
                        var axisX = parseFloat(yamlJson.levels.L1.vertices[ite][vertexAttrAxisX]);
                        var axisY = parseFloat(yamlJson.levels.L1.vertices[ite][vertexAttrAxisY]);

                        var mapObject = new MapObject();
                        mapObject.setName(ite);
                        mapObject.setObjectType(this.config.keys[itemKey]);
                        mapObject.setStartPoint(new THREE.Vector3(axisX, axisY, this.layerZ));
                        this.items[this.config.keys[itemKey]][mapObject.name] = mapObject;
                    }
                }
                else if (this.config.keys[itemKey] == this.config.keys.LANE) {
                    for (var ite = 0; ite < yamlJson.levels.L1.lanes.length; ite++) {
                        var startAxis = this.items[this.config.keys.VERTEX][yamlJson.levels.L1.lanes[ite][0]].startPoint;
                        var endAxis = this.items[this.config.keys.VERTEX][yamlJson.levels.L1.lanes[ite][1]].startPoint;

                        var mapObject = new MapObject();
                        mapObject.setName(ite);
                        mapObject.setObjectType(this.config.keys[itemKey]);
                        mapObject.setStartPoint(new THREE.Vector3(startAxis.x, startAxis.y, this.layerZ));
                        mapObject.setEndPoint(new THREE.Vector3(endAxis.x, endAxis.y, this.layerZ));
                        this.items[this.config.keys[itemKey]][mapObject.name] = mapObject;
                    }
                }
                else {
                    throw 'unknown type : '.concat(this.config.keys[itemKey]);
                }
            }
            return true;
        } catch (e) {
            console.log(e);
            return false;
        }
    }
}

class MapBackground {
    constructor() {
        this.name = undefined;
        this.objectType = undefined;
        this.startPoint = undefined;
        this.endPoint = undefined;
        this.modelComponent = undefined;

        this.layerZ = parseFloat(0);
    }
    setLayerZ(z) {
        this.layerZ = z;
    }
    setName(name) {
        this.name = name;
    }
    setObjectType(typ) {
        this.objectType = typ;
    }
    setStartPoint(sPoint) {
        this.startPoint = sPoint;
    }
    setEndPoint(ePoint) {
        this.endPoint = ePoint;
    }
    setModelComponent(model) {
        this.modelComponent = model;
    }
    getModelComponent() {
        return this.modelComponent;
    }

    async generateModelComonent(path) {
        try {
            var png = await this.getMapPng(path);
            var texture = new THREE.CanvasTexture(png);
            var mat = new THREE.MeshBasicMaterial({ color: 0xffffff, map: texture });
            var geo = new THREE.BoxGeometry(texture.image.width, texture.image.height, 1);
            var mapPng = new THREE.Mesh(geo, mat);
            var component = new ModelComponent();
            component.setId(this.name);
            component.setItem(mapPng);
            this.setModelComponent(component);
            return this.modelComponent;
        } catch (e) {
            console.log(e);
            return undefined;
        }
    }
    async getMapPng(mapFilePathPng) {
        var loader = new THREE.ImageLoader();
        loader.setCrossOrigin('*');
        var loadedImage = undefined;
        await this.getImage(loader, mapFilePathPng).then((image) => {
            loadedImage = image;
        });
        return loadedImage;
    }
    getImage(loader, path) {
        return new Promise((resolve, reject) => {
            loader.load(path, function (image) {
                resolve(image);
            });
        });
    }
}
class ModelComponent {
    constructor() {
        this.id = undefined;
        this.item = undefined;
    }
    getItem() {
        return this.item;
    }
    setItem(item) {
        this.item = item;
    }
    setId(id) {
        this.id = id;
    }
    setScale(v) {
        this.item.scale.set(v.x, v.y, v.z);
    }
    setPosition(v) {
        this.item.position.set(v.x, v.y, v.z);
    }
    setPositionZ(z) {
        this.item.position.z = z;
    }
    setRotateX(x) {
        this.item.rotateX(x);
    }
    setRotateY(y) {
        this.item.rotateY(y);
    }
    setRotateZ(z) {
        this.item.rotateZ(z);
    }
}

class MapBackgroundRepository {
    getModelComponentItem() {
        var modelComponentObjects = [];
        for (const [itemKey, items] of Object.entries(this.items)) {
            if (items.modelComponent == undefined) continue;
            var item = items.modelComponent.getItem();
            if (item == undefined) continue;
            modelComponentObjects.push(item);
        }
        return modelComponentObjects;
    }
    constructor() {
        this.items = {};
        this.config = undefined;
        this.layerZ = parseFloat(__layerZMapBackground);
        this.origin = 'origin';
    }
    async initializeMapBackgroundRepository(config) {
        this.config = config;
        return true;
    }
    async generateMapBackground() {
        try {
            var mapBackground = new MapBackground();
            mapBackground.setName(this.origin);
            mapBackground.setObjectType(this.origin);
            mapBackground.setStartPoint(new THREE.Vector3(0, 0, this.layerZ));
            this.items[this.origin] = mapBackground;
            return true;
        } catch (e) {
            console.log(e);
            return false;
        }
    }
    async firstLoad(scene) {
        for (const [itemKey, items] of Object.entries(this.items)) {
            var component = await items.generateModelComonent(this.config.paths);
            if (component == undefined) return false;
            items.setLayerZ(__layerZMapBackground);
            scene.add(component.item);
        }
        return true;
    }
    async firstDraw(mps, resolution) {
        for (const [itemKey, items] of Object.entries(this.items)) {
            var component = items.getModelComponent();
            component.setScale(new THREE.Vector3(resolution, resolution, this.layerZ * resolution));
            component.setPosition(
                new THREE.Vector3(
                    component.item.geometry.parameters.width * resolution * 0.5
                    , component.item.geometry.parameters.height * resolution * 0.5
                    , this.layerZ)
            );
        }
        return true;
    }
    calcPosition(mps, resolution) {
        var a = 0;
    }
    calcScale(mps, resolution) {
        var a = 0;

    }
}
class WorkerRepository {
    constructor() {
        this.config = undefined;

        this.items = undefined;
        this.workersRequestPool = undefined;
        this.mutexDatas = undefined;
        this.dataMutexs = undefined;
    }
    async initializeWorkerRepository(config) {
        this.config = config;
        this.items = {};

        this.workersRequestPool = {};
        this.mutexDatas = {};
        this.dataMutexs = {};
        return true;
    }
    requestPump(changedData) {
        for (var worker of Object.values(this.items)) {
            var key = getKeyByValue(this.items, worker);
            try {
                if (key == undefined) throw 'not defined key url';
                if (this.workersRequestPool[key] == undefined || this.workersRequestPool[key] == false) continue;
                var prevReadDate = undefined;
                if (changedData[key] == undefined) {
                    prevReadDate = new Date();
                } else {
                    prevReadDate = changedData[key].ReadDate;
                }

                var parms = {
                    methodName: 'get',
                    prevReadDate: prevReadDate
                }
                this.workersRequestPool[key] = false;
                worker.postMessage(parms);
            } catch (e) {
                console.log(e);
            }
        }
    }

    async firstWorker() {
        for (var ite = 0; ite < Object.entries(this.config.keys).length; ite++) {
            this.workersRequestPool[Object.entries(this.config.keys)[ite][1]] = true;
            this.mutexDatas[Object.entries(this.config.keys)[ite][1]] = undefined;
            this.dataMutexs[Object.entries(this.config.keys)[ite][1]] = new Mutex();
            this.items[Object.entries(this.config.keys)[ite][1]] =
                new Worker(Object.entries(this.config.paths)[ite][1],
                    {
                        'name': Object.entries(this.config.keys)[ite][1]
                    });
            this.items[Object.entries(this.config.keys)[ite][1]].onmessage = async (res) => {
                try {
                    if (res.data.success == false) throw res.data.data
                    await this.workersOnMessage(res.data.key, res.data.data, res.data.readDate);
                } catch (e) {
                    var concatMessage = 'error: ';
                    console.log(concatMessage.concat(e));
                } finally {
                    this.workersRequestPool[res.data.key] = true;
                }
            }
        }
    }
    async workersOnMessage(workerKey, data, readDate) {
        try {
            if (this.dataMutexs.hasOwnProperty(workerKey) == false) {
                throw 'unknown worker key'.concat(workerKey);
            }

            if (await this.setMutexData(workerKey, data, readDate) == false) {
                throw 'setMutexData error key : '.concat(workerKey);
            }
        } catch (e) {
            console.log(e);
        }
    }
    async setMutexData(mutexKey, mutexData, readDate) {
        ////workers data set
        try {
            var dataSet = {
                Key: mutexKey,
                Data: mutexData,
                ReadDate: readDate
            };
            //mutex lock
            await this.dataMutexs[mutexKey].acquire();
            this.mutexDatas[mutexKey] = dataSet;
            //mutex unlock
            await this.dataMutexs[mutexKey].release();
            return true;
        } catch (e) {
            await this.dataMutexs[mutexKey].release();
            return false;
        }
    }
    async getCloneMutexData(mutexKey) {
        if (this.dataMutexs.hasOwnProperty(mutexKey) == false) {
            throw 'unknown worker key'.concat(mutexKey);
        }
        try {
            //mutex lock
            await this.dataMutexs[mutexKey].acquire();
            ////workers data set
            if (this.mutexDatas[mutexKey] == undefined) {
                throw 'getCloneMutexData mutexDatas not found';
            }
            var dataSet = {
                Key: this.mutexDatas[mutexKey].Key,
                Data: deepCopyObject(this.mutexDatas[mutexKey].Data),
                ReadDate: this.mutexDatas[mutexKey].ReadDate
            };
            //mutex unlock
            await this.dataMutexs[mutexKey].release();
            return dataSet;
        } catch (e) {
            console.log(e);
            await this.dataMutexs[mutexKey].release();
            return undefined;
        }
    }
    async getCloneMutexDataAll() {
        var dataSheet = {};
        for (var ite = 0; ite < Object.entries(this.config.keys).length; ite++) {
            var data = await this.getCloneMutexData(Object.entries(this.config.keys)[ite][1]);
            if (data == undefined) continue;
            dataSheet[data.Key] = data;
        }
        return dataSheet;
    }
}
//----
function deepCopyObject(inObject) {
    var outObject, value, key
    if (typeof inObject !== "object" || inObject === null) {
        return inObject
    }
    outObject = Array.isArray(inObject) ? [] : {}
    for (key in inObject) {
        value = inObject[key]
        outObject[key] = (typeof value === "object" && value !== null) ? deepCopyObject(value) : value
    }
    return outObject
}
function getKeyByValue(object, value) {
    return Object.keys(object).find(key => object[key] === value);
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

class RepositoryConfig {
    constructor() {
        this.keys = undefined;
        this.paths = undefined;
        this.urls = undefined;
        this.invokes = undefined;
    }
    setKeys(keys) {
        this.keys = keys;
    }
    setPaths(paths) {
        this.paths = paths;
    }
    setUrls(urls) {
        this.urls = urls;
    }
    setInvoke(invokes) {
        this.invokes = invokes;
    }
}

class ThreeRenderObject {
    render() {
        this.renderer.render(this.scene, this.camera);
        this.orbit.update();
        if (this.stats != undefined) {
            this.stats.update();
        }
    }


    getRenderObjects() {
        var renders = [];
        if (this.testMode == ture) {
            renders.push(this.axesHelper);
            renders.push(this.gridHelper);
            renders.push(this.stats);
        }
        renders.push(this.camera);
        renders.push(this.light);
    }
    constructor(mode, dom) {
        this.testMode = mode;
        this.canvasDom = dom;
        this.scene = undefined;
        this.renderer = undefined;
        this.camera = undefined;
        this.orbit = undefined;
        this.light = undefined;
        this.axesHelper = undefined;
        this.gridHelper = undefined;
        this.stats = undefined;
    }
    initialize(renderBody) {
        try {
            this.initializeScene();
            this.initializeRenderer(renderBody);
            this.initializeCamera(renderBody);
            this.initializeOrbit();
            this.initializeLight();
            if (this.testMode == true) {
                this.initializeAxesHelper();
                this.initializeStats(renderBody);
                this.initializeGridHelper();
            }
            return true;
        } catch (e) {
            LogForce(e);
            return false;
        }
    }
    initializeScene() {
        this.scene = new THREE.Scene();
    }
    initializeRenderer(renderBody) {
        this.renderer = new THREE.WebGLRenderer({ antialias: true });
        this.renderer.setPixelRatio(window.devicePixelRatio);
        this.renderer.setSize(renderBody.offsetWidth, renderBody.offsetHeight);
        renderBody.appendChild(this.renderer.domElement);
    }
    initializeCamera(renderBody) {
        this.camera = new THREE.PerspectiveCamera(45, renderBody.offsetWidth / renderBody.offsetHeight, 1, 10000);
        this.camera.up = new THREE.Vector3(0, 0, 1);
        this.camera.lookAt(0, 0, 0);
    }
    initializeOrbit() {
        this.orbit = new THREE.OrbitControls(this.camera, this.canvasDom);
        this.orbit.screenSpacePanning = false;
        this.orbit.minDistance = 5;
        this.orbit.maxDistance = 1000;
        this.orbit.maxPolarAngle = Math.PI / 2;
        this.orbit.panSpeed = 0.5;
        this.orbit.rotateSpeed = 0.5;
        this.orbit.enabled = true;
        this.orbit.enablePan = true;
        this.orbit.enableRotate = true;
        this.orbit.enableZoom = true;
    }
    initializeLight() {
        this.light = new THREE.AmbientLight(0xa2a2a2);
        this.light.position.set(100, 50, 100);
        this.scene.add(this.light);
    }
    initializeAxesHelper() {
        if (this.testMode == true) {
            this.axesHelper = new THREE.AxesHelper(100);
            this.axesHelper.position.set(0, 0, 2);
            this.scene.add(this.axesHelper);
        }
    }
    initializeStats(renderBody) {
        if (this.testMode == true) {
            this.stats = new THREE.Stats();
            this.stats.domElement.style.position = 'absolute';
            this.stats.domElement.style.bottom = '50px';
            this.stats.domElement.style.right = "50px";
            this.stats.domElement.style.removeProperty('left');
            this.stats.domElement.style.removeProperty('top');
            var statsALL = this.stats.domElement.querySelectorAll("canvas");
            for (var i = 0; i < statsALL.length; i++) {
                statsALL[i].style.width = "130px";
                statsALL[i].style.height = "80px";
            }
            renderBody.appendChild(this.stats.domElement);
        }
    }
    initializeGridHelper() {
        if (this.testMode == true) {
            this.gridHelper = new THREE.GridHelper(1000, 400);
            this.gridHelper.rotateX(Math.PI / 2);
            this.scene.add(this.gridHelper);
        }
    }
}

class Basket {
    constructor() {
        this.backgrounds = [];
        this.nonCharacters = [];
        this.characters = [];
        this.rayObjects = [];
    }
    addBackground(item) {
        if (item == undefined) return;
        this.backgrounds.push(item);
    }
    addBackgrounds(item) {
        for (var ite = 0; ite < item.length; ite++) {
            if (item[ite] == undefined) continue;
            this.addBackground(item[ite]);
        }
    }
    addNonCharacter(item) {
        if (item == undefined) return;
        this.nonCharacters.push(item);
    }
    addNonCharacters(item) {
        for (var ite = 0; ite < item.length; ite++) {
            if (item[ite] == undefined) continue;
            this.addNonCharacter(item[ite]);
        }
    }
    addCharacter(item) {
        if (item == undefined) return;
        this.characters.push(item);
    }
    addCharacters(item) {
        for (var ite = 0; ite < item.length; ite++) {
            if (item[ite] == undefined) continue;
            this.addCharacter(item[ite]);
        }
    }
    addRayObject(item) {
        if (item == undefined) return;
        this.rayObjects.push(item);
    }
    addRayObjects(item) {
        for (var ite = 0; ite < item.length; ite++) {
            if (item[ite] == undefined) continue;
            addRayObject(item[ite]);
        }
    }

}
class Manager {
    async getCloneMutexDataAll() {
        return this.repoWorker.getCloneMutexDataAll();
    }
    requestWorker(changeData) {
        return this.repoWorker.requestPump(changeData);
    }
    async firstWorker() {
        if (await this.repoWorker.firstWorker() == false) {
            return false;
        }
    }

    async firstLoad() {
        //qwer
        if (await this.repoMapBackground.firstLoad(this.threeRenderObject.scene) == false) {
            return false;
        }
        var backgrounds = this.repoMapBackground.getModelComponentItem();
        this.basket.addBackgrounds(backgrounds);

        if (await this.repoMapObject.firstLoad
            (
                this.threeRenderObject.scene
                , this.repoFont.getPrevItem()
            ) == false) {
            return false;
        }
        var mapObjects = this.repoMapObject.getModelComponentItem();
        this.basket.addNonCharacters(mapObjects);

        if (await this.repoPrefab.firstLoad
            (
                this.dotNet
                , this.threeRenderObject.scene
                , this.repoFont.getPrevItem()
            ) == false) {
            return false;
        }
        var prefabObjects = this.repoPrefab.getModelComponentItem();
        this.basket.addCharacters(prefabObjects);
        return true;
    }
    async firstDraw() {
        if (await this.repoMapBackground.firstDraw(this.mps, this.resolution) == false) {
            return false;
        }
        if (await this.repoMapObject.firstDraw(this.mps, this.resolution) == false) {
            return false;
        }
        if (await this.repoPrefab.firstDraw
            (
                this.mps
                , this.resolution
            ) == false) {
            return false;
        }
        return true;
    }
    async generateFontRepo() {
        if (this.repoFont == undefined) return false;
        return await this.repoFont.generateFonts();
    }
    async generateMapObjectRepo() {
        if (this.repoMapObject == undefined) return false;
        return await this.repoMapObject.generateMapObjects(this.dotNet);
    }
    async generateMapBackgroundRepo() {
        if (this.repoMapBackground == undefined) return false;
        return await this.repoMapBackground.generateMapBackground();
    }
    async generatePrefabRepo() {
        if (this.repoPrefab == undefined) return false;
        return await this.repoPrefab.generatePrefab();
    }
    async generateWokrerRepo() {
        if (this.repoWorker == undefined) return false;
        return await this.repoWorker.generateWorker();
    }

    //#region three renderObject
    setThreeRenderObject(renderObject) {
        this.threeRenderObject = renderObject;
    }
    initializeThreeRenderObject() {
        return this.threeRenderObject.initialize(this.renderBody);
    }
    //#endregion

    //#region dotNet
    setDotNet(dotNet) {
        this.dotNet = dotNet;
    }
    //#endregion

    //#region const
    setMps(mps) {
        this.mps = mps;
    }
    setResolution(resolution) {
        this.resolution = resolution;
    }
    setKeyResultCode(resultCode) {
        this.resultCode = resultCode;
    }
    setColors(colors) {
        this.colors = colors;
    }
    //#endregion

    //#region dom
    setRenderBody(renderBody) {
        this.renderBody = renderBody;
    }
    //#endregion

    //#region repoConfig
    setRepoConfigFont(repoConfig) {
        this.repoConfigFont = repoConfig;
    }
    setRepoConfigMapObject(repoConfig) {
        this.repoConfigMapObject = repoConfig;
    }
    setRepoConfigMapBackground(repoConfig) {
        this.repoConfigMapBackground = repoConfig;
    }
    setRepoConfigPrefab(repoConfig) {
        this.repoConfigPrefab = repoConfig;
    }
    setRepoConfigWorker(repoConfig) {
        this.repoConfigWorker = repoConfig;
    }
    //#endregion

    //#region repo
    setRepoFont(repo) {
        this.repoFont = repo;
    }
    async initializeFontRepository() {
        return await this.repoFont.initializeFontRepository(this.repoConfigFont);
    }

    setRepoMapObject(repo) {
        this.repoMapObject = repo;
    }
    async initializeMapObjectRepository() {
        return await this.repoMapObject.initializeMapObjectRepository(this.repoConfigMapObject);
    }

    setRepoMapBackground(repo) {
        this.repoMapBackground = repo;
    }
    async initializeBackgroundRepository() {
        return await this.repoMapBackground.initializeMapBackgroundRepository(this.repoConfigMapBackground);
    }

    setRepoPrefab(repo) {
        this.repoPrefab = repo;
    }
    async initializePrefabRepository() {
        return await this.repoPrefab.initializePrefabRepository(this.repoConfigPrefab);
    }

    setRepoWorker(repo) {
        this.repoWorker = repo;
    }
    setFpsCounter(fpsCounter) {
        this.fpsCounter = fpsCounter;
    }
    async initializeWorkerRepository() {
        return await this.repoWorker.initializeWorkerRepository(this.repoConfigWorker);
    }
    //#endregion

    //#region render object
    //#endregion

    constructor(refDotNet) {
        //#region textMode
        this.threeRenderObject = undefined;
        //#endregion

        //#region dotnet
        this.dotNet = undefined;
        //#endregion

        //#region const
        this.mps = undefined;
        this.resolution = undefined;
        this.resultCode = undefined;
        this.colors = undefined;
        //#endregion

        //#region dom
        this.renderBody = undefined;
        //#endregion

        //#region repoConfig
        this.repoConfigFont = undefined;
        this.repoConfigMapObject = undefined;
        this.repoConfigMapBackground = undefined;
        this.repoConfigPrefab = undefined;
        this.repoConfigWorker = undefined;
        //#endregion

        //#region basket
        this.basket = new Basket();
        //#endregion

        //#region repos
        this.repoFont = undefined;
        this.repoMapObject = undefined;
        this.repoMapBackground = undefined;
        this.repoPrefab = undefined;
        this.repoWorker = undefined;
        //#endregion

        //#region render object
        this.threeRenderObject = undefined;
        //#endregion

        //#region fpsCounter
        this.fpsCounter = undefined;
        //#endregion
    }

    render() {
        //changedData에는 변경된 데이터만 들어옴
        //이전 데이터는 가지고 있어야함
        //이전 데이터와 비교해서 렌더링 로직 태워야 함
        //델타타임 들고와야됨
        //스피드 설정해야함
        this.threeRenderObject.render();
    }
    prefabRender(changedData) {
        var speed = 1500;
        this.threeRenderObject.orbit
        this.repoPrefab.render(changedData, this.fpsCounter.elapsed, speed, new THREE.Vector3(this.threeRenderObject.camera.rotation.x, this.threeRenderObject.camera.rotation.y, this.threeRenderObject.camera.rotation.z));
    }
}

class FpsCounter {
    constructor(useFps) {
        var now = Date.now();
        this.useFps = useFps;
        this.fpsInterval = 1000 / useFps;
        this.startTime = now;
        this.now = now;
        this.then = now;
        this.elapsed = parseFloat(0);
    }
}

function uuidv4() {
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    );
}