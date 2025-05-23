<template>
  <div class="waveform-chart-container bg-white p-3 rounded-lg shadow h-full">
    <div v-if="isLoading" class="flex items-center justify-center h-full">
      <p class="text-gray-500">加载波形数据中...</p>
    </div>
    <div v-else-if="error" class="flex items-center justify-center h-full">
      <p class="text-red-500">加载错误: {{ error }}</p>
    </div>
    <Line v-if="!isLoading && !error && chartData && chartData.labels && chartData.labels.length > 0" :data="chartData" :options="chartOptions" class="h-full" />
    <div v-else-if="!isLoading && !error" class="flex items-center justify-center h-full">
        <p class="text-gray-500">暂无波形数据。</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch, toRefs, computed, withDefaults } from 'vue';
import { Line } from 'vue-chartjs';
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  LineElement,
  CategoryScale, // For X axis (time)
  LinearScale,   // For Y axis (value)
  PointElement,
  TimeScale, // More appropriate for time-based data if timestamps are absolute
} from 'chart.js';
import 'chartjs-adapter-date-fns'; // Adapter for date/time functionalities
import { fetchWaveformData } from '@/services/api'; // 使用路径别名
import type { WaveformDataResponseItem, WaveformPoint } from '@/services/api'; // 使用路径别名

ChartJS.register(
  Title,
  Tooltip,
  Legend,
  LineElement,
  CategoryScale,
  LinearScale,
  PointElement,
  TimeScale
);

interface Props {
  channelIds: number[]; // 要为此测点显示的通道索引 (通常是3个)
  pointName: string;    // 测点名称，用于图表标题或调试
  refreshInterval?: number; //刷新间隔（毫秒），默认2000
}

const props = withDefaults(defineProps<Props>(), {
    refreshInterval: 2000,
});

const { channelIds, pointName, refreshInterval } = toRefs(props);

const isLoading = ref(true);
const isInitialLoading = ref(true);
const error = ref<string | null>(null);
const chartDataInternal = ref<WaveformDataResponseItem[]>([]);

let intervalId: number | undefined;

const lineColors = ['rgba(255, 99, 132, 0.8)', 'rgba(54, 162, 235, 0.8)', 'rgba(75, 192, 192, 0.8)'];
const pointColors = ['rgb(255, 99, 132)', 'rgb(54, 162, 235)', 'rgb(75, 192, 192)'];

const chartData = computed(() => {
  if (chartDataInternal.value.length === 0) {
    return {
      labels: [],
      datasets: []
    };
  }

  // 假设所有通道的时间戳是对齐的，我们取第一个通道的时间戳作为X轴标签
  // 后端返回的 timestamp_ms 是毫秒级时间戳
  const labels = chartDataInternal.value[0]?.data_points.map(dp => dp.timestamp_ms) || [];

  const datasets = chartDataInternal.value.map((channelWaveform, index) => {
    return {
      label: `通道 ${channelWaveform.channel_id + 1}`,
      data: channelWaveform.data_points.map(dp => dp.value * 100000),
      borderColor: lineColors[index % lineColors.length],
      backgroundColor: pointColors[index % pointColors.length],
      tension: 0.1, // Line tension for smoothing
      pointRadius: 2,
      borderWidth: 1.5,
    };
  });

  return {
    labels,
    datasets
  };
});

const chartOptions = computed(() => ({
  responsive: true,
  maintainAspectRatio: false,
  animation: {
      duration: 0 // 禁用动画以获得更平滑的实时更新感觉
  },
  layout: {
    padding: {
        left: 5,
        right: 10
    }
  },
  scales: {
    x: {
      type: 'time',
      time: {
        unit: 'second',
        tooltipFormat: 'HH:mm:ss', // e.g., 14:30:45
        displayFormats: {
          second: 'HH:mm:ss' // Format for scale labels
        }
      },
      title: {
        display: true,
        text: '时间'
      },
      ticks: {
        maxRotation: 0,
        minRotation: 0,
        autoSkip: true,
        maxTicksLimit: 10 //限制X轴刻度数量以避免拥挤
      }
    },
    y: {
      title: {
        display: true,
        text: '值'
      },
      beginAtZero: false // 根据数据自动调整Y轴起点
    }
  },
  plugins: {
    legend: {
      position: 'top' as const,
    },
    title: {
      display: true,
      text: `${pointName.value} 波形数据`
    }
  }
}));

const loadWaveformData = async () => {
  if (!channelIds.value || channelIds.value.length === 0) {
    isLoading.value = false;
    isInitialLoading.value = false;
    error.value = '未指定通道ID';
    return;
  }

  if (isInitialLoading.value) {
    isLoading.value = true;
  }
  error.value = null;

  try {
    const data = await fetchWaveformData(channelIds.value);
    chartDataInternal.value = data;
    if (isInitialLoading.value) {
        isLoading.value = false;
        isInitialLoading.value = false;
    }
  } catch (err) {
    console.error(`Error fetching waveform data for ${pointName.value} (Channels: ${channelIds.value.join(',')}):`, err);
    error.value = (err as Error).message || '获取波形数据失败';
    chartDataInternal.value = [];
    isLoading.value = false;
    isInitialLoading.value = false;
  }
};

watch(channelIds, (newVal, oldVal) => {
  if (JSON.stringify(newVal) !== JSON.stringify(oldVal)) {
    isInitialLoading.value = true;
    loadWaveformData();
  }
}, { immediate: true, deep: true });

onMounted(() => {
  if (channelIds.value && channelIds.value.length > 0 && isInitialLoading.value) {
      loadWaveformData();
  }
  intervalId = window.setInterval(loadWaveformData, refreshInterval.value);
});

onUnmounted(() => {
  if (intervalId) {
    clearInterval(intervalId);
  }
});

</script>

<style scoped>
.waveform-chart-container {
  min-height: 250px; /* 确保图表容器有最小高度 */
  height: 100%;
}
</style> 