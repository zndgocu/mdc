//#region var
const ORIGIN_MODEL_NAME = 'ORIGIN_MODEL';
//#endergion

// #region html data
let renderBody = undefined;
let razor = undefined;
let drawMode = false;
let uiOverlay = false;
// #endregion

//#region default items
let scene = undefined;
let camera = undefined;
let renderer = undefined;
let orbit = undefined;
let gridHelper = undefined;
//#endregion


// #region rendering item
let stop = false;
let frameCount = 0;
let fps, fpsInterval;
let startTime;
let now, then, elapsed;
// #endregion



// #region interop
export function initInterop(dotNetObject) {
    if (dotNetObject == null || dotNetObject == undefined) {
        return false;
    }
    razor = dotNetObject;
    init();

    return true;
}
// #endregion

// #region default init
function init() {
    renderBody = document.getElementById('render-container');

    if (initRenderer() == false) {
        console.log("Error :: initRenderer");
        return false;
    }
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
    
    if (addOrbit() == false) {
        console.log("Error :: addOrbit");
        return false;
    }

    if (initListener() == false) {
        console.log("Error :: initListener");
        return false;
    }

    loadSvg('./monitoring/model/svg/conveyer_donuts.svg');
    then = Date.now();
    fps = 60;
    fpsInterval = 1000 / fps;
    animate();
    return true;
}



function initRenderer() {
    renderer = new THREE.WebGLRenderer({ antialias: true });
    renderer.setPixelRatio(window.devicePixelRatio);
    renderer.setSize(renderBody.offsetWidth, renderBody.offsetHeight);
    renderBody.appendChild(renderer.domElement);
}

function initCamera() {
    camera = new THREE.PerspectiveCamera(45, renderBody.offsetWidth / renderBody.offsetHeight, 1, 1000);
    camera.position.set(0, 0, 500);
    camera.lookAt(scene.position);
    return true;
}

function initScene() {
    scene = new THREE.Scene();
    scene.background = new THREE.Color(0xffffff);
    return true;
}

function initGridHelper() {
    gridHelper = new THREE.GridHelper(1000, 100);
    gridHelper.rotation.x = THREE.Math.degToRad(90);
    scene.add(gridHelper);
    return true;
}

function addOrbit() {
    orbit = new THREE.OrbitControls(camera, renderer.domElement);
    orbit.target.set(0, 0, 0);
    orbit.minDistance = 40;
    orbit.maxDistance = 1000;
    orbit.enableRotate = false;
    orbit.enableZoom = true;
}

function initListener() {
    window.addEventListener('resize', onWindowResize);
    document.addEventListener('pointermove', onPointerMove);
    document.addEventListener('pointerdown', onPointerDown);
    document.addEventListener('keydown', onDocumentKeyDown);
    document.addEventListener('keyup', onDocumentKeyUp);
}
//#endregion

// #region listener func
function onWindowResize() {
    camera.aspect = renderBody.offsetWidth / renderBody.offsetHeight;
    camera.updateProjectionMatrix();
    renderer.setSize(renderBody.offsetWidth, renderBody.offsetHeight);
    render();
}
function onPointerMove(event) {
}
function onPointerDown(event) {
}
function onDocumentKeyDown(event) {
}
function onDocumentKeyUp(event) {
}
// #endregion



//#region render fucc
async function animate() {
    if (stop == false) {
        requestAnimationFrame(animate);
    }
    now = Date.now();
    elapsed = now - then;
    if (elapsed > fpsInterval) {
        then = now - (elapsed % fpsInterval);
        renderControl();
        //something render code
        render();
    }
}

function renderControl() {
    if (orbit.enabled == true) {
        orbit.update();
    }
}
    
function render() {
    renderer.render(scene, camera);
}


//#endregion


//#region svgFunc
function loadSvg(url) {
    const loader = new THREE.SVGLoader();
    //guiData = {
    //    currentURL: ,
    //    drawFillShapes: true,
    //    drawStrokes: true,
    //    fillShapesWireframe: false,
    //    strokesWireframe: false
    //};

    loader.load(url, function (data) {
        const paths = data.paths;
        const group = new THREE.Group();
        group.scale.multiplyScalar(1);
        group.position.x = - 70;
        group.position.y = 70;
        group.scale.y *= - 1;
        for (let i = 0; i < paths.length; i++) {
            const path = paths[i];
            const fillColor = path.userData.style.fill;
            if (true && fillColor !== undefined && fillColor !== 'none') {
                const material = new THREE.MeshBasicMaterial({
                    color: new THREE.Color().setStyle(fillColor),
                    opacity: path.userData.style.fillOpacity,
                    transparent: path.userData.style.fillOpacity < 1,
                    side: THREE.DoubleSide,
                    depthWrite: false,
                    wireframe: false //윤곽선
                });
                const shapes = THREE.SVGLoader.createShapes(path);
                for (let j = 0; j < shapes.length; j++) {
                    const shape = shapes[j];
                    const geometry = new THREE.ShapeGeometry(shape);
                    const mesh = new THREE.Mesh(geometry, material);
                    group.add(mesh);
                }
            }
            const strokeColor = path.userData.style.stroke;
            if (true && strokeColor !== undefined && strokeColor !== 'none') {
                const material = new THREE.MeshBasicMaterial({
                    color: new THREE.Color().setStyle(strokeColor),
                    opacity: path.userData.style.strokeOpacity,
                    transparent: path.userData.style.strokeOpacity < 1,
                    side: THREE.DoubleSide,
                    depthWrite: false,
                    wireframe: false //정밀도 쓰지마세요
                });

                for (let j = 0, jl = path.subPaths.length; j < jl; j++) {
                    const subPath = path.subPaths[j];
                    const geometry = THREE.SVGLoader.pointsToStroke(subPath.getPoints(), path.userData.style);
                    if (geometry) {
                        const mesh = new THREE.Mesh(geometry, material);
                        group.add(mesh);
                    }
                }
            }
        }
        scene.add(group);
    });
}
//#endregion

//#region extension function
function deepCopy(o) {
    var result = {};
    if (typeof o === "object" && o !== null)
        for (var i in o) result[i] = deepCopy(o[i]);
    else result = o;
    return result;
}
// #endregion