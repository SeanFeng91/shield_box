# 项目变更日志 (PROJECT_CHANGELOG.md)

本文档记录了"实时3D空间磁场监测与分析平台"从初始构思到当前状态的主要开发步骤、需求变更和问题修复过程。

## 初始主要重构阶段 (截至2025-05-23) 

本文档的此阶段详细记录了从Gradio原型到完成前后端分离架构的初步搭建和核心功能实现的整个过程。

### 一、背景：Gradio原型与初期探索

*   **原始目标**: 优化一个基于Python和Gradio的实时3D空间测点数据可视化应用。
*   **遇到的主要痛点**:
    *   Gradio中3D图表 (Plotly) 刷新时视角重置问题。
    *   数据表格刷新时闪烁以及滚动条体验不佳。
*   **在Gradio框架内的尝试**:
    *   针对3D图表视角问题，研究了Plotly的 `uirevision="true"` 属性。
    *   为改善表格刷新体验，尝试将 `gr.HTML` 替换为 `gr.DataFrame`，但因版本问题遇到参数不支持的情况。
*   **Gradio内3D可视化增强尝试**:
    *   需求：显示900mm屏蔽箱（一面隐藏）、200mm干扰源。
    *   实现：通过修改Gradio应用中的 `create_3d_plot` 函数，利用Plotly的 `Mesh3d` 对象来渲染这些3D元素。

### 二、核心决策：架构重构为前后端分离

*   **驱动因素**: 为克服Gradio在复杂交互和定制化方面的限制，并追求更专业、可扩展的解决方案。
*   **技术选型确定**:
    *   **前端**: Vue.js (v3, `<script setup>`), Three.js (3D可视化), Tailwind CSS (UI), Chart.js (via `vue-chartjs` for 波形图), Vue Router, Axios。
    *   **后端**: Python (v3.x), FastAPI (API框架), Tortoise ORM (SQLite异步ORM), Uvicorn (ASGI服务器), SQLite (数据库)。

### 三、后端系统开发 (`main_api.py`) - 本阶段成果

1.  **FastAPI应用基础**: 初始化FastAPI实例，配置CORS中间件允许所有源（开发便利）。
2.  **数据库集成**: 使用Tortoise ORM与SQLite (`sensor_data.db`)，定义了 `TimestampedChannelData` 模型（`channel_values` 以JSON形式存储30个通道数据）。数据库模式可自动生成和更新。
3.  **API数据模型 (Pydantic)**: 定义了严格的数据结构用于API请求和响应，包括：
    *   `DataIngestPayload` (接收来自采集脚本的数据)。
    *   `SpatialPointData`, `LatestSpatialDataResponse` (用于3D可视化和表格数据)。
    *   `WaveformPoint`, `WaveformDataResponse` (用于波形图数据)。
4.  **核心API端点**:
    *   **`POST /api/v1/data-ingest`**: 接收通道数据和时间戳，存入数据库，并更新内存中的波形历史数据队列。
    *   **`GET /api/v1/latest-spatial-data`**: 提供最新的原始通道数据和基于预设 `SPATIAL_POINTS` 配置（10个测点，含名称、坐标、关联通道）计算得到的空间测点数据（包括幅值和三轴分量）。原始通道值直接返回，数值放大操作移至前端。
    *   **`GET /api/v1/waveform-data`**: 根据请求的通道索引，从内存中的 `waveform_history` (`asyncio.Queue` 队列，每个通道一个，大小为10000点，约10秒@1000Hz）提供历史数据点。实现了无损读取队列数据返回给客户端。
5.  **内存数据处理**: `process_and_store_waveform_data` 函数负责将采集数据的时间戳转换为毫秒，并维护各通道的波形数据队列。

### 四、数据采集脚本改进 (`collect_data.py`) - 本阶段成果

*   **功能调整**: 移除了原有的本地CSV文件写入功能。
*   **与后端集成**: 使用 `requests` 库，在数据接收事件 (`handle_data_received`) 中，将实时采集的数据（30通道）和ISO格式时间戳通过HTTP POST请求发送至FastAPI后端的 `/api/v1/data-ingest` 端点。
*   **数据预处理**: 增加了对原始数据长度的检查。由于硬件实际返回32通道数据，而系统按30通道设计，脚本中加入了逻辑，在发送前截取前30个通道的数据。对非预期长度的数据进行警告和处理。

### 五、前端Vue.js应用开发 (`frontend-vue/`) - 本阶段成果

#### 1. 项目初始化与构建配置
*   使用Vite初始化Vue 3 + TypeScript项目。
*   安装并配置了核心依赖，包括UI框架 (Tailwind CSS)、3D渲染 (Three.js)、图表 (Chart.js, vue-chartjs)、路由 (Vue Router)、HTTP客户端 (Axios) 及相关类型定义和Vite插件 (`@vitejs/plugin-vue`)。
*   逐步建立和完善了包括 `package.json`, `tsconfig.json`, `index.html`, `tailwind.config.js`, `postcss.config.js`, `vite.config.ts` (含路径别名`@`) 等核心配置文件。

#### 2. API服务封装 (`src/services/api.ts`)
*   创建了 `axios` 实例，并封装了与后端API交互的函数：`fetchLatestSpatialData` 和 `fetchWaveformData`。
*   定义了与API对应的TypeScript数据接口 (`SpatialPoint`, `LatestSpatialDataResponse`, `WaveformDataResponseItem`等)。
*   在 `fetchLatestSpatialData` 中实现了关键的数据映射逻辑，将后端坐标数组和幅值转换为前端所需的对象结构，并加入了 `channel_values` 用于3D标签显示三轴分量。

#### 3. 应用路由与布局 (`src/router/index.ts`, `src/App.vue`, `src/components/layout/`)
*   配置了Vue Router，包含"实时数据监控" (`/`) 和"算法分析与预测" (`/analysis`) 两个主视图。
*   `App.vue` 作为根组件，集成了顶部导航栏 (`TheNavbar.vue`) 和路由视图 (`<router-view>`)。

#### 4. 核心视图与组件 (`src/views/`, `src/components/dashboard/`)

*   **`RealtimeDataView.vue`**: 
    *   作为实时监控页面的主容器，定期获取并分发最新数据给子组件。
    *   定义了 `spatialPointsConfig` 以驱动多个波形图和数据表的高亮。
    *   多次调整了其内部栅格布局和子组件的高度分配CSS，以优化显示效果。
*   **`AnalysisView.vue`**: 
    *   当前作为开发中的占位符页面。
    *   修复了因未导入 `onMounted` 导致的路由切换错误。
*   **`DataTableComponent.vue`**: 
    *   以表格形式展示30个原始通道的最新数值（已乘以100000），并高亮与空间测点相关的通道。
*   **`WaveformChartComponent.vue`**: 
    *   为每个空间测点（关联3个通道）渲染一个波形图。
    *   Y轴数值乘以100000显示。
    *   优化了加载和更新逻辑，避免图表闪烁。
    *   实现了X轴的10秒滑动时间窗口功能 (通过将X轴设为 `type: 'time'`，动态更新 `min`/`max` 时间戳)。
    *   修复了多次图表高度和父容器背景不匹配的问题。
*   **`ThreeSceneComponent.vue`**: 
    *   实现了包含屏蔽箱（可调坐标系，一面半透明）、干扰源和10个动态空间测点（球体颜色随值变化）的3D场景。
    *   为每个3D测点添加了CSS2D文本标签，显示其名称和三轴分量值。
    *   修复了3D测点初始只显示一个、标签定位不准、标签样式不佳等多个问题。
    *   集成了 `OrbitControls` 进行场景交互。

### 六、主要调试与问题解决回顾 - 本阶段成果

*   **类型与模块错误**: 通过完善 `tsconfig.json`、`vite.config.ts`（路径别名）、确保依赖完整（包括 `@types` 包）解决。
*   **API数据校验错误 (422)**: 通过在采集脚本中对数据进行截断（32->30通道）解决。
*   **Vite构建/运行问题**: 确保 `index.html` 存在，`@vitejs/plugin-vue` 正确配置。
*   **Vue特定问题**: 移除不必要的编译器宏导入；修复 `AnalysisView.vue` 中 `onMounted` 未导入的错误。
*   **数据逻辑与显示问题**:
    *   修正了 `api.ts` 中3D测点坐标映射，使其正确显示。
    *   统一了前端各处数值放大100000倍的显示逻辑。
    *   解决了波形图X轴不滑动、数据点格式错误的问题。
    *   解决了3D场景中标签的定位、样式及内容显示问题。
    *   解决了波形图卡片高度/背景不匹配的问题。

### 七、当前状态 (截至2025-05-23)

项目已具备前后端分离的完整框架。前端能实时展示3D空间数据、数据表格和多个独立的波形图，并具有基本的导航和页面结构。后端能接收数据、存入数据库并通过API提供服务。

---
*(未来新的迭代可以从这里开始，例如)*

## 迭代阶段：YYYY-MM-DD (新功能XX与优化YY)

### 需求1：...
*   **调整内容**: ...
*   **结果**: ...

### 问题修复：...
*   **问题描述**: ...
*   **修复方案**: ...
--- 