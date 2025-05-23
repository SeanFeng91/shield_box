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
    /// 异常数据文件写入器
    /// </summary>
    class AbnormalDataFileWriter
    {
        /// <summary>
        /// 唯一实例
        /// </summary>
        private static AbnormalDataFileWriter m_Instance = null;

        /// <summary>
        /// 数据文件存储路径
        /// </summary>
        private string m_strDataFilePath = "";

        /// <summary>
        /// 数据文件流
        /// </summary>
        private FileStream m_fsDataFile = null;

        /// <summary>
        /// 文件后缀
        /// </summary>
        private string m_strSuffix = ".csv";

        /// <summary>
        /// 写入数据文件的缓冲区大小
        /// </summary>
        private int m_nDataFileBuffSize = 1024;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        private AbnormalDataFileWriter()
        {
        }

        /// <summary>
        /// 获取唯一实例
        /// </summary>
        /// <returns>唯一的数据文件写入器实例</returns>
        public static AbnormalDataFileWriter GetInstance()
        {
            if (null == m_Instance)
            {
                m_Instance = new AbnormalDataFileWriter();
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
        /// 异步写入异常多通道磁场数据
        /// </summary>
        /// <param name="sMagDatas">异常多通道磁场数据</param>
        public void AsyncWrite(MultiChannelMagData[] sMagDatas)
        {

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
            string strDataFileName = m_strDataFilePath + "\\data" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + nChannelCount + m_strSuffix;
            m_fsDataFile = new FileStream(strDataFileName, FileMode.Append, FileAccess.Write, FileShare.Write, m_nDataFileBuffSize, true);
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
