using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCMag30FDevice;
using SigProcess;
using CalibrateSensor;

namespace MCMAG30F
{
    /// <summary>
    /// 数据缓冲器
    /// </summary>
    public class DataBuffer
    {
        /// <summary>
        /// 发布数据事件处理器
        /// </summary>
        public delegate void PublishDataEventHandler(MultiChannelMagData[] arrMagDatas);

        /// <summary>
        /// 停止接收数据事件处理器
        /// </summary>
        public delegate void StopReceiveDataEventHandler();

        /// <summary>
        /// 设置零偏完成的事件处理器
        /// </summary>
        public delegate void SetZeroOffsetDoneEventHandler();

        /// <summary>
        /// 发布数据事件
        /// </summary>
        public event PublishDataEventHandler PublishDataEvent;

        /// <summary>
        /// 停止接收数据事件
        /// </summary>
        public event StopReceiveDataEventHandler StopReceiveDataEvent;

        /// <summary>
        /// 设置零偏完成的事件
        /// </summary>
        public event SetZeroOffsetDoneEventHandler SetZeroOffsetDoneEvent;

        /// <summary>
        /// 唯一实例
        /// </summary>
        private static DataBuffer m_Instance = null;

        /// <summary>
        /// 要缓存数据的设备数量
        /// </summary>
        private int m_nDeviceCount = 0;

        /// <summary>
        /// 设备多通道磁场数据--只有部分设备接收到的数据，有些设备还没有接收到，数据在这里缓存
        /// </summary>
        private Dictionary<int, List<MultiChannelMagData>> m_DictDeviceReceivingDatas = new Dictionary<int, List<MultiChannelMagData>>();

        /// <summary>
        /// 接收数据的起始时间
        /// </summary>
        private DateTime m_StartDateTime;

        /// <summary>
        /// 上一次处理所有设备都接收到的数据的时间
        /// </summary>
        private DateTime m_LastDateTime;

        private DateTime m_ZeroDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        /// <summary>
        /// 相邻两个采样点的时间间隔，单位：us
        /// </summary>
        private int m_nSamplingInterval = 1000;

        /// <summary>
        /// 已采样样本总数
        /// </summary>
        private ulong m_nSamplingCount = 0;

        /// <summary>
        /// 每台设备包含的通道数
        /// </summary>
        private const int m_nChannelCountPerDevice = 30;

        /// <summary>
        /// 采集数据的模式
        /// </summary>
        private SamplingMode m_eSamplingMode = SamplingMode.UNKNOWN;    // 采集数据的模式，分为：连续采集、单次采集

        /// <summary>
        /// 采集通道比例系数D
        /// </summary>
        private Dictionary<uint, double[]> m_dictDeviceID2fD = new Dictionary<uint, double[]>();

        /// <summary>
        /// 采集通道零点d
        /// </summary>
        private Dictionary<uint, double[]> m_dictDeviceID2fd = new Dictionary<uint, double[]>();

        /// <summary>
        /// 传感器比例系数S
        /// </summary>
        private Dictionary<uint, double[]> m_dictDeviceID2fS = new Dictionary<uint, double[]>();

        /// <summary>
        /// 传感器修正的比例系数
        /// </summary>
        private Dictionary<uint, double[]> m_dictDeviceID2fK = new Dictionary<uint, double[]>();

        /// <summary>
        /// 传感器零点b
        /// </summary>
        private Dictionary<uint, double[]> m_dictDeviceID2fb = new Dictionary<uint, double[]>();

        /// <summary>
        /// XYZ轴系数
        /// </summary>
        private Dictionary<uint, double[,]> m_dictDeviceID2AxisCoefficient = new Dictionary<uint, double[,]>(); 

        /// <summary>
        /// 跳过的无效点点数
        /// </summary>
        private int m_nSkipPointCount = 0;

        /// <summary>
        /// 所有通道当前用于滤波的缓存数据
        /// </summary>
        private double[,] m_dAllChannelFilterCacheData = null;
        private int m_nAllChannelFilterCacheDataIndex = 0;
        private int m_nAllChannelFilterCacheDataCount = 0;
        private bool m_bLowPassFilterEnabled = false;
        private double[] m_dLowPassFilterCoeffs = null;
        private FilterByDiffFun[] m_pLowFilterByDiffFuns = null;
        private DownsampleConverter m_pDownSampleConvert = null;

        // 卡尔曼滤波器
        private KalmanFilter[] m_pKalmanFilter = null;

        // 传感器标定器
        private SensorCalibrater[] m_pSensorCalibrater = null;

        ////////////////////////////////////////////////////////
        // 数据归零功能相关
        private double[] m_dOffsetMultiChannelMagData = new double[300];
        private bool m_bSetZeroFlag = false;
        private int m_nSetZeroNum = 0;
        ////////////////////////////////////////////////////////

        /// <summary>
        /// 默认构造函数
        /// </summary>
        private DataBuffer()
        {        
            
        }

        /// <summary>
        /// 获取唯一实例
        /// </summary>
        /// <returns>数据缓冲器</returns>
        public static DataBuffer GetInstance()
        {
            if (null == m_Instance)
            {
                m_Instance = new DataBuffer();
            }

            return m_Instance;
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public void initData()
        {
            // 先清空数据，再重新加载
            m_dictDeviceID2fD.Clear();
            m_dictDeviceID2fd.Clear();
            m_dictDeviceID2fK.Clear();
            m_dictDeviceID2fb.Clear();
            m_dictDeviceID2fS.Clear();
            m_dictDeviceID2AxisCoefficient.Clear();

            Dictionary<uint, int> dictDeviceID2Index = MainConfig.GetInstance().GetDeviceID2IndexDict();          
            foreach (KeyValuePair<uint, int> kvp in dictDeviceID2Index)
            {
                m_dictDeviceID2fD.Add(kvp.Key, new double[30]);
                m_dictDeviceID2fd.Add(kvp.Key, new double[30]);
                m_dictDeviceID2fK.Add(kvp.Key, new double[30]);
                m_dictDeviceID2fb.Add(kvp.Key, new double[30]);
                m_dictDeviceID2fS.Add(kvp.Key, new double[10]);
                m_dictDeviceID2AxisCoefficient.Add(kvp.Key, new double[30, 3]);
            }
            int nDeviceCount = MainConfig.GetInstance().GetDeviceCount();    // 设备数
            float[] fDs = new float[nDeviceCount * 30];    // 采集通道比例系数Di
            float[] fds = new float[nDeviceCount * 30];    // 采集通道零点di
            float[] fKs = new float[nDeviceCount * 30];    // 传感器修正比例系数Ki
            float[] fbs = new float[nDeviceCount * 30];    // 传感器零点bi
            float[] fKx = new float[nDeviceCount * 30];    // X轴系数
            float[] fKy = new float[nDeviceCount * 30];    // Y轴系数
            float[] fKz = new float[nDeviceCount * 30];    // Z轴系数
            for (int i = 0; i < nDeviceCount * 30; ++i)
            {
                MainConfig.GetInstance().GetCollectDAndd(i, ref fDs[i], ref fds[i]);
                MainConfig.GetInstance().GetSensorKAndb(i, ref fKs[i], ref fbs[i]);
                MainConfig.GetInstance().GetAxisCoefficient(i, ref fKx[i], ref fKy[i], ref fKz[i]);
            }

            float[] fSs = new float[nDeviceCount * 30 / 3]; // 传感器比例系数Si
            for (int i = 0; i < nDeviceCount * 30 / 3; ++i)
            {
                MainConfig.GetInstance().GetSensorS(i, ref fSs[i]);
            }

            foreach (KeyValuePair<uint, int> kvp in dictDeviceID2Index)
            {
                int nDeviceIndex = kvp.Value;
                for (int i = 0; i < 30; ++i)
                {
                    m_dictDeviceID2fD[kvp.Key][i] = fDs[nDeviceIndex * 30 + i];
                    m_dictDeviceID2fd[kvp.Key][i] = fds[nDeviceIndex * 30 + i];
                    m_dictDeviceID2fK[kvp.Key][i] = fKs[nDeviceIndex * 30 + i];
                    m_dictDeviceID2fb[kvp.Key][i] = fbs[nDeviceIndex * 30 + i];
                    m_dictDeviceID2AxisCoefficient[kvp.Key][i, 0] = fKx[nDeviceIndex * 30 + i];
                    m_dictDeviceID2AxisCoefficient[kvp.Key][i, 1] = fKy[nDeviceIndex * 30 + i];
                    m_dictDeviceID2AxisCoefficient[kvp.Key][i, 2] = fKz[nDeviceIndex * 30 + i];
                }

                for (int i = 0; i < 10; ++i)
                {
                    m_dictDeviceID2fS[kvp.Key][i] = fSs[nDeviceIndex * 10 + i];
                }
            }

            m_bLowPassFilterEnabled = MainConfig.GetInstance().IsLowPassFilterEnabled();
            SamplingFreq eSamplingFreq = MainConfig.GetInstance().GetSamplingFreq();
            if (eSamplingFreq == SamplingFreq.FREQ_1 || eSamplingFreq == SamplingFreq.FREQ_10)
            {// 采样率为1Hz或者10Hz，强制低通滤波
                m_bLowPassFilterEnabled = true;
            }
            m_dLowPassFilterCoeffs = MainConfig.GetInstance().GetLowPassFilterCoeffs();
            m_dAllChannelFilterCacheData = new double[nDeviceCount * 30, m_dLowPassFilterCoeffs.Length];
            m_nAllChannelFilterCacheDataCount = 0;
            m_nAllChannelFilterCacheDataIndex = 0;

            m_nSkipPointCount = 0;
            m_nSamplingCount = 0;
        }

        /// <summary>
        /// 设备采样频率变化事件处理函数
        /// </summary>
        public void OnDeviceSamplingFreqChanged(uint nDeviceID, SamplingFreq eSamplingFreq)
        {
            switch (eSamplingFreq)
            {
                case SamplingFreq.FREQ_1:
                case SamplingFreq.FREQ_10:
                case SamplingFreq.FREQ_250:
                    {
                        m_nSamplingInterval = 4000; // 4ms，即4000us
                        break;
                    }
                case SamplingFreq.FREQ_500:
                    {
                        m_nSamplingInterval = 2000; // 2ms，即2000us
                        break;
                    }
                case SamplingFreq.FREQ_1000:
                    {
                        m_nSamplingInterval = 1000; // 1ms，即1000us
                        break;
                    }
                case SamplingFreq.FREQ_4000:
                    {
                        m_nSamplingInterval = 250;  // 0.25ms，即250us
                        break;
                    }
                default:
                    {
                        m_nSamplingInterval = 1000; // 1ms，即1000us
                        break;
                    }
            }
        }

        /// <summary>
        /// 设置要缓存数据的设备数量
        /// </summary>
        /// <param name="nDeviceCount"></param>
        public void SetDeviceCount(int nDeviceCount)
        {
            m_nDeviceCount = nDeviceCount;

            m_DictDeviceReceivingDatas.Clear();
            
            for (int i = 0; i < m_nDeviceCount; ++i)
            {
                m_DictDeviceReceivingDatas[i] = new List<MultiChannelMagData>();
            }
        }

        /// <summary>
        /// 获取要缓存数据的设备数量
        /// </summary>
        /// <returns>返回要缓存数据的设备数量</returns>
        public int GetDeviceCount()
        {
            return m_nDeviceCount;
        }

        /// <summary>
        /// 设置接收数据的起始时间戳
        /// </summary>
        /// <param name="dt">起始时间戳</param>
        public void SetStartDateTime(DateTime dt)
        {
            m_StartDateTime = dt;
        }

        /// <summary>
        /// 获取接收数据的起始时间戳
        /// </summary>
        /// <returns>起始时间戳</returns>
        public DateTime GetStartDateTime()
        {
            return m_StartDateTime;
        }

        /// <summary>
        /// 设置上次接收数据的时间戳
        /// </summary>
        /// <param name="dt">上次接收数据的时间戳</param>
        public void SetLastDateTime(DateTime dt)
        {
            m_LastDateTime = dt;
        }

        /// <summary>
        /// 获取上次接收数据的时间戳
        /// </summary>
        /// <returns>上次接收数据的时间戳</returns>
        public DateTime GetLastDateTime()
        {
            return m_LastDateTime;
        }

        /// <summary>
        /// 设置采集模式
        /// </summary>
        /// <param name="eSamplingMode">采集模式</param>
        public void SetSamplingMode(SamplingMode eSamplingMode)
        {
            m_eSamplingMode = eSamplingMode;
        }

        /// <summary>
        /// 获取采集模式
        /// </summary>
        /// <returns>采集模式</returns>
        public SamplingMode GetSamplingMode()
        {
            return m_eSamplingMode;
        }

        /// <summary>
        /// 添加多通道磁场数据
        /// </summary>
        /// <param name="nDeviceID">设备ID</param>
        /// <param name="sDeviceData">从设备接收到的数据</param>
        public void AddMagData(uint nDeviceID, ref DeviceData[] sDeviceData)
        {
            MultiChannelMagData[] allDeviceReceivedDatas = null;

            int nValidChannelCountPerDevice = MainConfig.GetInstance().GetValidChannelCountPerDevice();

            lock (m_DictDeviceReceivingDatas)
            {
                int nDeviceIndex = MainConfig.GetInstance().GetDeviceIndex(nDeviceID);  // 将设备ID转换为设备索引Index
                MultiChannelMagData[] sMultiChannelMagDatas = CalcMagData(nDeviceID, ref sDeviceData);
                m_DictDeviceReceivingDatas[nDeviceIndex].AddRange(sMultiChannelMagDatas); // 添加数据

                int nMinCount = CalcMinDataCount(); // 计算所有设备中包含的数据最少点数

                // 所有设备同时接收到的数据大于0，把所有设备同时接收到的数据放入allDeviceReceivedDatas缓存中
                if (nMinCount > 0)
                {
                    // 计算时间戳方案一：取当次时间-上次时间的差值，等间隔赋值给所有数据
                    //DateTime curDateTime = DateTime.Now;    // 当前时间
                    //double dMs = (curDateTime - m_LastDateTime).TotalMilliseconds; // 从上个最新时间到现在经历的毫秒数
                    //double dStep = dMs / nMinCount; // 时间间隔，单位：ms

                    //double us0 = (m_LastDateTime - m_ZeroDateTime).TotalMilliseconds;
                    //ulong us1 = Convert.ToUInt64(us0);
                    //us1 = us1 * 1000;
                    //// 赋值时间戳
                    //foreach (var item in m_DictDeviceReceivingDatas)
                    //{
                    //    for (int i = 0; i < nMinCount; ++i)
                    //    {
                    //        item.Value[i].m_nTimeStamp = us1 + Convert.ToUInt64(dStep * 1000 * (i + 1));
                    //    }
                    //}

                    /////////////////////////////////////////////////////////////////////////////////
                    // 计算时间戳方案二：根据开始采集时刻，采样率和采集总数据量，计算每个数据的时间戳
                    // 赋值时间戳
                    double us0 = (m_StartDateTime - m_ZeroDateTime).TotalMilliseconds;
                    ulong us1 = Convert.ToUInt64(us0) * 1000;
                    ulong nInterval = Convert.ToUInt64(m_nSamplingInterval);
                    foreach (var item in m_DictDeviceReceivingDatas)
                    {
                        for (int i = 0; i < nMinCount; ++i)
                        {
                            ulong nDataIndex = m_nSamplingCount + Convert.ToUInt64(i + 1);
                            item.Value[i].m_nTimeStamp = nDataIndex * nInterval + us1;
                        }
                    }
                    m_nSamplingCount += Convert.ToUInt64(nMinCount);
                    /////////////////////////////////////////////////////////////////////////////////

                    // 将多台设备的所有通道数据缓存到一个MultiChannelMagData对象里面，即将多台设备看做一台设备，只是设备的通道数变多了
                    allDeviceReceivedDatas = new MultiChannelMagData[nMinCount];
                    for (int i = 0; i < nMinCount; ++i)
                    {
                        // 创建磁场数据对象
                        allDeviceReceivedDatas[i] = new MultiChannelMagData();
                        allDeviceReceivedDatas[i].m_dMagData = new double[nValidChannelCountPerDevice * m_nDeviceCount];

                        // 给磁场数据对象赋值
                        allDeviceReceivedDatas[i].m_nTimeStamp = m_DictDeviceReceivingDatas[0][i].m_nTimeStamp; // 直接取第1台设备数据的时间戳 

                        for (int j = 0; j < m_nDeviceCount; ++j)
                        {
                            for (int k = 0; k < nValidChannelCountPerDevice; ++k)
                            {
                                allDeviceReceivedDatas[i].m_dMagData[j * nValidChannelCountPerDevice + k] = m_DictDeviceReceivingDatas[j][i].m_dMagData[k];
                            }
                        }
                    }

                    // 清除已发布的数据
                    foreach (var item in m_DictDeviceReceivingDatas)
                    {
                        item.Value.RemoveRange(0, nMinCount);
                    }

                    //m_LastDateTime = curDateTime;   // 更新上次接收数据的时间戳
                }
            }          

            if (allDeviceReceivedDatas != null && allDeviceReceivedDatas.Length > 0)
            {
                if (m_nSkipPointCount < MainConfig.GetInstance().GetSkipPointCount())
                {
                    // 计算需要移除的点数
                    int nRemoveCount = MainConfig.GetInstance().GetSkipPointCount() - m_nSkipPointCount;
                    if (nRemoveCount > allDeviceReceivedDatas.Length)
                    {
                        nRemoveCount = allDeviceReceivedDatas.Length;
                    }
                    m_nSkipPointCount += nRemoveCount;
                    // 剩余点数
                    int nRemainderCount = allDeviceReceivedDatas.Length - nRemoveCount;
                    if (nRemainderCount > 0)
                    {
                        // 记录有效数据
                        MultiChannelMagData[] tempData = new MultiChannelMagData[nRemainderCount];
                        for (int i = 0; i < nRemainderCount; ++i)
                        {
                            tempData[i] = allDeviceReceivedDatas[i + nRemoveCount];
                        }

                        allDeviceReceivedDatas = tempData;
                    }
                    else
                    {
                        return;
                    }
                }

                // 低通滤波
                MultiChannelMagData[] filteredData = null;
                if (m_bLowPassFilterEnabled)
                {
                    filteredData = new MultiChannelMagData[allDeviceReceivedDatas.Length];
                    for (int i = 0; i < allDeviceReceivedDatas.Length; ++i)
                    {
                        filteredData[i] = new MultiChannelMagData();
                        filteredData[i].m_nTimeStamp = allDeviceReceivedDatas[i].m_nTimeStamp;
                        filteredData[i].m_dMagData = new double[allDeviceReceivedDatas[i].m_dMagData.Length];

                        // 卡尔曼滤波
                        for (int j = 0; j < allDeviceReceivedDatas[i].m_dMagData.Length; ++j)
                        {
                            filteredData[i].m_dMagData[j] = m_pKalmanFilter[j].sigFilter(allDeviceReceivedDatas[i].m_dMagData[j]);
                        }
                    }
                }

                MultiChannelMagData[] pTempData = null;
                if (m_bLowPassFilterEnabled)
                {
                    pTempData = filteredData;
                }
                else
                {
                    pTempData = allDeviceReceivedDatas;
                }

                // 降采样
                if (m_pDownSampleConvert != null)
                {
                    pTempData = m_pDownSampleConvert.GetDownsampledData(pTempData);
                }

                // 计算归零偏移值
                if (m_nSetZeroNum == 1 && pTempData != null)
                {
                    if (pTempData.Length > 0 && pTempData[0].m_dMagData.Length > 0)
                    {
                        int nChannelCount = pTempData[0].m_dMagData.Length; // 通道数
                        double[] dSums = new double[nChannelCount];         // 总和

                        for (int i = 0; i < pTempData.Length; ++i)
                        {
                            for (int j = 0; j < pTempData[i].m_dMagData.Length; ++j)
                            {
                                dSums[j] += pTempData[i].m_dMagData[j];
                            }
                        }

                        for (int i = 0; i < nChannelCount; ++i)
                        {
                            m_dOffsetMultiChannelMagData[i] = dSums[i] / pTempData.Length;
                        }

                        // offset只赋值一次
                        m_nSetZeroNum = 2;
                        if (SetZeroOffsetDoneEvent != null)
                        {
                            SetZeroOffsetDoneEvent();
                        }
                    }                
                }

                if (m_bSetZeroFlag && pTempData != null)
                {// 如果归零，则数据都要减偏移值
                    for (int i = 0; i < pTempData.Length; ++i)
                    {
                        for (int j = 0; j < pTempData[i].m_dMagData.Length; ++j)
                        {
                            pTempData[i].m_dMagData[j] -= m_dOffsetMultiChannelMagData[j];
                        }
                    }
                }

                // 发布数据
                if (PublishDataEvent != null)
                {
                    if (pTempData != null)
                    {
                        PublishDataEvent(pTempData);
                    }
                }

                // 是否发送停止缓存数据的信号
                if (StopReceiveDataEvent != null && m_eSamplingMode == SamplingMode.SINGLE)
                {
                    double dElapsedMs = (m_LastDateTime - m_StartDateTime).TotalMilliseconds; // 从开始到现在经历的毫秒数
                    if (dElapsedMs > MainConfig.GetInstance().GetSamplingDuration() * 1000)
                    {
                        StopReceiveDataEvent();
                    }
                }

                // 清空数据
                allDeviceReceivedDatas = null;
                filteredData = null;
            }
        }

        /// <summary>
        /// 转换磁场数据
        /// </summary>
        /// <param name="sDeviceData">从设备接收到的数据</param>
        /// <returns>转换得到的磁场数据</returns>
        private MultiChannelMagData[] CalcMagData(uint nDeviceID, ref DeviceData[] sDeviceData)
        {
            if (null == sDeviceData)
                return null;

            int nDeviceIndex = MainConfig.GetInstance().GetDeviceIndex(nDeviceID);  // 将设备ID转换为设备索引Index

            int nValidChannelCount = MainConfig.GetInstance().GetValidChannelCountPerDevice();
            int[] nChannelDataIndexs = new int[nValidChannelCount];
            MainConfig.GetInstance().GetChannelDataIndex(out nChannelDataIndexs);

            MultiChannelMagData[] sMultiChannelMagData = new MultiChannelMagData[sDeviceData.Length];
            for (int i = 0; i < sDeviceData.Length; ++i)
            {
                sMultiChannelMagData[i] = new MultiChannelMagData();
                sMultiChannelMagData[i].m_dMagData = new double[nValidChannelCount];
                // 转换数据
                double[] fDs = m_dictDeviceID2fD[nDeviceID];
                double[] fds = m_dictDeviceID2fd[nDeviceID];
                double[] fKs = m_dictDeviceID2fK[nDeviceID];
                double[] fbs = m_dictDeviceID2fb[nDeviceID];
                double[] fSs = m_dictDeviceID2fS[nDeviceID];
                int nChannelIndex = 0;  // 通道索引，按照实际使用来，取设备的0~14,16~31通道共计30个，去掉15,31通道
                for (int j = 0; j < sDeviceData[i].dChannelDatas.Length; ++j)
                {
                    if (nChannelDataIndexs[j] != -1)
                    {
                        nChannelIndex = nChannelDataIndexs[j];
                        double dValue = sDeviceData[i].dChannelDatas[j] * 4;
                        dValue = (dValue - fds[nChannelIndex]) / fDs[nChannelIndex] * fSs[nChannelIndex / 3] / fKs[nChannelIndex] * 100 - fbs[nChannelIndex];
                        sMultiChannelMagData[i].m_dMagData[nChannelIndex] = dValue;

                        //++nChannelIndex;
                    }
                }

                // 传感器校正
                for (int j = 0; j < nValidChannelCount / 3; ++j)
                {
                    // 原始输入
                    double dInputX = sMultiChannelMagData[i].m_dMagData[j * 3 + 0];
                    double dInputY = sMultiChannelMagData[i].m_dMagData[j * 3 + 1];
                    double dInputZ = sMultiChannelMagData[i].m_dMagData[j * 3 + 2];
                    // 校正后的数据
                    double dOutX = 0;
                    double dOutY = 0;
                    double dOutZ = 0;
                    double dOutT = 0;

                    m_pSensorCalibrater[nDeviceIndex * 10 + j].CalibrateSensorData(dInputX, dInputY, dInputZ, ref dOutX, ref dOutY, ref dOutZ, ref dOutT);

                    sMultiChannelMagData[i].m_dMagData[j * 3 + 0] = dOutX;
                    sMultiChannelMagData[i].m_dMagData[j * 3 + 1] = dOutY;
                    sMultiChannelMagData[i].m_dMagData[j * 3 + 2] = dOutZ;
                }

                double[,] fKAxis = m_dictDeviceID2AxisCoefficient[nDeviceID];
                // 调整XYZ轴系数
                double[] dTempData = new double[nValidChannelCount];
                for (int j = 0; j < nValidChannelCount; ++j)
                {
                    if  (j % 3 == 0)
                    {
                        // X轴
                        dTempData[j] = fKAxis[j, 0] * sMultiChannelMagData[i].m_dMagData[j]
                            + fKAxis[j, 1] * sMultiChannelMagData[i].m_dMagData[j + 1]
                            + fKAxis[j, 2] * sMultiChannelMagData[i].m_dMagData[j + 2];
                    }
                    else if (j % 3 == 1)
                    {
                        // Y轴
                        dTempData[j] = fKAxis[j, 0] * sMultiChannelMagData[i].m_dMagData[j - 1]
                            + fKAxis[j, 1] * sMultiChannelMagData[i].m_dMagData[j]
                            + fKAxis[j, 2] * sMultiChannelMagData[i].m_dMagData[j + 1];
                    }
                    else if (j % 3 == 2)
                    {
                        // Z轴
                        dTempData[j] = fKAxis[j, 0] * sMultiChannelMagData[i].m_dMagData[j - 2]
                            + fKAxis[j, 1] * sMultiChannelMagData[i].m_dMagData[j - 1]
                            + fKAxis[j, 2] * sMultiChannelMagData[i].m_dMagData[j];
                    }
                }

                // 重新给MagData赋值
                for (int j = 0; j < nValidChannelCount; ++j)
                {
                    sMultiChannelMagData[i].m_dMagData[j] = dTempData[j];
                }

            }

            return sMultiChannelMagData;
        }

        /// <summary>
        /// 计算所有设备中包含的数据最少点数
        /// </summary>
        /// <returns>最少点数</returns>
        private int CalcMinDataCount()
        {          
            List<int> lstReceivedDataCounts = new List<int>();  // 每台设备接收到的数据点数
            // 获取所有数据点数
            foreach (var item in m_DictDeviceReceivingDatas)
            {
                lstReceivedDataCounts.Add(item.Value.Count);
            }

            // 计算最小点数
            int nMinCount = int.MaxValue;  // 所有设备中包含的数据最少点数
            for (int i = 0; i < lstReceivedDataCounts.Count; ++i)
            {
                if (lstReceivedDataCounts[i] < nMinCount)
                {
                    nMinCount = lstReceivedDataCounts[i];
                }
            }

            return nMinCount;
        }

        /// <summary>
        /// 计算指定通道当前用于滤波的有效数据的平均值
        /// </summary>
        /// <param name="nChannelIndex">通道索引</param>
        /// <returns></returns>
        private double CalcValidDataAvg(int nChannelIndex)
        {
            int nCoeffCount = m_dLowPassFilterCoeffs.Length;    // 滤波器系数个数
            int nValidCount = (m_nAllChannelFilterCacheDataCount < nCoeffCount) ? m_nAllChannelFilterCacheDataCount : nCoeffCount;  // 有效数据个数
            double dAvg = 0;    // 平均值
            for (int i = 0; i < nValidCount; ++i)
            {
                int nCurIndex = m_nAllChannelFilterCacheDataIndex - 1 - i;
                nCurIndex = (nCurIndex < 0) ? (nCurIndex + nCoeffCount) : nCurIndex;
                dAvg += m_dAllChannelFilterCacheData[nChannelIndex, nCurIndex];
            }
            
            if (nValidCount > 0)
            {
                dAvg = dAvg / (double)nValidCount;
            }

            return dAvg;
        }

        /// <summary>
        /// 计算指定通道低通滤波后的值
        /// </summary>
        /// <param name="nChannelIndex">通道索引</param>
        /// <returns></returns>
        private double CalcLowPassFilterValue(int nChannelIndex)
        {
            double dFilteredValue = 0.0;        // 低通滤波计算后的值
            int nCoeffCount = m_dLowPassFilterCoeffs.Length;    // 滤波器系数个数
            int nValidCount = (m_nAllChannelFilterCacheDataCount < nCoeffCount) ? m_nAllChannelFilterCacheDataCount : nCoeffCount;  // 有效数据个数

            double dAvg = CalcValidDataAvg(nChannelIndex);

            for (int i = 0; i < nValidCount; ++i)
            {
                int nCurIndex = m_nAllChannelFilterCacheDataIndex - 1 - i;
                nCurIndex = (nCurIndex < 0) ? (nCurIndex + nCoeffCount) : nCurIndex;    // 数组索引是循环的

                dFilteredValue += (m_dAllChannelFilterCacheData[nChannelIndex, nCurIndex] - dAvg) * m_dLowPassFilterCoeffs[i];
            }

            dFilteredValue += dAvg;

            return dFilteredValue;
        }

        /// <summary>
        /// 创建低通滤波器
        /// </summary>
        public void CreateFilterByDiffFun()
        {
            if (m_bLowPassFilterEnabled)
            {
                m_pLowFilterByDiffFuns = new FilterByDiffFun[m_nChannelCountPerDevice * m_nDeviceCount];
                for (int i = 0; i < m_nChannelCountPerDevice * m_nDeviceCount; ++i)
                {
                    m_pLowFilterByDiffFuns[i] = MainConfig.GetInstance().CreateLowPassFilterByDiffFun();
                }
            }
            else
            {
                m_pLowFilterByDiffFuns = null;
            }
        }

        /// <summary>
        /// 创建卡尔曼滤波器
        /// </summary>
        public void CreateKalmanFilter()
        {
            if (m_bLowPassFilterEnabled)
            {
                m_pKalmanFilter = new KalmanFilter[m_nChannelCountPerDevice * m_nDeviceCount];
                for (int i = 0; i < m_nChannelCountPerDevice * m_nDeviceCount; ++i)
                {
                    m_pKalmanFilter[i] = new KalmanFilter();
                    m_pKalmanFilter[i].setR(MainConfig.GetInstance().GetKalmanFilterParamR(i));
                }
            }
            else
            {
                m_pKalmanFilter = null;
            }
        }

        /// <summary>
        /// 重置卡尔曼滤波器
        /// </summary>
        public void resetKalmanFilter()
        {
            if (m_pKalmanFilter != null)
            {
                for (int i = 0; i < m_nChannelCountPerDevice * m_nDeviceCount; ++i)
                {
                    if (m_pKalmanFilter[i] != null)
                    {
                        m_pKalmanFilter[i].init();
                    }
                }
            }
        }
        

        /// <summary>
        /// 创建传感器标定器
        /// </summary>
        public void CreateSensorCalibrater()
        {
            int nNum = m_nChannelCountPerDevice / 3 * m_nDeviceCount;
            m_pSensorCalibrater = new SensorCalibrater[nNum];
            for (int i = 0; i < nNum; ++i)
            {
                double[] sensorParam = null;
                MainConfig.GetInstance().GetSensorCalibrateParam(i, out sensorParam);
                if (sensorParam != null && sensorParam.Length >= 9)
                {
                    m_pSensorCalibrater[i] = new SensorCalibrater(sensorParam[0], sensorParam[1], sensorParam[2],
                        sensorParam[3], sensorParam[4], sensorParam[5],
                        sensorParam[6], sensorParam[7], sensorParam[8]);
                }               
            }
        }

        /// <summary>
        /// 根据采样频率创建减采样转换器
        /// </summary>
        /// <param name="eSamplingFreq">采样频率</param>
        public void CreateDownsampleConverter(SamplingFreq eSamplingFreq)
        {
            if (eSamplingFreq == SamplingFreq.FREQ_1)
            {
                m_pDownSampleConvert = new DownsampleConverter();
                m_pDownSampleConvert.SetDecreaseSampleRateFactor(250);
            }
            else if (eSamplingFreq == SamplingFreq.FREQ_10)
            {
                m_pDownSampleConvert = new DownsampleConverter();
                m_pDownSampleConvert.SetDecreaseSampleRateFactor(25);
            }
            else
            {
                m_pDownSampleConvert = null;
            }
        }

        /// <summary>
        /// 设置零偏
        /// </summary>
        public void SetZeroOffset()
        {
            m_bSetZeroFlag = true;
            m_nSetZeroNum = 1;
        }

        /// <summary>
        /// 取消零偏
        /// </summary>
        public void CancelZeroOffset()
        {
            m_bSetZeroFlag = false;
            m_nSetZeroNum = 0;
        }

        /// <summary>
        /// 获取是否归零的标志
        /// </summary>
        /// <returns></returns>
        public bool GetSetZeroFlag()
        {
            return m_bSetZeroFlag;
        }

    }
}
