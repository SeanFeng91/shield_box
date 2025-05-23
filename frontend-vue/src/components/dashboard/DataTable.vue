<template>
  <div class="data-table-container bg-white rounded-lg shadow">
    <div v-if="!processedData || processedData.length === 0" class="p-4 text-center text-gray-500">
      <p>等待数据...</p>
    </div>
    <div v-else class="overflow-x-auto">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th scope="col" class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              通道
            </th>
            <th scope="col" class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              数值
            </th>
            <th scope="col" class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              关联测点
            </th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="item in processedData" :key="item.channelName" 
              :class="{'bg-blue-50': item.isSpatialPointChannel}">
            <td class="px-4 py-3 whitespace-nowrap text-sm font-medium text-gray-900">
              {{ item.channelName }}
            </td>
            <td class="px-4 py-3 whitespace-nowrap text-sm text-gray-700">
              {{ (item.value * 100000).toFixed(2) }} <!-- 乘以100000并保留两位小数 -->
            </td>
            <td class="px-4 py-3 whitespace-nowrap text-sm text-gray-500">
              {{ item.spatialPointName || '-' }}
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, toRefs } from 'vue';

interface Props {
  rawChannelData: number[] | null | undefined; // 30个原始通道数据
  // SPATIAL_POINTS的定义，用于查找通道关联的测点
  // 我们需要这个信息来高亮或标记与测点相关的通道
  spatialPointsConfig: Array<{ name: string; channels: number[] }>; 
}

const props = defineProps<Props>();
const { rawChannelData, spatialPointsConfig } = toRefs(props);

interface ProcessedRow {
  channelName: string;
  value: number;
  spatialPointName?: string;
  isSpatialPointChannel: boolean;
}

// 将原始通道数据处理为表格需要的格式
const processedData = computed<ProcessedRow[]>(() => {
  if (!rawChannelData.value || rawChannelData.value.length !== 30) {
    return [];
  }

  // 创建一个从通道索引到测点名称的映射
  const channelToPointMap = new Map<number, string>();
  if (spatialPointsConfig.value) {
    spatialPointsConfig.value.forEach(point => {
      point.channels.forEach(channelIndex => {
        channelToPointMap.set(channelIndex, point.name);
      });
    });
  }

  return rawChannelData.value.map((value, index) => {
    const channelName = `通道 ${index + 1}`;
    const spatialPointName = channelToPointMap.get(index);
    return {
      channelName,
      value,
      spatialPointName,
      isSpatialPointChannel: !!spatialPointName,
    };
  });
});

</script>

<style scoped>
/* 如果需要特定于此组件的样式，可以在这里添加 */
.data-table-container {
  max-height: 600px; /* 与RealtimeDataView中placeholder一致或按需调整 */
  overflow-y: auto;
}
</style> 