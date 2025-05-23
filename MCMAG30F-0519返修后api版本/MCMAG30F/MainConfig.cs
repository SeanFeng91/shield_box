using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MCMag30FDevice;

namespace MCMAG30F
{
    /// <summary>
    /// 软件的主要配置项，采用单例模式实现
    /// </summary>
    public class MainConfig
    {
        private static MainConfig m_Instance = null;        // 定义一个静态变量来保存唯一实例
        private static object m_lockObject = new object();

        private string m_strIniPath = null;                 // 配置文件路径
        private INIConfigHelper m_ConfigRWInstance = null;  // 配置文件读写对象实例

        private int m_nDeviceCount = 1;                     // 设备数量
        private Dictionary<uint, int> m_DictDeviceID2Index = new Dictionary<uint, int>(); // 设备ID与设备索引的Dict

        private const int m_nGroupCount = 30;                // 一共是30组传感器
        private const int m_nChannelCountPerGroup = 3;       // 每组传感器是3通道
        private const int m_nCurveCount = 30;                // 曲线控件个数是30个

        private int m_nUsedChannelCount = m_nGroupCount * m_nChannelCountPerGroup;   // 实际使用的通道数量
        private SamplingMode m_eSamplingMode = SamplingMode.UNKNOWN;                 // 采集模式
        private SamplingFreq m_eSamplingFreq = SamplingFreq.UNKNOWN;                // 采样频率
        private float m_fSamplingDuration = 0;                                    // 采样持续时间

        private ADCMode m_eADCMode = ADCMode.UNKNOWN;                    // AC/DC模式
        private ChanMode m_eChanMode = ChanMode.UNKNOWN;         // Chan Mode
        private TimingType m_eTimingType = TimingType.UNKNOWN;            // 授时类型，0--PC授时；1--GPS授时

        private float[] m_fSs = new float[m_nGroupCount];                           // 传感器比例系数Si,单位100uT/10V
        private float[] m_fKs = new float[m_nGroupCount * m_nChannelCountPerGroup]; // 传感器比例系数的修正系数
        private float[] m_fbs = new float[m_nGroupCount * m_nChannelCountPerGroup]; // 传感器零点bi
        private float[] m_fDs = new float[m_nGroupCount * m_nChannelCountPerGroup]; // 采集通道比例系数Di
        private float[] m_fds = new float[m_nGroupCount * m_nChannelCountPerGroup]; // 采集通道零点di

        private float[,] m_fAxis = new float[m_nGroupCount * m_nChannelCountPerGroup, 3];   // XYZ轴的调整系数，计算公式为Kx*X+Ky*Y+Kz*Z
        private double[][] m_fSensorCalibrateParam = new double[m_nGroupCount][]; // 传感器校正系数

        private int[] m_nChannelDataIndex = new int[32];    // 通道索引与数据索引的对应关系，如果数据索引为-1，则表示不是有效数据
        private int m_nValidChannelCount = 0;   // 一台设备的有效通道数量

        private int[] m_nCurveChannelIndexs = new int[30];          // 一共是30组曲线控件
        private int m_nShowCurveChannelCount = 30;                  // 要显示的曲线通道数
        private string m_strDataSavePath;                           // 数据文件保存路径
        private bool m_bNeedSaveData = true;                        // 是否需要保存数据

        private int m_nYAxisUnit = 1;                               // Y轴单位，1--nT，2--uT
        private int m_nXAxisUnit = 1;                               // X轴单位，1--秒

        private int m_nSkipPointCount = 2;                          // 需要跳过的无效数据

        private float m_dKxz = 0;                                   // Z轴校正系数Kxz
        private float m_dKyz = 0;                                   // Z轴校正系数Kyz
        private float m_dKzz = 1;                                   // Z轴校正系数Kzz

        private string m_strModbusTCPServerIP;                      // ModbusTCP服务器IP
        private int m_nModbusTCPServerPort;                         // ModbusTCP服务器端口号
        private int m_nModbusTimingCycle;                           // Modbus轮询定时周期
        private bool m_bModbusDebugEnable;                          // Modbus调试使能，true，会输出日志，false，不会输出日志
        private string m_strModbusLogFilePath;                      // Modbus日志文件
        private int m_nModbusNumberOfRetries;                       // Modbus重试次数
        private int m_nModbusTimeout = 500;                         // 通信超时
        private int m_nModbusReconnectInterval = 1000;              // 重连间隔时间

        private double[] m_dMagValueRanges = new double[90];        // 磁场正常值与零偏的范围
        private int m_nAbnormalDataMaxShowCount = 1000;             // 界面显示异常数据最大条数

        private bool m_bLowPassFilterEnabled = false;               // 是否需要低通滤波  
        private float m_dLowPassFilterPassFreq = 0;                // 低通滤波通带频率
        private float m_dLowPassFilterStopFreq = 0;                // 低通滤波阻带频率
        private float m_dLowPassFilterPassDB = 0;                  // 低通滤波通带衰减
        private float m_dLowPassFilterStopDB = 0;                  // 低通滤波阻带衰减
        private double[] m_dLowPassFilterCoeffs = null;             // 低通滤波系数

        // 卡尔曼滤波器系数R
        private double[] m_dKalmanFilter_R = new double[m_nGroupCount * m_nChannelCountPerGroup];  // 卡尔曼滤波器系数P

        /// <summary>
        /// 默认构造函数
        /// </summary>
        private MainConfig()
        {
            m_strIniPath = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "Config.ini");
            m_ConfigRWInstance = new INIConfigHelper(m_strIniPath);

            for (int i = 0; i < m_nGroupCount; ++i)
            {
                float.TryParse(CONFIG_DEFAULT_VALUE_SENSOR_S, out m_fSs[i]);
            }

            for (int i = 0; i < m_nGroupCount * m_nChannelCountPerGroup; ++i)
            {
                float.TryParse(CONFIG_DEFAULT_VALUE_SENSOR_K, out m_fKs[i]);
                float.TryParse(CONFIG_DEFAULT_VALUE_SENSOR_B, out m_fbs[i]);
                float.TryParse(CONFIG_DEFAULT_VALUE_COLLECT_D, out m_fDs[i]);
                float.TryParse(CONFIG_DEFAULT_VALUE_COLLECT_F, out m_fds[i]);
            }
        }

        /// <summary>
        /// 获取唯一实例
        /// </summary>
        /// <returns>主要配置对象</returns>
        public static MainConfig GetInstance()
        {
            if (null == m_Instance)
            {
                lock (m_lockObject)
                {
                    if (null == m_Instance)
                    {
                        m_Instance = new MainConfig();
                    }
                }
            }

            return m_Instance;
        }

        /// <summary>
        /// 读取配置项
        /// </summary>
        public void ReadConfig()
        {
            if (!System.IO.File.Exists(m_strIniPath))
            {
                // 设备数量
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_DEVICE, CONFIG_KEY_DEVICE_COUNT, CONFIG_DEFAULT_VALUE_DEVICE_COUNT);
                m_nDeviceCount = int.Parse(CONFIG_DEFAULT_VALUE_DEVICE_COUNT);

                // 设备VID和PID
                for (int i = 0; i < m_nDeviceCount; ++i)
                {
                    string strKey = CONFIG_KEY_VID + i.ToString();
                    string strValue = CONFIG_DEFAULT_VALUE_VID;
                    ushort nVID = Convert.ToUInt16(strValue, 16);
                    m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_DEVICE, strKey, strValue);

                    strKey = CONFIG_KEY_PID + i.ToString();
                    strValue = CONFIG_DEFAULT_VALUE_PID;
                    ushort nPID = Convert.ToUInt16(strValue, 16);
                    m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_DEVICE, strKey, strValue);

                    uint nDeviceID = GenDeviceID(nVID, nPID);
                    m_DictDeviceID2Index[nDeviceID] = i;
                }

                // 使用的通道数
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_CHANNEL, CONFIG_KEY_USED_CHANNEL_COUNT, CONFIG_DEFAULT_VALUE_USED_CHANNEL_COUNT);
                m_nUsedChannelCount = int.Parse(CONFIG_DEFAULT_VALUE_USED_CHANNEL_COUNT);

                // 传感器的比例系数Si
                for (int i = 0; i < m_fSs.Length; ++i)
                {
                    string strKey = CONFIG_KEY_SENSOR_S + i.ToString();
                    m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_SENSOR, strKey, CONFIG_DEFAULT_VALUE_SENSOR_S);
                    m_fSs[i] = float.Parse(CONFIG_DEFAULT_VALUE_SENSOR_S);
                }

                // 传感器比例系数Ki、零点bi
                // 采集通道比例系数Di、零点di
                // XYZ轴调整系数
                for (int i = 0; i < m_fKs.Length; ++i)
                {
                    string strKey = CONFIG_KEY_SENSOR_K + i.ToString();
                    m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_SENSOR, strKey, CONFIG_DEFAULT_VALUE_SENSOR_K);
                    m_fKs[i] = float.Parse(CONFIG_DEFAULT_VALUE_SENSOR_K);

                    strKey = CONFIG_KEY_SENSOR_B + i.ToString();
                    m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_SENSOR, strKey, CONFIG_DEFAULT_VALUE_SENSOR_B);
                    m_fbs[i] = float.Parse(CONFIG_DEFAULT_VALUE_SENSOR_B);

                    strKey = CONFIG_KEY_COLLECT_D + i.ToString();
                    m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_SENSOR, strKey, CONFIG_DEFAULT_VALUE_COLLECT_D);
                    m_fDs[i] = float.Parse(CONFIG_DEFAULT_VALUE_COLLECT_D);

                    strKey = CONFIG_KEY_COLLECT_F + i.ToString();
                    m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_SENSOR, strKey, CONFIG_DEFAULT_VALUE_COLLECT_F);
                    m_fds[i] = float.Parse(CONFIG_DEFAULT_VALUE_COLLECT_F);

                    strKey = CONFIG_KEY_AXIS_K + i.ToString();
                    if (i % 3 == 0)
                    {
                        m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_COEFFICIENT, strKey, CONFIG_DEFAULT_VALUE_AXIS_KX);
                        m_fAxis[i, 0] = 1;
                        m_fAxis[i, 1] = 0;
                        m_fAxis[i, 2] = 0;
                    }
                    else if (i % 3 == 1)
                    {
                        m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_COEFFICIENT, strKey, CONFIG_DEFAULT_VALUE_AXIS_KY);
                        m_fAxis[i, 0] = 0;
                        m_fAxis[i, 1] = 1;
                        m_fAxis[i, 2] = 0;
                    }
                    else if (i % 3 == 2)
                    {
                        m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_COEFFICIENT, strKey, CONFIG_DEFAULT_VALUE_AXIS_KZ);
                        m_fAxis[i, 0] = 0;
                        m_fAxis[i, 1] = 0;
                        m_fAxis[i, 2] = 1;
                    }
                }

                // 传感器校正系数
                for (int i = 0; i < m_nGroupCount; ++i)
                {
                    string strKey = CONFIG_KEY_SENSOR_CALIBRATE_PARAM + i.ToString();
                    m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_SENSOR_CALIBRATE_PARAM, strKey, CONFIG_DEFAULT_VALUE_SENSOR_CALIBRATE_PARAM);
                    m_fSensorCalibrateParam[i] = new double[9];
                    m_fSensorCalibrateParam[i][0] = 0;
                    m_fSensorCalibrateParam[i][1] = 0;
                    m_fSensorCalibrateParam[i][2] = 0;
                    m_fSensorCalibrateParam[i][3] = 1;
                    m_fSensorCalibrateParam[i][4] = 1;
                    m_fSensorCalibrateParam[i][5] = 1;
                    m_fSensorCalibrateParam[i][6] = 0;
                    m_fSensorCalibrateParam[i][7] = 0;
                    m_fSensorCalibrateParam[i][8] = 0;
                }

                // AC/DC
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_SENSOR, CONFIG_KEY_ADC, CONFIG_DEFAULT_VALUE_ADC);
                m_eADCMode = (ADCMode)int.Parse(CONFIG_DEFAULT_VALUE_ADC);

                // Chan-Mode
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_SENSOR, CONFIG_KEY_CHANMODE, ONFIG_DEFAULT_VALUE_CHANMODE);
                m_eChanMode = (ChanMode)int.Parse(ONFIG_DEFAULT_VALUE_CHANMODE);

                // 采样参数
                // 采样模式
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_SAMPLING_PARAM, CONFIG_KEY_SAMPLING_MODE, CONFIG_DEFAULT_VALUE_SAMPLING_MODE);
                m_eSamplingMode = (SamplingMode)int.Parse(CONFIG_DEFAULT_VALUE_SAMPLING_MODE);

                // 采样频率
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_SAMPLING_PARAM, CONFIG_KEY_SAMPLING_FREQ, CONFIG_DEFAULT_VALUE_SAMPLING_FREQ);
                m_eSamplingFreq = ConvertSamplingFreqStr2Enum(CONFIG_DEFAULT_VALUE_SAMPLING_FREQ);

                // 采样时间
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_SAMPLING_PARAM, CONFIG_KEY_SAMPLING_DURATION, CONFIG_DEFAULT_VALUE_SAMPLING_DURATION);
                m_fSamplingDuration = float.Parse(CONFIG_DEFAULT_VALUE_SAMPLING_DURATION);

                //  通道索引与数据索引的对应关系
                for (int i = 0; i < 32; ++i)
                {
                    string strKey = CONFIG_KEY_CHANNEL_INDEX + i.ToString();
                    if (i < 30)
                    {
                        m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_CHANNELDATAINDEX, strKey, i.ToString());
                        m_nChannelDataIndex[i] = i;
                    }
                    else
                    {
                        m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_CHANNELDATAINDEX, strKey, (-1).ToString());
                        m_nChannelDataIndex[i] = -1;
                    }
                }
                m_nValidChannelCount = 30;

                // 显示曲线通道数
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_CURVECHANNELINDEX, CONFIG_KEY_SHOWCURVECHANNELCOUNT, CONFIG_DEFAULT_VALUE_SHOWCURVECHANNELCOUNT);
                m_nShowCurveChannelCount = int.Parse(CONFIG_DEFAULT_VALUE_SHOWCURVECHANNELCOUNT);
                // 曲线——通道Dict
                for (int i = 0; i < m_nCurveCount; ++i)
                {
                    string strKey = CONFIG_KEY_CURVE + i.ToString();
                    m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_CURVECHANNELINDEX, strKey, i.ToString());
                    m_nCurveChannelIndexs[i] = i;
                }

                // Modbus TCP服务器的IP地址和端口号
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_PLC, CONFIG_KEY_PLC_SERVER_IP, CONFIG_DEFAULT_VALUE_PLC_SERVER_IP);
                m_strModbusTCPServerIP = CONFIG_DEFAULT_VALUE_PLC_SERVER_IP;
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_PLC, CONFIG_KEY_PLC_SERVER_PORT, CONFIG_DEFAULT_VALUE_PLC_SERVER_PORT);
                m_nModbusTCPServerPort = int.Parse(CONFIG_DEFAULT_VALUE_PLC_SERVER_PORT);
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_PLC, CONFIG_KEY_PLC_TIMING_CYCLE, CONFIG_DEFAULT_VALUE_PLC_TIMING_CYCLE);
                m_nModbusTimingCycle = int.Parse(CONFIG_DEFAULT_VALUE_PLC_TIMING_CYCLE);
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_PLC, CONFIG_KEY_PLC_DEBUG_ENABLE, CONFIG_DEFAULT_VALUE_PLC_DEBUG_ENABLE);
                int nTemp = int.Parse(CONFIG_DEFAULT_VALUE_PLC_DEBUG_ENABLE);
                m_bModbusDebugEnable = (nTemp == 1);
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_PLC, CONFIG_KEY_PLC_LOG_FILE_PATH, CONFIG_DEFAULT_VALUE_PLC_LOG_FILE_PATH);
                m_strModbusLogFilePath = CONFIG_DEFAULT_VALUE_PLC_LOG_FILE_PATH;
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_PLC, CONFIG_KEY_PLC_NUMBER_OF_RETRIES, CONFIG_DEFAULT_VALUE_NUMBER_OF_RETRIES);
                m_nModbusNumberOfRetries = int.Parse(CONFIG_DEFAULT_VALUE_NUMBER_OF_RETRIES);
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_PLC, CONFIG_KEY_PLC_TIMEOUT, CONFIG_DEFAULT_VALUE_PLC_TIMEOUT);
                m_nModbusTimeout = int.Parse(CONFIG_DEFAULT_VALUE_PLC_TIMEOUT);
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_PLC, CONFIG_KEY_PLC_RECONNECT_INTERVAL, CONFIG_DEFAULT_VALUE_PLC_RECONNECT_INTERVAL);
                m_nModbusReconnectInterval = int.Parse(CONFIG_DEFAULT_VALUE_PLC_RECONNECT_INTERVAL);

                // 磁场正常值与零偏的范围
                for (int i = 0; i < 90; ++i)
                {
                    string strKey = CONFIG_KEY_MAG_VALUE_RANGE + i.ToString();
                    m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_MAG_VALID_VALUE_RANGE, strKey, CONFIG_DEFAULT_VALUE_MAG_VALUE_RANGE);
                    double.TryParse(CONFIG_DEFAULT_VALUE_MAG_VALUE_RANGE, out m_dMagValueRanges[i]);
                }

                // 界面显示异常数据最大条数
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_ABNORMAL_DATA, CONFIG_KEY_ABNORMAL_DATA_MAX_SHOW_COUNT, CONFIG_DEFAULT_VALUE_ABNORMAL_DATA_MAX_SHOW_COUNT);
                m_nAbnormalDataMaxShowCount = int.Parse(CONFIG_DEFAULT_VALUE_ABNORMAL_DATA_MAX_SHOW_COUNT);

                // 数据文件保存路径
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_DATAFILE, CONFIG_KEY_SAVEPATH, CONFIG_DEFAULT_VALUE_SAVEPATH);
                m_strDataSavePath = CONFIG_DEFAULT_VALUE_SAVEPATH;
                // 是否需要保存数据
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_DATAFILE, CONFIG_KEY_NEEDSAVEDATA, CONFIG_DEFAULT_VALUE_NEEDSAVEDATA);
                nTemp = int.Parse(CONFIG_DEFAULT_VALUE_NEEDSAVEDATA);
                m_bNeedSaveData = Convert.ToBoolean(nTemp);
                // 需要跳过的无效数据点数
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_DATAFILE, CONFIG_KEY_SKIPPOINTCOUNT, CONFIG_DEFAULT_VALUE_SKIPPOINTCOUNT);
                m_nSkipPointCount = int.Parse(CONFIG_DEFAULT_VALUE_SKIPPOINTCOUNT);

                // 坐标轴设置
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_AXIS_SETTING, CONFIG_KEY_YAXIS_UNIT, CONFIG_DEFAULT_VALUE_YAXIS_UNIT);
                m_nYAxisUnit = int.Parse(CONFIG_DEFAULT_VALUE_YAXIS_UNIT);
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_AXIS_SETTING, CONFIG_KEY_XAXIS_UNIT, CONFIG_DEFAULT_VALUE_XAXIS_UNIT);
                m_nXAxisUnit = int.Parse(CONFIG_DEFAULT_VALUE_XAXIS_UNIT);

                // 低通滤波器系数
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_LOWPASS_FILTER_COEFF, CONFIG_KEY_LOWPASS_FILTER_ENABLE, CONFIG_DEFAULT_VALUE_LOWPASS_FILTER_ENABLE);
                m_bLowPassFilterEnabled = false;
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_LOWPASS_FILTER_COEFF, CONFIG_KEY_LOWPASS_FILTER_COEFF, CONFIG_DEFAULT_VALUE_LOWPASS_FILTER_COEFF);
                m_dLowPassFilterCoeffs = new double[2] { 0.5, 0.5 };

                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_LOWPASS_FILTER_COEFF, CONFIG_KEY_LOWPASS_FILTER_PASSFREQ, CONFIG_DEFAULT_VALUE_LOWPASS_FILTER_PASSFREQ);
                m_dLowPassFilterPassFreq = float.Parse(CONFIG_DEFAULT_VALUE_LOWPASS_FILTER_PASSFREQ);
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_LOWPASS_FILTER_COEFF, CONFIG_KEY_LOWPASS_FILTER_STOPFREQ, CONFIG_DEFAULT_VALUE_LOWPASS_FILTER_STOPFREQ);
                m_dLowPassFilterStopFreq = float.Parse(CONFIG_DEFAULT_VALUE_LOWPASS_FILTER_STOPFREQ);

                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_LOWPASS_FILTER_COEFF, CONFIG_KEY_LOWPASS_FILTER_PASSDB, CONFIG_DEFAULT_VALUE_LOWPASS_FILTER_PASSDB);
                m_dLowPassFilterPassDB = float.Parse(CONFIG_DEFAULT_VALUE_LOWPASS_FILTER_PASSDB);
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_LOWPASS_FILTER_COEFF, CONFIG_KEY_LOWPASS_FILTER_STOPDB, CONFIG_DEFAULT_VALUE_LOWPASS_FILTER_STOPDB);
                m_dLowPassFilterStopDB = float.Parse(CONFIG_DEFAULT_VALUE_LOWPASS_FILTER_STOPDB);

                // 卡尔曼滤波器参数
                for (int i = 0; i < m_nGroupCount * m_nChannelCountPerGroup; ++i)
                {
                    string strKey = CONFIG_KEY_KALMAN_FILTER_PARAM_R + i.ToString();
                    m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_KALMAN_FILTER_PARAM, strKey, CONFIG_DEFAULT_VALUE_KALMAN_FILTER_PARAM_R);

                    m_dKalmanFilter_R[i] = 1000;
                }

                // 授时类型
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_TIMING_TYPE, CONFIG_KEY_TIMING_TYPE, CONFIG_DEFAULT_VALUE_TIMING_TYPE);
                nTemp = int.Parse(CONFIG_DEFAULT_VALUE_TIMING_TYPE);
                m_eTimingType = (TimingType)nTemp;
            }
            else
            {
                // 设备数量
                string strDeviceCount = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_DEVICE, CONFIG_KEY_DEVICE_COUNT);
                int.TryParse(strDeviceCount, out m_nDeviceCount);

                // 设备VID和PID
                for (int i = 0; i < m_nDeviceCount; ++i)
                {
                    string strKeyVID = CONFIG_KEY_VID + i.ToString();
                    string strValueVID = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_DEVICE, strKeyVID);
                    ushort nVID = Convert.ToUInt16(strValueVID, 16);

                    string strKeyPID = CONFIG_KEY_PID + i.ToString();
                    string strValuePID = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_DEVICE, strKeyPID);
                    ushort nPID = Convert.ToUInt16(strValuePID, 16);
                    uint nDeviceID = GenDeviceID(nVID, nPID);
                    m_DictDeviceID2Index[nDeviceID] = i;
                }

                // 使用的通道数
                string strUsedDeviceCount = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_CHANNEL, CONFIG_KEY_USED_CHANNEL_COUNT);
                m_nUsedChannelCount = int.Parse(CONFIG_DEFAULT_VALUE_USED_CHANNEL_COUNT);
                int.TryParse(strUsedDeviceCount, out m_nUsedChannelCount);

                // 传感器的比例系数Si
                for (int i = 0; i < m_fSs.Length; ++i)
                {
                    string strKeyIndex = CONFIG_KEY_SENSOR_S + i.ToString();
                    string strValueS = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_SENSOR, strKeyIndex);
                    float.TryParse(strValueS, out m_fSs[i]);
                }

                // 传感器比例系数Ki、零点bi
                // 采集通道比例系数Di、零点di
                for (int i = 0; i < m_fKs.Length; ++i)
                {
                    string strKey = CONFIG_KEY_SENSOR_K + i.ToString();
                    string strValue = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_SENSOR, strKey);
                    float.TryParse(strValue, out m_fKs[i]);

                    strKey = CONFIG_KEY_SENSOR_B + i.ToString();
                    strValue = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_SENSOR, strKey);
                    float.TryParse(strValue, out m_fbs[i]);

                    strKey = CONFIG_KEY_COLLECT_D + i.ToString();
                    strValue = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_SENSOR, strKey);
                    float.TryParse(strValue, out m_fDs[i]);

                    strKey = CONFIG_KEY_COLLECT_F + i.ToString();
                    strValue = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_SENSOR, strKey);
                    float.TryParse(strValue, out m_fds[i]);

                    strKey = CONFIG_KEY_AXIS_K + i.ToString();
                    strValue = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_COEFFICIENT, strKey);
                    string[] strTempCoeffs = strValue.Split(',');
                    for (int j = 0; j < 3; ++j)
                    {
                        float.TryParse(strTempCoeffs[j], out m_fAxis[i, j]);
                    }
                }

                // 传感器校正系数
                for (int i = 0; i < m_nGroupCount; ++i)
                {
                    string strKey = CONFIG_KEY_SENSOR_CALIBRATE_PARAM + i.ToString();
                    string strValue = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_SENSOR_CALIBRATE_PARAM, strKey);
                    string[] strTempCoeffs = strValue.Split(',');
                    m_fSensorCalibrateParam[i] = new double[9];
                    for (int j = 0; j < 9; ++j)
                    {
                        double.TryParse(strTempCoeffs[j], out m_fSensorCalibrateParam[i][j]);
                    }
                }

                // AC/DC
                string strTemp = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_SENSOR, CONFIG_KEY_ADC);
                int nTemp = 0;
                int.TryParse(strTemp, out nTemp);
                m_eADCMode = (ADCMode)nTemp;

                // Chan-Mode
                strTemp = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_SENSOR, CONFIG_KEY_CHANMODE);
                int.TryParse(strTemp, out nTemp);
                m_eChanMode = (ChanMode)nTemp;

                // 采样参数
                // 采样模式
                strTemp = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_SAMPLING_PARAM, CONFIG_KEY_SAMPLING_MODE);
                int.TryParse(strTemp, out nTemp);
                m_eSamplingMode = (SamplingMode)nTemp;

                // 采样频率
                strTemp = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_SAMPLING_PARAM, CONFIG_KEY_SAMPLING_FREQ);
                m_eSamplingFreq = ConvertSamplingFreqStr2Enum(strTemp);

                // 采样时间
                strTemp = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_SAMPLING_PARAM, CONFIG_KEY_SAMPLING_DURATION);
                float.TryParse(strTemp, out m_fSamplingDuration);

                //  通道索引与数据索引的对应关系
                m_nValidChannelCount = 0;
                for (int i = 0; i < 32; ++i)
                {
                    string strKey = CONFIG_KEY_CHANNEL_INDEX + i.ToString();
                    strTemp = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_CHANNELDATAINDEX, strKey);
                    m_nChannelDataIndex[i] = -1;
                    int.TryParse(strTemp, out m_nChannelDataIndex[i]);

                    if (m_nChannelDataIndex[i] != -1)
                    {
                        ++m_nValidChannelCount;
                    }
                }

                // 显示曲线通道数
                strTemp = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_CURVECHANNELINDEX, CONFIG_KEY_SHOWCURVECHANNELCOUNT);
                int.TryParse(strTemp, out m_nShowCurveChannelCount);
                // 曲线——通道Dict
                for (int i = 0; i < m_nCurveCount; ++i)
                {
                    string strKey = CONFIG_KEY_CURVE + i.ToString();
                    strTemp = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_CURVECHANNELINDEX, strKey);
                    m_nCurveChannelIndexs[i] = int.Parse(CONFIG_DEFAULT_VALUE_CURVE);
                    int.TryParse(strTemp, out m_nCurveChannelIndexs[i]);
                }

                // Modbus TCP服务器的IP地址和端口号
                m_strModbusTCPServerIP = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_PLC, CONFIG_KEY_PLC_SERVER_IP);
                strTemp = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_PLC, CONFIG_KEY_PLC_SERVER_PORT);
                int.TryParse(strTemp, out m_nModbusTCPServerPort);
                strTemp = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_PLC, CONFIG_KEY_PLC_DEBUG_ENABLE);
                int.TryParse(strTemp, out nTemp);
                m_bModbusDebugEnable = (nTemp != 0);
                m_strModbusLogFilePath = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_PLC, CONFIG_KEY_PLC_LOG_FILE_PATH);
                strTemp = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_PLC, CONFIG_KEY_PLC_NUMBER_OF_RETRIES);
                int.TryParse(strTemp, out m_nModbusNumberOfRetries);
                strTemp = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_PLC, CONFIG_KEY_PLC_TIMING_CYCLE);
                int.TryParse(strTemp, out m_nModbusTimingCycle);
                strTemp = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_PLC, CONFIG_KEY_PLC_TIMEOUT);
                int.TryParse(strTemp, out m_nModbusTimeout);
                strTemp = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_PLC, CONFIG_KEY_PLC_RECONNECT_INTERVAL);
                int.TryParse(strTemp, out m_nModbusReconnectInterval);

                // 磁场正常值与零偏的范围
                for (int i = 0; i < 90; ++i)
                {
                    string strKey = CONFIG_KEY_MAG_VALUE_RANGE + i.ToString();
                    strTemp = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_MAG_VALID_VALUE_RANGE, strKey);
                    double.TryParse(strTemp, out m_dMagValueRanges[i]);
                }

                // 界面显示异常数据最大条数
                strTemp = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_ABNORMAL_DATA, CONFIG_KEY_ABNORMAL_DATA_MAX_SHOW_COUNT);
                int nNum = 0;
                bool bRet = int.TryParse(strTemp, out nNum);
                if (bRet)
                {
                    m_nAbnormalDataMaxShowCount = nNum;
                }

                // 数据文件保存路径
                m_strDataSavePath = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_DATAFILE, CONFIG_KEY_SAVEPATH);
                // 是否需要保存数据
                strTemp = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_DATAFILE, CONFIG_KEY_NEEDSAVEDATA);
                int.TryParse(strTemp, out nTemp);
                m_bNeedSaveData = Convert.ToBoolean(nTemp);
                // 需要跳过的无效数据点数
                strTemp = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_DATAFILE, CONFIG_KEY_SKIPPOINTCOUNT);
                int.TryParse(strTemp, out m_nSkipPointCount);

                // 坐标轴设置
                strTemp = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_AXIS_SETTING, CONFIG_KEY_YAXIS_UNIT);
                int.TryParse(strTemp, out m_nYAxisUnit);
                strTemp = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_AXIS_SETTING, CONFIG_KEY_XAXIS_UNIT);
                int.TryParse(strTemp, out m_nXAxisUnit);

                // 低通滤波器系数
                strTemp = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_LOWPASS_FILTER_COEFF, CONFIG_KEY_LOWPASS_FILTER_ENABLE);
                int.TryParse(strTemp, out nTemp);
                m_bLowPassFilterEnabled = (nTemp == 0 ? false : true);
                string strLowPass = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_LOWPASS_FILTER_COEFF, CONFIG_KEY_LOWPASS_FILTER_COEFF);
                string[] strSplitCoeffs = strLowPass.Split(',');
                m_dLowPassFilterCoeffs = new double[strSplitCoeffs.Length];
                for (int i = 0; i < strSplitCoeffs.Length; ++i)
                {
                    double.TryParse(strSplitCoeffs[i], out m_dLowPassFilterCoeffs[i]);
                }

                strTemp = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_LOWPASS_FILTER_COEFF, CONFIG_KEY_LOWPASS_FILTER_PASSFREQ);
                float.TryParse(strTemp, out m_dLowPassFilterPassFreq);
                strTemp = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_LOWPASS_FILTER_COEFF, CONFIG_KEY_LOWPASS_FILTER_STOPFREQ);
                float.TryParse(strTemp, out m_dLowPassFilterStopFreq);
                strTemp = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_LOWPASS_FILTER_COEFF, CONFIG_KEY_LOWPASS_FILTER_PASSDB);
                float.TryParse(strTemp, out m_dLowPassFilterPassDB);
                strTemp = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_LOWPASS_FILTER_COEFF, CONFIG_KEY_LOWPASS_FILTER_STOPDB);
                float.TryParse(strTemp, out m_dLowPassFilterStopDB);

                // 卡尔曼滤波器参数
                for (int i = 0; i < m_nGroupCount * m_nChannelCountPerGroup; ++i)
                {
                    string strKey = CONFIG_KEY_KALMAN_FILTER_PARAM_R + i.ToString();
                    string strValue = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_KALMAN_FILTER_PARAM, strKey);
                    double.TryParse(strValue, out m_dKalmanFilter_R[i]);
                }

                // 授时类型
                strTemp = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_TIMING_TYPE, CONFIG_KEY_TIMING_TYPE);
                int.TryParse(strTemp, out nTemp);
                m_eTimingType = (TimingType)nTemp;
            }
        }

        /// <summary>
        /// 获取传感器的比例系数S
        /// </summary>
        /// <param name="nSensorIndex">传感器索引</param>
        /// <param name="fS">传感器的比例系数S</param>
        public void GetSensorS(int nSensorIndex, ref float fS)
        {
            if (nSensorIndex >= 0 && nSensorIndex < m_fSs.Length)
            {
                fS = m_fSs[nSensorIndex];
            }
        }

        /// <summary>
        /// 设置传感器比例系数S
        /// </summary>
        /// <param name="nSensorIndex">传感器索引</param>
        /// <param name="fS">传感器比例系数S</param>
        public void SetSensorS(int nSensorIndex, float fS)
        {
            if (nSensorIndex >= 0 && nSensorIndex < m_fSs.Length && m_fSs[nSensorIndex] != fS)
            {
                m_fSs[nSensorIndex] = fS;

                if (m_ConfigRWInstance != null)
                {
                    string strKey = CONFIG_KEY_SENSOR_S + nSensorIndex.ToString();
                    m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_SENSOR, strKey, m_fSs[nSensorIndex].ToString("f6"));
                }
            }
        }

        /// <summary>
        /// 获取传感器的比例系数K和零点b
        /// </summary>
        /// <param name="nChannelIndex">传感器通道索引</param>
        /// <param name="fK">传感器的比例系数K</param>
        /// <param name="fb">传感器的零点b</param>
        public void GetSensorKAndb(int nChannelIndex, ref float fK, ref float fb)
        {
            if (nChannelIndex >= 0 && nChannelIndex < m_fKs.Length)
            {
                fK = m_fKs[nChannelIndex];
                fb = m_fbs[nChannelIndex];
            }
        }

        /// <summary>
        /// 获取采集通道比例系数D和零点d
        /// </summary>
        /// <param name="nChannelIndex">采集通道索引</param>
        /// <param name="fD">比例系数D</param>
        /// <param name="fd">零点d</param>
        public void GetCollectDAndd(int nChannelIndex, ref float fD, ref float fd)
        {
            if (nChannelIndex >= 0 && nChannelIndex < m_fDs.Length)
            {
                fD = m_fDs[nChannelIndex];
                fd = m_fds[nChannelIndex];
            }
        }

        /// <summary>
        /// 获取各通道的调整系数
        /// </summary>
        /// <param name="nChannelIndex">采集通道索引</param>
        /// <param name="fX">X轴调整系数</param>
        /// <param name="fY">Y轴调整系数</param>
        /// <param name="fZ">Z轴调整系数</param>
        public void GetAxisCoefficient(int nChannelIndex, ref float fX, ref float fY, ref float fZ)
        {
            if (nChannelIndex >= 0 && nChannelIndex < m_fAxis.Length)
            {
                fX = m_fAxis[nChannelIndex, 0];
                fY = m_fAxis[nChannelIndex, 1];
                fZ = m_fAxis[nChannelIndex, 2];
            }
        }

        /// <summary>
        /// 获取传感器校正系数
        /// </summary>
        /// <param name="nSensorIndex">传感器索引</param>
        /// <param name="sensorParam"></param>
        public void GetSensorCalibrateParam(int nSensorIndex, out double[] sensorParam)
        {
            if (nSensorIndex >= 0 && nSensorIndex < m_nGroupCount)
            {
                sensorParam = m_fSensorCalibrateParam[nSensorIndex];
            }
            else
            {
                sensorParam = null;
            }
        }

        /// <summary>
        /// 获取设备通道数
        /// </summary>
        /// <returns>设备通道数</returns>
        public int GetChannelCount()
        {
            return m_nGroupCount * m_nChannelCountPerGroup;
        }

        /// <summary>
        /// 获取使用的设备通道数
        /// </summary>
        /// <returns>使用的设备通道数</returns>
        public int GetUsedChannelCount()
        {
            return m_nUsedChannelCount;
        }

        /// <summary>
        /// 设置使用的设备通道数
        /// </summary>
        /// <param name="nUsedChannelCount">nUsedChannelCount</param>
        public void SetUsedChannelCount(int nUsedChannelCount)
        {
            if (m_nUsedChannelCount != nUsedChannelCount)
            {
                m_nUsedChannelCount = nUsedChannelCount;

                if (m_ConfigRWInstance != null)
                {
                    m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_CHANNEL, CONFIG_KEY_USED_CHANNEL_COUNT, m_nUsedChannelCount.ToString());
                }
            }
        }

        /// <summary>
        /// 获取AC/DC模式
        /// </summary>
        /// <returns></returns>
        public ADCMode GetADCMode()
        {
            return m_eADCMode;
        }

        /// <summary>
        ///  设置AC/DC模式
        /// </summary>
        /// <param name="eMode"></param>
        public void SetADCMode(ADCMode eMode)
        {
            if (m_eADCMode != eMode)
            {
                m_eADCMode = eMode;

                if (m_ConfigRWInstance != null)
                {
                    string strTemp = ((int)m_eADCMode).ToString();
                    m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_SENSOR, CONFIG_KEY_ADC, strTemp);
                }
            }
        }

        /// <summary>
        /// 获取Chan-Mode
        /// </summary>
        /// <returns></returns>
        public ChanMode GetChanMode()
        {
            return m_eChanMode;
        }

        /// <summary>
        /// 设置Chan-Mode
        /// </summary>
        /// <param name="eMode"></param>
        public void SetChanMode(ChanMode eMode)
        {
            if (m_eChanMode != eMode)
            {
                m_eChanMode = eMode;

                if (m_ConfigRWInstance != null)
                {
                    string strTemp = ((int)m_eChanMode).ToString();
                    m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_SENSOR, CONFIG_KEY_CHANMODE, strTemp);
                }
            }
        }

        /// <summary>
        /// 获取通道数据索引
        /// </summary>
        /// <param name="nChannelDataIndexs"></param>
        public void GetChannelDataIndex(out int[] nChannelDataIndexs)
        {
            nChannelDataIndexs = m_nChannelDataIndex;
        }

        /// <summary>
        /// 获取每台设备有效的数据通道个数
        /// </summary>
        /// <returns></returns>
        public int GetValidChannelCountPerDevice()
        {
            return m_nValidChannelCount;
        }

        /// <summary>
        /// 获取曲线通道索引的Dict
        /// </summary>
        /// <param name="nCurveChannelIndexs"></param>
        public void GetCurveChannelIndex(out int[] nCurveChannelIndexs)
        {
            nCurveChannelIndexs = m_nCurveChannelIndexs;
        }

        /// <summary>
        ///  设置曲线通道索引的Dict
        /// </summary>
        /// <param name="nCurveChannelIndexs"></param>
        public void SetCurveChannelIndex(int[] nCurveChannelIndexs)
        {
            for (int i = 0; i < nCurveChannelIndexs.Length; ++i)
            {
                m_nCurveChannelIndexs[i] = nCurveChannelIndexs[i];

                string strKey = CONFIG_KEY_CURVE + i.ToString();
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_CURVECHANNELINDEX, strKey, m_nCurveChannelIndexs[i].ToString());
            }
        }

        /// <summary>
        /// 返回要显示的曲线通道数
        /// </summary>
        /// <returns>要显示的曲线通道数</returns>
        public int GetShowCurveChannelCount()
        {
            return m_nShowCurveChannelCount;
        }

        /// <summary>
        /// 设置要显示的曲线通道数
        /// </summary>
        /// <param name="nShowCurveChannelCount">要显示的曲线通道数</param>
        public void SetShowCurveChannelCount(int nShowCurveChannelCount)
        {
            if (m_nShowCurveChannelCount != nShowCurveChannelCount)
            {
                m_nShowCurveChannelCount = nShowCurveChannelCount;

                if (m_ConfigRWInstance != null)
                {
                    m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_CURVECHANNELINDEX, CONFIG_KEY_SHOWCURVECHANNELCOUNT, m_nShowCurveChannelCount.ToString());
                }
            }
        }

        /// <summary>
        /// 获取是否需要保存数据
        /// </summary>
        /// <returns>是否需要保存数据</returns>
        public bool GetIsNeedSaveData()
        {
            return m_bNeedSaveData;
        }

        /// <summary>
        /// 设置是否需要保存数据
        /// </summary>
        /// <param name="bIsNeed">是否需要保存数据</param>
        public void SetIsNeedSaveData(bool bIsNeed)
        {
            if (m_bNeedSaveData != bIsNeed)
            {
                m_bNeedSaveData = bIsNeed;

                if (m_ConfigRWInstance != null)
                {
                    int nTemp = Convert.ToInt32(m_bNeedSaveData);
                    m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_DATAFILE, CONFIG_KEY_NEEDSAVEDATA, nTemp.ToString());
                }
            }
        }

        /// <summary>
        /// 获取数据保存路径
        /// </summary>
        /// <returns>数据保存路径</returns>
        public string GetDataSavePath()
        {
            return m_strDataSavePath;
        }

        /// <summary>
        /// 设置数据保存路径
        /// </summary>
        /// <param name="strDataSavePath">数据保存路径</param>
        public void SetDataSavePath(string strDataSavePath)
        {
            if (m_strDataSavePath != strDataSavePath)
            {
                m_strDataSavePath = strDataSavePath;

                if (m_ConfigRWInstance != null)
                {
                    m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_DATAFILE, CONFIG_KEY_SAVEPATH, m_strDataSavePath);
                }
            }
        }

        /// <summary>
        /// 获取传感器组数
        /// </summary>
        /// <returns>传感器组数</returns>
        public int GetGroupCount()
        {
            return m_nGroupCount;
        }

        /// <summary>
        /// 获取设备数量
        /// </summary>
        /// <returns>设备数量</returns>
        public int GetDeviceCount()
        {
            return m_nDeviceCount;
        }

        /// <summary>
        /// 根据设备VID和PID生成设备ID
        /// </summary>
        /// <param name="nVID">VID</param>
        /// <param name="nPID">PID</param>
        /// <returns>设备ID</returns>
        public uint GenDeviceID(ushort nVID, ushort nPID)
        {
            uint nDeviceID = ((uint)nVID << 16) + (uint)nPID;
            return nDeviceID;
        }

        /// <summary>
        /// 根据设备ID获取设备索引
        /// </summary>
        /// <param name="nDeviceID">设备ID</param>
        /// <returns>设备索引</returns>
        public int GetDeviceIndex(uint nDeviceID)
        {
            if (m_DictDeviceID2Index.ContainsKey(nDeviceID))
            {
                return m_DictDeviceID2Index[nDeviceID];
            }

            return -1;
        }

        /// <summary>
        /// 获取设备ID与索引的词典
        /// </summary>
        /// <returns>设备ID与索引的词典</returns>
        public Dictionary<uint, int> GetDeviceID2IndexDict()
        {
            return m_DictDeviceID2Index;
        }

        /// <summary>
        /// 根据设备索引获取设备VID和PID
        /// </summary>
        /// <param name="nDeviceIndex">设备索引，索引从0开始</param>
        /// <param name="nVID">设备VID</param>
        /// <param name="nPID">设备PID</param>
        public void GetDeviceVIDAndPIDByIndex(int nDeviceIndex, ref ushort nVID, ref ushort nPID)
        {
            string strKey = CONFIG_KEY_VID + nDeviceIndex.ToString();
            string strVID = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_DEVICE, strKey);
            nVID = Convert.ToUInt16(strVID, 16);

            strKey = CONFIG_KEY_PID + nDeviceIndex.ToString();
            string strPID = m_ConfigRWInstance.IniReadValue(CONFIG_SECTION_DEVICE, strKey);
            nPID = Convert.ToUInt16(strPID, 16);
        }

        /// <summary>
        /// 将采样频率字符串转换为枚举
        /// </summary>
        /// <param name="strSamplingFreq">采样频率字符串</param>
        /// <returns>采样频率枚举值</returns>
        public SamplingFreq ConvertSamplingFreqStr2Enum(string strSamplingFreq)
        {
            if (strSamplingFreq == "1")
            {
                return SamplingFreq.FREQ_1;
            }
            else if (strSamplingFreq == "10")
            {
                return SamplingFreq.FREQ_10;
            }
            if (strSamplingFreq == "250")
            {
                return SamplingFreq.FREQ_250;
            }
            else if (strSamplingFreq == "500")
            {
                return SamplingFreq.FREQ_500;
            }
            else if (strSamplingFreq == "1000")
            {
                return SamplingFreq.FREQ_1000;
            }
            else if (strSamplingFreq == "4000")
            {
                return SamplingFreq.FREQ_4000;
            }

            return SamplingFreq.FREQ_250;
        }

        /// <summary>
        /// 将采样频率的枚举值转换为字符串
        /// </summary>
        /// <param name="eSamplingFreq">采样频率枚举值</param>
        /// <returns>采样频率字符串</returns>
        public string ConvertSamplingFreqEnum2Str(SamplingFreq eSamplingFreq)
        {
            switch (eSamplingFreq)
            {
                case SamplingFreq.FREQ_1:
                    {
                        return "1";
                    }
                case SamplingFreq.FREQ_10:
                    {
                        return "10";
                    }
                case SamplingFreq.FREQ_250:
                    {
                        return "250";
                    }
                case SamplingFreq.FREQ_500:
                    {
                        return "500";
                    }
                case SamplingFreq.FREQ_1000:
                    {
                        return "1000";
                    }
                case SamplingFreq.FREQ_4000:
                    {
                        return "4000";
                    }
                default:
                    {
                        return "100";
                    }
            }
        }

        /// <summary>
        /// 将采样率的枚举值转换为double
        /// </summary>
        /// <param name="eSamplingFreq">采样频率枚举值</param>
        /// <returns>采样率的double值</returns>
        public double ConvertSamplingFreqEnum2Double(SamplingFreq eSamplingFreq)
        {
            switch (eSamplingFreq)
            {
                case SamplingFreq.FREQ_1:
                    {
                        return 1;
                    }
                case SamplingFreq.FREQ_10:
                    {
                        return 10;
                    }
                case SamplingFreq.FREQ_250:
                    {
                        return 250;
                    }
                case SamplingFreq.FREQ_500:
                    {
                        return 500;
                    }
                case SamplingFreq.FREQ_1000:
                    {
                        return 1000;
                    }
                case SamplingFreq.FREQ_4000:
                    {
                        return 4000;
                    }
                default:
                    {
                        return 100;
                    }
            }

            return 100;
        }

        /// <summary>
        /// 获取采样模式
        /// </summary>
        /// <returns>采样模式</returns>
        public SamplingMode GetSamplingMode()
        {
            return m_eSamplingMode;
        }

        /// <summary>
        /// 设置采样模式
        /// </summary>
        /// <param name="eSamplingMode">采样模式的枚举</param>
        public void SetSamplingMode(SamplingMode eSamplingMode)
        {
            if (m_eSamplingMode != eSamplingMode)
            {
                m_eSamplingMode = eSamplingMode;

                if (m_ConfigRWInstance != null)
                {
                    int nTemp = (int)m_eSamplingMode;
                    m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_SAMPLING_PARAM, CONFIG_KEY_SAMPLING_MODE, nTemp.ToString());
                }
            }
        }

        /// <summary>
        /// 获取采样频率
        /// </summary>
        /// <returns>采样频率的枚举</returns>
        public SamplingFreq GetSamplingFreq()
        {
            return m_eSamplingFreq;
        }

        /// <summary>
        /// 设置采样频率
        /// </summary>
        /// <param name="eSamplingFreq">采样频率的枚举</param>
        public void SetSamplingFreq(SamplingFreq eSamplingFreq)
        {
            if (m_eSamplingFreq != eSamplingFreq)
            {
                m_eSamplingFreq = eSamplingFreq;

                if (m_ConfigRWInstance != null)
                {
                    string strTemp = ConvertSamplingFreqEnum2Str(m_eSamplingFreq);
                    m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_SAMPLING_PARAM, CONFIG_KEY_SAMPLING_FREQ, strTemp);
                }
            }
        }

        /// <summary>
        /// 获取采样持续时间
        /// </summary>
        /// <returns></returns>
        public float GetSamplingDuration()
        {
            return m_fSamplingDuration;
        }

        /// <summary>
        /// 设置采样持续时间，单位：s
        /// </summary>
        /// <param name="fSamplingDuration">采样持续时间</param>
        public void SetSamplingDuration(float fSamplingDuration)
        {
            if (m_fSamplingDuration != fSamplingDuration)
            {
                m_fSamplingDuration = fSamplingDuration;

                if (m_ConfigRWInstance != null)
                {
                    string strTemp = m_fSamplingDuration.ToString("f6");
                    m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_SAMPLING_PARAM, CONFIG_KEY_SAMPLING_DURATION, strTemp);
                }
            }
        }

        /// <summary>
        /// 获取Y轴单位
        /// </summary>
        /// <returns>Y轴单位</returns>
        public int GetYAxisUnit()
        {
            return m_nYAxisUnit;
        }

        /// <summary>
        /// 设置Y轴单位
        /// </summary>
        /// <param name="nYAxisUnit">Y轴单位</param>
        public void SetYAxisUnit(int nYAxisUnit)
        {
            if (m_nYAxisUnit != nYAxisUnit)
            {
                m_nYAxisUnit = nYAxisUnit;

                if (m_ConfigRWInstance != null)
                {
                    m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_AXIS_SETTING, CONFIG_KEY_YAXIS_UNIT, m_nYAxisUnit.ToString());
                }
            }
        }

        /// <summary>
        /// 获取X轴单位
        /// </summary>
        /// <returns>X轴单位</returns>
        public int GetXAxisUnit()
        {
            return m_nXAxisUnit;
        }

        /// <summary>
        /// 设置X轴单位
        /// </summary>
        /// <param name="nXAxisUnit">X轴单位</param>
        public void SetXAxisUnit(int nXAxisUnit)
        {
            if (m_nXAxisUnit != nXAxisUnit)
            {
                m_nXAxisUnit = nXAxisUnit;

                if (m_ConfigRWInstance != null)
                {
                    m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_AXIS_SETTING, CONFIG_KEY_XAXIS_UNIT, m_nXAxisUnit.ToString());
                }
            }
        }

        /// <summary>
        /// 获取需要跳过的无效数据点数
        /// </summary>
        /// <returns>需要跳过的无效数据点数</returns>
        public int GetSkipPointCount()
        {
            return m_nSkipPointCount;
        }

        /// <summary>
        /// 获取Z轴校正系数Kxz
        /// </summary>
        /// <returns></returns>
        public double GetKxz()
        {
            return m_dKxz;
        }

        /// <summary>
        /// 获取Z轴校正系数Kyz
        /// </summary>
        /// <returns></returns>
        public double GetKyz()
        {
            return m_dKyz;
        }

        /// <summary>
        /// 获取Z轴校正系数Kyz
        /// </summary>
        /// <returns></returns>
        public double GetKzz()
        {
            return m_dKzz;
        }

        /// <summary>
        /// 低通滤波是否使能
        /// </summary>
        /// <returns></returns>
        public bool IsLowPassFilterEnabled()
        {
            return m_bLowPassFilterEnabled;
        }

        /// <summary>
        /// 设置低通滤波是否使能
        /// </summary>
        /// <param name="bEnabled">是否使能</param>
        public void SetIsLowPassFilterEnabled(bool bEnabled)
        {
            if (m_bLowPassFilterEnabled != bEnabled)
            {
                m_bLowPassFilterEnabled = bEnabled;

                if (m_ConfigRWInstance != null)
                {
                    int nTemp = bEnabled ? 1 : 0;
                    m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_LOWPASS_FILTER_COEFF, CONFIG_KEY_LOWPASS_FILTER_ENABLE, nTemp.ToString());
                }
            }
        }

        /// <summary>
        /// 获取低通滤波器参数
        /// </summary>
        /// <param name="dPassFreq">通带频率</param>
        /// <param name="dStopFreq">阻带频率</param>
        /// <param name="dPassDB">通带衰减</param>
        /// <param name="dStopDB">阻带衰减</param>
        public void GetLowPassFilterParameters(ref float dPassFreq, ref float dStopFreq, ref float dPassDB, ref float dStopDB)
        {
            dPassFreq = m_dLowPassFilterPassFreq;
            dStopFreq = m_dLowPassFilterStopFreq;
            dPassDB = m_dLowPassFilterPassDB;
            dStopDB = m_dLowPassFilterStopDB;
        }

        /// <summary>
        /// 设置低通滤波器参数
        /// </summary>
        /// <param name="dPassFreq">通带频率</param>
        /// <param name="dStopFreq">阻带频率</param>
        /// <param name="dPassDB">通带衰减</param>
        /// <param name="dStopDB">阻带衰减</param>
        public void SetLowPassFilterParameters(float dPassFreq, float dStopFreq, float dPassDB, float dStopDB)
        {
            m_dLowPassFilterPassFreq = dPassFreq;
            m_dLowPassFilterStopFreq = dStopFreq;
            m_dLowPassFilterPassDB = dPassDB;
            m_dLowPassFilterStopDB = dStopDB;

            if (m_ConfigRWInstance != null)
            {
                string strTemp = m_dLowPassFilterPassFreq.ToString("f6");
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_LOWPASS_FILTER_COEFF, CONFIG_KEY_LOWPASS_FILTER_PASSFREQ, strTemp);
                strTemp = m_dLowPassFilterStopFreq.ToString("f6");
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_LOWPASS_FILTER_COEFF, CONFIG_KEY_LOWPASS_FILTER_STOPFREQ, strTemp);
                strTemp = m_dLowPassFilterPassDB.ToString("f6");
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_LOWPASS_FILTER_COEFF, CONFIG_KEY_LOWPASS_FILTER_PASSDB, strTemp);
                strTemp = m_dLowPassFilterStopDB.ToString("f6");
                m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_LOWPASS_FILTER_COEFF, CONFIG_KEY_LOWPASS_FILTER_STOPDB, strTemp);
            }
        }

        public FilterByDiffFun CreateLowPassFilterByDiffFun()
        {
            double[] dB = null;
            double[] dA = null;
            int nOrder = 0;
            double dFs = ConvertSamplingFreqEnum2Double(m_eSamplingFreq);
            if (m_eSamplingFreq == SamplingFreq.FREQ_1 || m_eSamplingFreq == SamplingFreq.FREQ_10)
            {// 这里特殊处理一下，设备不支持1Hz和10Hz采样，实际上是100Hz采样，然后软件再对采集的数据进行减采样
                dFs = 100;
            }

            Algorithm.DesignButterLPFilter(m_dLowPassFilterPassFreq, m_dLowPassFilterStopFreq, m_dLowPassFilterPassDB,
                m_dLowPassFilterStopDB, dFs, ref dB, ref dA, ref nOrder);

            FilterByDiffFun pFilter = new FilterByDiffFun();
            pFilter.SetParameter(dB, dA, nOrder);

            return pFilter;
        }

        /// <summary>
        /// 获取低通滤波器系数
        /// </summary>
        /// <returns></returns>
        public double[] GetLowPassFilterCoeffs()
        {
            return m_dLowPassFilterCoeffs;
        }

        /// <summary>
        /// 获取卡尔曼滤波器参数R
        /// </summary>
        /// <param name="nFilterIndex">滤波器索引</param>
        /// <returns>卡尔曼滤波器参数R</returns>
        public double GetKalmanFilterParamR(int nFilterIndex)
        {
            if (nFilterIndex >= 0 && nFilterIndex < m_nGroupCount * m_nChannelCountPerGroup)
            {
                return m_dKalmanFilter_R[nFilterIndex];
            }

            return 1000.0;
        }

        /// <summary>
        /// 获取授时类型
        /// </summary>
        /// <returns></returns>
        public TimingType GetTimingType()
        {
            return m_eTimingType;
        }

        /// <summary>
        /// 设置授时类型
        /// </summary>
        /// <param name="nTimingType">授时类型</param>
        public void SetTimingType(TimingType eTimingType)
        {
            if (m_eTimingType != eTimingType)
            {
                m_eTimingType = eTimingType;

                if (m_ConfigRWInstance != null)
                {
                    m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_TIMING_TYPE, CONFIG_KEY_TIMING_TYPE, ((int)m_eTimingType).ToString());
                }
            }
        }

        /// <summary>
        /// 获取Modbus TCP服务器IP地址
        /// </summary>
        /// <returns>Modbus TCP服务器IP地址</returns>
        public string GetModbusTCPServerIP()
        {
            return m_strModbusTCPServerIP;
        }

        /// <summary>
        /// 获取Modbus TCP服务器端口号
        /// </summary>
        /// <returns>Modbus TCP服务器端口号</returns>
        public int GetModbusTCPServerPort()
        {
            return m_nModbusTCPServerPort;
        }

        /// <summary>
        /// 获取Modbus轮询定时周期
        /// </summary>
        public int GetModbusTimingCycle()
        {
            return m_nModbusTimingCycle;
        }

        /// <summary>
        /// 获取调试使能状态
        /// </summary>
        /// <returns></returns>
        public bool GetModbusDebugEnable()
        {
            return m_bModbusDebugEnable;
        }

        /// <summary>
        /// 获取Modbus日志文件路径
        /// </summary>
        /// <returns></returns>
        public string GetModbusLogFilePath()
        {
            return m_strModbusLogFilePath;
        }

        /// <summary>
        /// 获取Modbus重试次数
        /// </summary>
        /// <returns></returns>
        public int GetModbusNumberOfRetries()
        {
            return m_nModbusNumberOfRetries;
        }

        /// <summary>
        /// 获取Modbus通信超时
        /// </summary>
        /// <returns></returns>
        public int GetModbusTimeout()
        {
            return m_nModbusTimeout;
        }

        /// <summary>
        /// 获取Modbus重连间隔时间，单位：ms
        /// </summary>
        /// <returns></returns>
        public int GetModbusReconnectInterval()
        {
            return m_nModbusReconnectInterval;
        }

        /// <summary>
        /// 获取磁场正常值与零偏的范围
        /// </summary>
        /// <returns></returns>
        public double[] GetMagValueRanges()
        {
            return m_dMagValueRanges;
        }

        /// <summary>
        /// 设置指定通道的磁场正常值与零偏的范围
        /// </summary>
        /// <param name="nChannelIndex"></param>
        /// <param name="dMagValueRange"></param>
        public void SetMagValueRange(int nChannelIndex, double dMagValueRange)
        {
            if (nChannelIndex >= 0 && nChannelIndex < 90)
            {
                m_dMagValueRanges[nChannelIndex] = dMagValueRange;

                if (m_ConfigRWInstance != null)
                {                    
                    string strKey = CONFIG_KEY_MAG_VALUE_RANGE + nChannelIndex.ToString();
                    string strValue = m_dMagValueRanges[nChannelIndex].ToString();
                    m_ConfigRWInstance.IniWriteValue(CONFIG_SECTION_MAG_VALID_VALUE_RANGE, strKey, strValue);
                }
            }
        }

        /// <summary>
        /// 获取界面显示异常数据最大条数
        /// </summary>
        /// <returns></returns>
        public int GetAbnormalDataMaxShowCount()
        {
            return m_nAbnormalDataMaxShowCount;
        }

        const string CONFIG_SECTION_DEVICE = "Device";
        const string CONFIG_KEY_DEVICE_COUNT = "DeviceCount";   // 设备数
        const string CONFIG_DEFAULT_VALUE_DEVICE_COUNT = "3";
        const string CONFIG_KEY_VID = "VID";
        const string CONFIG_DEFAULT_VALUE_VID = "0x04B4";
        const string CONFIG_KEY_PID = "PID";
        const string CONFIG_DEFAULT_VALUE_PID = "0x0";

        const string CONFIG_SECTION_CHANNEL = "Channel";
        const string CONFIG_KEY_USED_CHANNEL_COUNT = "UsedChannelCount";    // 使用的通道数量
        const string CONFIG_DEFAULT_VALUE_USED_CHANNEL_COUNT = "90";

        const string CONFIG_SECTION_SENSOR = "Sensor";
        const string CONFIG_KEY_SENSOR_S = "S";   // 传感器比例系数Si
        const string CONFIG_DEFAULT_VALUE_SENSOR_S = "1.0";
        const string CONFIG_KEY_SENSOR_K = "K";  // 传感器比例系数修正系数K
        const string CONFIG_DEFAULT_VALUE_SENSOR_K = "1.0";
        const string CONFIG_KEY_SENSOR_B = "b";   // 传感器零点bi
        const string CONFIG_DEFAULT_VALUE_SENSOR_B = "0.0";
        const string CONFIG_KEY_COLLECT_D = "D";  // 采集通道比例系数
        const string CONFIG_DEFAULT_VALUE_COLLECT_D = "1.0";
        const string CONFIG_KEY_COLLECT_F = "f"; // 采集通道零点
        const string CONFIG_DEFAULT_VALUE_COLLECT_F = "0.0";

        const string CONFIG_KEY_ADC = "ADC";
        const string CONFIG_DEFAULT_VALUE_ADC = "2";
        const string CONFIG_KEY_CHANMODE = "ChanMode";
        const string ONFIG_DEFAULT_VALUE_CHANMODE = "1";

        const string CONFIG_SECTION_SAMPLING_PARAM = "SamplingParam";
        const string CONFIG_KEY_SAMPLING_MODE = "SamplingMode";
        const string CONFIG_DEFAULT_VALUE_SAMPLING_MODE = "2";  // 连续采集
        const string CONFIG_KEY_SAMPLING_FREQ = "SamplingFreq";
        const string CONFIG_DEFAULT_VALUE_SAMPLING_FREQ = "100";    // 采样频率
        const string CONFIG_KEY_SAMPLING_DURATION = "SamplingDuration";
        const string CONFIG_DEFAULT_VALUE_SAMPLING_DURATION = "0.0";    // 采样持续时间

        const string CONFIG_SECTION_CHANNELDATAINDEX = "ChannelDataIndex"; // 通道索引与数据索引的对应关系
        const string CONFIG_KEY_CHANNEL_INDEX = "Channel";
        const string CONFIG_DEFAULT_VALUE_CHANNEL_INDEX = "-1";

        const string CONFIG_SECTION_CURVECHANNELINDEX = "CurveChannelIndex";
        const string CONFIG_KEY_SHOWCURVECHANNELCOUNT = "ShowCurveChannelCount";
        const string CONFIG_DEFAULT_VALUE_SHOWCURVECHANNELCOUNT = "6";
        const string CONFIG_KEY_CURVE = "Curve";
        const string CONFIG_DEFAULT_VALUE_CURVE = "-1";

        const string CONFIG_SECTION_PLC = "PLC";
        const string CONFIG_KEY_PLC_SERVER_IP = "ServerIP";
        const string CONFIG_DEFAULT_VALUE_PLC_SERVER_IP = "192.168.2.1";
        const string CONFIG_KEY_PLC_SERVER_PORT = "ServerPort";
        const string CONFIG_DEFAULT_VALUE_PLC_SERVER_PORT = "502";
        const string CONFIG_KEY_PLC_TIMING_CYCLE = "TimingCycle";
        const string CONFIG_DEFAULT_VALUE_PLC_TIMING_CYCLE = "200";
        // 调试使能
        const string CONFIG_KEY_PLC_DEBUG_ENABLE = "DebugEnable";
        const string CONFIG_DEFAULT_VALUE_PLC_DEBUG_ENABLE = "0";
        // 日志文件路径
        const string CONFIG_KEY_PLC_LOG_FILE_PATH = "LogFilePath";
        const string CONFIG_DEFAULT_VALUE_PLC_LOG_FILE_PATH = "D:\\ModbusLogInfo.txt";
        // 重试次数
        const string CONFIG_KEY_PLC_NUMBER_OF_RETRIES = "NumberOfRetries";
        const string CONFIG_DEFAULT_VALUE_NUMBER_OF_RETRIES = "3";
        // 通信超时
        const string CONFIG_KEY_PLC_TIMEOUT = "Timeout";
        const string CONFIG_DEFAULT_VALUE_PLC_TIMEOUT = "500";
        // 重连间隔时间
        const string CONFIG_KEY_PLC_RECONNECT_INTERVAL = "ReconnectInterval";
        const string CONFIG_DEFAULT_VALUE_PLC_RECONNECT_INTERVAL = "1000";

        const string CONFIG_SECTION_MAG_VALID_VALUE_RANGE = "MagValidValueRange";
        const string CONFIG_KEY_MAG_VALUE_RANGE = "ValueRange";
        const string CONFIG_DEFAULT_VALUE_MAG_VALUE_RANGE = "2.5";

        const string CONFIG_SECTION_ABNORMAL_DATA = "AbnormalData";
        const string CONFIG_KEY_ABNORMAL_DATA_MAX_SHOW_COUNT = "AbnormalDataMaxShowCount";
        const string CONFIG_DEFAULT_VALUE_ABNORMAL_DATA_MAX_SHOW_COUNT = "1000";

        const string CONFIG_SECTION_DATAFILE = "DataFile";
        const string CONFIG_KEY_SAVEPATH = "SavePath";
        const string CONFIG_DEFAULT_VALUE_SAVEPATH = "D:\\";
        const string CONFIG_KEY_NEEDSAVEDATA = "NeedSaveData";
        const string CONFIG_DEFAULT_VALUE_NEEDSAVEDATA = "1";
        const string CONFIG_KEY_SKIPPOINTCOUNT = "SkipPointCount";
        const string CONFIG_DEFAULT_VALUE_SKIPPOINTCOUNT = "2";

        const string CONFIG_SECTION_AXIS_SETTING = "CurveAxisSetting";
        const string CONFIG_KEY_XAXIS_UNIT = "XAxisUnit";
        const string CONFIG_DEFAULT_VALUE_XAXIS_UNIT = "1";
        const string CONFIG_KEY_YAXIS_UNIT = "YAxisUnit";
        const string CONFIG_DEFAULT_VALUE_YAXIS_UNIT = "1";

        const string CONFIG_SECTION_COEFFICIENT = "Coefficient";
        const string CONFIG_KEY_AXIS_K = "K";
        const string CONFIG_DEFAULT_VALUE_AXIS_KX = "1,0,0";
        const string CONFIG_DEFAULT_VALUE_AXIS_KY = "0,1,0";
        const string CONFIG_DEFAULT_VALUE_AXIS_KZ = "0,0,1";

        const string CONFIG_SECTION_LOWPASS_FILTER_COEFF = "LowPassFilterCoeff";
        const string CONFIG_KEY_LOWPASS_FILTER_ENABLE = "Enabled";
        const string CONFIG_DEFAULT_VALUE_LOWPASS_FILTER_ENABLE = "0";
        const string CONFIG_KEY_LOWPASS_FILTER_COEFF = "Coeff";
        const string CONFIG_DEFAULT_VALUE_LOWPASS_FILTER_COEFF = "0.5,0.5";
        const string CONFIG_KEY_LOWPASS_FILTER_PASSFREQ = "PassFreq";
        const string CONFIG_DEFAULT_VALUE_LOWPASS_FILTER_PASSFREQ = "50";
        const string CONFIG_KEY_LOWPASS_FILTER_STOPFREQ = "StopFreq";
        const string CONFIG_DEFAULT_VALUE_LOWPASS_FILTER_STOPFREQ = "100";
        const string CONFIG_KEY_LOWPASS_FILTER_PASSDB = "PassDB";
        const string CONFIG_DEFAULT_VALUE_LOWPASS_FILTER_PASSDB = "1";
        const string CONFIG_KEY_LOWPASS_FILTER_STOPDB = "StopDB";
        const string CONFIG_DEFAULT_VALUE_LOWPASS_FILTER_STOPDB = "60";

        const string CONFIG_SECTION_TIMING_TYPE = "TimingType";
        const string CONFIG_KEY_TIMING_TYPE = "TimingType";
        const string CONFIG_DEFAULT_VALUE_TIMING_TYPE = "0";

        //=====================================================================
        // 校准配置
        const string CONFIG_SECTION_SENSOR_CALIBRATE_PARAM = "SensorCalibrateParam";
        // 传感器%d校准数据
        const string CONFIG_KEY_SENSOR_CALIBRATE_PARAM = "SensorCalibrateParam";
        // 传感器%d校准数据的默认值
        const string CONFIG_DEFAULT_VALUE_SENSOR_CALIBRATE_PARAM = "0,0,0,1,1,1,0,0,0";

        //=====================================================================
        // 卡尔曼滤波器参数
        const string CONFIG_SECTION_KALMAN_FILTER_PARAM = "KalmanFilterParam";
        // 卡尔曼滤波器%d的参数R
        const string CONFIG_KEY_KALMAN_FILTER_PARAM_R = "KalmanFilterParam_R";
        // 卡尔曼滤波器%d的参数R的默认值
        const string CONFIG_DEFAULT_VALUE_KALMAN_FILTER_PARAM_R = "1000";

    }
}
