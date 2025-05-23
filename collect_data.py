# collect_data.py
import clr # 导入 pythonnet 核心模块
import os
import time
import traceback # 用于打印详细错误信息
import sys
import csv
from datetime import datetime
import matplotlib.pyplot as plt
import matplotlib.animation as animation
import numpy as np
from threading import Lock
import requests # 新增导入

# 在import之后立即配置matplotlib
import matplotlib
# 设置非交互式后端，避免窗口显示问题
matplotlib.use('Agg')  # 使用Agg后端，这是一个非交互式后端
# 设置交互模式
plt.ion()

# --- 配置 ---
# 脚本和 DLL 文件都在 SDK_X86 目录下，所以 DLL 目录就是脚本所在的目录
DLL_DIR = os.path.dirname(__file__) # Corrected: DLLs are in the same directory as the script
MCMAG_DLL_NAME = "MCMag30FDevice" # DLL 的程序集名称 (通常是文件名去掉 .dll)
NAMESPACE = "MCMag30FDevice" # DLL 中的命名空间，根据 .txt 文件推断

# --- .NET 加载 ---
try:
    # 将 DLL 目录添加到系统路径，以便能找到依赖项 (如 CyUSB.dll)
    sys.path.append(DLL_DIR)
    # 加载 .NET 运行时需要的一些基础程序集
    clr.AddReference("System")
    # 加载目标 DLL
    mc_mag_dll_path = os.path.join(DLL_DIR, MCMAG_DLL_NAME)
    clr.AddReference(mc_mag_dll_path)
    # Import the System namespace directly after adding the reference
    import System

    # 从目标命名空间导入所需的类型
    # 注意：导入的具体名称需要与 DLL 中定义的完全一致
    print("尝试导入 MultiChanMagCollectDeviceManager...")
    exec(f"from {NAMESPACE} import MultiChanMagCollectDeviceManager")
    print("导入 MultiChanMagCollectDeviceManager 成功")

    print("尝试导入其他类型 (枚举, 类)...")
    exec(f"from {NAMESPACE} import DeviceStatus, ADCMode, ChanMode, SamplingFreq, ErrorCode, DeviceData, SamplingMode, TimingType")
    print("导入其他类型成功")

    # 导入设备类（如果需要直接与设备对象交互）
    print("尝试导入 MultiChanMagCollectDevice...")
    exec(f"from {NAMESPACE} import MultiChanMagCollectDevice")
    print("导入 MultiChanMagCollectDevice 成功")

    # 导入 .NET 字典类型
    from System.Collections.Generic import Dictionary
    # 导入 .NET 异常类型，以便更好地捕获特定错误
    from System import Exception as DotNetException
    # 导入无符号整数类型，用于字典键
    from System import UInt32

except ImportError as e:
    print(f"错误：无法加载 .NET 程序集或导入类型。")
    print(f"请确保 '{MCMAG_DLL_NAME}.dll' 和其依赖项 (如 'CyUSB.dll') 位于:")
    print(f"'{DLL_DIR}'")
    print(f"并且已安装所需的 .NET 运行时。")
    print(f"详细错误: {e}")
    sys.exit(1)
except Exception as e:
    print(f"加载 .NET 时发生未知错误: {e}")
    traceback.print_exc()
    sys.exit(1)

# --- 全局变量 ---
latest_data = None # 用于存储最新数据（可选）
data_received_flag = False # 用于标记是否收到数据
data_lock = Lock() # 用于线程安全访问数据
data_history = [] # 存储历史数据用于绘图
max_history_len = 100 # 保留的历史数据点数量
# csv_file = None # CSV文件对象 # 移除
# csv_writer = None # CSV写入器 # 移除
fig = None # 图表对象
axes = None # 轴对象列表
lines = None # 线条对象列表
time_points = [] # 时间点列表
# 更新默认显示的通道，包括几个常规通道和磁通门通道
selected_channels = [0, 1, 2, 24, 25, 26] # 显示前三个通道和磁通门数据(25-27通道)

# 初始化图表和文件
def init_visualization_and_storage():
    # global fig, axes, lines, csv_file, csv_writer # 移除 csv_file, csv_writer
    global fig, axes, lines
    
    # # 创建保存数据的CSV文件 # 移除此块
    # timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    # csv_filename = f"magnetic_data_{timestamp}.csv"
    # csv_file = open(csv_filename, 'w', newline='')
    # csv_writer = csv.writer(csv_file)
    
    # # 写入CSV头部 # 移除此块
    # header_row = ["时间戳"] + [f"通道{i+1}" for i in range(30)]  # 通道编号从1到30
    # csv_writer.writerow(header_row)
    
    # print(f"数据将保存到: {os.path.abspath(csv_filename)}") # 可以移除或修改
    print("数据采集程序已启动，将通过API发送数据。")
    print("数据可视化功能将由Vue.js前端提供。")
    
    # return csv_writer # 不再返回csv_writer
    return None # 或者不返回任何东西

# 更新图表并保存为图片
def update_plot():
    # 此函数保留但不做任何操作，避免修改其他代码的调用
    pass

# --- 事件处理函数 ---
def handle_data_received(device_id, device_data_array_ref):
    """
    当 .NET DLL 触发 DeviceDataReceivedEvent 时，此函数会被调用。
    注意：参数名可能与 C# 定义不同，但顺序和类型应匹配委托。
           `device_data_array_ref` 对应 C# 中的 `ref DeviceData[] sDeviceData`。
           `pythonnet` 通常会自动处理 `ref` 和数组。
    """
    # global latest_data, data_received_flag, data_history, csv_writer # 移除 csv_writer
    global latest_data, data_received_flag, data_history
    try:
        # --- 调试 --- 
        # print(f"[DEBUG] handle_data_received_BEGIN called for device {device_id}")
        # if device_data_array_ref is None:
        #     print("[DEBUG] device_data_array_ref is None. Exiting handle_data_received.")
        #     return
        
        # print(f"[DEBUG] device_data_array_ref type: {type(device_data_array_ref)}")
        # try:
        #     print(f"[DEBUG] device_data_array_ref length: {len(device_data_array_ref)}")
        # except TypeError:
        #     print("[DEBUG] device_data_array_ref does not have a len().")
        # --- 结束调试 ---
            
        # device_data_array_ref 是一个 .NET 数组 (DeviceData[])
        if device_data_array_ref is not None and len(device_data_array_ref) > 0:
            # 假设我们只关心数组中的第一个 DeviceData 对象
            device_data = device_data_array_ref[0]
            
            # --- 调试 --- 
            # print(f"[DEBUG] device_data (first element) type: {type(device_data)}")
            # if device_data is not None:
            #     print(f"[DEBUG] Attributes of device_data: {dir(device_data)}")
            # --- 结束调试 ---
            
            if device_data is not None and hasattr(device_data, 'dChannelDatas'):
                # dChannelDatas 是一个 .NET double[] 数组
                # 将 .NET double[] 转换为 Python list[float]
                py_data_list_raw = list(device_data.dChannelDatas)
                
                # 如果数据长度是32，则截取前30个通道
                if len(py_data_list_raw) == 32:
                    py_data_list = py_data_list_raw[:30]
                    # 可以选择打印一条信息，说明进行了截断
                    # print(f"[{time.strftime('%H:%M:%S')}] Info: Raw data has 32 channels, truncated to 30.")
                elif len(py_data_list_raw) == 30:
                    py_data_list = py_data_list_raw
                else:
                    # 长度既不是30也不是32，按原逻辑处理（可能会被后续长度检查跳过）
                    py_data_list = py_data_list_raw

                # 保存数据
                with data_lock:
                    latest_data = py_data_list # 使用处理后的 py_data_list
                    data_received_flag = True # 标记收到了新数据
                    
                    # 添加到历史数据，用于绘图
                    data_history.append(py_data_list)
                    if len(data_history) > max_history_len:
                        data_history = data_history[-max_history_len:]
                    
                    # 新增：发送数据到FastAPI后端
                    try:
                        precise_timestamp_iso = datetime.now().isoformat()
                        
                        # 确保 py_data_list 长度为30 (现在应该总是30，除非原始长度异常)
                        if len(py_data_list) != 30:
                            print(f"[{time.strftime('%H:%M:%S')}] API Send Warning: Data length is {len(py_data_list)}, expected 30. Skipping this data point.")
                        else:
                            payload = {
                                "timestamp_iso": precise_timestamp_iso,
                                "values": py_data_list
                            }
                            api_url = "http://localhost:8000/api/v1/data-ingest"
                            
                            response = requests.post(api_url, json=payload, timeout=1)

                            if response.status_code != 201:
                                print(f"[{time.strftime('%H:%M:%S')}] API Error: {response.status_code} - {response.text}")
                    except requests.exceptions.RequestException as e_req:
                        print(f"[{time.strftime('%H:%M:%S')}] API RequestException: {e_req}")
                    except Exception as e_send:
                        print(f"[{time.strftime('%H:%M:%S')}] API Send Error: {e_send}")

                # 打印数据（包括磁通门数据）
                fluxgate_data = py_data_list[24:27] if len(py_data_list) >= 30 else [] #修正索引以匹配25-27通道
                print(f"[{time.strftime('%H:%M:%S')}] 设备 {device_id} 收到数据:")
                print(f"  常规通道样本: {py_data_list[:3]}... (部分展示)")
                print(f"  磁通门数据(25-27通道): {fluxgate_data}")
                
                # 尝试更新图表
                update_plot()
            else:
                print(f"[DEBUG] 警告: 收到的 DeviceData 对象无效或缺少 dChannelDatas 属性!")
                if device_data is not None:
                    print(f"[DEBUG] 可用属性: {dir(device_data)}")
        else:
            print(f"[DEBUG] 警告: 收到的 device_data_array_ref 为空或长度为 0!")

    except DotNetException as e:
        print(f"!! 处理数据时发生 .NET 错误: {e}")
        # 可以根据需要记录更详细的 .NET 异常信息
    except Exception as e:
        print(f"!! 处理数据时发生 Python 错误: {e}")
        traceback.print_exc()

# --- 主程序逻辑 ---
def main():
    # global csv_file, csv_writer, max_history_len, selected_channels # 移除 csv_file, csv_writer
    global max_history_len, selected_channels
    
    manager = None
    is_collecting = False

    try:
        # 将历史数据长度增加到支持更长的历史记录
        max_history_len = 5000  # 增加历史数据缓存，便于Gradio界面显示更多数据
        
        # # 初始化CSV文件 # 移除或修改调用
        # csv_writer = init_visualization_and_storage()
        init_visualization_and_storage() # 调用修改后的函数，它不再返回writer
        
        print("正在获取设备管理器实例...")
        manager = MultiChanMagCollectDeviceManager.GetInstance()
        if manager is None:
            print("错误：无法获取设备管理器实例。")
            return

        print("正在获取所有已连接设备...")
        # GetAllDevices() 返回一个 .NET Dictionary<uint, MultiChanMagCollectDevice>
        all_devices_net = manager.GetAllDevices()

        if all_devices_net is None or all_devices_net.Count == 0:
            print("未检测到任何设备。请确保设备已连接并安装驱动程序。")
            return

        # 将 .NET Dictionary 转换为 Python 字典以便查看
        all_devices_py = {item.Key: item.Value for item in all_devices_net}
        print(f"发现设备 ID: {list(all_devices_py.keys())}")

        # --- 选择并设置要使用的设备 ---
        # 这里我们选择第一个发现的设备，您可以根据需要修改选择逻辑
        device_id_to_use = list(all_devices_py.keys())[0]
        _device_obj_for_setting = all_devices_py[device_id_to_use] # 获取 .NET 设备对象用于设置
        print(f"选择使用设备 ID: {device_id_to_use}")

        # 创建一个 .NET Dictionary<uint, MultiChanMagCollectDevice> 来设置使用中的设备
        # 指定正确的键类型为 UInt32
        in_use_devices_net = Dictionary[UInt32, MultiChanMagCollectDevice]()
        # 将 Python int 显式转换为 UInt32
        in_use_devices_net.Add(UInt32(device_id_to_use), _device_obj_for_setting)

        print("正在设置使用中的设备...")
        manager.SetInUseDevices(in_use_devices_net)

        # 检查设备状态
        status_enum_val = manager.GetDeviceStatus(device_id_to_use)
        # 将枚举的整数值转换为名称字符串以便阅读
        status_name = System.Enum.GetName(DeviceStatus, status_enum_val)
        print(f"设备 {device_id_to_use} 当前状态: {status_name} ({status_enum_val})")

        # --- 配置设备参数 ---
        print("正在配置设备参数...")
        # 使用枚举值进行设置
        adc_mode = ADCMode.DC       # 选择直流模式
        chan_mode = ChanMode.DIFFERENTIAL # 选择差分模式，更适合磁通门数据
        samp_freq = SamplingFreq.FREQ_1000 # 选择 1000 Hz 采样率
        # samp_mode = SamplingMode.CONTINUOUS # 采样模式不再直接通过manager或device对象设置

        # 新增 TimingType 配置
        # 请根据您的 DLL 中 MCMag30FDevice.TimingType 枚举的实际成员名称进行调整
        # 例如 TimingType.INTERNAL, TimingType.GPS_TIMING 等
        # 如果 TimingType.Internal 不存在，执行时会 AttributeError
        # timing_type = TimingType.Internal # 暂时注释掉，依赖默认值

        # Convert enum values to strings for printing
        adc_mode_name = System.Enum.GetName(ADCMode, adc_mode)
        chan_mode_name = System.Enum.GetName(ChanMode, chan_mode)
        samp_freq_name = System.Enum.GetName(SamplingFreq, samp_freq)
        # samp_mode_name = System.Enum.GetName(SamplingMode, samp_mode) # 移除
        # timing_type_name = System.Enum.GetName(TimingType, timing_type) # 暂时注释掉

        print(f"  - ADC 模式: {adc_mode_name}")
        res_adc = manager.SetADCMode(adc_mode)
        print(f"  - 通道模式: {chan_mode_name}")
        res_chan = manager.SetChanMode(chan_mode)
        print(f"  - 采样频率: {samp_freq_name}")
        res_freq = manager.SetSamplingFreq(samp_freq)
        # print(f"  - 采样模式: {samp_mode_name}") # 移除
        # # 注意：文档中没有直接在管理器上设置采样模式的方法，可能需要在每个设备上单独设置
        # # 这里我们直接对选定的设备操作：
        # if hasattr(device_obj_to_use, 'SetSamplingMode'): # 移除整个 if hasattr(device_obj_to_use, 'SetSamplingMode') 块
        #     print("  - 正在设置设备的采样模式...")
        #     res_samp_mode = device_obj_to_use.SetSamplingMode(samp_mode)
        #     print(f"  - 采样模式设置结果: {System.Enum.GetName(ErrorCode, res_samp_mode) if hasattr(res_samp_mode, 'value') else res_samp_mode}")
        
        # print(f"  - 授时类型: {timing_type_name}") # 暂时注释掉
        # res_timing = manager.SetTimingType(timing_type) # 暂时注释掉

        # 检查参数设置结果
        # if res_adc == ErrorCode.SUCCESS and res_chan == ErrorCode.SUCCESS and res_freq == ErrorCode.SUCCESS and res_timing == ErrorCode.SUCCESS: # 修改条件
        if res_adc == ErrorCode.SUCCESS and res_chan == ErrorCode.SUCCESS and res_freq == ErrorCode.SUCCESS:
            print("设备参数设置成功。")
        else:
            # Convert error code enum values to strings for printing
            err_adc = System.Enum.GetName(ErrorCode, res_adc) if res_adc != ErrorCode.SUCCESS else "OK"
            err_chan = System.Enum.GetName(ErrorCode, res_chan) if res_chan != ErrorCode.SUCCESS else "OK"
            err_freq = System.Enum.GetName(ErrorCode, res_freq) if res_freq != ErrorCode.SUCCESS else "OK"
            # err_timing = System.Enum.GetName(ErrorCode, res_timing) if res_timing != ErrorCode.SUCCESS else "OK" # 暂时注释掉
            # print(f"错误：设置参数失败。ADC: {err_adc}, Chan: {err_chan}, Freq: {err_freq}, Timing: {err_timing}") # 修改打印信息
            print(f"错误：设置参数失败。ADC: {err_adc}, Chan: {err_chan}, Freq: {err_freq}")
            return # 参数失败则不继续

        # --- 注册事件处理程序 ---
        print("正在注册数据接收事件处理程序...")
        
        # 在 pythonnet 中，可以直接使用 += 操作符将 Python 函数绑定到 .NET 事件
        # pythonnet 会自动处理从 Python 函数到 .NET 委托的转换
        print("尝试绑定事件处理函数...")
        
        # 直接绑定 Python 函数到事件
        manager.DeviceDataReceivedEvent += handle_data_received
        
        # # 尝试在设备对象上也注册事件 - 可能是数据直接从设备对象发出而不是管理器 # 移除此块
        # if hasattr(device_obj_to_use, 'DataReceivedEvent'):
        #     print("尝试绑定设备对象的数据接收事件...")
        #     device_obj_to_use.DataReceivedEvent += handle_data_received
        #     print("设备对象事件处理程序已注册。")
        
        print("事件处理程序已注册。")

        # --- 开始采集 ---
        print("正在启动数据采集...")
        start_result = manager.StartAcquireData()
        start_result_name = System.Enum.GetName(ErrorCode, start_result)

        if start_result == ErrorCode.SUCCESS:
            print(f"数据采集已成功启动 ({start_result_name})。")
            is_collecting = True
            print("等待数据接收... 按 Ctrl+C 停止采集。")
            print("数据正在实时保存到CSV文件。")
            print("请运行Gradio界面(gradio_interface.py)以实时查看数据。")
            # 保持运行以接收数据，直到用户中断
            try:
                while True:
                    try:
                        # 更温和的休眠方式
                        time.sleep(0.1)
                    except Exception as e:
                        print(f"主循环异常: {e}")
            except KeyboardInterrupt:
                print("\n收到停止信号 (Ctrl+C)...")
        else:
            print(f"错误：启动数据采集失败。错误代码: {start_result_name} ({start_result})")

    except DotNetException as e:
        print(f"\n!! 发生 .NET 运行时错误: {e}")
        # 打印 .NET 异常的详细信息可能需要特定处理
        print(f"  类型: {e.GetType().FullName}")
        print(f"  消息: {e.Message}")
        if hasattr(e, 'StackTrace'):
            print(f"  堆栈跟踪:\n{e.StackTrace}")
    except Exception as e:
        print(f"\n!! 发生 Python 错误: {e}")
        traceback.print_exc()
    finally:
        # --- 清理 ---
        print("\n正在执行清理操作...")
        if manager is not None:
            if is_collecting:
                print("正在停止数据采集...")
                stop_result = manager.StopAcquireData()
                stop_result_name = System.Enum.GetName(ErrorCode, stop_result)
                if stop_result == ErrorCode.SUCCESS:
                    print(f"数据采集已停止 ({stop_result_name})。")
                else:
                    print(f"警告：停止数据采集失败。错误代码: {stop_result_name} ({stop_result})")
                is_collecting = False

                # # 尝试调用设备的 AfterStopAcquireData 方法 # 移除此块
                # if hasattr(device_obj_to_use, 'AfterStopAcquireData'):
                #     print("正在调用 AfterStopAcquireData 清理设备...")
                #     after_result = device_obj_to_use.AfterStopAcquireData()
                #     print(f"清理结果: {after_result}")

            # if handler_delegate is not None: # 移除 handler_delegate 相关逻辑，直接使用函数名
            try:
                print("正在注销事件处理程序...")
                manager.DeviceDataReceivedEvent -= handle_data_received
                    
                # # 如果在设备对象上也注册了事件，需要取消注册 # 移除此块
                # if hasattr(device_obj_to_use, 'DataReceivedEvent'):
                #     device_obj_to_use.DataReceivedEvent -= handler_delegate # or handle_data_received
                #     print("设备对象事件处理程序已注销。")
                        
                print("事件处理程序已注销。")
            except Exception as e_unregister:
                print(f"警告：注销事件处理程序时出错: {e_unregister}")

        # # 关闭CSV文件 # 移除此块
        # if csv_file:
        #     csv_file.close()
        #     print(f"CSV文件已保存并关闭。")
        
        # 关闭matplotlib图表
        plt.close('all')
        
        print("清理完成。程序退出。")


if __name__ == "__main__":
    main()