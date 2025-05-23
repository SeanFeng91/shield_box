using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using MCMag30FDevice;
using System.Diagnostics;

namespace MCMAG30F
{
    public partial class ShowFFTControl : UserControl
    {
        private const int m_nChartCount = 10;       // 曲线控件的总个数
        private AxChartLib.AxChart[] m_AxCharts = new AxChartLib.AxChart[m_nChartCount];
        private int[] m_nSerieIDs = new int[m_nChartCount * 3]; // 每个曲线控件显示一个传感器的3通道数据

        private int m_nRowCount = 0;    // 曲线控件的行数
        private int m_nColCount = 0;    // 曲线控件的列数
        private int m_nShowChartCount = 0;  // 显示的曲线控件个数
        private bool m_bInitChart = false;  //是否初始化控件数组

        private int m_nRWFlag = 1;  // 切换读写缓存的标志,=1写第1组缓存，读第2组缓存；=2写第2组缓存，读第1组缓存
        private static object m_lockObject = new object();
        private List<double>[] m_dSamplingDatas_1 = new List<double>[m_nChartCount * 3];       // 第一组采样数据缓存
        private List<double>[] m_dSamplingDatas_2 = new List<double>[m_nChartCount * 3];       // 第一组采样数据缓存
        private Dictionary<int, List<bool>> m_bDictShowFreqFlags = new Dictionary<int, List<bool>>();
        private int[] m_nCurveChannelIndex = null;
        private SamplingFreq m_eSamplingFreq = SamplingFreq.FREQ_250;             // 采样频率
        private double[][] m_dPsdDatas = new double[m_nChartCount * 3][];            // 功率谱数据
        private double[][] m_dFreqDatas = new double[m_nChartCount * 3][];           // 频率数据

        private double m_dMagUnitCoeff = 1;                 // 磁场单位系数

        private Thread m_ThreadProcessData = null;      // 处理数据的线程，计算PSD，并刷新曲线

        /// <summary>
        /// 构造函数
        /// </summary>
        public ShowFFTControl()
        {
            InitializeComponent();

            InitCharts();
            OnChartCountChanged(10/*MainConfig.GetInstance().GetShowCurveChannelCount() / 3*/);
        }

        /// <summary>
        /// 窗体尺寸改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowFFTControl_Resize(object sender, EventArgs e)
        {
            if (m_bInitChart)
            {
                RelayoutCharts(m_nRowCount, m_nColCount);
            }
        }

        /// <summary>
        /// 缓存曲线控件到数组
        /// </summary>
        private void InitCharts()
        {
            if (!m_bInitChart)
            {
                m_AxCharts[0] = this.axChart1;
                m_AxCharts[1] = this.axChart2;
                m_AxCharts[2] = this.axChart3;
                m_AxCharts[3] = this.axChart4;
                m_AxCharts[4] = this.axChart5;
                m_AxCharts[5] = this.axChart6;
                m_AxCharts[6] = this.axChart7;
                m_AxCharts[7] = this.axChart8;
                m_AxCharts[8] = this.axChart9;
                m_AxCharts[9] = this.axChart10;

                for (int i = 0; i < m_nChartCount * 3; ++i)
                {
                    m_nSerieIDs[i] = -1;
                }

                for (int i = 0; i < m_nChartCount; ++i)
                {
                    List<bool> bListShowCurves = new List<bool>();
                    bListShowCurves.Add(true);
                    bListShowCurves.Add(true);
                    bListShowCurves.Add(true);
                    m_bDictShowFreqFlags.Add(i, bListShowCurves);
                }
            }

            m_bInitChart = true;
        }

        /// <summary>
        /// 重新排布曲线控件
        /// </summary>
        /// <param name="nRowCount">排布行数</param>
        /// <param name="nColCount">排布列数</param>
        private void RelayoutCharts(int nRowCount, int nColCount)
        {
            // 先全部隐藏
            for (int i = 0; i < m_nChartCount; ++i)
            {
                m_AxCharts[i].Visible = false;
            }

            // 计算尺寸，重新布局
            int nWidth = this.ClientRectangle.Width;    // 客户区宽度
            int nHeight = this.ClientRectangle.Height;  // 客户区高度
            int nChartWidth = (int)((double)nWidth / (double)nColCount);
            int nChartHeight = (int)((double)nHeight / (double)nRowCount);

            for (int i = 0; i < nColCount; ++i)
            {
                for (int j = 0; j < nRowCount; ++j)
                {
                    m_AxCharts[i * nRowCount + j].Location = new System.Drawing.Point(i * nChartWidth, j * nChartHeight);
                    m_AxCharts[i * nRowCount + j].Width = nChartWidth;
                    m_AxCharts[i * nRowCount + j].Height = nChartHeight;

                    // 重置为可见
                    m_AxCharts[i * nRowCount + j].Visible = true;
                }
            }
        }

        /// <summary>
        /// 曲线控件个数改变
        /// </summary>
        /// <param name="nChartCount">曲线控件个数</param>
        public void OnChartCountChanged(int nChartCount)
        {
            int nRowCount = 0;  // 行数
            int nColCount = 0;  // 列数

            m_nShowChartCount = nChartCount;    // 要显示的曲线控件个数
            // 计算需要布局的行数和列数
            CalculateChartLayoutRowAndCol(nChartCount, ref nRowCount, ref nColCount);

            if (m_nRowCount != nRowCount || m_nColCount != nColCount)
            {
                m_nRowCount = nRowCount;
                m_nColCount = nColCount;

                if (m_bInitChart)
                {
                    RelayoutCharts(nRowCount, nColCount);
                }
            }
        }

        /// <summary>
        /// 根据曲线控件总个数计算界面布局的行数和列数
        /// </summary>
        /// <param name="nChartCount">曲线控件总个数</param>
        /// <param name="nRowCount">行数</param>
        /// <param name="nColCount">列数</param>
        private void CalculateChartLayoutRowAndCol(int nChartCount, ref int nRowCount, ref int nColCount)
        {
            switch (nChartCount)
            {
                case 1:
                case 2:
                case 3:
                case 5:
                    {
                        nRowCount = nChartCount;
                        nColCount = 1;
                        break;
                    }
                case 4:
                case 6:
                case 7:
                case 8:
                case 10:
                    {
                        nRowCount = (int)Math.Round((double)nChartCount / 2.0);
                        nColCount = 2;
                        break;
                    }
                case 9:
                    {
                        nRowCount = (int)Math.Round((double)nChartCount / 3.0);
                        nColCount = 3;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        /// <summary>
        /// 初始化缓存结构
        /// </summary>
        public void InitData()
        {
            for (int i = 0; i < m_nShowChartCount * 3; ++i)
            {
                m_dSamplingDatas_1[i] = new List<double>();
                m_dSamplingDatas_2[i] = new List<double>();
            }

            for (int i = m_nShowChartCount * 3; i < m_nChartCount * 3; ++i)
            {
                m_dSamplingDatas_1[i] = null;
                m_dSamplingDatas_1[i] = null;
            }

            MainConfig.GetInstance().GetCurveChannelIndex(out m_nCurveChannelIndex);

            for (int i = 0; i < m_nShowChartCount; ++i)
            {
                if (m_nSerieIDs[3 * i] == -1)
                {
                    m_nSerieIDs[3 * i] = m_AxCharts[i].CreateLineSerie(false, false);
                    m_nSerieIDs[3 * i + 1] = m_AxCharts[i].CreateLineSerie(false, false);
                    m_nSerieIDs[3 * i + 2] = m_AxCharts[i].CreateLineSerie(false, false);
                }

                m_AxCharts[i].ClearSerie(m_nSerieIDs[3 * i]);
                m_AxCharts[i].ClearSerie(m_nSerieIDs[3 * i + 1]);
                m_AxCharts[i].ClearSerie(m_nSerieIDs[3 * i + 2]);
                // LeftAxis = 0
                // BottomAxis = 1
                // RightAxis = 2
                // TopAxis = 3
                m_AxCharts[i].SetAxisMinMax(0, 0, 1);
                m_AxCharts[i].SetAxisMinMax(1, 0, 100);
            }

            UpdateAxisText();

            m_eSamplingFreq = MainConfig.GetInstance().GetSamplingFreq();
        }

        /// <summary>
        /// 更新坐标轴
        /// </summary>
        public void UpdateAxisText()
        {
            string strXText = "Freq(Hz)";
            string strYText = "磁场(nT)";

            int nYAxisUnit = MainConfig.GetInstance().GetYAxisUnit();
            if (nYAxisUnit == 1)
            {
                m_dMagUnitCoeff = 1.0;
                strYText = "磁场(nT)";
            }
            else if (nYAxisUnit == 2)
            {
                m_dMagUnitCoeff = 1000.0;
                strYText = "磁场(uT)";
            }

            for (int i = 0; i < m_AxCharts.Length; ++i)
            {
                m_AxCharts[i].SetXAxisText(ref strXText);
                m_AxCharts[i].SetYAxisText(ref strYText);
            }
        }

        /// <summary>
        /// 接收到数据事件处理
        /// </summary>
        /// <param name="sMagDatas">接收到的磁场数据</param>
        public void OnDataReceived(MultiChannelMagData[] sMagDatas)
        {
            if (null == sMagDatas || sMagDatas.Length <= 0 || sMagDatas[0].m_dMagData.Length <= 0)
                return;

            lock(m_lockObject)
            {
                List<double>[] pSamplingDatas = null;
                if (1 == m_nRWFlag)
                {
                    pSamplingDatas = m_dSamplingDatas_1;
                }
                else if (2 == m_nRWFlag)
                {
                    pSamplingDatas = m_dSamplingDatas_2;
                }

                for (int i = 0; i < m_nShowChartCount; ++i)
                {
                    for (int j = 0; j < sMagDatas.Length; ++j)
                    {
                        // 只取Y值
                        pSamplingDatas[3 * i].Add(sMagDatas[j].m_dMagData[m_nCurveChannelIndex[3 * i]] / m_dMagUnitCoeff);
                        pSamplingDatas[3 * i + 1].Add(sMagDatas[j].m_dMagData[m_nCurveChannelIndex[3 * i + 1]] / m_dMagUnitCoeff);
                        pSamplingDatas[3 * i + 2].Add(sMagDatas[j].m_dMagData[m_nCurveChannelIndex[3 * i + 2]] / m_dMagUnitCoeff);
                    }
                }
            }
        }

        /// <summary>
        /// 处理并显示PSD数据
        /// </summary>
        public void ProcessAndShowPSDData()
        {
            // 启动一个线程处理
            m_ThreadProcessData = new Thread(new ThreadStart(ProcessDataThread));
            m_ThreadProcessData.IsBackground = true;
            m_ThreadProcessData.Priority = ThreadPriority.Highest;
            m_ThreadProcessData.Start();
        }

        /// <summary>
        /// 处理数据线程，计算PSD，并刷新曲线
        /// </summary>
        private void ProcessDataThread()
        {
            // 处理数据
            ProcessData();

            MethodInvoker mi = new MethodInvoker(() =>
            {
                // 刷新曲线
                RefreshCharts();
            });
            this.BeginInvoke(mi);
        }

        /// <summary>
        /// 开始刷新频谱
        /// </summary>
        public void StartRefresh()
        {
            this.timerRefreshFreq.Start();
        }

        /// <summary>
        /// 停止刷新频谱
        /// </summary>
        public void StopRefresh()
        {
            ClearData();
            this.timerRefreshFreq.Stop();
        }

        /// <summary>
        /// 定时刷新频谱
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerRefreshFreq_Tick(object sender, EventArgs e)
        {
            // 处理数据
            ProcessData();
            RefreshCharts();
        }

        /// <summary>
        /// 刷新曲线
        /// </summary>
        public void RefreshCharts()
        {
            // Disable the refresh
            EnableRefreshCharts(false);

            // 更新曲线数据
            UpdateCharts();

            // Enable the refresh
            EnableRefreshCharts(true);
        }

        /// <summary>
        /// 使能/禁止刷新曲线
        /// </summary>
        /// <param name="bEnabled">是否使能</param>
        private void EnableRefreshCharts(bool bEnabled)
        {
            for (int i = 0; i < m_nShowChartCount; ++i)
            {
                m_AxCharts[i].EnableRefresh(bEnabled);
            }
        }

        /// <summary>
        /// 更新曲线
        /// </summary>
        private void UpdateCharts()
        {
            for (int i = 0; i < m_nShowChartCount; ++i)
            {// 第i个曲线控件
                bool bShow = false;
                for (int j = 0; j < 3; ++j)
                {// 第j个曲线序列
                    if (m_dPsdDatas[i * 3 + j] == null || m_dFreqDatas[i * 3 + j] == null
                        || m_dPsdDatas[i * 3 + j].Length <= 0 || m_dFreqDatas[i * 3 + j].Length <= 0)
                    {
                        continue;
                    }

                    // 在Chart中显示
                    ShowPsd(i, j, m_dPsdDatas[i * 3 + j], m_dFreqDatas[i * 3 + j]);
                    bShow = true;
                }

                if (bShow)
                {
                    // 调整XY轴显示范围
                    AdjustChartXYAxisRange(i);
                }
            }
        }

        /// <summary>
        /// 调整曲线XY轴坐标范围
        /// </summary>
        /// <param name="nChartIndex">曲线控件索引</param>
        private void AdjustChartXYAxisRange(int nChartIndex)
        {
            double dXMin = 0;   // X轴最小值
            double dXMax = 0;   // X轴最大值
            double dYMin = 0;   // Y轴最小值
            double dYMax = 0;   // Y轴最大值

            for (int i = 0; i < 3; ++i)
            {
                double dXTempMin = 0;
                double dXTempMax = 0;
                double dYTempMin = 0;
                double dYTempMax = 0;
                if (m_dFreqDatas[3 * nChartIndex + i] != null)
                {
                    // 获取X轴范围
                    GetMinMax(m_dFreqDatas[3 * nChartIndex + i], ref dXTempMin, ref dXTempMax);
                    // 获取Y轴范围
                    GetMinMax(m_dPsdDatas[3 * nChartIndex + i], ref dYTempMin, ref dYTempMax);

                    if (dXTempMin < dXMin)
                    {
                        dXMin = dXTempMin;
                    }
                    if (dXTempMax > dXMax)
                    {
                        dXMax = dXTempMax;
                    }

                    if (dYTempMin < dYMin)
                    {
                        dYMin = dYTempMin;
                    }
                    if (dYTempMax > dYMax)
                    {
                        dYMax = dYTempMax;
                    }
                }
            }

            // XY轴显示范围
            double dXAxisMin = dXMin;
            double dXAxisMax = dXMax;
            double dYAxisMin = 0;
            double dYAxisMax = 0;
            // 调整Y轴范围
            CalcYAxisShowRange(dYMin, dYMax, ref dYAxisMin, ref dYAxisMax);

            // LeftAxis = 0
            // BottomAxis = 1
            // RightAxis = 2
            // TopAxis = 3
            m_AxCharts[nChartIndex].SetAxisMinMax(0, dYAxisMin, dYAxisMax);
            m_AxCharts[nChartIndex].SetAxisMinMax(1, dXAxisMin, dXAxisMax);
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        private void ClearData()
        {
            lock(m_lockObject)
            {
                // 清空缓存和曲线序列
                for (int i = 0; i < m_nShowChartCount; ++i)
                {
                    m_dSamplingDatas_1[3 * i].Clear();
                    m_dSamplingDatas_1[3 * i + 1].Clear();
                    m_dSamplingDatas_1[3 * i + 2].Clear();

                    m_dSamplingDatas_2[3 * i].Clear();
                    m_dSamplingDatas_2[3 * i + 1].Clear();
                    m_dSamplingDatas_2[3 * i + 2].Clear();

                    m_dSamplingDatas_1[3 * i].Capacity = 120000;
                    m_dSamplingDatas_1[3 * i + 1].Capacity = 120000;
                    m_dSamplingDatas_1[3 * i + 2].Capacity = 120000;

                    m_dSamplingDatas_2[3 * i].Capacity = 120000;
                    m_dSamplingDatas_2[3 * i + 1].Capacity = 120000;
                    m_dSamplingDatas_2[3 * i + 2].Capacity = 120000;
                }
            }
        }

        /// <summary>
        /// 清空曲线数据
        /// </summary>
        public void ClearCharts()
        {
            // 清空缓存和曲线序列
            for (int i = 0; i < m_nShowChartCount; ++i)
            {
                m_dSamplingDatas_1[3 * i].Clear();
                m_dSamplingDatas_1[3 * i + 1].Clear();
                m_dSamplingDatas_1[3 * i + 2].Clear();

                m_dSamplingDatas_2[3 * i].Clear();
                m_dSamplingDatas_2[3 * i + 1].Clear();
                m_dSamplingDatas_2[3 * i + 2].Clear();

                m_dSamplingDatas_1[3 * i].Capacity = 120000;
                m_dSamplingDatas_1[3 * i + 1].Capacity = 120000;
                m_dSamplingDatas_1[3 * i + 2].Capacity = 120000;

                m_dSamplingDatas_2[3 * i].Capacity = 120000;
                m_dSamplingDatas_2[3 * i + 1].Capacity = 120000;
                m_dSamplingDatas_2[3 * i + 2].Capacity = 120000;

                m_AxCharts[i].ClearSerie(m_nSerieIDs[3 * i]);
                m_AxCharts[i].ClearSerie(m_nSerieIDs[3 * i + 1]);
                m_AxCharts[i].ClearSerie(m_nSerieIDs[3 * i + 2]);
            }
        }

        /// <summary>
        ///  处理数据
        /// </summary>
        /// <returns></returns>
        public bool ProcessData()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            List<double>[] pSamplingDatas = null;
            // 加锁
            lock (m_lockObject)
            {             
                if (m_nRWFlag == 1)
                {// 读取第2组缓存
                    pSamplingDatas = m_dSamplingDatas_2;
                    if (pSamplingDatas.Length <= 0 || pSamplingDatas[0] == null || pSamplingDatas[0].Count <= 0)
                    {
                        m_nRWFlag = 2;
                        return false;
                    }
                }
                else if (m_nRWFlag == 2)
                {// 读取第1组缓存
                    pSamplingDatas = m_dSamplingDatas_1;
                    if (pSamplingDatas.Length <= 0 || pSamplingDatas[0] == null || pSamplingDatas[0].Count <= 0)
                    {
                        m_nRWFlag = 1;
                        return false;
                    }
                }
            }

            // 有数据才计算
            if (pSamplingDatas.Length > 0 && pSamplingDatas[0] != null && pSamplingDatas[0].Count > 0)
            {
                double dSamplingFreq = GetSamplingFreq(m_eSamplingFreq);    // 采样频率数值

                for (int i = 0; i < m_nShowChartCount * 3; ++i)
                {
                    //CalcPspectrum(pSamplingDatas[i].ToArray(), dSamplingFreq, ref m_dPsdDatas[i], ref m_dFreqDatas[i]);

                    //// 开方
                    //for (int j = 0; j < m_dPsdDatas[i].Length; ++j)
                    //{
                    //    m_dPsdDatas[i][j] = System.Math.Sqrt(m_dPsdDatas[i][j]);
                    //}
                    int nChartIndex = i / 3;
                    int nCurveIndex = i % 3;
                    if (m_bDictShowFreqFlags[nChartIndex][nCurveIndex] == true)
                    {// 只有要显示的通道才计算
                        Algorithm.CalcFFT(pSamplingDatas[i].ToArray(), ref m_dPsdDatas[i], ref m_dFreqDatas[i]);
                        for (int j = 0; j < m_dFreqDatas[i].Length; ++j)
                        {
                            m_dFreqDatas[i][j] = (double)j / (m_dFreqDatas[i].Length - 1) * dSamplingFreq / 2.0;
                        }
                    }
                    else
                    {
                        m_dPsdDatas[i] = null;
                        m_dFreqDatas[i] = null;
                    }
                }            
            }

            stopWatch.Stop();
            long times1 = stopWatch.ElapsedMilliseconds;

            for (int i = 0; i < m_nShowChartCount * 3; ++i)
            {
                if (pSamplingDatas[i] != null)
                {
                    pSamplingDatas[i].Clear();
                }
            }

            lock(m_lockObject)
            {
                // 切换读写缓存的标志
                if (m_nRWFlag == 1)
                {
                    m_nRWFlag = 2;
                }
                else if (m_nRWFlag == 2)
                {
                    m_nRWFlag = 1;
                }
            }

            return true;
        }

        /// <summary>
        /// 设置采样频率
        /// </summary>
        /// <param name="eSamplingFreq">采样频率</param>
        public void SetSamplingFreq(SamplingFreq eSamplingFreq)
        {
            m_eSamplingFreq = eSamplingFreq;
        }

        /// <summary>
        /// 显示功率谱数据
        /// </summary>
        public void ShowPsd(int nChartIndex, int nSerieIndex, double[] dPsdDatas, double[] dFreqDatas)
        {
            if (null != dPsdDatas && null != dFreqDatas)
            {
                // 清空曲线
                m_AxCharts[nChartIndex].ClearSerie(m_nSerieIDs[nChartIndex * 3 + nSerieIndex]);

                for (int i = 0; i < dPsdDatas.Length; ++i)
                {
                    m_AxCharts[nChartIndex].AddPointToTempBuffer(m_nSerieIDs[nChartIndex * 3 + nSerieIndex], dFreqDatas[i], dPsdDatas[i]);
                }
                m_AxCharts[nChartIndex].CopyTempPointsToLineSerie(m_nSerieIDs[nChartIndex * 3 + nSerieIndex]);
            }
        }

        /// <summary>
        /// 获取一个数组中的最小/最大值
        /// </summary>
        /// <param name="dDatas">数据数组</param>
        /// <param name="dMinValue">最小值</param>
        /// <param name="dMaxValue">最大值</param>
        private void GetMinMax(double[] dDatas, ref double dMinValue, ref double dMaxValue)
        {
            if (null != dDatas && dDatas.Length > 0)
            {
                dMinValue = dDatas[0];
                dMaxValue = dDatas[0];

                for (int i = 1; i < dDatas.Length; ++i)
                {
                    if (dDatas[i] < dMinValue)
                    {
                        dMinValue = dDatas[i];
                    }

                    if (dDatas[i] > dMaxValue)
                    {
                        dMaxValue = dDatas[i];
                    }
                }
            }
        }

        /// <summary>
        /// 计算Y轴显示范围
        /// </summary>
        /// <param name="dYMin">Y最小值</param>
        /// <param name="dYMax">Y最大值</param>
        /// <param name="dYAxisMin">Y显示最小值</param>
        /// <param name="dYAxisMax">Y显示最大值</param>
        private void CalcYAxisShowRange(double dYMin, double dYMax, ref double dYAxisMin, ref double dYAxisMax)
        {
            double dInterval = System.Math.Abs(dYMax - dYMin);
            dYAxisMin = dYMin - dInterval * 0.05;
            dYAxisMax = dYMax + dInterval * 0.05;
        }

        /// <summary>
        /// 设置曲线是否可见
        /// </summary>
        /// <param name="nChartIndex">曲线控件索引</param>
        /// <param name="nSerieIndex">曲线序列索引</param>
        /// <param name="bVisible">是否可见</param>
        public void SetCurveVisible(int nChartIndex, int nSerieIndex, bool bVisible)
        {
            // 加锁
            lock (m_lockObject)
            {
                // 检查参数
                if (!m_bDictShowFreqFlags.ContainsKey(nChartIndex))
                    return;

                if (nSerieIndex < 0 || nSerieIndex > m_bDictShowFreqFlags[nChartIndex].Count)
                    return;

                // 更新是否可见
                m_bDictShowFreqFlags[nChartIndex][nSerieIndex] = bVisible;

                // 保证所有nSerieID都是有的
                for (int i = 0; i < m_nChartCount; ++i)
                {
                    if (m_nSerieIDs[3 * i] == -1)
                    {
                        m_nSerieIDs[3 * i] = m_AxCharts[i].CreateLineSerie(false, false);
                        m_nSerieIDs[3 * i + 1] = m_AxCharts[i].CreateLineSerie(false, false);
                        m_nSerieIDs[3 * i + 2] = m_AxCharts[i].CreateLineSerie(false, false);
                    }
                }

                // 更新控件中的曲线是否可见
                // nSerieIndex=0, X轴，=1, Y轴， =2 Z轴
                int nSerieID = m_nSerieIDs[3 * nChartIndex + nSerieIndex];
                m_AxCharts[nChartIndex].SetSerieVisible(nSerieID, bVisible);

                // 调整曲线XY轴
                AdjustChartXYAxisRange(nChartIndex);
            }
        }

        /// <summary>
        /// 获取显示曲线的状态
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, List<bool>> GetShowCurveFlag()
        {
            return m_bDictShowFreqFlags;
        }

        /// <summary>
        /// 获取采样频率数值
        /// </summary>
        /// <param name="eSamplingFreq">采样频率枚举</param>
        /// <returns></returns>
        private double GetSamplingFreq(SamplingFreq eSamplingFreq)
        {
            double dSamplingFreq = 250;

            switch (eSamplingFreq)
            {
                case SamplingFreq.FREQ_1:
                    {
                        dSamplingFreq = 1;
                        break;
                    }
                case SamplingFreq.FREQ_10:
                    {
                        dSamplingFreq = 10;
                        break;
                    }
                case SamplingFreq.FREQ_250:
                    {
                        dSamplingFreq = 250;
                        break;
                    }
                case SamplingFreq.FREQ_500:
                    {
                        dSamplingFreq = 500;
                        break;
                    }
                case SamplingFreq.FREQ_1000:
                    {
                        dSamplingFreq = 1000;
                        break;
                    }
                case SamplingFreq.FREQ_4000:
                    {
                        dSamplingFreq = 4000;
                        break;
                    }
                default:
                    {
                        dSamplingFreq = 250;
                        break;
                    }
            }

            return dSamplingFreq;
        }
    }
}
