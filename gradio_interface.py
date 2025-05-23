import os
import csv
import time
import gradio as gr
import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
from datetime import datetime
import threading
from watchdog.observers import Observer
from watchdog.events import FileSystemEventHandler
import matplotlib.font_manager as fm
import traceback

# 设置中文字体，解决中文显示问题
try:
    # 尝试设置微软雅黑字体
    plt.rcParams['font.sans-serif'] = ['Microsoft YaHei', 'SimHei', 'Arial Unicode MS']
    plt.rcParams['axes.unicode_minus'] = False  # 解决负号显示问题
except:
    print("警告：无法设置中文字体，中文可能显示为方块")

# 全局变量
latest_data_file = None  # 最新的CSV文件路径
selected_channels = [24, 25, 26]  # 更新：默认显示新的磁通门通道 24, 25, 26
data_buffer = {ch: [] for ch in range(1, 31)}  # 每个通道的数据缓冲区，从1-30
timestamps = []  # 时间戳
buffer_size = 10000  # 缓冲区大小
update_interval = 0.5  # 更新间隔(秒)
reading_lock = threading.Lock()  # 用于线程安全的锁
amplification_factor = 100000  # 数据放大倍数

# 监控文件变化的类
class CSVFileHandler(FileSystemEventHandler):
    def __init__(self, callback):
        self.callback = callback
        
    def on_modified(self, event):
        if not event.is_directory and event.src_path.endswith('.csv'):
            self.callback(event.src_path)

# 更新最新的CSV文件路径
def update_latest_csv(csv_path):
    global latest_data_file
    latest_data_file = csv_path
    print(f"检测到CSV文件更新: {csv_path}")

# 读取CSV文件中的最新数据
def read_latest_data():
    global latest_data_file, data_buffer, timestamps
    
    if latest_data_file is None or not os.path.exists(latest_data_file):
        return
    
    try:
        with reading_lock:
            # 使用非UTF-8编码尝试读取CSV文件
            try:
                # 首先尝试用gbk编码读取
                df = pd.read_csv(latest_data_file, encoding='gbk')
            except:
                try:
                    # 如果失败，尝试用gb2312编码读取
                    df = pd.read_csv(latest_data_file, encoding='gb2312')
                except:
                    # 最后尝试自动检测编码
                    df = pd.read_csv(latest_data_file, encoding='latin1')
                    
            if len(df) == 0:
                return
                
            # 不再限制数据量，保留所有数据
            # 更新时间戳和数据缓冲区
            # 确保时间戳列被正确处理
            try:
                # 尝试转换为字符串格式
                timestamps = df['时间戳'].astype(str).tolist()
            except:
                # 如果失败，直接使用原始值
                timestamps = df['时间戳'].tolist()
                print("警告: 时间戳格式转换失败，使用原始格式")
            
            # 更新每个通道的数据
            for ch in range(1, 31):
                col_name = f"通道{ch}"
                if col_name in df.columns:
                    try:
                        # 尝试将数据转换为浮点数
                        data_buffer[ch] = df[col_name].astype(float).tolist()
                    except:
                        # 如果失败，使用原始值
                        data_buffer[ch] = df[col_name].tolist()
                    
            print(f"已读取CSV文件: {latest_data_file}, 包含{len(timestamps)}条数据记录")
    except Exception as e:
        print(f"读取CSV文件错误: {e}")
        traceback.print_exc()

# 生成数据图表
def generate_plot(channels_to_show=None, max_points_to_show=10000):
    if channels_to_show is None:
        channels_to_show = selected_channels
    
    if not timestamps or len(data_buffer) == 0 or all(len(data_buffer[ch]) == 0 for ch in selected_channels):
        # 返回空白图表
        fig, ax = plt.subplots(figsize=(10, 6))
        ax.text(0.5, 0.5, '等待数据...', ha='center', va='center', fontsize=14)
        ax.set_axis_off()
        return fig
    
    try:
        with reading_lock:
            # 创建图表
            fig, axes = plt.subplots(len(channels_to_show), 1, figsize=(10, 8), sharex=True)
            
            # 处理只有一个通道的情况
            if len(channels_to_show) == 1:
                axes = [axes]
            
            # 使用滑块指定的最大点数
            max_points = max_points_to_show
            
            # 准备时间轴
            if len(timestamps) > max_points:
                display_timestamps = timestamps[-max_points:]
                time_axis = list(range(len(display_timestamps)))
            else:
                display_timestamps = timestamps
                time_axis = list(range(len(timestamps)))
            
            # 为每个选定的通道绘制图表
            for i, ch in enumerate(channels_to_show):
                if ch >= 1 and ch < 31 and data_buffer[ch]:  # 确保通道有效且有数据
                    # 获取数据并应用放大倍数
                    if len(data_buffer[ch]) > max_points:
                        original_data = data_buffer[ch][-max_points:]
                    else:
                        original_data = data_buffer[ch]
                    
                    # 放大数据
                    channel_data = [val * amplification_factor for val in original_data]
                    
                    # 计算当前通道的数据范围，用于Y轴自适应
                    if channel_data:
                        chan_min = min(channel_data)
                        chan_max = max(channel_data)
                        # 增加15%的边距，让图表看起来更美观
                        margin = (chan_max - chan_min) * 0.15 if chan_max > chan_min else abs(chan_max) * 0.15 or 0.1
                        y_min = chan_min - margin
                        y_max = chan_max + margin
                    else:
                        y_min, y_max = -1, 1  # 默认范围
                    
                    # 为磁通门通道使用特殊样式
                    if ch in [24, 25, 26]:  # 更新：磁通门通道改为 24, 25, 26
                        line_style = '-'
                        line_width = 2.0
                        
                        # 为三个磁通门通道分配不同颜色
                        if ch == 24:  # 更新：X轴对应通道24
                            color = 'red'
                            marker = 'o'
                            markevery = max(1, len(channel_data) // 30)  # 每30个点标记一次
                        elif ch == 25:  # 更新：Y轴对应通道25
                            color = 'green'
                            marker = 's'  # 方形
                            markevery = max(1, len(channel_data) // 30)
                        else:  # Z轴 (对应通道26)
                            color = 'blue'
                            marker = '^'  # 三角形
                            markevery = max(1, len(channel_data) // 30)
                            
                        label = f"磁通门-{ch} (x{amplification_factor})"
                    else:  # 普通通道
                        line_style = '-'
                        line_width = 1.5
                        color = None  # 使用matplotlib默认颜色
                        marker = None
                        markevery = None
                        label = f"通道 {ch} (x{amplification_factor})"
                    
                    # 绘制数据
                    if ch in [24, 25, 26]:  # 更新：磁通门通道改为 24, 25, 26
                        axes[i].plot(time_axis, channel_data, linewidth=line_width, 
                                    color=color, marker=marker, markevery=markevery,
                                    linestyle=line_style, label=label)
                        axes[i].legend(loc='upper right')  # 添加图例
                    else:
                        axes[i].plot(time_axis, channel_data, linewidth=line_width)
                        
                    axes[i].set_ylabel(label)
                    axes[i].grid(True)
                    
                    # 设置自适应的Y轴范围，让每个通道的数据更好地显示
                    axes[i].set_ylim(y_min, y_max)
                    
                    # 设置X轴标签
                    if i == len(channels_to_show) - 1:  # 只在最后一个子图设置X轴标签
                        # 只标注少数几个时间点，避免拥挤
                        if len(time_axis) > 10:
                            tick_indices = list(range(0, len(time_axis), len(time_axis) // 5))
                            if tick_indices[-1] != len(time_axis) - 1:
                                tick_indices.append(len(time_axis) - 1)
                            
                            axes[i].set_xticks(tick_indices)
                            
                            # 将索引转换为实际时间，确保时间戳是字符串类型
                            tick_labels = []
                            for idx in tick_indices:
                                timestamp = display_timestamps[idx]
                                # 检查时间戳类型并进行适当转换
                                if isinstance(timestamp, (float, int)):
                                    # 如果是数值类型，直接使用格式化后的值
                                    tick_labels.append(f"{timestamp:.2f}")
                                elif isinstance(timestamp, str) and ' ' in timestamp:
                                    # 如果是带空格的字符串，拆分并获取第二部分
                                    tick_labels.append(timestamp.split(' ')[1])
                                else:
                                    # 其他情况，直接转为字符串
                                    tick_labels.append(str(timestamp))
                            
                            axes[i].set_xticklabels(tick_labels, rotation=45)
            
            # 添加标题
            current_time = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
            fig.suptitle(f"磁通门数据实时监控 (放大{amplification_factor}倍) - {current_time}")
            
            # 添加总的X轴标签
            fig.text(0.5, 0.04, '时间', ha='center')
            
            plt.tight_layout()
            return fig
    except Exception as e:
        print(f"绘图错误: {e}")
        traceback.print_exc()
        # 返回错误图表
        fig, ax = plt.subplots(figsize=(10, 6))
        ax.text(0.5, 0.5, f'绘图错误: {str(e)}', ha='center', va='center', fontsize=14)
        return fig

# 查找当前目录中最新的CSV文件
def find_latest_csv():
    csv_files = [f for f in os.listdir('.') if f.endswith('.csv') and f.startswith('magnetic_data_')]
    if not csv_files:
        return None
    
    # 按文件修改时间排序，返回最新的
    csv_files.sort(key=lambda x: os.path.getmtime(x), reverse=True)
    return csv_files[0]

# 通道选择器更新
def update_channels(ch1_selected, ch2_selected, ch3_selected, ch24_selected, ch25_selected, ch26_selected, max_points=1000):
    global selected_channels
    selected_channels = []
    
    # 添加选择的常规通道
    if ch1_selected:
        selected_channels.append(1)
    if ch2_selected:
        selected_channels.append(2)
    if ch3_selected:
        selected_channels.append(3)
    
    # 添加选择的磁通门通道
    if ch24_selected:
        selected_channels.append(24)
    if ch25_selected:
        selected_channels.append(25)
    if ch26_selected:
        selected_channels.append(26)
    
    # 确保至少有一个通道被选中，如果没有，则默认选择通道24
    if not selected_channels:
        selected_channels = [24]  # 默认显示通道24
        
    return generate_plot(selected_channels, max_points_to_show=max_points)

# 设置文件监控
def setup_file_monitoring():
    event_handler = CSVFileHandler(update_latest_csv)
    observer = Observer()
    observer.schedule(event_handler, path='.', recursive=False)
    observer.start()
    return observer

# 主界面
def create_interface():
    # 查找最新的CSV文件
    csv_file = find_latest_csv()
    if csv_file:
        update_latest_csv(csv_file)
        read_latest_data()
    
    # 创建一个状态文本变量
    last_update_time = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    
    # 创建Gradio界面
    with gr.Blocks(title="磁通门数据采集与可视化", theme=gr.themes.Default()) as interface:
        gr.Markdown("# 磁通门数据采集与可视化系统")
        
        # 顶部状态栏
        with gr.Row():
            status_output = gr.Markdown(f"**状态**: 等待数据... (程序启动于 {last_update_time})")
            auto_refresh_toggle = gr.Checkbox(label="启用自动刷新", value=True, interactive=True)
            auto_refresh_interval = gr.Number(
                value=5, 
                label="自动刷新间隔(秒)",
                minimum=1,
                maximum=60,
                step=1,
                interactive=True
            )
        
        with gr.Row():
            with gr.Column(scale=3):
                plot_output = gr.Plot(value=generate_plot(), format='png')
                
                with gr.Row():
                    refresh_btn = gr.Button("手动刷新", variant="primary")
            
            with gr.Column(scale=1):
                gr.Markdown("### 通道选择")
                with gr.Row():
                    ch1_cb = gr.Checkbox(label="通道 1", value=False)
                    ch2_cb = gr.Checkbox(label="通道 2", value=False)
                    ch3_cb = gr.Checkbox(label="通道 3", value=False)
                
                gr.Markdown("### 磁通门数据 (新通道号)")
                with gr.Row():
                    ch24_cb = gr.Checkbox(label="磁通门X (24)", value=True)
                    ch25_cb = gr.Checkbox(label="磁通门Y (25)", value=True)
                    ch26_cb = gr.Checkbox(label="磁通门Z (26)", value=True)
                
                apply_btn = gr.Button("应用选择")
                
                gr.Markdown("### 数据显示设置")
                max_points_slider = gr.Slider(
                    minimum=100, 
                    maximum=2000, 
                    value=1000, 
                    step=100, 
                    label="显示的最大数据点数"
                )
                
                gr.Markdown("### 数据统计")
                stats_output = gr.DataFrame(
                    headers=["通道", "最小值", "最大值", "平均值", "标准差"],
                    value=calculate_stats()
                )
        
        # 设置事件处理
        apply_btn.click(
            fn=update_channels,
            inputs=[ch1_cb, ch2_cb, ch3_cb, ch24_cb, ch25_cb, ch26_cb, max_points_slider],
            outputs=plot_output
        )
        
        # 创建一个支持两种返回值的刷新函数
        def refresh_data(max_points=1000):
            current_time = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
            read_latest_data()  # 确保读取最新数据
            # 使用滑块的值来控制显示的数据点数量
            new_plot = generate_plot(selected_channels, max_points_to_show=max_points)
            new_stats = calculate_stats()
            status_msg = f"**状态**: 刷新于 {current_time}"
            return new_plot, new_stats, status_msg
        
        # 刷新按钮处理
        refresh_btn.click(
            fn=refresh_data,
            inputs=max_points_slider,
            outputs=[plot_output, stats_output, status_output]
        )
        
        # --- 使用 Blocks 的 load 事件实现自动刷新 --- 
        # 定义一个函数，根据复选框状态决定是否刷新
        def auto_refresh_handler(auto_refresh_enabled, max_points):
            if auto_refresh_enabled:
                print(f"自动刷新触发 - {datetime.now().strftime('%H:%M:%S')}")
                # 调用现有的刷新函数
                return refresh_data(max_points)
            else:
                # 如果禁用，不更新任何内容
                return gr.update(), gr.update(), gr.update()
        
        # 将 load 事件连接到处理器函数
        # every 参数指定了触发的频率（秒）
        try:
            interface.load(
                fn=auto_refresh_handler,
                inputs=[auto_refresh_toggle, max_points_slider],
                outputs=[plot_output, stats_output, status_output],
                every=auto_refresh_interval.value # 使用 Number 组件的初始值
            )
            print(f"已启用基于 load 事件的自动刷新 (间隔: {auto_refresh_interval.value} 秒)")
            
            # (可选) 监听间隔变化，并更新定时器 (这部分可能需要更复杂的JS或Gradio内部机制)
            # auto_refresh_interval.change(...) 
            
        except TypeError as e:
            if "unexpected keyword argument 'every'" in str(e):
                print(f"警告：您的 Gradio 版本不支持 interface.load 的 'every' 参数。自动刷新将不可用。请使用手动刷新按钮。")
                # 这里可以考虑添加 gr.Timer 作为备选方案，但这会增加复杂性
            else:
                print(f"设置自动刷新时发生错误: {e}")
                traceback.print_exc()
        except Exception as e:
            print(f"设置自动刷新时发生未知错误: {e}")
            traceback.print_exc()

    return interface

# 计算数据统计信息
def calculate_stats():
    stats_data = []
    
    for ch in selected_channels:
        if ch in data_buffer and data_buffer[ch]:
            try:
                # 确保数据是数值类型，并应用放大倍数
                original_values = np.array(data_buffer[ch], dtype=float)
                values = original_values * amplification_factor
                
                stats_data.append([
                    f"通道 {ch} (x{amplification_factor})",
                    f"{np.min(values):.6f}",
                    f"{np.max(values):.6f}",
                    f"{np.mean(values):.6f}",
                    f"{np.std(values):.6f}"
                ])
            except (ValueError, TypeError) as e:
                # 如果转换失败，显示错误信息
                stats_data.append([f"通道 {ch}", "错误", "错误", "错误", f"数据类型问题: {e}"])
        else:
            stats_data.append([f"通道 {ch}", "N/A", "N/A", "N/A", "N/A"])
    
    return stats_data

if __name__ == "__main__":
    print("正在启动磁通门数据实时监控界面...")
    print("请确保数据采集程序(collect_data.py)正在运行并生成CSV文件")
    
    # 设置文件监控
    observer = setup_file_monitoring()
    
    try:
        # 启动Gradio界面
        interface = create_interface()
        
        # 自动读取数据线程 - 仅读取数据，不刷新UI
        def auto_read_data_thread():
            """后台线程，仅负责读取最新数据到缓存"""
            last_read_time = time.time()
            
            while True:
                try:
                    # 短睡眠，更灵敏地检查
                    time.sleep(0.5)
                    current_time = time.time()
                    
                    # 每秒都读取最新数据，确保数据缓存是最新的
                    if current_time - last_read_time >= 1:  # 每1秒读取一次最新数据
                        read_latest_data()
                        last_read_time = current_time
                except Exception as e:
                    print(f"数据读取错误: {e}")
                    traceback.print_exc()
        
        # 启动自动读取数据线程
        print("启动数据读取线程...")
        threading.Thread(target=auto_read_data_thread, daemon=True).start()
        
        # 启动界面
        print("正在启动Web界面...")
        interface.queue(max_size=20).launch(
            share=True,          
            inbrowser=True,      
            server_name="0.0.0.0", 
            show_error=True,     
            quiet=False          
        )
        
    except Exception as e:
        print(f"启动界面错误: {e}")
        traceback.print_exc()
    finally:
        # 停止文件监控
        if observer:
            observer.stop()
            observer.join()
        
        print("程序退出") 