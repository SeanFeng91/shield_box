using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MCMAG30F
{
    /// <summary>
    /// 显示设置窗体
    /// </summary>
    public partial class SetDisplaySettingForm : Form
    {
        /// <summary>
        /// 显示模式
        /// </summary>
        private int m_nDisplayMode = 0;

        /// <summary>
        /// 曲线通道索引对应关系
        /// </summary>
        private int[] m_nCurveChannelIndexs = new int[30];

        /// <summary>
        /// 显示曲线通道数
        /// </summary>
        private int m_nShowCurveChannelCount = 30;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="nDisplayMode"></param>
        public SetDisplaySettingForm(int nDisplayMode)
        {
            m_nDisplayMode = nDisplayMode;
            InitializeComponent();

            InitDispalyModeUI();
        }

        /// <summary>
        /// 获取显示模式
        /// </summary>
        /// <returns></returns>
        public int GetDisplayMode()
        {
            return m_nDisplayMode;
        }

        /// <summary>
        /// 设置显示模式
        /// </summary>
        /// <param name="nDisplayMode"></param>
        public void SetDisplayMode(int nDisplayMode)
        {
            m_nDisplayMode = nDisplayMode;
        }

        /// <summary>
        /// 初始化显示模式UI
        /// </summary>
        private void InitDispalyModeUI()
        {
            switch (m_nDisplayMode)
            {
                case 0:
                    {
                        this.radioButtonDisplayWave.Checked = true;
                        break;
                    }
                case 1:
                    {
                        this.radioButtonDisplayData.Checked = true;
                        break;
                    }
                case 2:
                    {
                        this.radioButtonDisplayFFT.Checked = true;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        /// <summary>
        /// 更新显示模式
        /// </summary>
        private void UpdateDisplayMode()
        {
            if (this.radioButtonDisplayWave.Checked)
            {
                m_nDisplayMode = 0;
            }
            else if (this.radioButtonDisplayData.Checked)
            {
                m_nDisplayMode = 1;
            }
            else if (this.radioButtonDisplayFFT.Checked)
            {
                m_nDisplayMode = 2;
            }
        }

        /// <summary>
        /// 设置曲线通道索引
        /// </summary>
        public void SetCurveChannelIndexs()
        {
            MainConfig.GetInstance().SetCurveChannelIndex(m_nCurveChannelIndexs);
        }

        /// <summary>
        /// 设置显示曲线通道数
        /// </summary>
        public void SetShowCurveChannelCount()
        {
            MainConfig.GetInstance().SetShowCurveChannelCount(m_nShowCurveChannelCount);
        }

        /// <summary>
        /// “取消”按钮单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// “确定”按钮单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            UpdateDisplayMode();        // 更新显示模式
            SetShowCurveChannelCount(); // 设置显示曲线通道数
            SetCurveChannelIndexs();    // 更新曲线通道索引映射关系
            SetCurveAxisSetting();      // 设置曲线坐标轴设置

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// 设置曲线坐标轴设置
        /// </summary>
        private void SetCurveAxisSetting()
        {
            // 设置Y轴单位，X轴单位固定
            int nYAxisUnit = 1;
            if (this.radioButtonYAxis_uT.Checked == true)
            {
                nYAxisUnit = 2;
            }

            MainConfig.GetInstance().SetYAxisUnit(nYAxisUnit);
        }

        /// <summary>
        /// 验证单元格数值格式是否正确
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_CurveDeviceChannelMap_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (this.dataGridView_CurveDeviceChannelMap.CurrentCell != null)
            {
                int nColumnIndex = this.dataGridView_CurveDeviceChannelMap.CurrentCell.ColumnIndex;
                if (nColumnIndex == 1)
                {
                    int nValue = -1;
                    if (int.TryParse(e.FormattedValue.ToString(), out nValue))
                    {
                        e.Cancel = false;
                    }
                    else
                    {
                        e.Cancel = true;    // 数据格式不正确，则还原
                        this.dataGridView_CurveDeviceChannelMap.CancelEdit();
                    }
                }
            }
        }

        /// <summary>
        /// 单元格数值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_CurveDeviceChannelMap_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dataGridView_CurveDeviceChannelMap.CurrentCell != null)
            {
                // 当前行索引
                int nRowIndex = this.dataGridView_CurveDeviceChannelMap.CurrentCell.RowIndex;
                // 曲线索引
                string strCurveIndex = this.dataGridView_CurveDeviceChannelMap.Rows[nRowIndex].Cells[0].Value.ToString();
                int nCurveIndex = -1;
                int.TryParse(strCurveIndex, out nCurveIndex);
                // 通道索引
                string strChannelIndex = this.dataGridView_CurveDeviceChannelMap.Rows[nRowIndex].Cells[1].EditedFormattedValue.ToString();
                int nChannelIndex = -1;
                int.TryParse(strChannelIndex, out nChannelIndex);

                m_nCurveChannelIndexs[nCurveIndex] = nChannelIndex;
            }
        }

        /// <summary>
        /// 窗体加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetDisplaySettingForm_Load(object sender, EventArgs e)
        {
            MainConfig.GetInstance().GetCurveChannelIndex(out m_nCurveChannelIndexs);
            m_nShowCurveChannelCount = MainConfig.GetInstance().GetShowCurveChannelCount();

            this.comboBoxShowChannelCount.SelectedIndex = this.comboBoxShowChannelCount.Items.IndexOf(m_nShowCurveChannelCount.ToString());

            InitDataGridView_CurveDeviceChannelMap(m_nShowCurveChannelCount);

            // 曲线坐标轴设置
            int nYAxisUnit = MainConfig.GetInstance().GetYAxisUnit();
            int nXAxisUnit = MainConfig.GetInstance().GetXAxisUnit();
            this.radioButtonXAxis_S.Checked = true;
            if (nYAxisUnit == 1)
            {
                this.radioButtonYAxis_nT.Checked = true;
            }
            else
            {
                this.radioButtonYAxis_uT.Checked = true;
            }

        }

        /// <summary>
        /// 窗口布局
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetDisplaySettingForm_Layout(object sender, LayoutEventArgs e)
        {
            int nClientWidth = this.dataGridView_CurveDeviceChannelMap.ClientSize.Width;   // 窗体客户区宽度
            int nColWidth = 100;    // DataGridView两列的列宽
            bool bDisplayedAllRow = this.dataGridView_CurveDeviceChannelMap.DisplayedRowCount(false) == this.dataGridView_CurveDeviceChannelMap.RowCount;
            if (bDisplayedAllRow)
            {
                nColWidth = (nClientWidth - 5) / 2;
            }
            else
            {
                nColWidth = (nClientWidth - 30) / 2;
            }

            this.CurveIndex.Width = nColWidth;
            this.ChannelIndex.Width = nColWidth;
        }

        /// <summary>
        /// 显示通道数 选中项改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxShowChannelCount_SelectedIndexChanged(object sender, EventArgs e)
        {
            int.TryParse(this.comboBoxShowChannelCount.SelectedItem.ToString(), out m_nShowCurveChannelCount);
            InitDataGridView_CurveDeviceChannelMap(m_nShowCurveChannelCount);
        }

        /// <summary>
        /// 根据要显示的曲线通道数初始化DataGridView控件
        /// </summary>
        /// <param name="nShowChannelCount">要显示的曲线通道数</param>
        private void InitDataGridView_CurveDeviceChannelMap(int nShowChannelCount)
        {
            this.dataGridView_CurveDeviceChannelMap.Rows.Clear();

            for (int i = 0; i < nShowChannelCount; ++i)
            {
                DataGridViewRow row = new DataGridViewRow();
                // CurveIndex
                DataGridViewTextBoxCell textBoxCell1 = new DataGridViewTextBoxCell();
                textBoxCell1.Value = i.ToString();
                row.Cells.Add(textBoxCell1);

                // ChannelIndex
                DataGridViewTextBoxCell textBoxCell2 = new DataGridViewTextBoxCell();
                textBoxCell2.Value = m_nCurveChannelIndexs[i].ToString();
                row.Cells.Add(textBoxCell2);

                textBoxCell1.ReadOnly = true;
                textBoxCell2.ReadOnly = false;
                this.dataGridView_CurveDeviceChannelMap.Rows.Add(row);
            }
        }

        private void SetDisplaySettingForm_SizeChanged(object sender, EventArgs e)
        {
            //asc.controlAutoSize(this);
        }

        private void tableLayoutPanelXYUnit_Paint(object sender, PaintEventArgs e)
        {

        }

        private void labelXAxis_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanelCurveConfig_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanelMain_Paint(object sender, PaintEventArgs e)
        {

        }

        /// <summary>
        /// 快捷切换一组采集器的通道
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSwitchShowChannel_Click(object sender, EventArgs e)
        {
            int nChannelCount = MainConfig.GetInstance().GetValidChannelCountPerDevice();
            int nDeviceIndex = -1;
            bool bRet = int.TryParse(this.textBoxDeviceIndex.Text, out nDeviceIndex);
            if (!bRet)
            {
                return;
            }

            this.comboBoxShowChannelCount.SelectedIndex = this.comboBoxShowChannelCount.Items.IndexOf(nChannelCount.ToString());

            for (int i = 0; i < nChannelCount; ++i)
            {
                m_nCurveChannelIndexs[i] = (nDeviceIndex - 1) * nChannelCount + i;
            }

            InitDataGridView_CurveDeviceChannelMap(nChannelCount);
        }
    }
}
