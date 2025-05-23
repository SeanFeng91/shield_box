using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using MCMag30FDevice;
using PLC;

namespace MCMAG30F
{
    public partial class ShowDataControl : UserControl
    {
        private int m_nRWFlag = 1;  // 切换读写缓存的标志,=1写第1组缓存，读第2组缓存；=2写第2组缓存，读第1组缓存
        private static object m_lockObject = new object();
        private List<double>[] m_dMagDataBuffs_1 = null;    // 第一组缓存
        private List<double>[] m_dMagDataBuffs_2 = null;    // 第二组缓存
        private int m_nChannelCount = 0;                    // 通道数
        private double m_dMagUnitCoeff = 1;                 // 磁场单位系数

        private double[] m_dAvgMultiChannelMagData = new double[300];
        private Dictionary<int, bool> m_DictAbnormalChannels = new Dictionary<int, bool>();
        private bool m_bExistAbnormalState = false;

        private bool m_bNeedCheckData = false;  // 是否需要检查数据是否异常
        private double[] m_dMagValueRanges = new double[90];  // 有效数据范围
        private bool[] m_bMagValueValid = new bool[300];    // 磁场数据是否有效

        private string m_strDataFileName;   // 保存数据文件名
        private bool m_bWriteHeaderFlag = false; // 是否写了文件头的标志
        private int m_nWriteNum = 0;        // 写入次数
        private string m_strDataSavePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Data"); // 数据保存路径
        private string m_strAbnormalDataFileName;   // 保存异常数据的文件名
        private bool m_bWriteAbnormalHeaderFlag = false;    // 是否写了异常数据文件头的标志

        ///////////////////////////////////////////
        //private FileStream m_fsDataFile = null;
        ///////////////////////////////////////////

        /// <summary>
        /// 有异常数据时的事件处理器
        /// </summary>
        public delegate void AbnormalStateEventHandler();

        public event AbnormalStateEventHandler AbnormalStateEvent;


        /// <summary>
        /// 构造函数
        /// </summary>
        public ShowDataControl()
        {
            InitializeComponent();

            initListView();

            m_strDataFileName = "SnapShotData_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
            m_strAbnormalDataFileName = "AbnormalData_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";

            for (int i = 0; i < 90; ++i)
            {
                m_dMagValueRanges[i] = Double.MaxValue;
                m_DictAbnormalChannels[i] = false;
            }

            double[] dAbnormalValueRanges = MainConfig.GetInstance().GetMagValueRanges();
            if (dAbnormalValueRanges != null && dAbnormalValueRanges.Length > 0)
            {
                this.textBoxAbnormalValueRange.Text = dAbnormalValueRanges[0].ToString();
            }
        }

        /// <summary>
        /// 初始化缓存数据结构
        /// </summary>
        public void InitData()
        {
            //int nUsedChannelCount = MainConfig.GetInstance().GetUsedChannelCount(); // 实际使用的通道个数

            int nUsedChannelCount = 90; // 固定为90，如果只使用30或者60通道，后面的冗余。
            m_dMagDataBuffs_1 = new List<double>[nUsedChannelCount];
            m_dMagDataBuffs_2 = new List<double>[nUsedChannelCount];

            for (int i = 0; i < nUsedChannelCount; ++i)
            {
                m_dMagDataBuffs_1[i] = new List<double>();
                m_dMagDataBuffs_2[i] = new List<double>();
            }

            // 更新表头
            UpdateColumnHeadText();
        }

        /// <summary>
        /// 更新表头
        /// </summary>
        public void UpdateColumnHeadText()
        {
            int nYAxisUnit = MainConfig.GetInstance().GetYAxisUnit();
            if (nYAxisUnit == 1)
            {
                m_dMagUnitCoeff = 1.0;
                this.columnHeaderX1.Text = "X(nT)";
                this.columnHeaderY1.Text = "Y(nT)";
                this.columnHeaderZ1.Text = "Z(nT)";
                this.columnHeaderT1.Text = "总场(nT)";

                this.columnHeaderMagValue2.Text = "磁场值(nT)";
            }
            else if (nYAxisUnit == 2)
            {
                m_dMagUnitCoeff = 1000.0;
                this.columnHeaderX1.Text = "X(uT)";
                this.columnHeaderY1.Text = "Y(uT)";
                this.columnHeaderZ1.Text = "Z(uT)";
                this.columnHeaderT1.Text = "总场(uT)";

                this.columnHeaderMagValue2.Text = "磁场值(uT)";
            }
        }

        /// <summary>
        /// 初始化列表
        /// </summary>
        private void initListView()
        {
            int nHeight = this.Height / 11;
            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(1, nHeight);

            this.listViewDevice1.SmallImageList = imgList;
            this.listViewDevice2.SmallImageList = imgList;

            this.listViewDevice1.View = View.Details;

            for (int i = 0; i < 16; ++i)
            {
                ListViewItem lvi1 = new ListViewItem();
                lvi1.Text = (i + 1).ToString();
                ListViewItem.ListViewSubItem lvi1_index = new ListViewItem.ListViewSubItem();
                lvi1_index.Text = (i + 1).ToString();
                lvi1.SubItems.Add(lvi1_index);
                ListViewItem.ListViewSubItem lvi1_x = new ListViewItem.ListViewSubItem();
                lvi1_x.Text = "-";
                lvi1.SubItems.Add(lvi1_x);
                ListViewItem.ListViewSubItem lvi1_y = new ListViewItem.ListViewSubItem();
                lvi1_y.Text = "-";
                lvi1.SubItems.Add(lvi1_y);
                ListViewItem.ListViewSubItem lvi1_z = new ListViewItem.ListViewSubItem();
                lvi1_z.Text = "-";
                lvi1.SubItems.Add(lvi1_z);
                ListViewItem.ListViewSubItem lvi1_t = new ListViewItem.ListViewSubItem();
                lvi1_t.Text = "-";
                lvi1.SubItems.Add(lvi1_t);
                this.listViewDevice1.Items.Add(lvi1);
            }
        }

        /// <summary>
        /// 开始刷新数据
        /// </summary>
        public void StartRefresh()
        {
            this.timerRefreshData.Start();

            //setZerobutton.Enabled = true;
            //cancelZerobutton.Enabled = true;
            //saveDatabutton.Enabled = true;
        }

        /// <summary>
        /// 停止刷新数据
        /// </summary>
        public void StopRefresh()
        {
            this.timerRefreshData.Stop();

            //setZerobutton.Enabled = false;
            //cancelZerobutton.Enabled = false;
            //saveDatabutton.Enabled = false;
        }

        /// <summary>
        /// 接收到数据事件处理
        /// </summary>
        /// <param name="sMagDatas">接收到的磁场数据</param>
        public void OnDataReceived(MultiChannelMagData[] sMagDatas)
        {
            if (null == sMagDatas || sMagDatas.Length <= 0 || sMagDatas[0].m_dMagData.Length <= 0)
                return;

            if (m_nChannelCount != sMagDatas[0].m_dMagData.Length)
            {
                m_nChannelCount = sMagDatas[0].m_dMagData.Length;
            }

            lock(m_lockObject)
            {
                List<double>[] pMagDataBuffs = null;
                if (1 == m_nRWFlag)
                {
                    pMagDataBuffs = m_dMagDataBuffs_1;
                }
                else if (2 == m_nRWFlag)
                {
                    pMagDataBuffs = m_dMagDataBuffs_2;
                }

                for (int i = 0; i < sMagDatas.Length; ++i)
                {
                    for (int j = 0; j < sMagDatas[i].m_dMagData.Length; ++j)
                    {
                        pMagDataBuffs[j].Add(sMagDatas[i].m_dMagData[j] / m_dMagUnitCoeff);
                    }
                }
            }
        }

        /// <summary>
        /// 设置零偏
        /// </summary>
        public void SetZeroOffset()
        {
            DataBuffer.GetInstance().SetZeroOffset();
        }

        /// <summary>
        /// 取消零偏
        /// </summary>
        public void CancelZeroOffset()
        {
            DataBuffer.GetInstance().CancelZeroOffset();
        }

        /// <summary>
        /// 设置是否需要检查数据是否异常
        /// </summary>
        /// <param name="bNeed"></param>
        public void SetNeedCheckData(bool bNeed)
        {
            m_bNeedCheckData = bNeed;
        }

        /// <summary>
        /// 设置有效数据值范围
        /// </summary>
        /// <param name="dValueRange"></param>
        public void SetMagValueRanges(double[] dValueRanges)
        {
            m_dMagValueRanges = dValueRanges;
        }

        /// <summary>
        /// 检查数据是否异常
        /// </summary>
        public void CheckMagValue()
        {
            if (!m_bNeedCheckData)
            {
                return;
            }

            if (!DataBuffer.GetInstance().GetSetZeroFlag())
            {
                return;
            }

            for (int i = 0; i < m_nChannelCount; ++i)
            {
                if (Math.Abs(m_dAvgMultiChannelMagData[i]) * m_dMagUnitCoeff > Math.Abs(m_dMagValueRanges[i]))
                {
                    m_DictAbnormalChannels[i] = true;
                    m_bExistAbnormalState = true;
                }
                else
                {
                    m_DictAbnormalChannels[i] = false;
                }
            }
        }

        /// <summary>
        /// 定时刷新数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerRefreshData_Tick(object sender, EventArgs e)
        {
            RefreshDatas();
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        public void RefreshDatas()
        {
            int nUsedChannelCount = MainConfig.GetInstance().GetUsedChannelCount(); // 使用通道数

            this.listViewDevice1.BeginUpdate(); // 开始更新
            if (nUsedChannelCount > 30)
            {
                this.listViewDevice2.BeginUpdate();
            }

            UpdateDatas();  // 更新数据

            this.listViewDevice1.EndUpdate();
            if (nUsedChannelCount > 30)
            {
                this.listViewDevice2.EndUpdate();
            }
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        private void UpdateDatas()
        {
            // 加锁
            lock(m_lockObject)
            {
                List<double>[] pMagDataBuffs = null;

                if (m_nRWFlag == 1)
                {// 读取第2组缓存
                    pMagDataBuffs = m_dMagDataBuffs_2;
                    if (pMagDataBuffs.Length <= 0 || pMagDataBuffs[0].Count <= 0)
                    {
                        m_nRWFlag = 2;
                        return;
                    }
                }
                else if (m_nRWFlag == 2)
                {// 读取第1组缓存
                    pMagDataBuffs = m_dMagDataBuffs_1;
                    if (pMagDataBuffs.Length <= 0 || pMagDataBuffs[0].Count <= 0)
                    {
                        m_nRWFlag = 1;
                        return;
                    }
                }

                // 计算每个通道的平均值
                for (int i = 0; i < pMagDataBuffs.Length; ++i)
                {
                    if (pMagDataBuffs[i].Count > 0)
                    {
                        double dSum = 0;
                        for (int j = 0; j < pMagDataBuffs[i].Count; ++j)
                        {
                            dSum += pMagDataBuffs[i][j];
                        }

                        m_dAvgMultiChannelMagData[i] = dSum / pMagDataBuffs[i].Count;
                    }
                    else
                    {
                        m_dAvgMultiChannelMagData[i] = 0;
                    }

                    pMagDataBuffs[i].Clear();
                    pMagDataBuffs[i].Capacity = 100;
                }

                // 显示数据
                ShowDataInListView(m_dAvgMultiChannelMagData);

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
        }

        /// <summary>
        ///  在ListView控件中显示数据
        /// </summary>
        /// <param name="pAvgMultiChannelMagData"></param>
        private void ShowDataInListView(double[] pAvgMultiChannelMagData)
        {
            // 在ListView1中的显示
            int nList1RowCount = m_nChannelCount / 3;
            for (int i = 0; i < nList1RowCount; ++i)
            {
                double dTemp = 0.0;
                for (int j = 0; j < 3; ++j)
                {
                    this.listViewDevice1.Items[i].SubItems[j + 2].Text = pAvgMultiChannelMagData[i * 3 + j].ToString("f2");

                    dTemp += pAvgMultiChannelMagData[i * 3 + j] * pAvgMultiChannelMagData[i * 3 + j];
                }

                dTemp = Math.Sqrt(dTemp);
                this.listViewDevice1.Items[i].SubItems[5].Text = dTemp.ToString("f2");
            }

            // 检查数据是否存在异常
            CheckMagValue();

            // 处理异常数据
            DateTime curDateTime = DateTime.Now;
            for (int i = 0; i < m_nChannelCount; ++i)
            {
                int nRow = i / 3;
                int nCol = i % 3;
                if (m_DictAbnormalChannels[i])
                {
                    // 将ListViewDevice1中的指定单元格变红
                    this.listViewDevice1.Items[nRow].SubItems[nCol + 2].ForeColor = Color.Red;
                    Font f = this.listViewDevice1.Items[nRow].SubItems[nCol + 2].Font;
                    Font fBold = new Font(f.FontFamily, f.Size, FontStyle.Bold);
                    this.listViewDevice1.Items[nRow].SubItems[nCol + 2].Font = fBold;

                    // 添加一条异常数据
                    addOneAbnoramlData(curDateTime, i, m_dAvgMultiChannelMagData[i]);
                    // 保存一条异常数据到文件中
                    saveOneAbnormalData(curDateTime, i, m_dAvgMultiChannelMagData[i]);
                }
                else
                {
                    this.listViewDevice1.Items[nRow].SubItems[nCol + 2].ForeColor = Color.Black;
                    Font f = this.listViewDevice1.Items[nRow].SubItems[nCol + 2].Font;
                    Font fRegular = new Font(f.FontFamily, f.Size, FontStyle.Regular);
                    this.listViewDevice1.Items[nRow].SubItems[nCol + 2].Font = fRegular;
                }
            }

            if (m_bExistAbnormalState && AbnormalStateEvent != null)
            {
                AbnormalStateEvent();
            }

            // 清空异常缓存
            for (int i = 0; i < m_nChannelCount; ++i)
            {
                m_DictAbnormalChannels[i] = false;
            }
            m_bExistAbnormalState = false;
        }

        /// <summary>
        /// 添加一条异常数据到异常表中
        /// </summary>
        /// <param name="sDateTime"></param>
        /// <param name="nChannelIndex"></param>
        /// <param name="dValue"></param>
        private void addOneAbnoramlData(DateTime sDateTime, int nChannelIndex, double dValue)
        {
            string strDateTime = sDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            string sSensor = ((nChannelIndex / 3) + 1).ToString();

            int nDir = (nChannelIndex % 3);
            string sChannel = "";
            switch (nDir)
            {
                case 0:
                    sChannel = "X";
                    break;
                case 1:
                    sChannel = "Y";
                    break;
                case 2:
                    sChannel = "Z";
                    break;
            }

            ListViewItem lvi2 = new ListViewItem();
            lvi2.Text = strDateTime;

            ListViewItem.ListViewSubItem lvi2_datetime = new ListViewItem.ListViewSubItem();
            lvi2_datetime.Text = strDateTime;
            lvi2.SubItems.Add(lvi2_datetime);

            ListViewItem.ListViewSubItem lvi2_sensor = new ListViewItem.ListViewSubItem();
            lvi2_sensor.Text = sSensor;
            lvi2.SubItems.Add(lvi2_sensor);

            ListViewItem.ListViewSubItem lvi2_channel = new ListViewItem.ListViewSubItem();
            lvi2_channel.Text = sChannel;
            lvi2.SubItems.Add(lvi2_channel);

            ListViewItem.ListViewSubItem lvi2_value = new ListViewItem.ListViewSubItem();
            lvi2_value.Text = dValue.ToString("f2");
            lvi2.SubItems.Add(lvi2_value);

            this.listViewDevice2.Items.Insert(0, lvi2);

            // 超过了最大条数，就移除最旧的一条记录
            if (this.listViewDevice2.Items.Count > MainConfig.GetInstance().GetAbnormalDataMaxShowCount())
            {
                this.listViewDevice2.Items.RemoveAt(this.listViewDevice2.Items.Count - 1);
            }
        }

        /// <summary>
        /// 保存一条异常数据到文件中
        /// </summary>
        /// <param name="sDateTime"></param>
        /// <param name="nChannelIndex"></param>
        /// <param name="dValue"></param>
        private void saveOneAbnormalData(DateTime sDateTime, int nChannelIndex, double dValue)
        {
            if (!Directory.Exists(m_strDataSavePath))
            {
                Directory.CreateDirectory(m_strDataSavePath);
            }

            FileStream fileStream = new FileStream(m_strDataSavePath + "/" + m_strAbnormalDataFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8);

            if (m_bWriteAbnormalHeaderFlag == false)
            {
                string strHeaderLine = "时间（年月日时分秒）,传感器,通道,磁场值(nT)\n";
                streamWriter.Write(strHeaderLine);
                m_bWriteAbnormalHeaderFlag = true;
            }

            string sSensor = ((nChannelIndex / 3) + 1).ToString();

            int nDir = (nChannelIndex % 3);
            string sChannel = "";
            switch (nDir)
            {
                case 0:
                    sChannel = "X";
                    break;
                case 1:
                    sChannel = "Y";
                    break;
                case 2:
                    sChannel = "Z";
                    break;
            }

            string strLine = sDateTime.ToString("yyyyMMdd-HHmmss") + "," + sSensor + "," + sChannel + "," + dValue.ToString("+000000.0;-000000.0") + "\n";
            streamWriter.Write(strLine);
            streamWriter.Flush();
            streamWriter.Close();
        }

        /// <summary>
        /// 窗体尺寸变化，重新调整ListView的布局
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowDataControl_SizeChanged(object sender, EventArgs e)
        {
            if (this.Height > 0)
            {
                int nHeight = this.Height / 19 - 2;

                ImageList imgList = new ImageList();
                imgList.ImageSize = new Size(1, nHeight);

                this.listViewDevice1.SmallImageList = imgList;
                this.listViewDevice2.SmallImageList = imgList;
            }
           
            if (this.Width > 0)
            {
                this.listViewDevice1.Columns[0].Width = 0;
                this.listViewDevice1.Columns[1].Width = this.Width / 10;
                for (int i = 2; i < this.listViewDevice1.Columns.Count; ++i)
                {
                    this.listViewDevice1.Columns[i].Width = (int)((double)this.Width / 100.0 * 22.5) - 5;
                }

                this.listViewDevice2.Columns[0].Width = 0;
                for (int i = 1; i < this.listViewDevice2.Columns.Count; ++i)
                {
                    this.listViewDevice2.Columns[i].Width = (int)((double)this.Width / 4) - 5;
                }
            }
        }

        private void listViewDevice1_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            if (e.ColumnIndex == 0)
                e.Cancel = true;
        }

        private void listViewDevice1_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (this.listViewDevice1.Columns[0].Width != 0)
            {
                this.listViewDevice1.Columns[0].Width = 0;
            }
        }

        private void listViewDevice2_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            if (e.ColumnIndex == 0)
                e.Cancel = true;
        }

        private void listViewDevice2_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (this.listViewDevice2.Columns[0].Width != 0)
            {
                this.listViewDevice2.Columns[0].Width = 0;
            }
        }

        /// <summary>
        /// 归零
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setZerobutton_Click(object sender, EventArgs e)
        {
            SetZeroOffset();
            m_bNeedCheckData = true;
        }

        /// <summary>
        /// 取消归零
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelZerobutton_Click(object sender, EventArgs e)
        {
            CancelZeroOffset();
            m_bNeedCheckData = false;
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveDatabutton_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(m_strDataSavePath))
            {
                Directory.CreateDirectory(m_strDataSavePath);
            }

            FileStream fileStream = new FileStream(m_strDataSavePath + "/" + m_strDataFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8);

            int nValidChannelCountPerDevice = MainConfig.GetInstance().GetValidChannelCountPerDevice();
            int nDeviceCount = MainConfig.GetInstance().GetDeviceCount();
            int nTotalChannel = nValidChannelCountPerDevice * nDeviceCount;

            if (m_bWriteHeaderFlag == false)
            {
                string strHeaderLine = "时间（年月日时分秒）,样点数";
                for (int i = 0; i < nTotalChannel; ++i)
                {
                    strHeaderLine = strHeaderLine + ",Channel" + (i + 1).ToString();
                }
                strHeaderLine = strHeaderLine + "\n";

                streamWriter.Write(strHeaderLine);
                m_bWriteHeaderFlag = true;
            }
            
            string strLine = DateTime.Now.ToString("yyyyMMdd-HHmmss") + "," + (++m_nWriteNum).ToString("00") + ",";
            for (int i = 0; i < nTotalChannel; i++)
            {
                strLine += m_dAvgMultiChannelMagData[i].ToString("+000000.0;-000000.0");
                strLine += ",";
            }
            streamWriter.Write(strLine + "\n");
            streamWriter.Flush();
            streamWriter.Close();
        }

        /// <summary>
        /// 设置数据保存路径
        /// </summary>
        /// <param name="strDataFilePath"></param>
        public void SetDataSavePath(string strDataFilePath)
        {
            m_strDataSavePath = strDataFilePath;
        }

        /// <summary>
        /// 设置异常阈值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSetAbnormalValueRange_Click(object sender, EventArgs e)
        {
            string strValue = this.textBoxAbnormalValueRange.Text;
            double dValue = 0;
            bool bRet = double.TryParse(strValue, out dValue);
            if (bRet)
            {
                for (int i = 0; i < 90; ++i)
                {
                    MainConfig.GetInstance().SetMagValueRange(i, dValue);
                }

                double[] dMagValueRanges = MainConfig.GetInstance().GetMagValueRanges();
                SetMagValueRanges(dMagValueRanges);
            }
        }

        /// <summary>
        /// 只允许输入浮点数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxAbnormalValueRange_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // 只允许一个小数点
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// 设置PLC状态字符串
        /// </summary>
        /// <param name="strStatus"></param>
        public void SetPLCStatusString(string strStatus)
        {
            if (strStatus.Contains("断开连接") || strStatus.Contains("采集0"))
            {
                this.labelPLCStatus.ForeColor = Color.Red;
            }
            else
            {
                this.labelPLCStatus.ForeColor = Color.Black;
            }

            this.labelPLCStatus.Text = strStatus;       
        }

        public void OnPLCStatusChangedEventHandler(object sender)
        {
            this.Invoke(new Action(() =>
            {
                try
                {
                    S7_200 pPLC = (S7_200)sender;
                    if (pPLC != null)
                    {
                        string strStatus = pPLC.GetStatusString();
                        SetPLCStatusString(strStatus);
                    }     
                }
                catch (Exception ex)
                {

                }
            }));        
        }
    }
}
