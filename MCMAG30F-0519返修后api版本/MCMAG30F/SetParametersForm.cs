using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MCMag30FDevice;

namespace MCMAG30F
{
    public partial class SetParametersForm : Form
    {
        private float[] m_fSs = null;     // 传感器的比例因子

        private ADCMode m_eADCMode = ADCMode.AC;    // AC/DC Mode
        private ChanMode m_eChanMode = ChanMode.SINGLE_END; // Chan-Mode
        private int m_nUsedChannelCount = 0;    // 使用的通道数
        private SamplingMode m_eSamplingMode = SamplingMode.CONTINUOUS; // 采样模式，单次/连续
        private SamplingFreq m_eSamplingFreq = SamplingFreq.FREQ_250;   // 采样频率
        private float m_fSamplingDuration = 0; // 采样时间
        private bool m_bLowPassFilterEnabled = false;   // 是否使能低通滤波
        private float m_dLowPassFilterPassFreq = 0;                // 低通滤波通带频率
        private float m_dLowPassFilterStopFreq = 0;                // 低通滤波阻带频率
        private float m_dLowPassFilterPassDB = 0;                  // 低通滤波通带衰减
        private float m_dLowPassFilterStopDB = 0;                  // 低通滤波阻带衰减

        private TimingType m_eTimingType = 0;                 // 授时类型

        private bool m_bIsNeedSavaData = true;         // 是否需要保存数据
        private string m_strDataSavePath = null;       // 数据保存路径

        /// <summary>
        /// 构造函数
        /// </summary>
        public SetParametersForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetParametersForm_Load(object sender, EventArgs e)
        {
            int nGroupCount = MainConfig.GetInstance().GetGroupCount(); // 传感器组数
            // 从配置文件中读取传感器的比例因子Si
            m_fSs = new float[nGroupCount];
            for (int nIndex = 0; nIndex < nGroupCount; ++nIndex)
            {
                MainConfig.GetInstance().GetSensorS(nIndex, ref m_fSs[nIndex]);
            }

            // 初始化传感器的比例因子Si
            InitSensorS(m_fSs);

            // AC/DC Mode
            m_eADCMode = MainConfig.GetInstance().GetADCMode();
            InitADCModeComboBox(m_eADCMode);

            // 初始化可选的通道数
            InitChannelCount();

            // 初始化采样设置
            InitSamplingSetting();

            // 初始化是否滤波的设置
            InitFilterSetting();

            // 初始化授时
            InitTiming();

            // 数据保存路径
            m_bIsNeedSavaData = MainConfig.GetInstance().GetIsNeedSaveData();
            m_strDataSavePath = MainConfig.GetInstance().GetDataSavePath();
            this.checkBoxNeedSaveData.Checked = m_bIsNeedSavaData;
            this.buttonSelectDataSavePath.Enabled = m_bIsNeedSavaData;
            this.textBoxDataSavePath.Text = m_strDataSavePath;
        }

        /// <summary>
        /// 初始化传感器的比例因子Si
        /// </summary>
        /// <param name="fSs">传感器的比例因子Si</param>
        private void InitSensorS(float[] fSs)
        {
            if (null == fSs)
            {
                return;
            }

            for (int i = 0; i < fSs.Length; ++i)
            {
                DataGridViewRow row = new DataGridViewRow();
                // ChannelIndex
                DataGridViewTextBoxCell textBoxCell1 = new DataGridViewTextBoxCell();
                textBoxCell1.Value = i.ToString();
                row.Cells.Add(textBoxCell1);

                // Si
                DataGridViewTextBoxCell textBoxCell2 = new DataGridViewTextBoxCell();
                textBoxCell2.Value = fSs[i].ToString();
                row.Cells.Add(textBoxCell2);

                textBoxCell1.ReadOnly = true;
                textBoxCell2.ReadOnly = false;
                this.dataGridView_SensorParams.Rows.Add(row);
            }
        }


        /// <summary>
        /// 初始化可选的通道数
        /// </summary>
        private void InitChannelCount()
        {
            int nTotalChannelCount = MainConfig.GetInstance().GetChannelCount();
            m_nUsedChannelCount = MainConfig.GetInstance().GetUsedChannelCount();
            int nStepCount = 30;
            int nSelectedIndex = 0;
            for (int i = nStepCount; i <= nTotalChannelCount; i += nStepCount)
            {
                int nIndex = this.comboBoxChannelCount.Items.Add(i.ToString());
                if (i == m_nUsedChannelCount)
                {
                    nSelectedIndex = nIndex;
                }
            }

            this.comboBoxChannelCount.SelectedIndex = nSelectedIndex;
        }

        /// <summary>
        /// 初始化AC/DC Mode组合框
        /// </summary>
        /// <param name="eADCMode">AC/DC Mode</param>
        private void InitADCModeComboBox(ADCMode eADCMode)
        {
             switch (eADCMode)
            {
                case ADCMode.AC:
                    {
                        this.comboBoxADC.SelectedIndex = (int)ADCMode.AC - 1;
                        //this.comboBoxADC.SelectedText = "AC";
                        break;
                    }
                case ADCMode.DC:
                    {
                        this.comboBoxADC.SelectedIndex = (int)ADCMode.DC - 1;
                        //this.comboBoxADC.SelectedText = "DC";
                        break;
                    }
                default:
                    {
                        this.comboBoxADC.SelectedIndex = (int)ADCMode.AC - 1;
                        //this.comboBoxADC.SelectedText = "AC";
                        break;
                    }
            }
        }
    
        /// <summary>
        /// 初始化采样设置
        /// </summary>
        private void InitSamplingSetting()
        {
            m_eSamplingMode = MainConfig.GetInstance().GetSamplingMode();
            m_eSamplingFreq = MainConfig.GetInstance().GetSamplingFreq();
            m_fSamplingDuration = MainConfig.GetInstance().GetSamplingDuration();

            switch (m_eSamplingMode)
            {
                case SamplingMode.SINGLE:
                    {
                        this.radioButtonACQModeSingle.Checked = true;
                        break;
                    }
                case SamplingMode.CONTINUOUS:
                    {
                        this.radioButtonACQModeContinuous.Checked = true;
                        break;
                    }
                default:
                    {
                        this.radioButtonACQModeContinuous.Checked = true;
                        break;
                    }
            }

            switch (m_eSamplingFreq)
            {
                case SamplingFreq.FREQ_1:
                    {
                        this.comboBoxSamplingFreq.SelectedIndex = 0;
                        break;
                    }
                case SamplingFreq.FREQ_10:
                    {
                        this.comboBoxSamplingFreq.SelectedIndex = 1;
                        break;
                    }
                case SamplingFreq.FREQ_250:
                    {
                        this.comboBoxSamplingFreq.SelectedIndex = 2;
                        break;
                    }
                case SamplingFreq.FREQ_500:
                    {
                        this.comboBoxSamplingFreq.SelectedIndex = 3;
                        break;
                    }
                case SamplingFreq.FREQ_1000:
                    {
                        this.comboBoxSamplingFreq.SelectedIndex = 4;
                        break;
                    }
                case SamplingFreq.FREQ_4000:
                    {
                        this.comboBoxSamplingFreq.SelectedIndex = 5;
                        break;
                    }
                default:
                    {
                        this.comboBoxSamplingFreq.SelectedIndex = 2;
                        break;
                    }
            }

            string strSamplingDuration = m_fSamplingDuration.ToString();
            this.textBoxSamplingDuration.Text = strSamplingDuration;
        }

        /// <summary>
        /// 初始化滤波设置
        /// </summary>
        private void InitFilterSetting()
        {
            m_bLowPassFilterEnabled = MainConfig.GetInstance().IsLowPassFilterEnabled();

            if (m_bLowPassFilterEnabled)
            {
                this.checkBoxLowPass.Checked = true;
            }
            else
            {
                this.checkBoxLowPass.Checked = false;
            }

            MainConfig.GetInstance().GetLowPassFilterParameters(ref m_dLowPassFilterPassFreq, ref m_dLowPassFilterStopFreq,
                ref m_dLowPassFilterPassDB, ref m_dLowPassFilterStopDB);
            this.textBoxPassFreq.Text = m_dLowPassFilterPassFreq.ToString();
            this.textBoxStopFreq.Text = m_dLowPassFilterStopFreq.ToString();
            this.textBoxPassDB.Text = m_dLowPassFilterPassDB.ToString();
            this.textBoxStopDB.Text = m_dLowPassFilterStopDB.ToString();
        }

        /// <summary>
        /// 初始化授时
        /// </summary>
        private void InitTiming()
        {
            m_eTimingType = MainConfig.GetInstance().GetTimingType();
            if (m_eTimingType == TimingType.TIMINGTYPE_PC)
            {
                this.comboBoxTiming.SelectedIndex = 0;
            }
            else
            {
                this.comboBoxTiming.SelectedIndex = 1;
            }
        }

        /// <summary>
        /// 验证单元格数值格式是否正确
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_SensorParams_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (this.dataGridView_SensorParams.CurrentCell != null)
            {
                int nColumnIndex = this.dataGridView_SensorParams.CurrentCell.ColumnIndex;
                if (nColumnIndex == 1)
                {
                    float fValue = 0.0f;
                    if (float.TryParse(e.FormattedValue.ToString(), out fValue))
                    {
                        if (Math.Abs(fValue) <= float.Epsilon)
                        {
                            e.Cancel = true;    // 第1列比例因子不能为零
                            this.dataGridView_SensorParams.CancelEdit();

                            return;
                        }

                        e.Cancel = false;
                    }
                    else
                    {
                        e.Cancel = true;    // 数据格式不正确，则还原
                        this.dataGridView_SensorParams.CancelEdit();
                    }
                }
            }
        }

        /// <summary>
        /// 单元格数值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_SensorParams_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dataGridView_SensorParams.CurrentCell != null)
            {
                // 当前行索引
                int nRowIndex = this.dataGridView_SensorParams.CurrentCell.RowIndex;
                // 通道号
                string strChannel = this.dataGridView_SensorParams.Rows[nRowIndex].Cells[0].Value.ToString();
                int nChannel = 0;
                int.TryParse(strChannel, out nChannel);
                // 比例因子
                string strScaling = this.dataGridView_SensorParams.Rows[nRowIndex].Cells[1].EditedFormattedValue.ToString();
                float.TryParse(strScaling, out m_fSs[nChannel]);
            }
        }

        /// <summary>
        /// 选择数据保存路径按钮单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSelectDataSavePath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.RootFolder = Environment.SpecialFolder.Desktop;
            //设置默认根目录是桌面
            fb.Description = "请选择文件目录:";
            //设置对话框说明
            if (fb.ShowDialog() == DialogResult.OK)
            {
                m_strDataSavePath = fb.SelectedPath;

                this.textBoxDataSavePath.Text = m_strDataSavePath;
            }
        }

        /// <summary>
        /// “确定”按钮单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            SetAllSensorSs();
            SetADCMode();
            SetDataSavePath();
            SetUsedChannelCount();
            SetSamplingSetting();
            SetFilterSetting();
            SetTimingType();

            this.DialogResult = DialogResult.OK;
            this.Close();
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
        /// 设置所有传感器的比例因子Si
        /// </summary>
        private void SetAllSensorSs()
        {
            for (int i = 0; i < m_fSs.Length; ++i)
            {
                MainConfig.GetInstance().SetSensorS(i, m_fSs[i]);
            }
        }

        /// <summary>
        /// 设置AC/DC模式
        /// </summary>
        private void SetADCMode()
        {
            string strTemp = this.comboBoxADC.Text;
            if (strTemp == "AC")
            {
                m_eADCMode = ADCMode.AC;
            }
            else if (strTemp == "DC")
            {
                m_eADCMode = ADCMode.DC;
            }

            MainConfig.GetInstance().SetADCMode(m_eADCMode);
        }

        /// <summary>
        /// 设置数据保存路径
        /// </summary>
        private void SetDataSavePath()
        {
            MainConfig.GetInstance().SetIsNeedSaveData(m_bIsNeedSavaData);
            MainConfig.GetInstance().SetDataSavePath(m_strDataSavePath);
        }

        /// <summary>
        /// 设置使用的通道数
        /// </summary>
        private void SetUsedChannelCount()
        {
            string strTemp = this.comboBoxChannelCount.Text;
            int.TryParse(strTemp, out m_nUsedChannelCount);

            MainConfig.GetInstance().SetUsedChannelCount(m_nUsedChannelCount);
        }

        /// <summary>
        /// 设置采样设置
        /// </summary>
        private void SetSamplingSetting()
        {
            if (this.radioButtonACQModeSingle.Checked)
            {
                m_eSamplingMode = SamplingMode.SINGLE;
            }
            else if (this.radioButtonACQModeContinuous.Checked)
            {
                m_eSamplingMode = SamplingMode.CONTINUOUS;
            }

            MainConfig.GetInstance().SetSamplingMode(m_eSamplingMode);

            switch (this.comboBoxSamplingFreq.SelectedIndex)
            {
                case 0:
                    {
                        m_eSamplingFreq = SamplingFreq.FREQ_1;
                        break;
                    }
                case 1:
                    {
                        m_eSamplingFreq = SamplingFreq.FREQ_10;
                        break;
                    }
                case 2:
                    {
                        m_eSamplingFreq = SamplingFreq.FREQ_250;
                        break;
                    }
                case 3:
                    {
                        m_eSamplingFreq = SamplingFreq.FREQ_500;
                        break;
                    }
                case 4:
                    {
                        m_eSamplingFreq = SamplingFreq.FREQ_1000;
                        break;
                    }
                case 5:
                    {
                        m_eSamplingFreq = SamplingFreq.FREQ_4000;
                        break;
                    }
                default:
                    {
                        m_eSamplingFreq = SamplingFreq.FREQ_250;
                        break;
                    }
            }

            MainConfig.GetInstance().SetSamplingFreq(m_eSamplingFreq);

            float.TryParse(this.textBoxSamplingDuration.Text, out m_fSamplingDuration);
            MainConfig.GetInstance().SetSamplingDuration(m_fSamplingDuration);
        }

        /// <summary>
        /// 设置滤波设置
        /// </summary>
        private void SetFilterSetting()
        {
            if (this.checkBoxLowPass.Checked)
            {
                m_bLowPassFilterEnabled = true;
            }
            else
            {
                m_bLowPassFilterEnabled = false;
            }

            MainConfig.GetInstance().SetIsLowPassFilterEnabled(m_bLowPassFilterEnabled);
            float.TryParse(this.textBoxPassFreq.Text, out m_dLowPassFilterPassFreq);
            float.TryParse(this.textBoxStopFreq.Text, out m_dLowPassFilterStopFreq);
            float.TryParse(this.textBoxPassDB.Text, out m_dLowPassFilterPassDB);
            float.TryParse(this.textBoxStopDB.Text, out m_dLowPassFilterStopDB);
            MainConfig.GetInstance().SetLowPassFilterParameters(m_dLowPassFilterPassFreq, m_dLowPassFilterStopFreq,
                m_dLowPassFilterPassDB, m_dLowPassFilterStopDB);
        }

        /// <summary>
        /// 设置授时类型
        /// </summary>
        private void SetTimingType()
        {
            if (this.comboBoxTiming.SelectedIndex == 0)
            {
                m_eTimingType = TimingType.TIMINGTYPE_PC;
            }
            else if (this.comboBoxTiming.SelectedIndex == 1)
            {
                m_eTimingType = TimingType.TIMINGTYPE_GPS;
            }

            MainConfig.GetInstance().SetTimingType(m_eTimingType);
        }

        /// <summary>
        /// 复选框改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxNeedSaveData_CheckedChanged(object sender, EventArgs e)
        {
            m_bIsNeedSavaData = this.checkBoxNeedSaveData.Checked;

            this.buttonSelectDataSavePath.Enabled = m_bIsNeedSavaData;
        }

        private void comboBoxTiming_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
