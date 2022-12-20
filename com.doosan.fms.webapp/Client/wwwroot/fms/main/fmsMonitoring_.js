var _started = false;
var _startedSignalR = false;
var _workerSignalRFmsDataHubClient = undefined;
var _parent = undefined;
var _renderRepository = undefined;
var _renderFps = 60;

//#region class
class FpsCounter {
    constructor(useFps) {
        var _now = Date.now();
        this._useFps = _useFps;
        this._fpsInterval = 1000 / _useFps;
        this._startTime = _now;
        this._now = _now;
        this._then = _now;
        this._elapsed = parseFloat(0);
    }
}

class RenderRepository {
    Render() {
        if (this._renderer != undefined &&
            this._scene != undefined &&
            this._camera != undefined)
        {
            this._renderer.render(this._scene, this._camera);
        }

        if (this._orbit != undefined) this._orbit.update();
        if (this._stats != undefined) this._stats.update();
    }
    constructor() {
        this._fpsCounter = undefined;
        this._renderBody = undefined;
        this._scene = undefined;
        this._renderer = undefined;
        this._camera = undefined;
        this._orbit = undefined;
        this._light = undefined;
        this._stats = undefined;
        this._axesHelper = undefined;
        this._gridHelper = undfined;
    }

    InitializeRenderRepository(renderElementId) {
        this._fpsCounter = new FpsCounter(_renderFps);

        this._renderBody = document.getElementById(renderElementId);

        this._scene = new THREE.Scene();

        this._renderer = new THREE.WebGLRenderer({ antialias: true });
        this._renderer.setPixelRatio(window.devicePixelRatio);
        this._renderer.setSize(this._renderBody.offsetWidth, this._renderBody.offsetHeight);
        this._renderBody.appendChild(this._renderer.domElement);

        this._camera = new THREE.PerspectiveCamera(45, this._renderBody.offsetWidth / this._renderBody.offsetHeight, 1, 10000);
        this._camera.up = new THREE.Vector3(0, 0, 1);
        this._camera.lookAt(0, 0, 0);

        this._orbit = new THREE.OrbitControls(this.camera, this.canvasDom);
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

        this._axesHelper = new THREE.AxesHelper(100);
        this._axesHelper.position.set(0, 0, 2);
        this._axesHelper.add(this.axesHelper);

        this._gridHelper = new THREE.GridHelper(1000, 400);
        this._gridHelper.rotateX(Math.PI / 2);

        this._stats = new THREE.Stats();
        this._stats.domElement.style.position = 'absolute';
        this._stats.domElement.style.bottom = '50px';
        this._stats.domElement.style.right = "50px";
        this._stats.domElement.style.removeProperty('left');
        this._stats.domElement.style.removeProperty('top');
        var statsALL = this.stats.domElement.querySelectorAll("canvas");
        for (var i = 0; i < statsALL.length; i++) {
            statsALL[i].style.width = "130px";
            statsALL[i].style.height = "80px";
        }
    }

    InitializeSceneObjects() {
        this._scene.add(this._light);
        this._scene.add(this._axesHelper);
        this._scene.add(this._gridHelper);
        this._scene.add(this._stats);
    }
}
//#endregion

export async function start(request) {
    var start = false;
    _parent = request.dotNet;

    try {
        _renderRepository = new RenderRepository();
        if (_renderRepository.InitializeRenderRepository(request.renderElementId) == false) throw '_renderRepository.InitializeRenderRepository() fail';
        if (_renderRepository.InitializeSceneObjects() == false) throw '_renderRepository.InitializeSceneObjects() fail';
    }
    catch (e) {
        start = false;
    }

    _started = start;
    if (_started == true) {
        animate();
    }

    return _started;
}

async function animate() {
    requestAnimationFrame(animate);
    _renderRepository._fpsCounter._now = Date.now();
    _renderRepository._fpsCounter._elapsed = _renderRepository._fpsCounter._now - _renderRepository._fpsCounter._then;
    if (_renderRepository._fpsCounter._elapsed > _renderRepository._fpsCounter._fpsInterval) {
        _renderRepository._fpsCounter._then = _renderRepository._fpsCounter._now - (_renderRepository._fpsCounter._elapsed % _renderRepository._fpsCounter._fpsInterval);
        _renderRepository.Render();
    }
}

export async function workerSignalRFmsDataHubClientStart(request) {
    if (request.requestCommand == 'start') {
        _workerSignalRFmsDataHubClient = new Worker(request.data);
        _workerSignalRFmsDataHubClient.onmessage = function (e) {
            workerSignalRFmsDataHubClientOnMessage(e);
        }
        _startedSignalR = true;
        dotnet.postMessage("");
    }
    else if (request.requestCommand == 'end') {
        close();
    }
    else {

    }
    return true;
}

function workerSignalRFmsDataHubClientOnMessage(e) {
    postMessage('workerSignalRFmsDataHubClientOnMessage encount');
    console.log(e.data);
}