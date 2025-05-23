import csv
import glob
import os
import chardet

# 查找最新的CSV文件
def find_latest_csv():
    csv_files = glob.glob("magnetic_data_*.csv")
    if not csv_files:
        print("未找到CSV文件")
        return None
    
    # 按文件创建时间排序，最新的在最前
    latest_file = max(csv_files, key=os.path.getmtime)
    print(f"找到最新CSV文件: {latest_file}")
    return latest_file

# 检查CSV文件
def check_csv_file(file_path):
    try:
        # 检测文件编码
        with open(file_path, 'rb') as f:
            raw_data = f.read(10000)  # 读取前10000个字节用于编码检测
            result = chardet.detect(raw_data)
            encoding = result['encoding']
            confidence = result['confidence']
            
            print(f"检测到文件编码: {encoding}，置信度: {confidence}")
            
        # 尝试几种可能的编码
        encodings_to_try = ['utf-8', 'gbk', 'gb2312', 'gb18030', 'cp936', encoding]
        
        # 尝试使用不同编码读取文件
        for enc in encodings_to_try:
            try:
                print(f"\n尝试使用 {enc} 编码读取文件...")
                with open(file_path, 'r', newline='', encoding=enc) as f:
                    reader = csv.reader(f)
                    data = list(reader)
                    
                    if not data:
                        print("CSV文件为空")
                        continue
                    
                    header = data[0]
                    print(f"表头 ({len(header)} 列):")
                    for i, col in enumerate(header):
                        print(f"  列 {i+1}: {col}")
                    
                    # 检查前几行数据
                    print("\n前2行数据:")
                    for i, row in enumerate(data[1:3]):
                        if len(row) > 0:  # 确保行不为空
                            print(f"行 {i+1} ({len(row)} 列): {row[:5]}... (部分显示)")
                    
                    # 检查最后一行数据
                    last_row = data[-1]
                    print(f"\n最后一行 ({len(last_row)} 列):")
                    
                    # 打印所有值
                    for i in range(len(last_row)):
                        if i < len(header):
                            print(f"  列 {i+1}: {header[i]} = {last_row[i]}")
                        else:
                            print(f"  列 {i+1} (表头外): 值 = {last_row[i]}")
                    
                    # 检查列数不匹配的情况
                    if len(header) != len(last_row):
                        print(f"\n警告: 表头有 {len(header)} 列，但最后一行数据有 {len(last_row)} 列")
                        
                        # 检查差异
                        if len(last_row) < len(header):
                            missing_cols = len(header) - len(last_row)
                            print(f"缺少 {missing_cols} 列数据")
                            # 显示缺少的列名
                            missing_headers = header[len(last_row):]
                            print(f"缺少的列: {missing_headers}")
                        else:
                            extra_cols = len(last_row) - len(header)
                            print(f"多出 {extra_cols} 列数据")
                            # 显示多出的值
                            extra_vals = last_row[len(header):]
                            print(f"多出的值: {extra_vals}")
                            
                    print(f"\n成功使用 {enc} 编码读取文件!")
                    return  # 如果成功读取，跳出循环
                    
            except Exception as e:
                print(f"使用 {enc} 编码读取失败: {e}")
                continue
                
    except Exception as e:
        print(f"检查CSV文件时出错: {e}")

if __name__ == "__main__":
    csv_file = find_latest_csv()
    if csv_file:
        check_csv_file(csv_file) 