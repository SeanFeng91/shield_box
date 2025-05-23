import axios from 'axios';

// 后端API的基础URL，确保与您的FastAPI服务地址一致
// 在开发过程中，如果前端和后端在不同端口上运行，您可能需要处理CORS（已在FastAPI中配置）
const API_BASE_URL = 'http://localhost:8000/api/v1';

// 创建一个axios实例，可以进行一些全局配置
const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// 定义从后端获取的数据类型 (与main_api.py中的Pydantic模型对应)
// 建议将这些类型定义在一个共享的类型文件中，或者从后端API规范自动生成
// 为简化，这里手动定义一些关键类型

export interface SpatialPoint {
  id: number; // 或 string，取决于后端如何生成
  name: string;
  x: number;    // 坐标 x (mm)
  y: number;    // 坐标 y (mm)
  z: number;    // 坐标 z (mm)
  value: number; // 幅值 (magnitude)
  channel_values?: number[]; // <-- 新增：原始通道值
}

export interface LatestSpatialDataResponse {
  status_message: string;
  spatial_points_data: SpatialPoint[] | null;
  data_timestamp: string | null; // ISO 格式的时间戳
  raw_channel_data: number[] | null; // 30个原始通道数据
}

export interface WaveformPoint {
  timestamp_ms: number; // 毫秒级时间戳
  value: number;
}

export interface WaveformDataResponseItem {
  channel_id: number;
  data_points: WaveformPoint[];
}

// --- Raw types from Backend to help with mapping ---
interface BackendSpatialPointData {
    name: string;
    coords: [number, number, number]; // [x, y, z]
    channels_indices: number[];
    current_values: number[]; 
    magnitude: number;
    // id?: number; // Backend might also send an id
}

interface BackendLatestSpatialDataResponse {
  data_timestamp: string | null;
  raw_channel_data: number[] | null; 
  spatial_points_data: BackendSpatialPointData[] | null;
  status_message: string;
}

/**
 * 从后端获取最新的空间数据和原始通道数据。
 */
export const fetchLatestSpatialData = async (): Promise<LatestSpatialDataResponse> => {
  try {
    // Fetch with the backend's response type
    const response = await apiClient.get<BackendLatestSpatialDataResponse>('/latest-spatial-data');
    const backendData = response.data;

    // Map backend data to frontend data structure
    let mappedSpatialPoints: SpatialPoint[] | null = null;
    if (backendData.spatial_points_data) {
      mappedSpatialPoints = backendData.spatial_points_data.map((bp, index) => ({
        id: index, // Using index as a temporary ID. Ideally, backend sends a stable ID.
        name: bp.name,
        x: bp.coords[0],
        y: bp.coords[1],
        z: bp.coords[2],
        value: bp.magnitude, // map magnitude to frontend's 'value'
        channel_values: bp.current_values, // <-- 新增：映射原始通道值
      }));
    }

    return {
      status_message: backendData.status_message,
      spatial_points_data: mappedSpatialPoints,
      data_timestamp: backendData.data_timestamp,
      raw_channel_data: backendData.raw_channel_data,
    };

  } catch (error) {
    console.error('Error fetching latest spatial data:', error);
    // 根据您的错误处理策略，可以抛出错误或返回一个错误状态的对象
    throw error; 
  }
};

/**
 * 从后端获取指定通道的波形数据。
 * @param channelIndices - 一个包含通道索引的数组，例如 [0, 1, 2]
 * @param durationSeconds - 可选的持续时间（秒），后端当前实现可能不完全按此过滤
 */
export const fetchWaveformData = async (
  channelIndices: number[],
  durationSeconds?: number
): Promise<WaveformDataResponseItem[]> => {
  try {
    const params = new URLSearchParams();
    params.append('channel_indices', channelIndices.join(','));
    if (durationSeconds !== undefined) {
      params.append('duration_seconds', durationSeconds.toString());
    }

    const response = await apiClient.get<WaveformDataResponseItem[]>(`/waveform-data`, { params });
    return response.data;
  } catch (error) {
    console.error('Error fetching waveform data:', error);
    throw error;
  }
};

// 未来可以添加更多与后端API交互的函数
// 例如:
// export const triggerAlgorithmAnalysis = async (params: any) => { ... }
// export const fetchPredictionData = async () => { ... } 