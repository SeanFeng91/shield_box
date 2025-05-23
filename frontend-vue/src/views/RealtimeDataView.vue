<template>
  <div class="container mx-auto p-4 space-y-8">
    <header class="mt-4 mb-8">
      <h1 class="text-4xl font-bold text-gray-800 tracking-tight">实时数据监控</h1>
      <p class="text-lg text-gray-600 mt-1">查看最新的传感器数据和空间分布。</p>
    </header>

    <!-- 主要内容区域: 3D图在左，数据表在右 -->
    <div class="grid grid-cols-1 lg:grid-cols-7 gap-6">
      <div class="lg:col-span-5  p-6 rounded-xl  h-[600px] lg:h-[700px]">
        <h2 class="text-2xl font-semibold text-gray-700 mb-4">三维空间可视化</h2>
        <ThreeSceneComponent 
            :spatialData="latestData?.spatial_points_data"
            class="w-full h-[calc(100%-2rem)] border rounded-lg bg-gray-50" 
        />
      </div>

      <div class="lg:col-span-2 p-6 rounded-xl ">
        <h2 class="text-2xl font-semibold text-gray-700 mb-1">最新通道数据</h2>
        <p class="text-sm text-gray-500 mb-4">时间: {{ latestData?.data_timestamp ? new Date(latestData.data_timestamp).toLocaleString() : '加载中...' }}</p>
        <DataTableComponent class="h-full"
            :rawChannelData="latestData?.raw_channel_data"
            :spatialPointsConfig="spatialPointsConfig" 
        />
      </div>
    </div>

    <!-- 波形图区域 -->
    <div class="mt-8">
      <h2 class="text-2xl font-semibold text-gray-700 mb-4">通道波形图</h2>
      <div class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-2 gap-6">
        <div v-for="point in spatialPointsConfig" :key="point.name" class=" p-4 rounded-xl  h-[340px]">
          <h3 class="text-xl font-semibold text-gray-700 mb-2">{{ point.name }}</h3>
          <p class="text-sm text-gray-500 mb-2">通道: {{ point.channels.map(c => c + 1).join(', ') }}</p>
          <WaveformChartComponent :channelIds="point.channels" :pointName="point.name" class="h-[calc(90%-0rem)]" />
        </div>
      </div>
    </div>

    <footer class="mt-8 py-4 text-center">
        <p class="text-sm text-gray-600">状态: {{ latestData?.status_message || '正在连接服务器...' }}</p>
    </footer>

  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue';
import { fetchLatestSpatialData } from '@/services/api'; 
import type { LatestSpatialDataResponse } from '@/services/api';
import DataTableComponent from '../components/dashboard/DataTable.vue';
import WaveformChartComponent from '../components/dashboard/WaveformChart.vue';
import ThreeSceneComponent from '../components/dashboard/ThreeScene.vue'; // 导入3D场景组件

interface SpatialPointConfigItem {
    name: string;
    channels: number[];
}

const spatialPointsConfig = ref<SpatialPointConfigItem[]>([
    { name: "测点1", channels: [0, 1, 2] }, { name: "测点2", channels: [3, 4, 5] },
    { name: "测点3", channels: [6, 7, 8] }, { name: "测点4", channels: [9, 10, 11] },
    { name: "测点5", channels: [12, 13, 14] }, { name: "测点6", channels: [15, 16, 17] },
    { name: "测点7", channels: [18, 19, 20] }, { name: "测点8", channels: [21, 22, 23] },
    { name: "测点9 (磁通门X,Y,Z)", channels: [24, 25, 26] },
    { name: "测点10 (备用)", channels: [27, 28, 29] },
]);

const latestData = ref<LatestSpatialDataResponse | null>(null);
let intervalId: number | undefined;

const loadData = async () => {
  try {
    const response = await fetchLatestSpatialData();
    latestData.value = response;
  } catch (error) {
    console.error("实时数据视图 - 获取最新数据错误:", error);
    latestData.value = {
      status_message: `获取数据失败: ${(error as Error).message}`,
      spatial_points_data: latestData.value?.spatial_points_data || [],
      data_timestamp: null,
      raw_channel_data: null,
    };
  }
};

onMounted(() => {
  console.log("RealtimeDataView mounted");
  loadData(); 
  intervalId = window.setInterval(loadData, 2000);
});

onUnmounted(() => {
  if (intervalId) {
    clearInterval(intervalId);
  }
  console.log("RealtimeDataView unmounted");
});

</script>

<style scoped>
/* #three-scene-container 已被移除，因为 ThreeSceneComponent 自己管理容器 */
</style> 