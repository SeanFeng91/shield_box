# gradio_spatial_visualization.py
import os
import glob
import time
import pandas as pd
import numpy as np
import plotly.graph_objects as go
import gradio as gr
from datetime import datetime, timedelta
import chardet  # 添加chardet库用于检测文件编码
import json  # 用于JSON处理
from collections import deque  # 用于高效存储滚动数据

# 定义10个空间测点的坐标 (x, y, z)，单位为毫米
# 在900x900x900的立方空间内分布
SPATIAL_POINTS = [
    {"name": "测点1", "coords": (100, 100, 300), "channels": [0, 1, 2]},
    {"name": "测点2", "coords": (100, 500, 300), "channels": [3, 4, 5]},
    {"name": "测点3", "coords": (100, 800, 300), "channels": [6, 7, 8]},
    {"name": "测点4", "coords": (500, 100, 300), "channels": [9, 10, 11]},
    {"name": "测点5", "coords": (500, 500, 300), "channels": [12, 13, 14]},
    {"name": "测点6", "coords": (500, 800, 300), "channels": [15, 16, 17]},
    {"name": "测点7", "coords": (800, 100, 600), "channels": [18, 19, 20]},
    {"name": "测点8", "coords": (800, 500, 600), "channels": [21, 22, 23]},
    {"name": "测点9", "coords": (800, 800, 600), "channels": [24, 25, 26]},
    {"name": "测点10", "coords": (450, 450, 800), "channels": [27, 28, 29]}
]

# 波形图的历史数据存储（存储最近30秒的数据，以便显示最新10秒）
# 使用deque实现高效的数据滚动
waveform_data = {
    'timestamps': deque(maxlen=1000),  # 假设采样率<100Hz，30秒最多3000个样本
    'values': {} # 每个通道的值，格式为 {'通道1': deque([值1, 值2, ...]), ...}
}

# 辅助函数，初始化波形数据存储
def init_waveform_data():
    global waveform_data
    waveform_data['timestamps'] = deque(maxlen=1000)
    waveform_data['values'] = {}
    for i in range(30):
        channel_key = f"通道{i+1}"
        waveform_data['values'][channel_key] = deque(maxlen=1000)

# 初始化波形数据存储
init_waveform_data()

# 查找最新的CSV文件
def find_latest_csv():
    csv_files = glob.glob("magnetic_data_*.csv")
    if not csv_files:
        return None
    
    # 按文件创建时间排序，最新的在最前
    latest_file = max(csv_files, key=os.path.getmtime)
    print(f"找到最新CSV文件: {latest_file}")
    return latest_file

# 从CSV文件读取最新数据
def read_latest_data(file_path):
    try:
        # 首先尝试检测文件编码
        with open(file_path, 'rb') as f:
            raw_data = f.read(10000)  # 读取前10000个字节用于编码检测
            result = chardet.detect(raw_data)
            encoding = result['encoding']
            confidence = result['confidence']
            
            print(f"检测到文件编码: {encoding}，置信度: {confidence}")
            
        # 直接使用csv模块读取原始数据（不使用pandas），避免列映射错误
        import csv
        with open(file_path, 'r', encoding='gbk') as f:
            reader = csv.reader(f)
            rows = list(reader)
            
            if not rows:
                return None, "CSV文件为空"
                
            header = rows[0]  # 表头行
            if len(rows) < 2:
                return None, "CSV文件只有表头行，没有数据"
            
            # 获取最后一行数据
            last_row = rows[-1]
            
            print(f"表头列数: {len(header)}, 最后一行数据列数: {len(last_row)}")
            
            # 创建时间戳和通道值的映射
            latest_data = {}
            
            # 设置时间戳
            latest_data["时间戳"] = last_row[0] if len(last_row) > 0 else "未知时间"
            
            # 正确映射通道值，注意列的偏移
            # 根据检查结果，通道数据从第2列开始，而不是pandas自动解析的结果
            for i in range(30):
                channel_key = f"通道{i+1}"
                # 通道值在原始CSV中从第2列开始（索引1）
                col_index = i + 1  # 通道1对应第2列，索引1
                
                if col_index < len(last_row):
                    try:
                        # 尝试将值转换为浮点数
                        channel_value = float(last_row[col_index])
                        # 放大100000倍使数值更明显
                        latest_data[channel_key] = channel_value * 100000
                    except (ValueError, TypeError):
                        print(f"警告: 通道{i+1}数据'{last_row[col_index]}'无法转换为浮点数")
                        latest_data[channel_key] = 0.0
                else:
                    print(f"警告: CSV数据行中没有通道{i+1}的数据")
                    latest_data[channel_key] = 0.0
            
            # 打印通道映射关系以便调试
            print("\n===== 通道映射关系 =====")
            for i in range(min(10, 30)):  # 只打印前10个通道作为示例
                channel_key = f"通道{i+1}"
                col_index = i + 1
                if col_index < len(last_row):
                    print(f"{channel_key}: CSV列{col_index+1}，原始值={last_row[col_index]}，放大后={latest_data[channel_key]:.1f}")
            
            # 添加时间戳和数据到波形图数据存储
            current_time = datetime.now()
            waveform_data['timestamps'].append(current_time)
            for i in range(30):
                channel_key = f"通道{i+1}"
                if channel_key in latest_data:
                    if channel_key not in waveform_data['values']:
                        waveform_data['values'][channel_key] = deque(maxlen=1000)
                    waveform_data['values'][channel_key].append(latest_data[channel_key])
            
            return latest_data, None
    except Exception as e:
        import traceback
        traceback.print_exc()
        return None, f"读取CSV文件出错: {str(e)}"

# 创建3D测点可视化图
def create_3d_plot(data):
    if data is None:
        # 如果没有数据，创建一个空图
        fig = go.Figure()
        fig.update_layout(
            title="测点数据可视化 - 无数据",
            scene=dict(
                xaxis_title="X (mm)",
                yaxis_title="Y (mm)",
                zaxis_title="Z (mm)",
                aspectmode='cube'
            )
        )
        return fig
    
    # 提取测点坐标
    x_coords = [point["coords"][0] for point in SPATIAL_POINTS]
    y_coords = [point["coords"][1] for point in SPATIAL_POINTS]
    z_coords = [point["coords"][2] for point in SPATIAL_POINTS]
    
    # 计算每个测点的幅值
    magnitudes = []
    hover_texts = []
    text_labels = []
    
    for point in SPATIAL_POINTS:
        ch_indices = point["channels"]
        # 确保ch_indices中的索引有效
        if all(0 <= idx < 30 for idx in ch_indices) and len(ch_indices) == 3:
            # 获取该测点对应的三通道数据
            ch_values = [data[f"通道{idx+1}"] for idx in ch_indices]
            
            # 计算幅值 (x^2 + y^2 + z^2)^0.5
            magnitude = np.sqrt(sum(val**2 for val in ch_values))
            magnitudes.append(magnitude)
            
            # 创建悬停文本（包含更详细信息）
            hover_text = (
                f"{point['name']}<br>"
                f"坐标: ({point['coords'][0]}, {point['coords'][1]}, {point['coords'][2]})<br>"
                f"通道 {ch_indices[0]+1}: {ch_values[0]:.1f}<br>"
                f"通道 {ch_indices[1]+1}: {ch_values[1]:.1f}<br>"
                f"通道 {ch_indices[2]+1}: {ch_values[2]:.1f}<br>"
                f"幅值: {magnitude:.1f}"
            )
            hover_texts.append(hover_text)
            
            # 创建直接显示在图上的文本标签（简洁版本）
            text_label = (
                f"{point['name']}"
                f"({point['coords'][0]},{point['coords'][1]},{point['coords'][2]})"
                f"\n({ch_values[0]:.1f},{ch_values[1]:.1f},{ch_values[2]:.1f})"
            )
            text_labels.append(text_label)
        else:
            magnitudes.append(0)
            hover_texts.append(f"{point['name']}<br>数据无效")
            text_labels.append(f"{point['name']}\n数据无效")
    
    # 创建3D散点图
    fig = go.Figure()
    
    # 1. 定义立方体顶点 (0-900范围)
    cube_vertices = np.array([
        [0, 0, 0], [900, 0, 0], [900, 900, 0], [0, 900, 0],  # 底面 Z=0
        [0, 0, 900], [900, 0, 900], [900, 900, 900], [0, 900, 900]  # 顶面 Z=900
    ])

    # 定义立方体的6个面 (每个面由2个三角形组成)
    # 面的顶点顺序很重要，影响法向量和显示
    cube_faces = np.array([
        [0, 1, 2], [0, 2, 3],  # 底面 Z=0
        [4, 5, 6], [4, 6, 7],  # 顶面 Z=900
        [0, 1, 5], [0, 5, 4],  # 侧面 Y=0
        [2, 3, 7], [2, 7, 6],  # 侧面 Y=900
        [1, 2, 6], [1, 6, 5],  # 侧面 X=900 (干扰源所在的面)
        [0, 3, 7], [0, 7, 4]   # 侧面 X=0 (要设为半透明的面)
    ])

    # 添加立方体的面
    # 面 X=0 (索引10, 11的三角形) 将设为半透明
    opacities = [0.8] * 10 + [0.2] * 2 # 其他面不透明，X=0的面半透明

    fig.add_trace(go.Mesh3d(
        x=cube_vertices[:, 0],
        y=cube_vertices[:, 1],
        z=cube_vertices[:, 2],
        i=cube_faces[:, 0],
        j=cube_faces[:, 1],
        k=cube_faces[:, 2],
        opacity=1.0, # 先设置整体为1，然后通过facecolor控制单个面
        facecolor=[
            'lightgray', 'lightgray', # Z=0
            'lightgray', 'lightgray', # Z=900
            'lightgray', 'lightgray', # Y=0
            'lightgray', 'lightgray', # Y=900
            'lightgray', 'lightgray', # X=900
            'rgba(211,211,211,0.2)', 'rgba(211,211,211,0.2)' # X=0 (半透明)
        ],
        hoverinfo='none',
        name='屏蔽箱'
    ))

    # 2. 绘制干扰源 (直径200mm的圆盘，在X=900mm的面上，中心为(900, 450, 450))
    source_radius = 100  # 半径100mm
    source_center = np.array([900, 450, 450])
    source_color = 'orange'
    N_source_points = 50 # 圆盘边缘点数

    # 创建圆盘的顶点
    # 圆盘平行于YZ平面 (因为在X=900的面上)
    theta = np.linspace(0, 2 * np.pi, N_source_points)
    source_disk_y = source_center[1] + source_radius * np.cos(theta)
    source_disk_z = source_center[2] + source_radius * np.sin(theta)
    source_disk_x = np.full_like(source_disk_y, source_center[0])

    # 为了形成一个可见的圆盘，我们可以创建两个略微分开的圆形，然后用Mesh3d连接它们
    # 或者更简单地，先尝试用一个粗的3D线来表示圆环，或者一个填充的2D圆投影到3D
    # Plotly的Mesh3d更适合做这个，我们需要定义三角面片
    # 圆盘顶点：中心点 + 边缘点
    disk_vertices_x = np.concatenate(([source_center[0]], source_disk_x))
    disk_vertices_y = np.concatenate(([source_center[1]], source_disk_y))
    disk_vertices_z = np.concatenate(([source_center[2]], source_disk_z))

    disk_i = []
    disk_j = []
    disk_k = []
    for n in range(N_source_points):
        disk_i.append(0) # 中心点
        disk_j.append(n + 1)
        disk_k.append(((n + 1) % N_source_points) + 1)
    
    fig.add_trace(go.Mesh3d(
        x=disk_vertices_x,
        y=disk_vertices_y,
        z=disk_vertices_z,
        i=disk_i,
        j=disk_j,
        k=disk_k,
        color=source_color,
        opacity=0.9,
        hoverinfo='text',
        text='干扰源',
        name='干扰源'
    ))

    # 3. 添加原来的测点散点图和文本标注
    # (确保这些在立方体内部并且可见)
    fig.add_trace(go.Scatter3d(
        x=x_coords,
        y=y_coords,
        z=z_coords,
        mode='markers',
        marker=dict(
            size=6,  # 减小球体大小
            color=magnitudes,  # 根据幅值设置颜色
            colorscale='Viridis',  # 颜色标度
            colorbar=dict(title="幅值"),
            showscale=True
        ),
        hovertext=hover_texts,
        hoverinfo="text",
        name="测点",
        visible=True # 确保测点可见
    ))
    
    # 添加坐标和通道数据文本标注
    fig.add_trace(go.Scatter3d(
        x=x_coords,
        y=y_coords,
        z=z_coords,
        mode='text',
        text=text_labels,
        textposition="top center",
        textfont=dict(
            size=10,
            color='black'
        ),
        hoverinfo="none",  # 不显示悬停信息
        name="标签",
        visible=True # 确保标签可见
    ))
    
    # 设置图表布局
    fig.update_layout(
        title="测点数据实时可视化",
        scene=dict(
            xaxis_title="X (mm)",
            yaxis_title="Y (mm)",
            zaxis_title="Z (mm)",
            aspectmode='cube',  # 等比例显示
            xaxis=dict(range=[0, 900]),
            yaxis=dict(range=[0, 900]),
            zaxis=dict(range=[0, 900])
        ),
        margin=dict(l=0, r=0, b=0, t=30),
        showlegend=True,  # 显示图例 (屏蔽箱, 干扰源, 测点, 标签)
        uirevision="true",   # 关键：保持用户交互状态（如缩放、旋转）
        height=1200  # 增加图表高度以匹配表格
    )
    
    return fig

# 为单个测点创建波形图
def create_point_waveform_plot(data, point_idx):
    if data is None or len(waveform_data['timestamps']) == 0:
        # 如果没有数据，创建一个空图
        fig = go.Figure()
        fig.update_layout(
            title=f"{SPATIAL_POINTS[point_idx]['name']} - 无数据",
            xaxis_title="时间",
            yaxis_title="值"
        )
        return fig
    
    # 获取该测点对应的三个通道
    point = SPATIAL_POINTS[point_idx]
    ch_indices = point["channels"]
    
    # 转换时间戳为相对秒数
    time_list = list(waveform_data['timestamps'])
    if not time_list:
        return go.Figure()  # 返回空图表
    
    # 计算相对时间（以秒为单位）
    if isinstance(time_list[0], str):
        try:
            time_list = [datetime.fromisoformat(t) for t in time_list]
        except:
            try:
                time_list = [datetime.strptime(t, "%Y-%m-%d %H:%M:%S.%f") for t in time_list]
            except:
                # 如果无法解析，使用序号
                time_list = list(range(len(time_list)))
    
    # 如果是datetime对象，计算相对时间（秒）
    if isinstance(time_list[0], datetime):
        start_time = time_list[0]
        rel_times = [(t - start_time).total_seconds() for t in time_list]
    else:
        # 否则使用序号
        rel_times = list(range(len(time_list)))
    
    # 计算要显示的时间窗口（最近10秒）
    if rel_times and len(rel_times) > 1:
        current_time = rel_times[-1]
        window_start = max(0, current_time - 10)  # 显示最近10秒
        
        # 找到窗口起始点对应的索引
        start_idx = 0
        for i, t in enumerate(rel_times):
            if t >= window_start:
                start_idx = i
                break
        
        # 截取时间窗口内的数据
        visible_times = rel_times[start_idx:]
    else:
        visible_times = rel_times
    
    # 创建Plotly图表
    fig = go.Figure()
    
    # 为该测点的每个通道添加一条线
    colors = ['red', 'blue', 'green']
    channel_names = [f"通道{idx+1}" for idx in ch_indices]
    
    for i, channel_name in enumerate(channel_names):
        if channel_name in waveform_data['values'] and len(waveform_data['values'][channel_name]) > 0:
            # 获取对应通道的数据
            values = list(waveform_data['values'][channel_name])
            
            # 确保数据点数量与时间点数量一致
            if len(values) > len(rel_times):
                values = values[-len(rel_times):]
            elif len(values) < len(rel_times):
                # 用NaN填充缺失值
                values = [float('nan')] * (len(rel_times) - len(values)) + values
            
            # 截取时间窗口内的数据
            if len(visible_times) < len(rel_times):
                visible_values = values[start_idx:]
            else:
                visible_values = values
            
            # 添加线条
            fig.add_trace(go.Scatter(
                x=visible_times,
                y=visible_values,
                mode='lines',
                name=channel_name,
                line=dict(color=colors[i % len(colors)], width=2),
            ))
    
    # 设置图表布局
    fig.update_layout(
        title=f"{point['name']} 通道波形图 (最近10秒数据)",
        xaxis=dict(
            title="时间 (秒)",
            range=[visible_times[0] if visible_times else 0, 
                   visible_times[-1] if visible_times else 10]
        ),
        yaxis=dict(title="值"),
        legend=dict(
            orientation="h",
            yanchor="bottom",
            y=1.02,
            xanchor="center",
            x=0.5
        ),
        uirevision="true",  # 保持用户交互状态
        height=400  # 增加高度以便更清晰地查看波形
    )
    
    return fig

# 创建数据表格HTML
def create_data_for_dataframe(data, timestamp):
    if data is None:
        # 返回一个空的DataFrame和"无数据"标题
        return pd.DataFrame(columns=["通道", "数值", "测点"]), "### 没有数据可显示"

    table_rows = []
    channel_to_point = {}
    for point_idx, point in enumerate(SPATIAL_POINTS):
        for ch_csv_idx in point["channels"]: # ch_csv_idx is 0-29 (CSV channel index)
            channel_to_point[ch_csv_idx] = point["name"]
    
    for i in range(30): # i is 0-29
        channel_key = f"通道{i+1}"
        value = data.get(channel_key, 0.0) 
            point_name = channel_to_point.get(i, "-")
            
        # 为测点对应的通道添加高亮 (DataFrame本身不支持直接行高亮，此信息可以作为一列或在前端处理)
        # 暂时不直接处理高亮，专注于数据内容
        
        table_rows.append({
            "通道": channel_key, 
            "数值": f"{value:.1f}",  # 格式化数值
            "测点": point_name
        })
        
    df = pd.DataFrame(table_rows)
    table_title_md = f"### 最新数据 - {timestamp}"
    return df, table_title_md

# 更新函数 - 将被Gradio的定时器调用
def update_visualization():
    # global previous_data, previous_table_html # 删除此行
    
    # 查找最新的CSV文件
    csv_file = find_latest_csv()
    if csv_file is None:
        # previous_data = None # 删除此行
        # previous_table_html = None # 删除此行
        empty_fig = go.Figure()
        empty_df = pd.DataFrame(columns=["通道", "数值", "测点"])
        return (
            empty_fig,
            "### 未找到数据文件", 
            empty_df,
            "状态: 未找到magnetic_data_*.csv文件",
            *[empty_fig for _ in range(10)]  # 10个空波形图
        )
    
    # 读取最新数据
    data, error = read_latest_data(csv_file)
    if error:
        # previous_data = None # 删除此行
        # previous_table_html = None # 删除此行
        empty_fig = go.Figure()
        empty_df = pd.DataFrame(columns=["通道", "数值", "测点"])
        return (
            empty_fig,
            "### 读取数据出错",
            empty_df,
            f"状态: {error}",
            *[empty_fig for _ in range(10)]  # 10个空波形图
        )
    
    # 获取时间戳
    timestamp = data.get("时间戳", "未知时间")
    
    # 创建3D图和数据表
    plot_fig = create_3d_plot(data)
    df_data, table_title_md = create_data_for_dataframe(data, timestamp)
    status_msg = f"状态: 正在显示来自 {csv_file} 的数据, 更新于 {timestamp}"
    
    # 为每个测点创建独立的波形图
    point_plots_figs = [create_point_waveform_plot(data, i) for i in range(len(SPATIAL_POINTS))]
    
    return plot_fig, table_title_md, df_data, status_msg, *point_plots_figs

# 创建Gradio界面
with gr.Blocks(title="空间测点数据可视化", css="""
    /* 自定义CSS，防止组件闪烁和页面重置 */
    .gradio-container {
        scroll-behavior: auto !important;
    }
    /* .gradio-html {  // 此规则针对旧的gr.HTML，gr.DataFrame表现可能不同
        transition: opacity 0.1s ease;
    } */
    /* 保持滚动位置的辅助类 */
    #main-container {
        min-height: 100vh;
    }
""") as demo:
    with gr.Row():
        gr.Markdown("# 三维空间测点实时数据可视化")
    
    # 使用一个主容器包装所有内容，帮助保持滚动位置
    with gr.Column(elem_id="main-container"):
        with gr.Row():
            # 左侧3D图
            with gr.Column(scale=2):
                plot_output = gr.Plot(label="3D测点可视化")
            
            # 右侧数据表格
            with gr.Column(scale=1):
                table_title_display = gr.Markdown(label="数据表状态") # 用于显示 "最新数据 - 时间戳"
                # 初始化时提供一个有30行的空DataFrame，有助于Gradio预估高度
                initial_df = pd.DataFrame([{ "通道": f"通道{i+1}", "数值": "-", "测点": "-" } for i in range(30)])
                dataframe_output = gr.DataFrame(
                    value=initial_df, # 设置初始值
                    headers=["通道", "数值", "测点"], 
                    label="数据详情", 
                    wrap=True, 
                    interactive=False
                )
        
        # 添加10个波形图，每行2个，共5行
        waveform_plots_outputs = []
        
        for i in range(0, 10, 2):
            with gr.Row():
                for j in range(2):
                    if i + j < len(SPATIAL_POINTS):
                        point = SPATIAL_POINTS[i + j]
                        with gr.Column():
                            wf_plot = gr.Plot(label=f"{point['name']} 波形图")
                            waveform_plots_outputs.append(wf_plot)
        
        # 状态栏
        status_bar = gr.Markdown("状态: 等待数据...")
    
    # 添加自定义JS以保持滚动位置
    demo.load(js="""
    function saveScrollPosition() {
        window._scrollPosition = window.scrollY || document.documentElement.scrollTop;
    }
    
    function restoreScrollPosition() {
        if (window._scrollPosition !== undefined) {
            setTimeout(function() {
                window.scrollTo(0, window._scrollPosition);
            }, 10);
        }
    }
    
    // 保存滚动位置
    document.addEventListener('scroll', saveScrollPosition);
    
    // 在DOM更新后恢复滚动位置
    const targetNode = document.body;
    const config = { childList: true, subtree: true };
    const observer = new MutationObserver(restoreScrollPosition);
    observer.observe(targetNode, config);
    """)
    
    # 创建一个2秒触发一次的定时器
    timer = gr.Timer(2)
    
    # 将定时器连接到更新函数
    # 顺序：plot_output, table_title_display, dataframe_output, status_bar, *waveform_plots_outputs
    timer.tick(update_visualization, 
               inputs=None, 
               outputs=[plot_output, table_title_display, dataframe_output, status_bar, *waveform_plots_outputs])
    
    # 初始加载
    # 顺序：plot_output, table_title_display, dataframe_output, status_bar, *waveform_plots_outputs
    demo.load(update_visualization, 
              outputs=[plot_output, table_title_display, dataframe_output, status_bar, *waveform_plots_outputs])

# 启动Gradio应用
if __name__ == "__main__":
    print("正在启动空间测点数据可视化界面...")
    print("请确保collect_data.py正在运行并生成CSV数据文件。")
    print("请先安装chardet库：pip install chardet")
    demo.launch()