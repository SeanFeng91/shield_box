# 实时3D空间磁场监测与分析平台

## 1. 项目概览

本项目是一个前后端分离的Web应用，旨在实时采集、可视化和分析多通道传感器数据，特别是空间磁场数据。它提供了一个用户友好的界面来监控传感器状态、查看3D空间中的测点分布及其数值、观察各通道数据的实时波形，并为未来的算法分析与预测功能预留了扩展空间。

## 2. 主要功能

*   **实时数据采集**：后端通过Python脚本与硬件设备接口，实时获取多通道（当前为30通道）数据。
*   **数据存储**：采集到的数据通过API发送到后端，并存储在SQLite数据库中。
*   **后端API服务**：使用FastAPI框架构建，提供以下主要接口：
    *   数据接收接口 (`/api/v1/data-ingest`)：用于采集脚本向后端推送数据。
    *   最新空间数据接口 (`/api/v1/latest-spatial-data`)：提供最新的原始通道数据和处理后的10个空间测点数据（含坐标和幅值/三轴分量）。
    *   波形数据接口 (`/api/v1/waveform-data`)：提供指定通道的历史波形数据（最近10秒）。
*   **前端实时可视化 (Vue.js)**：
    *   **导航**：清晰的顶部导航栏，可在"实时数据监控"和"算法分析与预测"页面间切换。
    *   **三维空间可视化 (Three.js)**：
        *   展示一个900mm x 900mm x 900mm的立方体屏蔽箱（一个面半透明以便观察内部）。
        *   在屏蔽箱一侧中心显示一个直径200mm的圆形干扰源。
        *   实时展示10个空间测点（以球体表示），球体颜色根据其测量值的强度动态变化。
        *   在每个3D测点旁显示其名称和三个通道的实时数值（X, Y, Z分量）。
        *   支持通过鼠标进行3D场景的旋转、缩放和平移。
    *   **数据表格**：实时展示所有30个原始通道的最新数值，并高亮与空间测点相关的通道。数值按100000倍放大显示。
    *   **通道波形图 (Chart.js)**：为每个空间测点（及其关联的3个通道）生成实时滚动的波形图，显示最近10秒的数据。Y轴数值按100000倍放大显示。
*   **可扩展性**："算法分析与预测"页面为未来集成数据分析模型和预测算法提供了框架。

## 3. 技术栈

*   **后端**：
    *   Python 3.x
    *   FastAPI: 用于构建API服务。
    *   Tortoise ORM: 异步ORM，用于与SQLite数据库交互。
    *   Uvicorn: ASGI服务器，用于运行FastAPI应用。
    *   SQLite: 轻量级磁盘数据库。
    *   `requests`: 用于`collect_data.py`向FastAPI发送数据。
    *   `pythonnet`: 用于`collect_data.py`与.NET DLL硬件驱动交互。
*   **前端**：
    *   Vue.js 3 (使用 `<script setup>` 语法)
    *   Vite: 构建工具和开发服务器。
    *   TypeScript: 用于类型安全。
    *   Three.js: 用于3D场景渲染。
    *   Chart.js (via `vue-chartjs`): 用于绘制2D波形图。
    *   Vue Router: 用于前端路由。
    *   Axios: 用于HTTP API请求。
    *   Tailwind CSS: 工具优先的CSS框架，用于快速构建UI。
*   **硬件交互** (包含在后端Python脚本中):
    *   依赖特定的 `.dll` 文件 (例如 `MCMag30FDevice.dll`, `CyUSB.dll`) 和 .NET 运行时。

## 4. 项目结构

```
SDK_X86/
├── frontend-vue/         # 前端Vue项目目录
│   ├── public/
│   │   ├── assets/
│   │   ├── src/
│   │   │   ├── components/   # Vue组件 (布局, 仪表盘子组件等)
│   │   │   ├── router/       # Vue Router配置
│   │   │   ├── services/     # API服务封装 (api.ts)
│   │   │   ├── views/        # 页面级组件 (RealtimeDataView.vue, AnalysisView.vue)
│   │   │   ├── App.vue       # 根组件
│   │   │   ├── main.ts       # Vue应用入口
│   │   │   └── style.css     # 全局及Tailwind CSS入口
│   │   ├── index.html
│   │   ├── package.json
│   │   ├── vite.config.ts
│   │   ├── tsconfig.json
│   │   └── ... (其他配置文件)
│   ├── main_api.py           # FastAPI 后端应用
│   ├── collect_data.py       # 数据采集脚本 (与硬件交互并发送数据到后端)
│   ├── sensor_data.db        # SQLite 数据库文件 (后端运行时自动创建)
│   ├── MCMag30FDevice.dll    # 示例：硬件驱动DLL (需用户提供)
│   ├── CyUSB.dll             # 示例：硬件驱动依赖DLL (需用户提供)
│   ├── README.md             # 本文件
│   └── PROJECT_CHANGELOG.md  # 项目变更日志 (稍后创建)
```

## 5. 安装与运行

**前提条件:**
*   Node.js (v16+推荐) 和 npm (或 yarn/pnpm) 用于前端。
*   Python (v3.8+推荐) 和 pip 用于后端。
*   如果运行 `collect_data.py` 与实际硬件交互：
    *   相应的.NET运行时环境。
    *   将硬件所需的 `.dll` 文件 (如 `MCMag30FDevice.dll`, `CyUSB.dll`) 放置在 `SDK_X86` 根目录下 (与 `collect_data.py` 同级)。

### 5.1 后端设置与运行

1.  **导航到项目根目录**:
    ```bash
    cd SDK_X86
    ```

2.  **创建并激活Python虚拟环境** (推荐):
    ```bash
    python -m venv venv
    # Windows
    .\venv\Scripts\activate
    # macOS/Linux
    # source venv/bin/activate
    ```

3.  **安装Python依赖**:
    ```bash
    pip install fastapi uvicorn tortoise-orm aiosqlite pydantic requests pythonnet numpy
    ```
    *(注意: `pythonnet` 可能需要在Windows上更容易安装，其他系统可能需要额外配置。)*

4.  **运行FastAPI后端服务**:
    ```bash
    uvicorn main_api:app --reload --host 0.0.0.0 --port 8000
    ```
    *   `--reload` 参数使得在代码更改时服务器会自动重启，方便开发。
    *   服务器将在 `http://localhost:8000` 上运行。API文档可在 `http://localhost:8000/docs` 查看。
    *   首次运行时，如果 `sensor_data.db` 不存在，Tortoise ORM会自动创建它。

### 5.2 前端设置与运行

1.  **导航到前端项目目录**:
    ```bash
    cd SDK_X86/frontend-vue
    ```

2.  **安装npm依赖**:
    ```bash
    npm install
    ```
    *(如果项目中提供了 `package-lock.json`，通常使用 `npm ci` 以确保依赖版本一致性)*

3.  **运行Vite开发服务器**:
    ```bash
    npm run dev
    ```
    *   开发服务器通常会运行在 `http://localhost:5173` (或其他可用端口，请查看终端输出)。
    *   在浏览器中打开此地址即可访问应用。

### 5.3 运行数据采集脚本 (可选，用于模拟或真实数据输入)

1.  确保后端API服务正在运行。
2.  如果需要与真实硬件交互，请确保DLL文件已放置在 `SDK_X86` 目录下，并且硬件已连接。
3.  在**新的终端**中，导航到项目根目录 `SDK_X86` 并激活虚拟环境 (如果之前未激活或已关闭)。
4.  运行采集脚本:
    ```bash
    python collect_data.py
    ```
    *   此脚本会尝试连接硬件（如果配置如此）或模拟数据，并将数据通过HTTP POST请求发送到运行中的FastAPI后端的 `/api/v1/data-ingest` 端点。
    *   终端会输出采集和发送状态。

## 6. 使用说明

1.  **启动后端服务** (见 5.1)。
2.  **启动前端开发服务器** (见 5.2)。
3.  **（可选）启动数据采集脚本** (见 5.3)。
4.  在浏览器中打开前端应用的URL (通常是 `http://localhost:5173`)。
5.  **实时数据监控页面**：
    *   查看3D空间可视化区域，通过鼠标操作场景。观察测点颜色和标签数值的变化。
    *   查看最新的30个通道数据表格。
    *   查看下方各个测点的实时波形图。
6.  **算法分析与预测页面**：
    *   此页面当前为占位符，显示"正在开发中"。

## 7. 注意事项与未来扩展

*   **硬件依赖**：`collect_data.py` 脚本当前配置为与特定的.NET DLL交互。如需适配不同硬件，需要修改此脚本中的DLL加载和设备控制逻辑。
*   **数据持久性**：波形图数据当前主要存储在后端的内存队列中 (`waveform_history`)，这意味着如果后端重启，最近的波形历史会丢失（除了已存入数据库的快照）。如需更强的持久性，可考虑将波形数据也存入数据库或时序数据库。
*   **错误处理**：已实现基本的错误处理和日志输出，但可以根据生产环境需求进一步增强。
*   **安全性**：当前的CORS配置 (`allow_origins=["*"]`) 较为宽松，适合开发。生产部署时应配置为仅允许前端应用的源。API也未包含认证授权机制。
*   **性能优化**：对于极高频率的数据更新或大量历史数据查询，可能需要进一步优化数据库查询、API响应和前端渲染。
*   **算法集成**：后端和前端已为未来的算法分析模块预留了结构，可以方便地添加新的API端点和前端组件来支持特定算法的参数配置、执行和结果展示。 