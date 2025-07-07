import * as THREE from './threeModule.js';
import { OBJLoader } from './objLoader.js';

let camera, scene, renderer;
const clock = new THREE.Clock();
let sceneObject;
let sceneReady = false;

init();

function init() {
    scene = new THREE.Scene();

    camera = new THREE.PerspectiveCamera(45, 1, 0.1, 1000);
    camera.position.z = 25;

    // ќсвещение
    const hemisphereLight = new THREE.HemisphereLight(0xffffff, 0x000000, 1);
    scene.add(hemisphereLight);

    const dirLight = new THREE.DirectionalLight(0xffffff, 2);
    dirLight.position.set(-10, 10, 10);
    scene.add(dirLight);

    const ambientLight = new THREE.AmbientLight(0xffffff, 0.3); // дополнительный м€гкий свет
    scene.add(ambientLight);

    function loadModel() {
        const newMaterial = new THREE.MeshStandardMaterial({
            map: texture,
            emissive: new THREE.Color(0x222222), // тЄмное м€гкое свечение
            emissiveMap: texture,
            emissiveIntensity: 25,
            metalness: 0.2,
            roughness: 0.8
        });

        sceneObject.traverse((child) => {
            if (child.isMesh) {
                if (Array.isArray(child.material)) {
                    child.material.forEach((mat, i) => {
                        child.material[i] = newMaterial.clone();
                    });
                } else {
                    child.material = newMaterial.clone();
                }
            }
        });

        const box = new THREE.Box3().setFromObject(sceneObject);
        const center = box.getCenter(new THREE.Vector3());
        sceneObject.position.sub(center);
        sceneObject.scale.set(1, 1, 1);

        scene.add(sceneObject);
        sceneReady = true;

        // ¬ызов onThreeJsLoaded после того, как модель добавлена в сцену
        if (typeof window.onThreeJsLoaded === 'function') {
            window.onThreeJsLoaded();
        }
    }

    const manager = new THREE.LoadingManager(loadModel);

    const textureLoader = new THREE.TextureLoader(manager);
    const texture = textureLoader.load('./obj/Planet_Texture.jpg');
    texture.colorSpace = THREE.SRGBColorSpace;

    const loader = new OBJLoader(manager);
    loader.load('./obj/Planet.obj', function (obj) {
        sceneObject = obj;
    });

    renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });
    renderer.setClearColor(0x000000, 0);
    document.getElementById("three-canvas").appendChild(renderer.domElement);

    window.addEventListener('resize', onWindowResize);
    onWindowResize();
    render();
}

function onWindowResize() {
    const container = document.getElementById("three-canvas");
    const size = Math.min(window.innerWidth, window.innerHeight, 512);
    renderer.setSize(size, size);
    camera.aspect = 1;
    camera.updateProjectionMatrix();

    renderer.domElement.style.width = size + 'px';
    renderer.domElement.style.height = size + 'px';
}

function render() {
    requestAnimationFrame(render);
    if (sceneReady) {
        const delta = clock.getDelta();
        sceneObject.rotation.y += delta;
    }
    renderer.render(scene, camera);
}