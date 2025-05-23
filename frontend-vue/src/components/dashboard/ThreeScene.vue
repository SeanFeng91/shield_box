<template>
  <div ref="sceneContainer" class="three-scene-container w-full h-full">
    <div v-if="!isInitialized" class="flex items-center justify-center h-full">
      <p class="text-gray-500">正在初始化3D场景...</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch, toRefs, nextTick, withDefaults } from 'vue';
import * as THREE from 'three';
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls.js';
import { CSS2DRenderer, CSS2DObject } from 'three/examples/jsm/renderers/CSS2DRenderer.js';
import type { SpatialPoint } from '@/services/api'; // 使用路径别名

interface Props {
  spatialData: SpatialPoint[] | null | undefined;
  boxSize?: number; // 屏蔽箱尺寸 (mm)
  interferenceDiameter?: number; // 干扰源直径 (mm)
}

const props = withDefaults(defineProps<Props>(), {
  boxSize: 900,
  interferenceDiameter: 200,
});

const { spatialData, boxSize, interferenceDiameter } = toRefs(props);

const sceneContainer = ref<HTMLDivElement | null>(null);
const isInitialized = ref(false);

let scene: THREE.Scene;
let camera: THREE.PerspectiveCamera;
let renderer: THREE.WebGLRenderer;
let controls: OrbitControls;
let labelRenderer: CSS2DRenderer;
// let animationFrameId: number;

// 3D对象引用
let shieldingBoxMesh: THREE.Group; // 使用Group来组合面
let interferenceSourceMesh: THREE.Mesh;
const spatialPointMeshes: THREE.Mesh[] = []; // 存储测点网格对象

const initThreeScene = () => {
  if (!sceneContainer.value) return;

  const width = sceneContainer.value.clientWidth;
  const height = sceneContainer.value.clientHeight;

  // Scene
  scene = new THREE.Scene();
  scene.background = new THREE.Color(0xf0f0f0);

  // Camera
  camera = new THREE.PerspectiveCamera(50, width / height, 1, 2000);
  camera.position.set(boxSize.value * 0.75, boxSize.value * 0.75, boxSize.value * 1.5); // Initial position relative to (0,0,0)
  camera.lookAt(boxSize.value / 2, boxSize.value / 2, boxSize.value / 2); // Look at the center of the box

  // Renderer
  renderer = new THREE.WebGLRenderer({ antialias: true });
  renderer.setSize(width, height);
  renderer.setAnimationLoop(animate);
  sceneContainer.value.appendChild(renderer.domElement);

  // Label Renderer
  labelRenderer = new CSS2DRenderer();
  labelRenderer.setSize(width, height);
  labelRenderer.domElement.style.position = 'absolute';
  labelRenderer.domElement.style.top = '0px';
  labelRenderer.domElement.style.pointerEvents = 'none'; //标签不应捕获鼠标事件
  sceneContainer.value.appendChild(labelRenderer.domElement);

  // Controls
  controls = new OrbitControls(camera, renderer.domElement);
  controls.enableDamping = true;
  controls.dampingFactor = 0.05;
  controls.screenSpacePanning = false;
  controls.minDistance = boxSize.value / 5;
  controls.maxDistance = boxSize.value * 3;
  controls.target.set(boxSize.value / 2, boxSize.value / 2, boxSize.value / 2);
  controls.update();

  // Lighting
  const ambientLight = new THREE.AmbientLight(0xffffff, 0.6);
  scene.add(ambientLight);
  const directionalLight = new THREE.DirectionalLight(0xffffff, 0.8);
  directionalLight.position.set(1, 1, 1).normalize();
  scene.add(directionalLight);
  
  // Axes Helper (for debugging)
  // const axesHelper = new THREE.AxesHelper(boxSize.value * 1.5); // 稍微增大坐标轴辅助线长度
  // scene.add(axesHelper); // Temporarily commented out

  // Create static scene objects
  createShieldingBox();
  createInterferenceSource();

  isInitialized.value = true;
  animate();
};

const createShieldingBox = () => {
  const size = boxSize.value;
  // halfSize is not really needed if box corner is at 0,0,0 and center at size/2,size/2,size/2
  shieldingBoxMesh = new THREE.Group();

  const geometry = new THREE.PlaneGeometry(size, size);
  
  const materialParams = { color: 0xcccccc, side: THREE.DoubleSide, transparent: true, opacity: 0.8 };
  const edgeMaterial = new THREE.LineBasicMaterial({ color: 0x333333, linewidth: 2 });

  // Faces relative to the group's origin (which will be the box center)
  const facesInfo = [
    { name: 'front', position: [0, 0, size / 2], rotation: [0, 0, 0], material: new THREE.MeshPhongMaterial({...materialParams, opacity: 0.2}) },
    { name: 'back', position: [0, 0, -size / 2], rotation: [0, Math.PI, 0], material: new THREE.MeshPhongMaterial(materialParams) },
    { name: 'top', position: [0, size / 2, 0], rotation: [-Math.PI / 2, 0, 0], material: new THREE.MeshPhongMaterial(materialParams) },
    { name: 'bottom', position: [0, -size / 2, 0], rotation: [Math.PI / 2, 0, 0], material: new THREE.MeshPhongMaterial(materialParams) },
    { name: 'left', position: [-size / 2, 0, 0], rotation: [0, -Math.PI / 2, 0], material: new THREE.MeshPhongMaterial(materialParams) },
    { name: 'right', position: [size / 2, 0, 0], rotation: [0, Math.PI / 2, 0], material: new THREE.MeshPhongMaterial(materialParams) },
  ];

  facesInfo.forEach(faceInfo => {
    const faceMesh = new THREE.Mesh(geometry, faceInfo.material);
    faceMesh.position.set(faceInfo.position[0], faceInfo.position[1], faceInfo.position[2]);
    faceMesh.rotation.set(faceInfo.rotation[0], faceInfo.rotation[1], faceInfo.rotation[2]);
    shieldingBoxMesh.add(faceMesh);

    const edges = new THREE.EdgesGeometry(geometry);
    const lineSegments = new THREE.LineSegments(edges, edgeMaterial);
    faceMesh.add(lineSegments);
  });

  // Set the entire group's position so that its corner (min x,y,z) is at (0,0,0)
  // This means the center of the group (box) should be at (size/2, size/2, size/2)
  shieldingBoxMesh.position.set(size / 2, size / 2, size / 2);

  scene.add(shieldingBoxMesh);
};

const createInterferenceSource = () => {
  const diameter = interferenceDiameter.value;
  const radius = diameter / 2;
  // const boxHalfSize = boxSize.value / 2; // No longer needed if we use absolute coords

  const geometry = new THREE.CircleGeometry(radius, 32);
  const material = new THREE.MeshPhongMaterial({ color: 0xff0000, side: THREE.DoubleSide });
  interferenceSourceMesh = new THREE.Mesh(geometry, material);

  // Position it relative to the new box coordinates.
  // Example: On the face x = boxSize (used to be x = boxHalfSize)
  // Let's place it on the center of the face at x = boxSize.value
  // The box spans from 0 to boxSize.value on X,Y,Z.
  // So, center of that face is (boxSize.value, boxSize.value/2, boxSize.value/2)
  interferenceSourceMesh.position.set(boxSize.value + 1, boxSize.value / 2, boxSize.value / 2); 
  interferenceSourceMesh.rotation.y = Math.PI / 2; 

  scene.add(interferenceSourceMesh);
};

const updateSpatialPoints = () => {
  // Clear existing points
  spatialPointMeshes.forEach(mesh => scene.remove(mesh));
  spatialPointMeshes.length = 0;

  if (!spatialData.value) {
    console.log('ThreeScene: spatialData is null or undefined, no points to update.');
    return;
  }
  
  console.log('ThreeScene: Updating spatial points. Received data:', JSON.parse(JSON.stringify(spatialData.value))); // Log received data

  // 定义测点基础几何体和材质
  const pointGeometry = new THREE.SphereGeometry(15, 16, 16); // 测点半径15mm

  spatialData.value.forEach((point, index) => {
    console.log(`ThreeScene: Processing point ${index}:`, JSON.parse(JSON.stringify(point))); // Log each point

    // 幅值到颜色的映射 (示例: 蓝-绿-红)
    const color = new THREE.Color();
    
    const rawValue = point.value || 0;
    const displayValue = rawValue * 100000; // 先乘以100000

    const MAX_EXPECTED_DISPLAY_VALUE = 500000; // 放大后值的归一化上限
    const normalizedValue = Math.min(Math.max(displayValue, 0), MAX_EXPECTED_DISPLAY_VALUE) / MAX_EXPECTED_DISPLAY_VALUE; 
    
    color.setHSL( (1.0 - normalizedValue) * 0.7, 1.0, 0.5); // 从蓝色 (0.7 HSL hue) 到红色 (0 HSL hue)

    const pointMaterial = new THREE.MeshPhongMaterial({ color: color });
    const pointMesh = new THREE.Mesh(pointGeometry, pointMaterial);
    
    // 设置测点位置，需要根据API数据中的坐标系进行调整
    // 假设API返回的x, y, z单位是mm，并且中心是(0,0,0)
    // 假设 point.x, point.y, point.z 是相对于屏蔽箱中心的坐标
    pointMesh.position.set(point.x || 0, point.y || 0, point.z || 0); 
    console.log(`ThreeScene: Point ${index} position set to:`, pointMesh.position.x, pointMesh.position.y, pointMesh.position.z);
    
    scene.add(pointMesh);
    spatialPointMeshes.push(pointMesh);
  });
  console.log('ThreeScene: Total point meshes created:', spatialPointMeshes.length);
};


const animate = () => {
  // animationFrameId = requestAnimationFrame(animate); // 使用 setAnimationLoop 后不再需要
  controls.update();
  renderer.render(scene, camera);
  labelRenderer.render(scene, camera); // 在动画循环中渲染标签
};

const onWindowResize = () => {
  if (!sceneContainer.value || !camera || !renderer || !labelRenderer) return; // 添加 labelRenderer 检查
  const width = sceneContainer.value.clientWidth;
  const height = sceneContainer.value.clientHeight;

  camera.aspect = width / height;
  camera.updateProjectionMatrix();

  renderer.setSize(width, height);
  labelRenderer.setSize(width, height); // 调整标签渲染器大小
};

onMounted(async () => {
  await nextTick(); // 等待DOM元素渲染完成
  if (sceneContainer.value) {
    initThreeScene();
    window.addEventListener('resize', onWindowResize);
  } else {
    console.error("ThreeScene: sceneContainer is not available on mount.");
  }
});

onUnmounted(() => {
  console.log("ThreeScene unmounting...");
  if (renderer) {
    renderer.setAnimationLoop(null); // 停止动画循环
    renderer.dispose();
    if (renderer.domElement.parentElement) {
      renderer.domElement.parentElement.removeChild(renderer.domElement);
    }
  }
  if (labelRenderer && labelRenderer.domElement && labelRenderer.domElement.parentElement) { // 清理标签渲染器
      labelRenderer.domElement.parentElement.removeChild(labelRenderer.domElement);
  }
  if (controls) {
    controls.dispose();
  }
  // 其他清理，如几何体、材质、纹理等，如果复杂场景需要手动管理
  isInitialized.value = false;
  window.removeEventListener('resize', onWindowResize);
});

watch(spatialData, (newData) => {
  if (isInitialized.value && newData) {
    updateSpatialPoints();
  }
}, { deep: true });

// 暴露给父组件或模板引用的方法 (如果需要)
// defineExpose({ });
</script>

<style scoped>
.three-scene-container {
  background-color: #f0f0f0; /* 与场景背景色一致 */
}
</style> 