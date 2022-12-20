let __UI_FRAME = 60;

let _dotNet = undefined;
let _renderCanavsName = undefined;
let _managerRepo = undefined;
let _workers = {};

let _manager = undefined;
let _auth = undefined;

const OBJECT_ENTRY_KEY = 0;
const OBJECT_ENTRY_VALUE = 1;



/*
 *public enum PrefabType
 * {
       Vertex = 0,
       Lane = 1,
       Robot = 2,
   }
   */
const __PrefabAbnormalType = { VERTEX: 0, LANE: 1 };
const __PrefabNormalType = { Robot: 2 };

const __PrefabType = { VERTEX: 0, LANE: 1, Robot: 2 };
//버텍스랑 레인은 프리팹에 포함되지 않음
const __PrefabModelPath = { "VERTEX": 0, "LANE": 1, "./monitoring/model/fbx/robot.fbx": 2, };

//public enum PrefabUpdateType {
//    All = 0,
//    Background = 1,
//    Animate = 2,
//}
const __PrefabUpdateType = { ALL: 0, Background: 1, Animate: 2 };

//public enum PrefabComponentType {
//    Transform = 0,
//}
const __ComponentType = { TRANSFORM: 0, SCALE: 1, };



class FpsCounter {
    constructor(useFps) {
        var now = Date.now();
        this._useFps = useFps;
        this._fpsInterval = 1000 / useFps;
        this._startTime = now;
        this._now = now;
        this._then = now;
        this._elapsed = parseFloat(0);
    }
}

class Manager {
    constructor() {
        this._sharedData = new MutexData();

        this._rayCaster = new THREE.Raycaster();
        this._prefabRepository = new PrefabRepository();
        this._prefabContainer = new PrefabContainer();
        this._prefabAbnormalRepository = new PrefabRepository();
        this._prefabAbnormalContainer = new PrefabContainer();

        this._dotnet = undefined;
        this._dotnetManagerDataRepo = undefined;
        this._renderBody = undefined;
        this._renderer = undefined;
        this._fpsCounter = undefined;
        this._scene = undefined;
        this._camera = undefined;
        this._orbit = undefined;
        this._light = undefined;
        this._worker = {};
    }
    RayCast() {

    }
    SetRenderBody(renderBodyElementId) {
        this._renderBody = document.getElementById(renderBodyElementId);
    }
    GenerateFpsCounter(frame) {
        this._fpsCounter = new FpsCounter(frame);
    }
    GenerateRenderer() {
        this._renderer = new THREE.WebGLRenderer({ antialias: true });
        this._renderer.setPixelRatio(window.devicePixelRatio);
        this._renderer.setSize(this._renderBody.offsetWidth, this._renderBody.offsetHeight);
        this._renderer.setClearColor(0xffffff);
        this._renderBody.appendChild(this._renderer.domElement);

        this._scene = new THREE.Scene();

        this._camera = new THREE.PerspectiveCamera(45, this._renderBody.offsetWidth / this._renderBody.offsetHeight, 1, 10000);
        this._camera.up = new THREE.Vector3(0, 0, 1);
        this._camera.lookAt(0, 0, 0);

        this._orbit = new THREE.OrbitControls(this._camera, this._renderBody);
        this._orbit.screenSpacePanning = false;
        this._orbit.minDistance = 5;
        this._orbit.maxDistance = 1000;
        this._orbit.maxPolarAngle = Math.PI / 2;
        this._orbit.panSpeed = 0.5;
        this._orbit.rotateSpeed = 0.5;
        this._orbit.enabled = true;
        this._orbit.enablePan = true;
        this._orbit.enableRotate = true;
        this._orbit.enableZoom = true;

        this._light = new THREE.AmbientLight(0xa2a2a2);
        this._light.position.set(100, 50, 100);
        this._scene.add(this._light);
    }
    async Render() {
        try {
            var data = await this._sharedData.GetFirst();
            if (data != undefined) {
                var parsedData = JSON.parse(data);
                if (this._prefabContainer != undefined) {
                    this._prefabContainer.Update(parsedData);
                }
            }

            this._renderer.render(this._scene, this._camera);
            if (this._dotnetManagerDataRepo.orbit.b) {
                this._orbit.update();
            }
        } catch (e) {
            console.log(e);
        }
    }
    async InitializeDefault(initData) {
        for (const [prefabName, prefabDatas] of Object.entries(initData.Prefabs)) {
            for (var itePrefab = 0; itePrefab < prefabDatas.length; itePrefab++) {
                if (Object.values(__PrefabAbnormalType).some(x => x == prefabDatas[itePrefab].PrefabType) == true) {
                    var prefab = this._prefabAbnormalRepository.GetPrefab(prefabDatas[itePrefab].PrefabType);
                    prefab.DefaultData(prefabDatas[itePrefab]); //qq
                    prefab.HitAbnormal();
                    this.AddAbnormalPrefab(prefab);
                }
                else {
                    var prefab = this._prefabRepository.GetPrefab(prefabDatas[itePrefab].PrefabType);
                    prefab.DefaultData(prefabDatas[itePrefab]);
                    prefab.Hit();
                    this.AddPrefab(prefab);
                }
            }
        }
    }

    async InitializeScene() {
        var sceneObjects = this._prefabContainer.GetSceneObjects();
        for (var iteObject = 0; iteObject < sceneObjects.length; iteObject++) {
            _manager._scene.add(sceneObjects[iteObject]);
        }

        var abnormalSceneObjectVertices = this._prefabAbnormalContainer.GetSceneAbnormalObjectVertices();
        for (var iteObject = 0; iteObject < abnormalSceneObjectVertices.length; iteObject++) {
            _manager._scene.add(abnormalSceneObjectVertices[iteObject]);
        }

        var abnormalSceneObjectLanes = this._prefabAbnormalContainer.GetSceneAbnormalObjectLanes();
        for (var iteObject = 0; iteObject < abnormalSceneObjectLanes.length; iteObject++) {
            _manager._scene.add(abnormalSceneObjectLanes[iteObject]);
        }

        InitListener();

    }
    async AddPrefab(prefab) {
        this._prefabContainer.AddPrefab(prefab);
    }
    async AddAbnormalPrefab(prefab) {
        this._prefabAbnormalContainer.AddPrefab(prefab);
    }
}


class PrefabRepository {
    constructor() {
        this._loader = undefined;
        this._prefabPaths = {};
        this._prefabModels = {};
    }
    Initialze(loader, paths) {
        this._loader = loader;
        this._prefabPaths = paths;
    }
    GetModelClone(prefabType) {
        if (this._prefabModels[prefabType] == undefined) return undefined;
        return this._prefabModels[prefabType].clone();
    }
    GetPrefab(prefabType) {
        var prefabModel = this.GetModelClone(prefabType);
        var result = new Prefab();
        result._prefabType = prefabType;
        result._model = prefabModel;
        return result;
    }
    async HitAbnormal() {
        for (const [path, prefabType] of Object.entries(this._prefabPaths)) {
            if (this._prefabModels[prefabType] != undefined) continue;
            if (Object.values(__PrefabNormalType).some(x => x == prefabType) == true) continue;
        }
    }
    async Hit() {
        for (const [path, prefabType] of Object.entries(this._prefabPaths)) {
            if (this._prefabModels[prefabType] != undefined) continue;
            if (Object.values(__PrefabAbnormalType).some(x => x == prefabType) == true) continue;
            var model = await this.LoadFbx(path, this._loader);
            if (model == null) {
                console.log('fail::PrefabRepository.LoadFbx()');
                continue;
            }
            this._prefabModels[prefabType] = model;
        }
    }
    async LoadFbx(path, loader) {
        return new Promise((resolve, reject) => {
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
}

class PrefabContainer {
    constructor() {
        this._prefabs = {};
    }
    Update(data) {
        if (data == undefined) return;
        if (data.Prefabs == undefined) return;
        if (this._prefabs == undefined) return;
        if (this._prefabs.length < 1) return;
        for (const [prefabKey, prefabDataGroups] of Object.entries(data.Prefabs)) {
            if (__PrefabNormalType[prefabKey] == undefined) continue;
            for (const prefabData of Object.values(prefabDataGroups)) {
                var prefab = this._prefabs[prefabData.Key];
                if (prefab == undefined) continue;
                prefab.Update(prefabData);
            }
        }
    }
    AddPrefab(prefab) {
        if (prefab == undefined) return;
        if (prefab._key == undefined || prefab._key == '') return;
        this._prefabs[prefab._key] = prefab;
    }
    GetSceneObjects() {
        var result = new Array();
        for (const [prefabKey, prefab] of Object.entries(this._prefabs)) {
            if (prefab._model == undefined) continue;
            result.push(prefab._model);
        }
        return result;
    }

    GetSceneAbnormalObjectVertices() {
        var result = new Array();
        var vertices = Object.values(this._prefabs).filter(x => x._prefabType == __PrefabAbnormalType.VERTEX);
        if (vertices == null || vertices.length < 1) return result;
        for (var iteVertex = 0; iteVertex < vertices.length; iteVertex++) {
            var prefab = vertices[iteVertex];
            prefab.GenerateAbnormalModelVertex();
            result.push(prefab._model);

            var transformData = prefab._defaultData.ComponentContainer.PrefabComponents.find(x => x.PrefabComponentType == __ComponentType.TRANSFORM);
            if (transformData == undefined) continue;

            var transformComponent = Object.values(prefab._componentContainer._components).find(x => x._componentType == __ComponentType.TRANSFORM);
            if (transformComponent != undefined) {
                transformComponent._position.x = transformData.Position.X;
                transformComponent._position.y = transformData.Position.Y;
                transformComponent._position.z = transformData.Position.Z;
            }

            prefab.UpdateComponent(__ComponentType.TRANSFORM);
        }
        return result;
    }
    GetSceneAbnormalObjectLanes() {
        var result = new Array();
        var lanes = Object.values(this._prefabs).filter(x => x._prefabType == __PrefabAbnormalType.LANE);
        if (lanes == null || lanes.length < 1) return result;

        var vertices = Object.values(this._prefabs).filter(x => x._prefabType == __PrefabAbnormalType.VERTEX);
        if (vertices == null || vertices.length < 1) return result;

        for (var iteLane = 0; iteLane < lanes.length; iteLane++) {
            var prefab = lanes[iteLane];
            if (prefab == undefined) continue;

            var transformData = prefab._defaultData.ComponentContainer.PrefabComponents.find(x => x.PrefabComponentType == __ComponentType.TRANSFORM);
            if (transformData == undefined) continue;

            var posStartVertexTransformComponent = Object.values(vertices[transformData.Position.X]._componentContainer._components).find(x => x._componentType == __ComponentType.TRANSFORM);
            if (posStartVertexTransformComponent == undefined) continue;
            var posEndVertexTransformComponent = Object.values(vertices[transformData.Position.Y]._componentContainer._components).find(x => x._componentType == __ComponentType.TRANSFORM);
            if (posEndVertexTransformComponent == undefined) continue;

            var posStartVector3 = new THREE.Vector3(
                posStartVertexTransformComponent._position.x
                , posStartVertexTransformComponent._position.y
                , posStartVertexTransformComponent._position.z
            );
            var posEndVector3 = new THREE.Vector3(
                posEndVertexTransformComponent._position.x
                , posEndVertexTransformComponent._position.y
                , posEndVertexTransformComponent._position.z
            );

            prefab.GenerateAbnormalModelLane(posStartVector3, posEndVector3);
            result.push(prefab._model);
        }
        return result;
    }
}

class Prefab {
    constructor() {
        this._defaultData = undefined;

        this._key = undefined;
        this._prefabType = undefined;
        this._prefabUpdateType = undefined;
        this._model = undefined;

        this._componentContainer = new ComponentContainer();
    }
    Update(data) {
        this._componentContainer.Update(this._model, data);
    }
    UpdateComponent(componentType) {
        this._componentContainer.UpdateAssign(componentType, this._model);
    }
    SetModel(model) {
        this._model = model;
        this._model.name = _key;
    }
    DefaultData(data) {
        this._defaultData = data;

        this._key = this._defaultData.Key;
        this._prefabType = this._defaultData.PrefabType;
        this._prefabUpdateType = this._defaultData.Key;

        if (this._model != undefined) {
            this._model.name = this._key;
        }
    }
    GenerateAbnormalModelVertex() {
        var radius = parseFloat(1),
            segments = 32,
            mat = new THREE.LineBasicMaterial({ color: 0xff0000 }),
            geo = new THREE.CircleGeometry(radius, segments);
        var circle = new THREE.Line(geo, mat);
        this._model = circle;
        this._model.name = this._key;
        return this._model;
    }
    GenerateAbnormalModelLane(posStart, posEnd) {
        const mat = new THREE.LineBasicMaterial({ color: 0xff0000 });
        var drawVector3Arr = [];
        drawVector3Arr.push(posStart, posEnd);
        var geo = new THREE.BufferGeometry().setFromPoints(drawVector3Arr);
        var lane = new THREE.Line(geo, mat);
        this._model = lane;
        this._model.name = this._key;
        return this._model;
    }
    HitAbnormal() {
        for (var iteComponent = 0; iteComponent < this._defaultData.ComponentContainer.PrefabComponents.length; iteComponent++) {
            var component = this._componentContainer.GenerateComponent(this._defaultData.ComponentContainer.PrefabComponents[iteComponent]);
            if (component == undefined) continue;
            this._componentContainer.AddComponent(component);
        }
    }
    Hit() {
        for (var iteComponent = 0; iteComponent < this._defaultData.ComponentContainer.PrefabComponents.length; iteComponent++) {
            var component = this._componentContainer.GenerateComponent(this._defaultData.ComponentContainer.PrefabComponents[iteComponent]);
            if (component == undefined) continue;
            this._componentContainer.AddComponent(component);
        }
    }
}

class TransformComponent {
    constructor() {
        this._componentType = __ComponentType.TRANSFORM;
        this._position = new THREE.Vector3(0, 0, 0);
        this._rotation = new THREE.Vector3(0, 0, 0);
    }
    Update(model) {
        model.position.x = this._position.x;
        model.position.y = this._position.y;
        model.position.z = this._position.z;
    }
    UpdateData(model, data) {
        this._position.set(data.Position.X, data.Position.Y, data.Position.Z);
        this._rotation.set(data.Rotation.X, data.Rotation.Y, data.Rotation.Z);
        this.Update(model);
    }
}

class ScaleComponent {
    constructor() {
        this._componentType = __ComponentType.SCALE;
        this._scale = new THREE.Vector3(0, 0, 0);
    }
    Update(model) {
        model.scale.set(this._scale.x, this._scale.y, this._scale.z);
    }
    UpdateData(model, data) {
        this._scale.set(data.Scale.X, data.Scale.Y, data.Scale.Z);
        this.Update(model);
    }
}

class ComponentContainer {
    constructor() {
        this._components = {};
    }
    Update(model, data) {
        if (model == undefined) return;
        for (const componentData of Object.values(data.ComponentContainer.PrefabComponents)) {
            var component = undefined;
            try {
                if (componentData.PrefabComponentType == __ComponentType.TRANSFORM) {
                    component = this._components[__ComponentType.TRANSFORM];
                }
                else if (componentData.PrefabComponentType == __ComponentType.SCALE) {
                    component = this._components[__ComponentType.SCALE];
                }
                else {
                    console.log('undefined __ComponentType');
                }
            }
            catch (e) {
                console.log(e);
            }
            finally {
                if (component != undefined) {
                    component.UpdateData(model, componentData);
                }
            }
        }
    }
    GenerateComponent(args) {
        if (args == undefined) return undefined;
        if (args.PrefabComponentType == undefined) return undefined;
        if (args.PrefabComponentType == __ComponentType.TRANSFORM) {
            return new TransformComponent();
        }
        if (args.PrefabComponentType == __ComponentType.SCALE) {
            return new ScaleComponent();
        }
        else {
            console.log('undefined componentType::ComponentContainer.GenerateComponent()');
        }
        return undefined;
    }
    AddComponent(component) {
        this._components[component._componentType] = component;
    }
    UpdateAssign(componentType, model) {
        if (this._components[componentType] == undefined) return;
        this._components[componentType].Update(model);
    }
}

class StructWorkerIF {
    constructor(request, data) {
        this.request = request;
        this.data = data;
    }
}

export async function focusModel(data) {

}

export async function initializeCore(request) { 
    _manager = new Manager();
    _manager._dotnet = request.dotNet;
    _manager._dotnetManagerDataRepo = request.managerRepo;
    _manager.SetRenderBody(request.renderElementId);
    _manager.GenerateFpsCounter(__UI_FRAME);
    _manager.GenerateRenderer();

    _manager._prefabAbnormalRepository.Initialze(new THREE.FBXLoader(), __PrefabModelPath);
    await _manager._prefabAbnormalRepository.HitAbnormal();

    _manager._prefabRepository.Initialze(new THREE.FBXLoader(), __PrefabModelPath);
    await _manager._prefabRepository.Hit();
    return true;
}
export async function startWorker(args) {
    if (_workers[args.workerName] == undefined) {
        _workers[args.workerName] = new Worker(args.workerScript);
        _workers[args.workerName].onmessage = function (e) {
            workerOnMessage(e);
        }
        _auth = args.auth;

        var send = new StructWorkerIF('startWorker', args);
        _workers[args.workerName].postMessage(send);
    }
    else {
        return false;
    }
    return true;
}
async function workerOnMessage(e) {
    //var send = {
    //     request: 'HubCliFuncReqeustMdcInitData',
    //     data: message.message
    // }
    try {
        if (e.data.request == 'initialize') {
            if (e.data.data == true) {
                animate();
            }
        }
        else if (e.data.request == 'HubCliFuncReqeustMdcInitData') {
            await _manager.InitializeDefault(JSON.parse(e.data.data));
            await _manager.InitializeScene(JSON.parse(e.data.data));
            animate();
        }
        else if (e.data.request == 'HubCliFuncBroadCastMdc') {
            await _manager._sharedData.AddFirst(e.data.data.message);
        }
        else {
            console.log(e.data);
        }
    } catch (e) {
        console.log(e);
    }
}

async function animate() {
    requestAnimationFrame(animate);
    _manager._fpsCounter._now = Date.now();
    _manager._fpsCounter._elapsed = _manager._fpsCounter._now - _manager._fpsCounter._then;
    if (_manager._fpsCounter._elapsed > _manager._fpsCounter._fpsInterval) {
        _manager._fpsCounter._then = _manager._fpsCounter._now - (_manager._fpsCounter._elapsed % _manager._fpsCounter._fpsInterval);
        await _manager.Render();
    }
}



class MutexData {
    constructor() {
        this.data = new Array();
        this.mutex = new Mutex();
    }

    async AddFirst(data) {
        try {
            await this.mutex.acquire();
            this.data.unshift(data);
        } catch (e) {

        }
        finally {
            this.mutex.release();
        }
    }

    async GetFirst() {
        try {
            await this.mutex.acquire();
            return this.data[0];
        } catch (e) {

        }
        finally {
            this.mutex.release();
        }
    }
}

class Mutex {
    constructor() {
        this.lock = false;
    }
    async acquire() {
        while (true) {
            if (this.lock === false) { break; }
            await Sleep(3);
        }
        this.lock = true;
    }

    release() {
        this.lock = false;
    }
}

function Sleep(ms) {
    return new Promise((r) => setTimeout(r, ms));
}



function InitListener() {
    document.addEventListener('pointerdown', onPointerDown);
}

function onPointerDown(event) {
    if (event.button == 0) {

        var mousePointer = new THREE.Vector2();
        mousePointer.set((event.offsetX / _manager._renderBody.offsetWidth) * 2 - 1, - (event.offsetY / _manager._renderBody.offsetHeight) * 2 + 1);
        _manager._rayCaster.setFromCamera(mousePointer, _manager._camera);
        var castObjects = _manager._prefabContainer.GetSceneObjects();
        const intersects = _manager._rayCaster.intersectObjects(castObjects, true);
        if (intersects.length < 1) return;
        if (intersects[0].object == undefined) return;
        if (intersects[0].object.parent == undefined) return;
        const intersect = intersects[0].object.parent;
        
        _manager._camera.position.set(intersect.position.x, intersect.position.y, intersect.position.z);
        _manager._orbit .update();
    }
}

export function RayCaster(key) {

    if (key == null) return;
    //key = "AGF0&";
    var castObjects = _manager._prefabContainer.GetSceneObjects();
    var castRobotObjects = castObjects.find(x => x.name == key);

    const intersects = _manager._rayCaster.intersectObjects(castRobotObjects, true);

    if (intersects.length < 1) return;
    if (intersects[0].object == undefined) return;
    if (intersects[0].object.parent == undefined) return;
    const intersect = intersects[0].object.parent;

    _manager._camera.position.set(intersect.position.x, intersect.position.y, intersect.position.z);
    _manager._orbit.update();
}
