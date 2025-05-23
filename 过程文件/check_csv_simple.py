import pandas as pd
import glob
import os

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
        # 尝试使用不同编码读取
        for encoding in ['gbk', 'gb2312', 'gb18030', 'utf-8']:
            try:
                print(f"尝试使用 {encoding} 编码读取文件...")
                df = pd.read_csv(file_path, encoding=encoding)
                
                # 成功读取数据
                print(f"成功使用 {encoding} 编码读取!")
                
                # 检查列名
                print(f"\n列名 ({len(df.columns)} 列):")
                for i, col in enumerate(df.columns):
                    print(f"  列 {i+1}: {col}")
                
                # 检查数据前几行
                print("\n数据前2行:")
                print(df.head(2))
                
                # 检查最后一行数据
                print("\n最后一行数据:")
                last_row = df.iloc[-1]
                print(last_row)
                
                # 检查实际数据的行列数
                print(f"\n数据形状: {df.shape} (行 x 列)")
                
                # 如果列数与pandas检测的不一致，直接读取原始文件查看
                import csv
                with open(file_path, 'r', encoding=encoding) as f:
                    reader = csv.reader(f)
                    rows = list(reader)
                    header = rows[0]
                    first_row = rows[1]
                    last_row = rows[-1]
                    
                    print(f"\n原始CSV文件分析:")
                    print(f"  表头行列数: {len(header)}")
                    print(f"  第一行数据列数: {len(first_row)}")
                    print(f"  最后一行数据列数: {len(last_row)}")
                    
                    # 显示最后两列的值（如果有额外的列）
                    if len(last_row) > len(header):
                        print(f"\n额外列值:")
                        extra_cols = len(last_row) - len(header)
                        for i in range(extra_cols):
                            idx = len(header) + i
                            print(f"  额外列 {i+1}: {last_row[idx]}")
                
                break  # 成功读取后跳出循环
                
            except Exception as e:
                print(f"使用 {encoding} 编码读取失败: {e}")
    
    except Exception as e:
        print(f"检查CSV文件出错: {e}")

if __name__ == "__main__":
    csv_file = find_latest_csv()
    if csv_file:
        check_csv_file(csv_file) 