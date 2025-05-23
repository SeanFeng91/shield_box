using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MCMag30FDevice;

namespace MCMAG30F
{
    /// <summary>
    /// 数据文件写入器
    /// </summary>
    public class DataFileWriter
    {
        /// <summary>
        /// 唯一实例
        /// </summary>
        private static DataFileWriter m_Instance = null;

        /// <summary>
        /// 数据文件存储路径
        /// </summary>
        private string m_strDataFilePath = "";

        /// <summary>
        /// 按照日期建立的数据存储路径
        /// </summary>
        private string m_strDataFilePathByDate = "";

        /// <summary>
        /// 数据文件流
        /// </summary>
        private FileStream m_fsDataFile = null;

        /// <summary>
        /// 文件后缀，当采样率不同时，文件后缀也不一样。100Hz--.TSL,1000Hz--.TSM,10000Hz--.TSH
        /// </summary>
        private string m_strSuffix = ".csv";

        /// <summary>
        /// 时间零点
        /// </summary>
        private DateTime m_ZeroDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        /// <summary>
        /// 单个文件写入次数
        /// </summary>
        private int m_nFileWriteNum = 57600;

        /// <summary>
        /// 写入数据到文件的次数——写入固定大小的数据后就会换一个文件再写，然后m_nPkgNum归零，重新计数
        /// </summary>
        private int m_nPkgNum = 0;

        /// <summary>
        /// 当前日期时间
        /// </summary>
        private DateTime m_sCurDateTime = DateTime.Now;

        /// <summary>
        /// 写入数据文件的缓冲区大小
        /// </summary>
        private int m_nDataFileBuffSize = 1024;

        /// <summary>
        /// 设备采样频率
        /// </summary>
        private SamplingFreq m_eSamplingFreq = SamplingFreq.FREQ_250;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        private DataFileWriter()
        {
        }

        /// <summary>
        /// 获取唯一实例
        /// </summary>
        /// <returns>唯一的数据文件写入器实例</returns>
        public static DataFileWriter GetInstance()
        {
            if (null == m_Instance)
            {
                m_Instance = new DataFileWriter();
            }

            return m_Instance;
        }

        /// <summary>
        /// 设置数据文件存储路径
        /// </summary>
        /// <param name="strDataFilePath">数据文件存储路径</param>
        public void SetDataFilePath(string strDataFilePath)
        {
            m_strDataFilePath = strDataFilePath;
        }

        /// <summary>
        /// 获取数据文件存储路径
        /// </summary>
        /// <returns>数据文件存储路径</returns>
        public string GetDataFilePath()
        {
            return m_strDataFilePath;
        }

        /// <summary>
        /// 根据采样频率同步数据文件写入器的参数
        /// </summary>
        /// <param name="eSamplingFreq">采样频率</param>
        public void SyncParameterBySamplingFreq(SamplingFreq eSamplingFreq)
        {
            switch (eSamplingFreq)
            {
                case SamplingFreq.FREQ_1:       // 1Hz
                    {
                        m_nDataFileBuffSize = 1024;
                        m_nFileWriteNum = 57600;
                        //m_strSuffix = ".TSA";
                        break;
                    }
                case SamplingFreq.FREQ_10:      // 10Hz
                    {
                        m_nDataFileBuffSize = 1024;
                        m_nFileWriteNum = 57600;
                        //m_strSuffix = ".TSB";
                        break;
                    }
                case SamplingFreq.FREQ_250:     //  250Hz
                    {
                        m_nDataFileBuffSize = 1024;
                        m_nFileWriteNum = 57600;
                        //m_strSuffix = ".TSL";
                        break;
                    }
                case SamplingFreq.FREQ_500:     // 500Hz
                    {
                        m_nDataFileBuffSize = 1024 * 2;
                        m_nFileWriteNum = 3600;
                        //m_strSuffix = ".TSM";
                        break;
                    }
                case SamplingFreq.FREQ_1000:     // 1000Hz
                    {
                        m_nDataFileBuffSize = 1024 * 4;
                        m_nFileWriteNum = 900;
                        //m_strSuffix = ".TSH";
                        break;
                    }
                case SamplingFreq.FREQ_4000:     // 4000Hz
                    {
                        m_nDataFileBuffSize = 1024 * 16;
                        m_nFileWriteNum = 900;
                        //m_strSuffix = ".TSI";
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        /// <summary>
        /// 设备采样频率变化事件处理函数
        /// </summary>
        public void OnDeviceSamplingFreqChanged(uint nDeviceID, SamplingFreq eSamplingFreq)
        {
            if (m_eSamplingFreq == eSamplingFreq)
            {// 没有变化
                return;
            }

            // 同步数据文件写入器的参数
            SyncParameterBySamplingFreq(eSamplingFreq);

            m_eSamplingFreq = eSamplingFreq;
        }

        /// <summary>
        /// 异步写入多通道磁场数据
        /// </summary>
        /// <param name="sMagDatas">多通道磁场数据</param>
        public void AsyncWrite(MultiChannelMagData[] sMagDatas)
        {
            if (null == m_fsDataFile)
            {
                if (null != sMagDatas && sMagDatas.Length > 0 && sMagDatas[0].m_dMagData.Length > 0)
                {
                    m_sCurDateTime = DateTime.Now;
                    GenDataFolderByDate(m_sCurDateTime);    // 生成文件夹

                    int nChannelCount = sMagDatas[0].m_dMagData.Length; // 保存的数据通道数量
                    OpenNewFile(nChannelCount); // 打开一个新文件
                }
                else
                {
                    return;
                }        
            }

            DateTime sNow = DateTime.Now;
            if (m_sCurDateTime.Year != sNow.Year 
                || m_sCurDateTime.Month != sNow.Month 
                || m_sCurDateTime.Day != sNow.Day)
            {
                if (m_fsDataFile != null)
                {
                    m_fsDataFile.Close();
                    m_fsDataFile = null;

                    m_sCurDateTime = sNow;
                    GenDataFolderByDate(m_sCurDateTime);

                    int nChannelCount = sMagDatas[0].m_dMagData.Length; // 保存的数据通道数量
                    OpenNewFile(nChannelCount); // 打开一个新文件
                }
            }
         
            for (int i = 0; i < sMagDatas.Length; ++i)
            {
                int nLineLength = 0;    // 一行数据的长度

                ulong nMs = sMagDatas[i].m_nTimeStamp / 1000;
                ulong nUs = sMagDatas[i].m_nTimeStamp % 1000;
                DateTime sTimeStamp = m_ZeroDateTime.AddMilliseconds((double)nMs);
                string strTimeStamp = sTimeStamp.ToString("yyyyMMddHHmmssfff") + nUs.ToString("D3");
                //string strTimeStamp = sMagDatas[i].m_nTimeStamp.ToString();
                byte[] byTimeStamp = Encoding.UTF8.GetBytes(strTimeStamp);
                nLineLength += byTimeStamp.Length;

                string strValueLine = ",";
                for (int j = 0; j < sMagDatas[i].m_dMagData.Length; ++j)
                {
                    string strValue = string.Format("{0:0.###}", sMagDatas[i].m_dMagData[j]);
                    if (j != sMagDatas[i].m_dMagData.Length - 1)
                    {
                        strValueLine = strValueLine + strValue + ",";
                    }
                    else
                    {
                        strValueLine = strValueLine + strValue + "\n";
                    }
                }

                byte[] byMagData = Encoding.UTF8.GetBytes(strValueLine);
                nLineLength += byMagData.Length;

                byte[] byLineMagData = new byte[nLineLength];       // 一行数据的字节流
                // 拷贝时间戳
                Buffer.BlockCopy(byTimeStamp, 0, byLineMagData, 0, byTimeStamp.Length);
                // 拷贝数据
                Buffer.BlockCopy(byMagData, 0, byLineMagData, byTimeStamp.Length, byMagData.Length);

                // 将数据写入到文件中
                m_fsDataFile.BeginWrite(byLineMagData, 0, byLineMagData.Length, new AsyncCallback(EndWriteCallBack), m_fsDataFile);
                m_fsDataFile.Flush();
            }
            
            ++m_nPkgNum;    // 写入次数+1
            if (m_nPkgNum == m_nFileWriteNum)   // 写入达到固定次数，文件大小达到固定大小，换一个文件继续写入
            {
                m_nPkgNum = 0;

                m_fsDataFile.Close();   // 关闭文件
                m_fsDataFile = null;

                // 新文件名
                int nChannelCount = sMagDatas[0].m_dMagData.Length; // 保存的数据通道数量
                OpenNewFile(nChannelCount);
            }
        }

        /// <summary>
        /// 结束写入
        /// </summary>
        public void EndWrite()
        {
            if (m_fsDataFile != null)
            {
                m_fsDataFile.Flush();
                m_fsDataFile.Close();
                m_fsDataFile = null;
            }
        }

        /// <summary>
        /// 数据文件写入器是否正在工作
        /// </summary>
        /// <returns>数据文件写入器是否正在工作</returns>
        public bool IsRunning()
        {
            return m_fsDataFile != null;
        }

        /// <summary>
        /// 打开一个新文件
        /// </summary>
        /// <param name="nChannelCount">通道数，用于生成新的文件名</param>
        private void OpenNewFile(int nChannelCount)
        {
            string strDataFileName = m_strDataFilePathByDate + "\\data" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + nChannelCount + m_strSuffix;
            m_fsDataFile = new FileStream(strDataFileName, FileMode.Append, FileAccess.Write, FileShare.Write, m_nDataFileBuffSize, true);

            // 写文件头
            string strHeaderLine = "TimeStamp,";
            for (int i = 0; i < nChannelCount; ++i)
            {
                if (i != nChannelCount - 1)
                {
                    strHeaderLine = strHeaderLine + "Channel" + (i + 1).ToString() + ",";
                }
                else
                {
                    strHeaderLine = strHeaderLine + "Channel" + (i + 1).ToString() + "\n";
                }
            }

            byte[] byHeaderLine = Encoding.UTF8.GetBytes(strHeaderLine);
            m_fsDataFile.Write(byHeaderLine, 0, byHeaderLine.Length);
        }

        /// <summary>
        /// 按照日期建立数据文件夹
        /// </summary>
        /// <param name="sDateTime">日期时间</param>
        private void GenDataFolderByDate(DateTime sDateTime)
        {
            string strDate = sDateTime.ToString("yyyy_MM_dd");
            m_strDataFilePathByDate = m_strDataFilePath + "\\" + strDate;
            CreateFolderRecursive(m_strDataFilePathByDate);
        }

        /// <summary>
        /// 递归创建文件夹
        /// </summary>
        /// <param name="path">给定的路径</param>
        private static void CreateFolderRecursive(string path)
        {
            string[] folders = path.Split('\\');
            for (int i = 0; i < folders.Length; ++i)
            {
                string curFolderPath = string.Join("\\", folders, 0, i + 1);
                if (!Directory.Exists(curFolderPath))
                {
                    Directory.CreateDirectory(curFolderPath);
                }
            }
        }

        /// <summary>
        /// 结束异步写入回调
        /// </summary>
        /// <param name="result"></param>
        private void EndWriteCallBack(IAsyncResult result)
        {
            FileStream fs = (FileStream)result.AsyncState;
            //结束异步写入
            fs.EndWrite(result);
        }

    }
}
