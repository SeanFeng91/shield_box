using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MCMag30FDevice;

namespace MCMAG30F
{
    /// <summary>
    /// 显示曲线用的XY坐标数据
    /// </summary>
    public struct SChartXYPoint
    {
        public double X;
        public double Y;

        public SChartXYPoint(double dX, double dY)
        {
            this.X = dX;
            this.Y = dY;
        }
    }

    public partial class ShowCurveControl : UserControl
    {
        private const int m_nChartCount = 10;       // 曲线控件的总个数
        private AxChartLib.AxChart[] m_AxCharts = new AxChartLib.AxChart[m_nChartCount];
        private int[] m_nSerieIDs = new int[m_nChartCount * 3]; // 每个曲线控件显示一个传感器的3通道数据

        private int m_nRowCount = 0;    // 曲线控件的行数
        private int m_nColCount = 0;    // 曲线控件的列数
        private int m_nShowChartCount = 0;  // 显示的曲线控件个数
        private bool m_bInitChart = false;  //是否初始化控件数组
        double m_dBottomAxisRange = 10;				    // 底轴范围
        private double m_dYAxisUnitCoeff = 1;           // Y轴单位系数
        private double m_dXAxisUnitCoeff = 1000;        // X轴单位系数，固定为1000，原始数据时间单位为ms,显示横轴为s

        private int m_nRWFlag = 1;  // 切换读写缓存的标志,=1写第1组缓存，读第2组缓存；=2写第2组缓存，读第1组缓存
        private static object m_lockObject = new object();
        private List<SChartXYPoint>[] m_XYSerieBuffs_1 = new List<SChartXYPoint>[m_nChartCount * 3];    // 第一组缓存
        private List<SChartXYPoint>[] m_XYSerieBuffs_2 = new List<SChartXYPoint>[m_nChartCount * 3];    // 第二组缓存
        private int[] m_nCurveChannelIndex = null;
        private int m_nStep = 1;
        private Dictionary<int, List<bool>> m_bDictShowCurveFlags = new Dictionary<int, List<bool>>();
        private int m_nFirstVisibleChartIndex = -1;
        private int m_nFirstVisibleSerieIndex = -1;

        private DateTime m_StartDateTime;   // 采集数据的起始时刻
        private DateTime m_ZeroDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        /// <summary>
        /// 构造函数
        /// </summary>
        public ShowCurveControl()
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
        private void ShowCurveControl_Resize(object sender, EventArgs e)
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
                    m_bDictShowCurveFlags.Add(i, bListShowCurves);
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
                m_XYSerieBuffs_1[i] = new List<SChartXYPoint>();
                m_XYSerieBuffs_2[i] = new List<SChartXYPoint>();
            }

            for (int i = m_nShowChartCount * 3; i < m_nChartCount * 3; ++i)
            {
                m_XYSerieBuffs_1[i] = null;
                m_XYSerieBuffs_2[i] = null;
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
                m_AxCharts[i].SetAxisMinMax(0, -1, 1);
                m_AxCharts[i].SetAxisMinMax(1, 0, 10000);
            }

            UpdateAxisText();

            SamplingFreq eSamplingFre = MainConfig.GetInstance().GetSamplingFreq();
            if (eSamplingFre == SamplingFreq.FREQ_250 || eSamplingFre == SamplingFreq.FREQ_1 || eSamplingFre == SamplingFreq.FREQ_10)
            {
                m_nStep = 1;
            }
            else if (eSamplingFre == SamplingFreq.FREQ_500)
            {
                m_nStep = 1;
            }
            else if (eSamplingFre == SamplingFreq.FREQ_1000)
            {
                m_nStep = 2;
            }
            else if (eSamplingFre == SamplingFreq.FREQ_4000)
            {
                m_nStep = 8;
            }
        }

        /// <summary>
        /// 更新坐标轴
        /// </summary>
        public void UpdateAxisText()
        {
            int nYAxisUnit = MainConfig.GetInstance().GetYAxisUnit();
            string strYAxisUnit = "磁场(nT)";
            string strXAxisUnit = "时间(s)";
            if (nYAxisUnit == 1)
            {
                m_dYAxisUnitCoeff = 1.0;
            }
            else if (nYAxisUnit == 2)
            {
                m_dYAxisUnitCoeff = 1000;
                strYAxisUnit = "磁场(uT)";
            }
            for (int i = 0; i < m_AxCharts.Length; ++i)
            {
                if (m_AxCharts[i] != null)
                {
                    m_AxCharts[i].SetYAxisText(ref strYAxisUnit);
                    m_AxCharts[i].SetXAxisText(ref strXAxisUnit);
                }   
            }
        }

        public void SetStartTime(DateTime StartTime)
        {
            m_StartDateTime = StartTime;
        }

        /// <summary>
        /// 设置显示曲线时抽取数据点的间隔
        /// </summary>
        /// <param name="nStep">数据点间隔</param>
        public void SetStep(int nStep)
        {
            m_nStep = nStep;
        }

        /// <summary>
        /// 开始刷新数据
        /// </summary>
        public void StartRefresh()
        {
            this.timerRefreshCurve.Start();
        }

        /// <summary>
        /// 停止刷新数据
        /// </summary>
        public void StopRefresh()
        {
            RefreshCharts();    // 最后刷新一次曲线
            RefreshCharts();    // 这里之所以调用两次，是因为采用了双缓存，两次调用正好使用完所有数据
            this.timerRefreshCurve.Stop();
        }

        /// <summary>
        /// 接收到数据事件处理
        /// </summary>
        /// <param name="sMagDatas">接收到的磁场数据</param>
        public void OnDataReceived(MultiChannelMagData[] sMagDatas)
        {
            if (null == sMagDatas || sMagDatas.Length <= 0 || sMagDatas[0].m_dMagData.Length <= 0)
                return;

            double dOffsetMs = (m_StartDateTime - m_ZeroDateTime).TotalMilliseconds;

            lock (m_lockObject)
            {
                List<SChartXYPoint>[] pXYSerieBuffs = null;
                if (1 == m_nRWFlag)
                {
                    pXYSerieBuffs = m_XYSerieBuffs_1;
                }
                else if (2 == m_nRWFlag)
                {
                    pXYSerieBuffs = m_XYSerieBuffs_2;
                }        

                for (int i = 0; i < m_nShowChartCount; ++i)
                {
                    for (int j = 0; j < sMagDatas.Length; j+= m_nStep)
                    {
                        SChartXYPoint oXYPoint = new SChartXYPoint((sMagDatas[j].m_nTimeStamp / 1000.0 - dOffsetMs) / m_dXAxisUnitCoeff, sMagDatas[j].m_dMagData[m_nCurveChannelIndex[ 3 * i]] / m_dYAxisUnitCoeff);
                        pXYSerieBuffs[3 * i].Add(oXYPoint);

                        oXYPoint = new SChartXYPoint((sMagDatas[j].m_nTimeStamp / 1000.0 - dOffsetMs) / m_dXAxisUnitCoeff, sMagDatas[j].m_dMagData[m_nCurveChannelIndex[3 * i + 1]] / m_dYAxisUnitCoeff);
                        pXYSerieBuffs[3 * i + 1].Add(oXYPoint);

                        oXYPoint = new SChartXYPoint((sMagDatas[j].m_nTimeStamp / 1000.0 - dOffsetMs) / m_dXAxisUnitCoeff, sMagDatas[j].m_dMagData[m_nCurveChannelIndex[3 * i + 2]] / m_dYAxisUnitCoeff);
                        pXYSerieBuffs[3 * i + 2].Add(oXYPoint);
                    }               
                }
            }
        }

        /// <summary>
        /// 定时刷新曲线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerRefreshCurve_Tick(object sender, EventArgs e)
        {
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
            // 更新序列数据
            bool bUpdate = UpdateSerieData();
            if (!bUpdate)
                return;

            // 调整曲线坐标轴
            AdjustChartAxis();
        }

        /// <summary>
        /// 清空曲线数据
        /// </summary>
        public void ClearCharts()
        {
            // 加锁
            lock(m_lockObject)
            {
                // 清空缓存和曲线序列
                for (int i = 0; i < m_nShowChartCount; ++i)
                {
                    m_XYSerieBuffs_1[3 * i].Clear();
                    m_XYSerieBuffs_1[3 * i + 1].Clear();
                    m_XYSerieBuffs_1[3 * i + 2].Clear();
                    m_XYSerieBuffs_2[3 * i].Clear();
                    m_XYSerieBuffs_2[3 * i + 1].Clear();
                    m_XYSerieBuffs_2[3 * i + 2].Clear();

                    m_XYSerieBuffs_1[3 * i].Capacity = 100;
                    m_XYSerieBuffs_1[3 * i + 1].Capacity = 100;
                    m_XYSerieBuffs_1[3 * i + 2].Capacity = 100;
                    m_XYSerieBuffs_2[3 * i].Capacity = 100;
                    m_XYSerieBuffs_2[3 * i + 1].Capacity = 100;
                    m_XYSerieBuffs_2[3 * i + 2].Capacity = 100;

                    m_AxCharts[i].ClearSerie(m_nSerieIDs[3 * i]);
                    m_AxCharts[i].ClearSerie(m_nSerieIDs[3 * i + 1]);
                    m_AxCharts[i].ClearSerie(m_nSerieIDs[3 * i + 2]);

                    // 重置X坐标轴
                    m_AxCharts[i].SetAxisMinMax(1, 0, m_dBottomAxisRange);
                }   
            }
        }

        /// <summary>
        /// 更新序列的数据
        /// </summary>
        /// <returns>是否更新了数据</returns>
        private bool UpdateSerieData()
        {
            // 加锁
            lock (m_lockObject)
            {
                List<SChartXYPoint>[] pXYSerieBuffs = null;

                if (m_nRWFlag == 1)
                {// 读取第2组缓存
                    pXYSerieBuffs = m_XYSerieBuffs_2;
                    if (pXYSerieBuffs.Length <= 0 || pXYSerieBuffs[0].Count <= 0)
                    {
                        m_nRWFlag = 2;
                        return false;
                    }
                }
                else if (m_nRWFlag == 2)
                {// 读取第1组缓存
                    pXYSerieBuffs = m_XYSerieBuffs_1;
                    if (pXYSerieBuffs.Length <= 0 || pXYSerieBuffs[0].Count <= 0)
                    {
                        m_nRWFlag = 1;
                        return false;
                    }
                }

                for (int i = 0; i < m_nShowChartCount; ++i)
                {// 循环次数为显示出来的控件个数
                    for (int j = 0; j < pXYSerieBuffs[3 * i].Count; ++j)
                    {
                        m_AxCharts[i].AddPointToTempBuffer(m_nSerieIDs[3 * i], pXYSerieBuffs[3 * i][j].X, pXYSerieBuffs[3 * i][j].Y);
                    }
                    m_AxCharts[i].CopyTempPointsToLineSerie(m_nSerieIDs[3 * i]);

                    for (int j = 0; j < pXYSerieBuffs[3 * i + 1].Count; ++j)
                    {
                        m_AxCharts[i].AddPointToTempBuffer(m_nSerieIDs[3 * i + 1], pXYSerieBuffs[3 * i + 1][j].X, pXYSerieBuffs[3 * i + 1][j].Y);
                    }
                    m_AxCharts[i].CopyTempPointsToLineSerie(m_nSerieIDs[3 * i + 1]);

                    for (int j = 0; j < pXYSerieBuffs[3 * i + 2].Count; ++j)
                    {
                        m_AxCharts[i].AddPointToTempBuffer(m_nSerieIDs[3 * i + 2], pXYSerieBuffs[3 * i + 2][j].X, pXYSerieBuffs[3 * i + 2][j].Y);
                    }
                    m_AxCharts[i].CopyTempPointsToLineSerie(m_nSerieIDs[3 * i + 2]);

                    pXYSerieBuffs[3 * i].Clear();
                    pXYSerieBuffs[3 * i + 1].Clear();
                    pXYSerieBuffs[3 * i + 2].Clear();
                    pXYSerieBuffs[3 * i].Capacity = 1000;
                    pXYSerieBuffs[3 * i + 1].Capacity = 1000;
                    pXYSerieBuffs[3 * i + 2].Capacity = 1000;
                }

                // 切换读写缓存的标志
                if (m_nRWFlag == 1)
                {
                    m_nRWFlag = 2;
                }
                else if (m_nRWFlag == 2)
                {
                    m_nRWFlag = 1;
                }

                return true;
            }
        }

        /// <summary>
        /// 调整曲线坐标轴
        /// </summary>
        private void AdjustChartAxis()
        {
            // 加锁
            lock (m_lockObject)
            {
                // 找到第一个可见的Chart和Serie
                bool bFind = FindFirstVisibleChartAndSerie(ref m_nFirstVisibleChartIndex, ref m_nFirstVisibleSerieIndex);
                if (!bFind)
                    return;

                // 调整曲线X轴
                AdjustChartXAxis();

                // 调整曲线Y轴
                AdjustChartYAxis();
            }
        }

        /// <summary>
        /// 调整曲线X轴
        /// </summary>
        private void AdjustChartXAxis()
        {
            double dXMin = 0.0;
            double dXMax = 0.0;
            uint nRemoveCount = 0;      // 要移除的点数
            bool bResetBottomAxisFlag = false;  // 是否需要重置底轴范围的标志

            // 先从第一个可见的曲线移除点
            m_AxCharts[m_nFirstVisibleChartIndex].GetSerieXMinMax(m_nSerieIDs[3 * m_nFirstVisibleChartIndex + m_nFirstVisibleSerieIndex], ref dXMin, ref dXMax);
            while (dXMax - dXMin > m_dBottomAxisRange)
            {
                // 从曲线起始位置移除点
                m_AxCharts[m_nFirstVisibleChartIndex].RemovePointsFromBegin(m_nSerieIDs[3 * m_nFirstVisibleChartIndex + m_nFirstVisibleSerieIndex], 2);
                nRemoveCount += 2;
                bResetBottomAxisFlag = true;

                m_AxCharts[m_nFirstVisibleChartIndex].GetSerieXMinMax(m_nSerieIDs[3 * m_nFirstVisibleChartIndex + m_nFirstVisibleSerieIndex], ref dXMin, ref dXMax);
            }

            // 检查所有曲线，移除多余的点
            if (nRemoveCount > 0)
            {
                for (int i = 0; i < m_nShowChartCount; ++i)
                {
                    for (int j = 0; j < 3; ++j)
                    {
                        // 已经移除过点了，跳过
                        if (i == m_nFirstVisibleChartIndex && j == m_nFirstVisibleSerieIndex)
                            continue;

                        m_AxCharts[i].RemovePointsFromBegin(m_nSerieIDs[3 * i + j], nRemoveCount);
                    }

                    // LeftAxis = 0
                    // BottomAxis = 1
                    // RightAxis = 2
                    // TopAxis = 3
                    if (bResetBottomAxisFlag)
                    {
                        m_AxCharts[i].SetAxisMinMax(1, dXMin, dXMax);
                    }
                }
            }
        }

        /// <summary>
        /// 调整曲线Y轴
        /// </summary>
        private void AdjustChartYAxis()
        {
            for (int i = 0; i < m_nShowChartCount; ++i)
            {
                double dYMin = double.MaxValue;
                double dYMax = double.MinValue;
                double dYMinTmp = double.MaxValue;
                double dYMaxTmp = double.MinValue;
                for (int j = 0; j < 3; ++j)
                {
                    if (m_bDictShowCurveFlags[i][j])
                    {
                        m_AxCharts[i].GetSerieYMinMax(m_nSerieIDs[3 * i + j], ref dYMinTmp, ref dYMaxTmp);
                        if (dYMinTmp < dYMin)
                        {
                            dYMin = dYMinTmp;
                        }
                        if (dYMaxTmp > dYMax)
                        {
                            dYMax = dYMaxTmp;
                        }
                    }
                }

                double dYShowMin = dYMin;
                double dYShowMax = dYMax;
                CalcYAxisShowRange(dYMin, dYMax, ref dYShowMin, ref dYShowMax);
                m_AxCharts[i].SetAxisMinMax(0, dYShowMin, dYShowMax);
            }
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
                if (!m_bDictShowCurveFlags.ContainsKey(nChartIndex))
                    return;

                if (nSerieIndex < 0 || nSerieIndex > m_bDictShowCurveFlags[nChartIndex].Count)
                    return;

                // 更新是否可见
                m_bDictShowCurveFlags[nChartIndex][nSerieIndex] = bVisible;

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

                // 调整曲线Y轴
                AdjustChartYAxis();
            }
        }

        /// <summary>
        /// 找第一个可见的曲线控件索引和序列索引
        /// </summary>
        /// <param name="">曲线控件索引</param>
        /// <param name="">序列索引</param>
        /// <returns>是否找到</returns>
        private bool FindFirstVisibleChartAndSerie(ref int nFirstVisibleChartIndex, ref int nFirstVisibleSerieIndex)
        {
            for (int i = 0; i < m_nShowChartCount; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    if (m_bDictShowCurveFlags[i][j] == true)
                    {
                        nFirstVisibleChartIndex = i;
                        nFirstVisibleSerieIndex = j;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 获取显示曲线的状态
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, List<bool>> GetShowCurveFlag()
        {
            return m_bDictShowCurveFlags;
        }

        /// <summary>
        /// 计算Y轴显示范围
        /// </summary>
        /// <param name="dYMin">Y轴最小值</param>
        /// <param name="dYMax">Y轴最大值</param>
        /// <param name="dYShowMin">Y轴显示最小值</param>
        /// <param name="dYShowMax">Y轴显示最大值</param>
        private void CalcYAxisShowRange(double dYMin, double dYMax, ref double dYShowMin, ref double dYShowMax)
        {
            double dYRange = dYMax - dYMin;
            dYShowMin = dYMin - dYRange * 0.05;
            dYShowMax = dYMax + dYRange * 0.05;
        }

        private void ShowCurveControl_Load(object sender, EventArgs e)
        {
            this.UpdateAxisText();
        }

        private void ShowCurveControl_SizeChanged(object sender, EventArgs e)
        {
            this.UpdateAxisText();
        }
    }
}
