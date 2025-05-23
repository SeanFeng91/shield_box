using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using MCMag30FDevice;
using PLC;

namespace MCMAG30F
{
    public partial class MCMAG30FForm : Form
    {
        private ShowDataControl m_ShowDataControl;          // 显示数据界面
        private ShowCurveControl m_ShowCurveControl;        // 显示曲线界面
        private ShowFFTControl m_ShowFFTControl;            // 显示FFT界面
        private int m_nDisplayMode = 1;                     // 显示模式
        private Thread m_ThreadParseDataFile = null;        // 解析数据文件的线程
        private int m_nReadDataFilePercentage = 0;          // 读取文件的进度
        private bool m_bAbortReadDataFile = false;          // 是否中断读取数据文件
        private int m_nStatus = 0;                          // 软件当前的状态，0--初始状态，没有解析数据文件，也没有采集设备数据
                                                            // 1--正在解析数据文件，2--正在采集数据

        private int m_nInit = 0;                            // 初始化标志

        private ProgressForm m_progressForm = null;         // 进度对话框

        private Thread m_hConvertDataThread = null;         // 将二进制数据转换为dat文本文件的线程

        private MultiChanMagCollectDeviceManager m_pDeviceManager = MultiChanMagCollectDeviceManager.GetInstance();

        private Dictionary<int, List<bool>> m_bDictShowCurveFlags = new Dictionary<int, List<bool>>();

        private S7_200 m_pS7_200 = null;       // 西门子S7 200 PLC

        public MCMAG30FForm()
        {
            // 读取配置文件中的参数
            MainConfig.GetInstance().ReadConfig();

            InitializeComponent();

            m_ShowDataControl = new ShowDataControl();
            m_ShowCurveControl = new ShowCurveControl();
            m_ShowFFTControl = new ShowFFTControl();

            double[] dMagValueRanges = MainConfig.GetInstance().GetMagValueRanges();
            m_ShowDataControl.SetMagValueRanges(dMagValueRanges);

            ((System.Windows.Forms.Control)this.labelLegend).ForeColor = System.Drawing.Color.Empty;
            // 更新显示界面

            // 注册设备采集频率变化事件处理函数
            m_pDeviceManager.DeviceSamplingFreqEvent += DataFileWriter.GetInstance().OnDeviceSamplingFreqChanged;
            // 根据配置文件初始化文件写入配置
            DataFileWriter.GetInstance().SyncParameterBySamplingFreq(MainConfig.GetInstance().GetSamplingFreq());

            m_pS7_200 = new S7_200();       // 西门子S7 200 PLC
            m_pS7_200.SetDebugEnable(MainConfig.GetInstance().GetModbusDebugEnable());
            m_pS7_200.SetLogFilePath(MainConfig.GetInstance().GetModbusLogFilePath());
            m_pS7_200.SetNumberOfRetries(MainConfig.GetInstance().GetModbusNumberOfRetries());
            m_pS7_200.SetTimeout(MainConfig.GetInstance().GetModbusTimeout());

            m_ShowDataControl.SetPLCStatusString(m_pS7_200.GetStatusString());
            m_pS7_200.PLCStatusChangedEvent += m_ShowDataControl.OnPLCStatusChangedEventHandler;

            //////////////////////////////////////////////////////////////////////////
            this.backgroundWorkerReadFile.WorkerReportsProgress = true;     // 设置能报告进度更新
            this.backgroundWorkerReadFile.WorkerSupportsCancellation = true;    // 设置支持异步取消
            this.backgroundWorkerReadFile.DoWork += new DoWorkEventHandler(BackgroundWorkerReadFile_DoWork);
            this.m_progressForm = new ProgressForm(this.backgroundWorkerReadFile);
            // 绑定进度条改变事件
            this.backgroundWorkerReadFile.ProgressChanged += new ProgressChangedEventHandler(this.m_progressForm.BackgroundWorker_ProgressChanged);
            // 绑定后台操作完成，取消，异常时事件
            this.backgroundWorkerReadFile.RunWorkerCompleted += this.m_progressForm.BackgroundWorker_RunWorkerCompleted;
            //////////////////////////////////////////////////////////////////////////

            for (int i = 0; i < 10; ++i)
            {
                List<bool> bListShowCurves = new List<bool>();
                bListShowCurves.Add(true);
                bListShowCurves.Add(true);
                bListShowCurves.Add(true);
                m_bDictShowCurveFlags.Add(i, bListShowCurves);
            }
            this.toolStripComboBoxCurveControlIndex.SelectedIndex = 0;

            Application.Idle += new EventHandler(OnApplicationIdle); // 应用程序空闲事件
        }

        /// <summary>
        /// 应用程序空闲事件响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationIdle(object sender, EventArgs e)
        {
            if (m_nStatus == 0)
            {// 初始状态
                this.toolStripButtonOpen.Enabled = true;        // 打开文件按钮
                this.toolStripButtonStart.Enabled = true;       // 开始采集按钮
                this.toolStripButtonStop.Enabled = false;       // 停止采集按钮
                this.toolStripButtonSetting.Enabled = true;     // 参数设置按钮
                this.toolStripButtonDisplay.Enabled = true;     // 显示设置按钮
            }
            else if (m_nStatus == 1)
            {// 正在解析数据文件
                this.toolStripButtonOpen.Enabled = false;       // 打开文件按钮
                this.toolStripButtonStart.Enabled = false;      // 开始采集按钮
                this.toolStripButtonStop.Enabled = false;       // 停止采集按钮
                this.toolStripButtonSetting.Enabled = false;    // 参数设置按钮
                this.toolStripButtonDisplay.Enabled = false;    // 显示设置按钮
            }
            else if (m_nStatus == 2)
            {// 正在采集数据
                this.toolStripButtonOpen.Enabled = false;       // 打开文件按钮
                this.toolStripButtonStart.Enabled = false;      // 开始采集按钮
                this.toolStripButtonStop.Enabled = true;        // 停止采集按钮
                this.toolStripButtonSetting.Enabled = false;    // 参数设置按钮
                this.toolStripButtonDisplay.Enabled = false;    // 显示设置按钮
            }

            if (m_nInit == 0)
            {
                m_nDisplayMode = 2;
                RefreshDispalyModeUI();
                this.m_ShowFFTControl.UpdateAxisText();
                m_nInit++;
            }
            else if (m_nInit == 1)
            {
                m_nDisplayMode = 0;
                RefreshDispalyModeUI();
                this.m_ShowCurveControl.UpdateAxisText();
                m_nInit++;
            }
            else if (m_nInit == 2)
            {
                this.m_ShowCurveControl.OnChartCountChanged(MainConfig.GetInstance().GetShowCurveChannelCount() / 3);
                this.m_ShowFFTControl.OnChartCountChanged(MainConfig.GetInstance().GetShowCurveChannelCount() / 3);

                m_nDisplayMode = 1;
                RefreshDispalyModeUI();

                m_nInit++;
            }
            else if (m_nInit == 3)
            {
                this.m_ShowCurveControl.UpdateAxisText();
                this.m_ShowDataControl.UpdateColumnHeadText();
                this.m_ShowFFTControl.UpdateAxisText();

                m_nInit++;
            }

        }

        /// <summary>
        /// 根据显示模式UI刷新界面
        /// </summary>
        private void RefreshDispalyModeUI()
        {
            switch (m_nDisplayMode)
            {
                case 0: // 显示曲线界面
                    {
                        this.centerPanel.Controls.Clear();
                        this.centerPanel.Controls.Add(m_ShowCurveControl);
                        this.m_ShowCurveControl.Dock = System.Windows.Forms.DockStyle.Fill;
                        this.m_ShowCurveControl.Show();
                        this.labelLegend.Visible = true;
                        m_bDictShowCurveFlags = this.m_ShowCurveControl.GetShowCurveFlag();
                        break;
                    }
                case 1: // 显示数据界面
                    {
                        this.centerPanel.Controls.Clear();
                        this.centerPanel.Controls.Add(m_ShowDataControl);
                        this.m_ShowDataControl.Dock = System.Windows.Forms.DockStyle.Fill;
                        this.m_ShowDataControl.Show();
                        this.labelLegend.Visible = false;
                        break;
                    }
                case 2: // 显示FFT界面
                    {
                        this.centerPanel.Controls.Clear();
                        this.centerPanel.Controls.Add(m_ShowFFTControl);
                        this.m_ShowFFTControl.Dock = System.Windows.Forms.DockStyle.Fill;
                        this.m_ShowFFTControl.Show();
                        this.labelLegend.Visible = true;
                        m_bDictShowCurveFlags = this.m_ShowFFTControl.GetShowCurveFlag();
                        break;
                    }
                default:
                    {
                        // 不可能发生
                        break;
                    }
            }
        }

        /// <summary>
        /// 打开数据文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonOpen_Click(object sender, EventArgs e)
        {
            GC.Collect();
            OpenFileDialog SelectFileDlg = new OpenFileDialog();
            SelectFileDlg.Multiselect = false;
            SelectFileDlg.Title = "请选择要处理的文件";
            SelectFileDlg.Filter = "数据文件(*.TSL,*.TSM,*.TSH,*.TSA,*.TSB)|*.TS*";
            if (SelectFileDlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            m_nStatus = 1;  // 设置状态--正在解析数据文件

            m_nReadDataFilePercentage = 0;      // 重置读取文件的进度为0
            m_bAbortReadDataFile = false;

            ///////////////////////////////////////////////////////////////////////////
            this.backgroundWorkerReadFile.RunWorkerAsync();     // 运行backgroundWorker组件
            ///////////////////////////////////////////////////////////////////////////

            if (m_nDisplayMode == 0)    // 显示波形
            {
                this.m_ShowCurveControl.InitData(); // 初始化曲线缓存
                this.m_ShowCurveControl.SetStep(1);
                this.m_ShowCurveControl.ClearCharts();  // 清空曲线
                this.m_ShowCurveControl.StartRefresh(); // 开始刷新数据
            }
            else if (m_nDisplayMode == 1)   // 显示数据
            {
                this.m_ShowDataControl.InitData();  // 初始化数据显示
                this.m_ShowDataControl.StartRefresh();  // 开始刷新数据
            }
            else if (m_nDisplayMode == 2)   // 显示FFT
            {
                this.m_ShowFFTControl.InitData();   // 初始化曲线缓存
                this.m_ShowFFTControl.SetSamplingFreq(ParseSamplingFreq(SelectFileDlg.FileName));
                this.m_ShowFFTControl.ClearCharts();
                this.m_ShowFFTControl.StartRefresh();   // 开始刷新数据
            }        
            
            // 启动分析数据文件的线程
            ParseDataFileThread(SelectFileDlg.FileName);

            ///////////////////////////////////////////////////////////////////////////
            this.m_progressForm.StartPosition = FormStartPosition.CenterScreen;
			this.m_progressForm.SetTitleText("正在读取数据文件...");
            this.m_progressForm.Init();
            this.m_progressForm.Show();
            ///////////////////////////////////////////////////////////////////////////
        }

        private void BackgroundWorkerReadFile_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            do
            {
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    m_bAbortReadDataFile = true;
                    break;
                }

                bw.ReportProgress(m_nReadDataFilePercentage);
                Thread.Sleep(200);
            } while (m_nReadDataFilePercentage < 100);
        }

        /// <summary>
        /// 解析数据文件的线程
        /// </summary>
        /// <param name="strDataFilePath">数据文件完整的路径</param>
        private void ParseDataFileThread(string strDataFilePath)
        {
            int nChannelCount = ParseChannelCount(strDataFilePath);
            if (nChannelCount <= 0)
            {
                MessageBox.Show("解析通道数错误");
            }

            m_ThreadParseDataFile = new Thread(new ParameterizedThreadStart(ParseDataFile));
            m_ThreadParseDataFile.IsBackground = true;
            m_ThreadParseDataFile.Start(strDataFilePath);
        }

        /// <summary>
        /// 解析通道数
        /// </summary>
        /// <param name="strDataFilePath">数据文件完整的路径</param>
        /// <returns></returns>
        private int ParseChannelCount(string strDataFilePath)
        {
            int nStartIndex = strDataFilePath.LastIndexOf("_") + 1;
            int nEndIndex = strDataFilePath.LastIndexOf(".");
            string strTemp = strDataFilePath.Substring(nStartIndex, nEndIndex - nStartIndex);
            int nChannelCount = 0;
            int.TryParse(strTemp, out nChannelCount);
            return nChannelCount;
        }

        /// <summary>
        /// 解析采集频率
        /// </summary>
        /// <param name="strDataFilePath">数据文件完整的路径</param>
        /// <returns></returns>
        private SamplingFreq ParseSamplingFreq(string strDataFilePath)
        {
            string strSuffix = strDataFilePath.Substring(strDataFilePath.Length - 3, 3);
            strSuffix = strSuffix.ToUpper();
            if (strSuffix == "TSL")
            {
                return SamplingFreq.FREQ_250;
            }
            else if (strSuffix == "TSM")
            {
                return SamplingFreq.FREQ_500;
            }
            else if (strSuffix == "TSH")
            {
                return SamplingFreq.FREQ_1000;
            }
            else if (strSuffix == "TSI")
            {
                return SamplingFreq.FREQ_4000;
            }
            else if (strSuffix == "TSA")
            {
                return SamplingFreq.FREQ_1;
            }
            else if (strSuffix == "TSB")
            {
                return SamplingFreq.FREQ_10;
            }

            return SamplingFreq.FREQ_250;
        }

        /// <summary>
        /// 解析数据文件
        /// </summary>
        /// <param name="strDataFilePath">数据文件完整的路径</param>
        private void ParseDataFile(object oDataFilePath)
        {
            string strDataFilePath = (string)oDataFilePath;
            int nChannelCount = ParseChannelCount(strDataFilePath);

            int nFreq = 250;
            if (strDataFilePath.EndsWith("L"))
            {
                nFreq = 250;
            }
            else if (strDataFilePath.EndsWith("M"))
            {
                nFreq = 500;
            }
            else if (strDataFilePath.EndsWith("H"))
            {
                nFreq = 1000;
            }
            else if (strDataFilePath.EndsWith("I"))
            {
                nFreq = 4000;
            }
            else if (strDataFilePath.EndsWith("A"))
            {
                nFreq = 1;
            }
            else if (strDataFilePath.EndsWith("B"))
            {
                nFreq = 10;
            }

            // 读取文件
            using (FileStream fsRead = new FileStream(strDataFilePath, FileMode.Open, FileAccess.Read))
            {
                Int64 nfsLen = (int)fsRead.Length;
                Int64 nTotalReadLen = 0;    // 累计读取长度
                int nAllChannelLength = (nChannelCount + 1) * sizeof(double);   // 一条包含所有通道数据的数据长度
                byte[] byAllChannelData = new byte[nAllChannelLength];
                int nReadLen = 0;   // 读取长度
                int nSleepCount = 0;
                bool bFirstDataFlag = true;
                do
                {
                    nReadLen = fsRead.Read(byAllChannelData, 0, nAllChannelLength);
                    nTotalReadLen += nReadLen;
                    m_nReadDataFilePercentage = (int)((double)nTotalReadLen * 100.0 / (double)nfsLen);  // 更新进度

                    int nStartIndex = 0;
                    MultiChannelMagData[] sData = new MultiChannelMagData[1];
                    sData[0] = new MultiChannelMagData();
                    sData[0].m_nTimeStamp = BitConverter.ToUInt64(byAllChannelData, nStartIndex);
                    nStartIndex += 8;
                    sData[0].m_dMagData = new double[nChannelCount];
                    for (int i = 0; i < nChannelCount; ++i)
                    {
                        sData[0].m_dMagData[i] = BitConverter.ToDouble(byAllChannelData, nStartIndex);
                        nStartIndex += 8;
                    }

                    if (bFirstDataFlag)
                    {
                        DateTime StartTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                        StartTime = StartTime.AddMilliseconds(sData[0].m_nTimeStamp / 1000.0);
                        this.m_ShowCurveControl.SetStartTime(StartTime);
                        bFirstDataFlag = false;
                    }

                    if (m_nDisplayMode == 0)    // 显示波形
                    {
                        this.m_ShowCurveControl.OnDataReceived(sData);
                    }
                    else if (m_nDisplayMode == 1)   // 显示数据
                    {
                        this.m_ShowDataControl.OnDataReceived(sData);
                    }
                    else if (m_nDisplayMode == 2)   // 显示FFT
                    {
                        this.m_ShowFFTControl.OnDataReceived(sData);
                    }

                    if (m_bAbortReadDataFile)
                    {
                        break;
                    }

                    nSleepCount++;
                    if (nFreq == 100 || nFreq == 1 || nFreq == 10)
                    {
                        Thread.Sleep(1);
                    }
                    else if (nFreq == 1000)
                    {
                        if (nSleepCount % 100 == 0)
                        {
                            Thread.Sleep(1);
                        }
                    }
                    else if (nFreq == 10000)
                    {
                        if (nSleepCount % 1000 == 0)
                        {
                            Thread.Sleep(1);
                        }
                    }

                } while (nReadLen > 0);
            }

            m_nStatus = 0;  // 恢复初始状态
        }

        /// <summary>
        /// 显示设备状态对话框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonDeviceStatus_Click(object sender, EventArgs e)
        {
            ShowDeviceStatusForm ShowDeviceStatusDlg = new ShowDeviceStatusForm();
            ShowDeviceStatusDlg.ShowDialog();
        }

        /// <summary>
        /// 打开设置对话框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonSetting_Click(object sender, EventArgs e)
        {
            SetParametersForm SetParamDlg = new SetParametersForm();
            if (SetParamDlg.ShowDialog() == DialogResult.OK)
            {
                ;
            }
        }

        /// <summary>
        /// 打开显示设置对话框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonDisplay_Click(object sender, EventArgs e)
        {
            SetDisplaySettingForm SetDiaplaySettingDlg = new SetDisplaySettingForm(m_nDisplayMode);
            if (SetDiaplaySettingDlg.ShowDialog() == DialogResult.OK)
            {
                int nDisplayMode = SetDiaplaySettingDlg.GetDisplayMode();
                if (m_nDisplayMode != nDisplayMode)
                {
                    m_nDisplayMode = nDisplayMode;
                    RefreshDispalyModeUI();
                }

                this.m_ShowCurveControl.OnChartCountChanged(MainConfig.GetInstance().GetShowCurveChannelCount() / 3);
                this.m_ShowFFTControl.OnChartCountChanged(MainConfig.GetInstance().GetShowCurveChannelCount() / 3);

                m_nInit = 3;    // 需要刷新坐标轴文字
            }
        }

        /// <summary>
        /// 设置需要使用的设备
        /// </summary>
        private bool SetInUseDevices()
        {
            Dictionary<uint, MultiChanMagCollectDevice> dictAllDevices = m_pDeviceManager.GetAllDevices(); // 所有已连接的设备
            Dictionary<uint, MultiChanMagCollectDevice> dictInUseDevices = new Dictionary<uint, MultiChanMagCollectDevice>();

            Dictionary<uint, int> dictDeviceID2Index = MainConfig.GetInstance().GetDeviceID2IndexDict();
            Dictionary<int, uint> dictDeviceIndex2ID = new Dictionary<int, uint>();
            foreach (KeyValuePair<uint, int> kvp in dictDeviceID2Index)
            {
                dictDeviceIndex2ID[kvp.Value] = kvp.Key;
            }

            int nUsedChannelCount = MainConfig.GetInstance().GetUsedChannelCount(); // 需要使用的通道数
            if (nUsedChannelCount <= 30)
            {
                if (!dictAllDevices.ContainsKey(dictDeviceIndex2ID[0]) || null == dictAllDevices[dictDeviceIndex2ID[0]])
                    return false;

                dictInUseDevices[dictDeviceIndex2ID[0]] = dictAllDevices[dictDeviceIndex2ID[0]];
            }
            else if (nUsedChannelCount <= 60)
            {
                if (!dictAllDevices.ContainsKey(dictDeviceIndex2ID[0]) || null == dictAllDevices[dictDeviceIndex2ID[0]]
                    || !dictAllDevices.ContainsKey(dictDeviceIndex2ID[1]) || null == dictAllDevices[dictDeviceIndex2ID[1]])
                    return false;

                dictInUseDevices[dictDeviceIndex2ID[0]] = dictAllDevices[dictDeviceIndex2ID[0]];             
                dictInUseDevices[dictDeviceIndex2ID[1]] = dictAllDevices[dictDeviceIndex2ID[1]];      
            }
            else
            {
                if (!dictAllDevices.ContainsKey(dictDeviceIndex2ID[0]) || null == dictAllDevices[dictDeviceIndex2ID[0]]
                    || !dictAllDevices.ContainsKey(dictDeviceIndex2ID[1]) || null == dictAllDevices[dictDeviceIndex2ID[1]]
                    || !dictAllDevices.ContainsKey(dictDeviceIndex2ID[2]) || null == dictAllDevices[dictDeviceIndex2ID[2]])
                    return false;

                dictInUseDevices[dictDeviceIndex2ID[0]] = dictAllDevices[dictDeviceIndex2ID[0]];
                dictInUseDevices[dictDeviceIndex2ID[1]] = dictAllDevices[dictDeviceIndex2ID[1]];
                dictInUseDevices[dictDeviceIndex2ID[2]] = dictAllDevices[dictDeviceIndex2ID[2]];               
            }

            // 确保这些设备都是正常状态
            foreach (KeyValuePair<uint, MultiChanMagCollectDevice> kvp in dictInUseDevices)
            {
                DeviceStatus eDeviceStatus = kvp.Value.GetDeviceStatus();
                if (eDeviceStatus == DeviceStatus.UNKNOWN
                    || eDeviceStatus == DeviceStatus.DISCONNECTED
                    || eDeviceStatus == DeviceStatus.UNAVAILABLE)
                    return false;
            }

            m_pDeviceManager.SetInUseDevices(dictInUseDevices);

            return true;
        }

        /// <summary>
        /// 初始化设备参数
        /// </summary>
        /// <returns></returns>
        private bool InitDevices()
        {
            // 获取设置的设备参数
            ChanMode eChanMode = MainConfig.GetInstance().GetChanMode();    // 通道模式
            ADCMode eADCMode = MainConfig.GetInstance().GetADCMode();       // AC/DC模式
            SamplingFreq eSamplingFreq = MainConfig.GetInstance().GetSamplingFreq();    // 采集频率
            TimingType eTimingType = MainConfig.GetInstance().GetTimingType();  // 授时类型

            ErrorCode err = m_pDeviceManager.SetChanMode(eChanMode);
            if (err != ErrorCode.SUCCESS)
            {
                string msg = "设置通道模式错误！";
                MessageBox.Show(msg);
                return false;
            }

            err = m_pDeviceManager.SetADCMode(eADCMode);
            if (err != ErrorCode.SUCCESS)
            {
                string msg = "设置AC/DC模式错误！";
                MessageBox.Show(msg);
                return false;
            }

            if (eSamplingFreq == SamplingFreq.FREQ_1 || eSamplingFreq == SamplingFreq.FREQ_10)
            {// 设备只支持100Hz，1000Hz，10000Hz，1Hz和10Hz实际还是用100Hz采样，然后软件再抽样
                eSamplingFreq = SamplingFreq.FREQ_250;
            }
            err = m_pDeviceManager.SetSamplingFreq(eSamplingFreq);
            if (err != ErrorCode.SUCCESS)
            {
                string msg = "设置采样频率错误！";
                MessageBox.Show(msg);
                return false;
            }

            err = m_pDeviceManager.SetTimingType(eTimingType);
            if (err != ErrorCode.SUCCESS)
            {
                string msg = "设置授时错误！";
                MessageBox.Show(msg);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 检查配置，如果不合法，则返回false；合法，返回true
        /// </summary>
        /// <returns></returns>
        private bool CheckConfig()
        {
            //if (m_nDisplayMode == 2 && MainConfig.GetInstance().GetSamplingMode() != SamplingMode.SINGLE)
            //{
            //    MessageBox.Show("只有单次采集时可以显示FFT，请检查参数设置");
            //    return false;
            //}

            return true;
        }

        /// <summary>
        /// 点击开始按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonStart_Click(object sender, EventArgs e)
        {
            if (m_nStatus == 2 || m_nStatus == 1)
            {// 正在采集
                return;
            }

            bool bRet = SetInUseDevices();  // 初始化使用中的设备
            if (!bRet)
            {
                MessageBox.Show("设备状态异常，请检查设备连接状态");
                return;
            }

            if (!CheckConfig())
            {
                return;
            }

            InitDevices();  // 初始化设备参数
            DataFileWriter.GetInstance().OnDeviceSamplingFreqChanged(0, MainConfig.GetInstance().GetSamplingFreq());
            DataBuffer.GetInstance().OnDeviceSamplingFreqChanged(0, MainConfig.GetInstance().GetSamplingFreq());

            // 让数据缓冲器知道需要使用多少台设备，才能决定后面什么时候发布数据
            Dictionary<uint, MultiChanMagCollectDevice> dictInUseDevices = m_pDeviceManager.GetInUseDevices();
            DataBuffer.GetInstance().initData();
            DataBuffer.GetInstance().SetDeviceCount(dictInUseDevices.Count);
            DataBuffer.GetInstance().CreateFilterByDiffFun();           // 创建低通滤波器
            DataBuffer.GetInstance().CreateKalmanFilter();              // 创建卡尔曼滤波器
            DataBuffer.GetInstance().CreateSensorCalibrater();          // 创建传感器数据标定器

            SamplingFreq eSamplingFreq = MainConfig.GetInstance().GetSamplingFreq();    // 采集频率
            DataBuffer.GetInstance().CreateDownsampleConverter(eSamplingFreq);
            DataBuffer.GetInstance().SetStartDateTime(DateTime.Now);    // 设置起始时间戳
            DataBuffer.GetInstance().SetLastDateTime(DateTime.Now);     // 初始化上次接收数据的时间戳
            this.m_ShowCurveControl.SetStartTime(DateTime.Now);
            string strDataSavePath = MainConfig.GetInstance().GetDataSavePath(); // 数据文件保存路径
            if (!Directory.Exists(strDataSavePath))
            {
                MessageBox.Show("数据保存路径无效，请重新设置");
                return;
            }
            DataFileWriter.GetInstance().SetDataFilePath(strDataSavePath);  // 初始化数据保存路径
            m_ShowDataControl.SetDataSavePath(strDataSavePath);

            if (m_nDisplayMode == 0) // 显示波形
            {
                this.m_ShowCurveControl.InitData(); // 初始化曲线缓存
                this.m_ShowCurveControl.ClearCharts();  // 清空曲线
                this.m_ShowCurveControl.StartRefresh(); // 开始刷新数据
            }
            else if (m_nDisplayMode == 1)   // 显示数据
            {
                this.m_ShowDataControl.InitData();     // 初始化显示数据
                this.m_ShowDataControl.StartRefresh();  // 开始刷新数据
            }
            else if (m_nDisplayMode == 2)   // 显示FFT
            {
                this.m_ShowFFTControl.InitData();       // 初始化曲线缓存
                this.m_ShowFFTControl.SetSamplingFreq(MainConfig.GetInstance().GetSamplingFreq());  // 设置采集频率
                this.m_ShowFFTControl.ClearCharts();    // 清空曲线
                this.m_ShowFFTControl.StartRefresh();
            }

            // 开始采集数据之前设置事件处理器
            SetEventHandlerBeforeStart();

            m_nStatus = 2;  // 设置状态--正在采集数据

            m_pDeviceManager.StartAcquireData();    // 开始采集数据
        }

        /// <summary>
        /// 点击结束按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonStop_Click(object sender, EventArgs e)
        {
            m_pDeviceManager.StopAcquireData(); // 停止采集数据

            // 停止采集数据之后重置事件处理器
            ResetEventHandlerAfterStop();

            if (DataFileWriter.GetInstance().IsRunning())
            {
                DataFileWriter.GetInstance().EndWrite();
            }

            if (m_nDisplayMode == 0) // 显示波形
            {
                this.m_ShowCurveControl.StopRefresh();
            }
            else if (m_nDisplayMode == 1)    // 显示数据
            {
                this.m_ShowDataControl.StopRefresh();
            }
            else if (m_nDisplayMode == 2)
            {// 只在单次采集时会计算FFT
                this.m_ShowFFTControl.StopRefresh();
            }     

            m_nStatus = 0;  // 恢复初始状态
        }

        /// <summary>
        /// 开始采集数据之前设置事件处理器
        /// </summary>
        private void SetEventHandlerBeforeStart()
        {
            // 向设备管理器注册数据接收事件处理函数
            MultiChanMagCollectDeviceManager.GetInstance().DeviceDataReceivedEvent += DataBuffer.GetInstance().AddMagData;

            // 当DataBuffer接收到数据后，会进行数据发布，数据去向包括：（1）写入文件；（2）曲线显示；（3）数据显示；（4）显示FFT
            if (MainConfig.GetInstance().GetIsNeedSaveData())
            {// 保存数据
                DataBuffer.GetInstance().PublishDataEvent += DataFileWriter.GetInstance().AsyncWrite;
            }

            if (m_nDisplayMode == 0) // 显示波形
            {
                DataBuffer.GetInstance().PublishDataEvent += this.m_ShowCurveControl.OnDataReceived;
            }
            else if (m_nDisplayMode == 1)   // 显示数据
            {
                DataBuffer.GetInstance().PublishDataEvent += this.m_ShowDataControl.OnDataReceived;
            }
            else if (m_nDisplayMode == 2)   // 显示FFT
            {
                DataBuffer.GetInstance().PublishDataEvent += this.m_ShowFFTControl.OnDataReceived;
            } 

            DataBuffer.GetInstance().SetSamplingMode(MainConfig.GetInstance().GetSamplingMode());
            DataBuffer.GetInstance().StopReceiveDataEvent += this.StopCollect;
        }

        /// <summary>
        /// 停止采集数据之后重置事件处理器
        /// </summary>
        private void ResetEventHandlerAfterStop()
        {
            MultiChanMagCollectDeviceManager.GetInstance().DeviceDataReceivedEvent -= DataBuffer.GetInstance().AddMagData;

            // 当停止数据采集后，同时也停止数据发布
            if (MainConfig.GetInstance().GetIsNeedSaveData())
            {// 保存数据
                DataBuffer.GetInstance().PublishDataEvent -= DataFileWriter.GetInstance().AsyncWrite;
            }

            if (m_nDisplayMode == 0) // 显示波形
            {
                DataBuffer.GetInstance().PublishDataEvent -= this.m_ShowCurveControl.OnDataReceived;
            }
            else if (m_nDisplayMode == 1)   // 显示数据
            {
                DataBuffer.GetInstance().PublishDataEvent -= this.m_ShowDataControl.OnDataReceived;
            }
            else if (m_nDisplayMode == 2)   // 显示FFT
            {
                DataBuffer.GetInstance().PublishDataEvent -= this.m_ShowFFTControl.OnDataReceived;
            } 

            DataBuffer.GetInstance().StopReceiveDataEvent -= this.StopCollect;
        }

        /// <summary>
        /// 停止采集数据
        /// </summary>
        private void StopCollect()
        {
            this.BeginInvoke(new Action<object, EventArgs>(this.toolStripButtonStop_Click), null, null);
        }

        /// <summary>
        /// 窗体显示后事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MCMAG30FForm_Shown(object sender, EventArgs e)
        {
            //更新显示界面
            m_nDisplayMode = 0;
            RefreshDispalyModeUI();
            this.m_ShowCurveControl.UpdateAxisText();

            m_nDisplayMode = 2;
            RefreshDispalyModeUI();
            this.m_ShowFFTControl.UpdateAxisText();        
        }

        /// <summary>
        /// 点击数据转换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonDataConvert_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.Multiselect = false;
            fileDlg.Title = "请选择要处理的文件";
            fileDlg.Filter = "数据文件(*.tsl,*.tsm,*.tsh)|*.ts*";

            if (fileDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {// 单击“确定”按钮
                ///////////////////////////////////////////////////////////////////////////
                m_nReadDataFilePercentage = 0;      // 重置读取文件的进度为0
                m_bAbortReadDataFile = false;
                this.backgroundWorkerReadFile.RunWorkerAsync();     // 运行backgroundWorker组件
                this.m_progressForm.StartPosition = FormStartPosition.CenterScreen;
                this.m_progressForm.SetTitleText("正在转换数据...");
                this.m_progressForm.Init();
                this.m_progressForm.Show();
                ///////////////////////////////////////////////////////////////////////////

                // 新启一个线程，用于转换文件数据
                ParameterizedThreadStart ParamStart = new ParameterizedThreadStart(ConvertDataFile);
                m_hConvertDataThread = new Thread(ParamStart);
                m_hConvertDataThread.IsBackground = true;
                m_hConvertDataThread.Priority = ThreadPriority.Normal;
                // 启动线程
                m_hConvertDataThread.Start(fileDlg.FileName);
            }
        }

        /// <summary>
        /// 将二进制文件转换为.dat文本文件
        /// </summary>
        /// <param name="oDataFileName"></param>
        private void ConvertDataFile(object oDataFileName)
        {
            string strDataFileName = oDataFileName as string;
            string strDirPath = Path.GetDirectoryName(strDataFileName);  // 选中文件的路径
            string strExtension = Path.GetExtension(strDataFileName);    // 选中文件的后缀名，如“.TSL”
            string strFileNameWithoutExtension = Path.GetFileNameWithoutExtension(strDataFileName);  // 不包括路径和后缀的文件名
            //DateTime sInitTimeStamp = ParseInitTimeStamp(strFileNameWithoutExtension);
            DateTime sInitTimeStamp = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string strNewFileName = strDirPath + "\\" + strFileNameWithoutExtension + ".dat"; // 在相同目录下生成一个新的文件名，后缀改成".bat"
            int nChannelCount = ParseChannelCount(strDataFileName);

            string strSamplingFreqDesc = null;
            if (strExtension.ToUpper() == ".TSA")
            {
                strSamplingFreqDesc = "SamplingRate:1 Hz,";
            }
            else if (strExtension.ToUpper() == ".TSB")
            {
                strSamplingFreqDesc = "SamplingRate:10 Hz,";
            }
            else if (strExtension.ToUpper() == ".TSL")
            {
                strSamplingFreqDesc = "SamplingRate:250 Hz,";
            }
            else if (strExtension.ToUpper() == ".TSM")
            {
                strSamplingFreqDesc = "SamplingRate:500 Hz,";
            }
            else if (strExtension.ToUpper() == ".TSH")
            {
                strSamplingFreqDesc = "SamplingRate:1000 Hz,";
            }
            else if (strExtension.ToUpper() == ".TSI")
            {
                strSamplingFreqDesc = "SamplingRate:4000 Hz,";
            }
            else
            {
                strSamplingFreqDesc = "SamplingRate:100 Hz,";
            }

            string strChannelCount = "ChannelCount:" + nChannelCount.ToString() + "\n";
            string strChannelTxt = "Time,";
            for (int i = 0; i < nChannelCount; ++i)
            {
                strChannelTxt += "ch" + (i + 1).ToString();
                if (i != nChannelCount - 1)
                {
                    strChannelTxt += ",";
                }
                else
                {
                    strChannelTxt += "\n";
                }
            }

            FileStream fsWrite = new FileStream(strNewFileName, FileMode.Append);

            // strSamplingFreqDesc
            byte[] ByteData = System.Text.Encoding.UTF8.GetBytes(strSamplingFreqDesc);
            fsWrite.Write(ByteData, 0, ByteData.Length);
            // strChannelCount
            ByteData = System.Text.Encoding.UTF8.GetBytes(strChannelCount);
            fsWrite.Write(ByteData, 0, ByteData.Length);
            // channelData
            byte[] ByteChannelData = System.Text.Encoding.UTF8.GetBytes(strChannelTxt);
            fsWrite.Write(ByteChannelData, 0, ByteChannelData.Length);

            // 读取文件
            using (FileStream fsRead = new FileStream(strDataFileName, FileMode.Open))
            {
                Int64 nfsLen = (int)fsRead.Length;
                Int64 nTotalReadLen = 0;    // 累计读取长度
                int nAllChannelLength = (nChannelCount + 1) * sizeof(double);   // 一条包含所有通道数据的数据长度
                byte[] byAllChannelData = new byte[nAllChannelLength];
                int nReadLen = 0;   // 读取长度
                do
                {
                    nReadLen = fsRead.Read(byAllChannelData, 0, nAllChannelLength);
                    nTotalReadLen += nReadLen;

                    m_nReadDataFilePercentage = (int)((double)nTotalReadLen * 100.0 / (double)nfsLen);  // 更新进度

                    int nStartIndex = 0;
                    MultiChannelMagData[] sData = new MultiChannelMagData[1];
                    sData[0] = new MultiChannelMagData();
                    sData[0].m_nTimeStamp = BitConverter.ToUInt64(byAllChannelData, nStartIndex);
                    nStartIndex += 8;
                    sData[0].m_dMagData = new double[nChannelCount];
                    for (int i = 0; i < nChannelCount; ++i)
                    {
                        sData[0].m_dMagData[i] = BitConverter.ToDouble(byAllChannelData, nStartIndex);
                        nStartIndex += 8;
                    }

                    string strDataTxt = "";
                    //strDataTxt += sData[0].m_dTimeStamp.ToString("F2") + ",";
                    ulong nMillSecond = (ulong)(sData[0].m_nTimeStamp / 1000);   // 整数毫秒
                    DateTime sDateTimeStamp = sInitTimeStamp.AddMilliseconds(nMillSecond);
                    double dMill = sData[0].m_nTimeStamp / 1000.0 - nMillSecond;
                    int n100Mill = (int)(dMill * 100 + 0.5);
                    if (n100Mill == 100)
                    {
                        sDateTimeStamp = sDateTimeStamp.AddMilliseconds(1);
                        n100Mill = 0;
                    }
                    strDataTxt = sDateTimeStamp.ToString("yyyyMMddHHmmssfff");
                    strDataTxt += n100Mill.ToString("D2") + ",";

                    for (int i = 0; i < nChannelCount; ++i)
                    {
                        strDataTxt += sData[0].m_dMagData[i].ToString("F2");
                        if (i != nChannelCount - 1)
                        {
                            strDataTxt += ",";
                        }
                        else
                        {
                            strDataTxt += "\n";
                        }
                    }

                    ByteChannelData = System.Text.Encoding.UTF8.GetBytes(strDataTxt);
                    fsWrite.Write(ByteChannelData, 0, ByteChannelData.Length);
                    fsWrite.Flush();

                    if (m_bAbortReadDataFile)
                    {
                        break;
                    }

                } while (nReadLen > 0);
            }

            // 关闭文件
            fsWrite.Close();
        }

        /// <summary>
        /// 从文件名中解析起始时间
        /// </summary>
        /// <param name="strFileName">包含起始时间的文件名</param>
        /// <returns>起始时间</returns>
        private DateTime ParseInitTimeStamp(string strFileName)
        {
            string strDateTime = strFileName.Substring(4, 14);

            int nYear = int.Parse(strDateTime.Substring(0, 4));
            int nMonth = int.Parse(strDateTime.Substring(4, 2));
            int nDay = int.Parse(strDateTime.Substring(6, 2));
            int nHour = int.Parse(strDateTime.Substring(8, 2));
            int nMin = int.Parse(strDateTime.Substring(10, 2));
            int nSec = int.Parse(strDateTime.Substring(12, 2));

            DateTime sInitTimeStamp = new DateTime(nYear, nMonth, nDay, nHour, nMin, nSec);
            return sInitTimeStamp;
        }

        private void ShowXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowXToolStripMenuItem.Checked = !this.ShowXToolStripMenuItem.Checked;

            int nCurveControlIndex = int.Parse(this.toolStripComboBoxCurveControlIndex.Text);
            m_bDictShowCurveFlags[nCurveControlIndex][0] = this.ShowXToolStripMenuItem.Checked;

            if (m_nDisplayMode == 0)
            {
                m_ShowCurveControl.SetCurveVisible(nCurveControlIndex, 0, m_bDictShowCurveFlags[nCurveControlIndex][0]);
            }
            else if (m_nDisplayMode == 2)
            {
                m_ShowFFTControl.SetCurveVisible(nCurveControlIndex, 0, m_bDictShowCurveFlags[nCurveControlIndex][0]);
            }
        }

        private void ShowYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowYToolStripMenuItem.Checked = !this.ShowYToolStripMenuItem.Checked;

            int nCurveControlIndex = int.Parse(this.toolStripComboBoxCurveControlIndex.Text);
            m_bDictShowCurveFlags[nCurveControlIndex][1] = this.ShowYToolStripMenuItem.Checked;

            if (m_nDisplayMode == 0)
            {
                m_ShowCurveControl.SetCurveVisible(nCurveControlIndex, 1, m_bDictShowCurveFlags[nCurveControlIndex][1]);
            }
            else if (m_nDisplayMode == 2)
            {
                m_ShowFFTControl.SetCurveVisible(nCurveControlIndex, 1, m_bDictShowCurveFlags[nCurveControlIndex][1]);
            }
        }

        private void ShowZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowZToolStripMenuItem.Checked = !this.ShowZToolStripMenuItem.Checked;

            int nCurveControlIndex = int.Parse(this.toolStripComboBoxCurveControlIndex.Text);
            m_bDictShowCurveFlags[nCurveControlIndex][2] = this.ShowZToolStripMenuItem.Checked;

            if (m_nDisplayMode == 0)
            {
                m_ShowCurveControl.SetCurveVisible(nCurveControlIndex, 2, m_bDictShowCurveFlags[nCurveControlIndex][2]);
            }
            else if (m_nDisplayMode == 2)
            {
                m_ShowFFTControl.SetCurveVisible(nCurveControlIndex, 2, m_bDictShowCurveFlags[nCurveControlIndex][2]);
            }
        }

        /// <summary>
        /// 弹出下拉菜单时更新CheckState
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripDropDownButtonCurveIndex_DropDownOpening(object sender, EventArgs e)
        {
            int nCurveControlIndex = int.Parse(this.toolStripComboBoxCurveControlIndex.Text);
            this.ShowXToolStripMenuItem.Checked = m_bDictShowCurveFlags[nCurveControlIndex][0];
            this.ShowYToolStripMenuItem.Checked = m_bDictShowCurveFlags[nCurveControlIndex][1];
            this.ShowZToolStripMenuItem.Checked = m_bDictShowCurveFlags[nCurveControlIndex][2];
        }

        /// <summary>
        /// 点击“连接PLC”
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonConnectPLC_Click(object sender, EventArgs e)
        {
            if (!m_pS7_200.IsRunning())
            {
                string strModbusTCPServerIP = MainConfig.GetInstance().GetModbusTCPServerIP();
                int nModbusTCPServerPort = MainConfig.GetInstance().GetModbusTCPServerPort();
                int nTimingCycle = MainConfig.GetInstance().GetModbusTimingCycle();
                int nTimeout = MainConfig.GetInstance().GetModbusTimeout();
                int nReconnectInterval = MainConfig.GetInstance().GetModbusReconnectInterval();

                m_pS7_200.StartAcqCmdReceivedEvent += StartAcqCmdReceivedEventHandler;
                m_pS7_200.SetZeroOffsetCmdReceivedEvent += SetZeroOffsetCmdReceivedEventHandler;

                DataBuffer.GetInstance().SetZeroOffsetDoneEvent += m_pS7_200.SetZeroOffsetDone;
                m_ShowDataControl.AbnormalStateEvent += m_pS7_200.GenAbnormalState;

                // 连接PLC
                bool bRet = m_pS7_200.Connect(strModbusTCPServerIP, nModbusTCPServerPort);
                if (!bRet)
                {
                    MessageBox.Show("PLC连接错误：" + m_pS7_200.GetLastErrMsg());
                    return;
                }
                else
                {
                    MessageBox.Show("PLC连接成功");
                }

                m_pS7_200.SetReadTimerPeroid(nTimingCycle);
                m_pS7_200.SetTimeout(nTimeout);
                m_pS7_200.SetReconnectInterval(nReconnectInterval);
                m_pS7_200.Start();
            }  
        }

        /// <summary>
        /// 点击“断开PLC”
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonDisconnectPLC_Click(object sender, EventArgs e)
        {
            if (m_pS7_200.IsRunning())
            {
                m_pS7_200.Stop();
				m_pS7_200.Disconnect();
            }         

            m_pS7_200.StartAcqCmdReceivedEvent -= StartAcqCmdReceivedEventHandler;
            m_pS7_200.SetZeroOffsetCmdReceivedEvent -= SetZeroOffsetCmdReceivedEventHandler;

            DataBuffer.GetInstance().SetZeroOffsetDoneEvent -= m_pS7_200.SetZeroOffsetDone;
            m_ShowDataControl.AbnormalStateEvent -= m_pS7_200.GenAbnormalState;
        }

        /// <summary>
        /// 响应PLC的开始采集命令
        /// </summary>
        private void StartAcqCmdReceivedEventHandler()
        {
            this.toolStripMain.Invoke(new Action(() =>
            {
                // 触发ToolStripButton的点击事件
                toolStripButtonStart_Click(null, null);
            }));
        }

        /// <summary>
        /// 响应PLC的设置零偏命令
        /// </summary>
        private void SetZeroOffsetCmdReceivedEventHandler()
        {
            m_ShowDataControl.SetNeedCheckData(true);

            m_ShowDataControl.SetZeroOffset();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            DeviceData[] sDeviceData = new DeviceData[10];
            for (int i = 0; i < 10; ++i)
            {
                DeviceData item = new DeviceData();
                item.dChannelDatas = new double[30];
                for (int j = 0; j < 30; ++j)
                {
                    item.dChannelDatas[j] = Math.Sin((i * 30 + j) / 180.0 * Math.PI);
                }

                sDeviceData[i] = item;
            }

            uint nDeviceID = ((uint)0x04B4 << 16) + (uint)0x04B0;
            DataBuffer.GetInstance().AddMagData(nDeviceID, ref sDeviceData);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            DeviceData[] sDeviceData = new DeviceData[10];
            for (int i = 0; i < 10; ++i)
            {
                DeviceData item = new DeviceData();
                item.dChannelDatas = new double[30];
                for (int j = 0; j < 30; ++j)
                {
                    item.dChannelDatas[j] = Math.Cos((i * 30 + j) / 180.0 * Math.PI);
                }

                sDeviceData[i] = item;
            }

            uint nDeviceID = ((uint)0x04B4 << 16) + (uint)0x04B1;
            DataBuffer.GetInstance().AddMagData(nDeviceID, ref sDeviceData);
        }
    }
}
