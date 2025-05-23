using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MCMag30FDevice;

namespace MCMAG30F
{
    public partial class ShowDeviceStatusForm : Form
    {
        private MultiChanMagCollectDeviceManager m_DeviceManager = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ShowDeviceStatusForm()
        {
            m_DeviceManager = MultiChanMagCollectDeviceManager.GetInstance();
            if (m_DeviceManager != null)
            {
                m_DeviceManager.DeviceStatusChangedEvent += this.OnDeviceStatusChanged;
            }

            InitializeComponent();

            /// 初始化设备信息
            InitDeviceInfo();
        }

        /// <summary>
        /// 初始化设备信息
        /// </summary>
        private void InitDeviceInfo()
        {
            int nDeviceCount = MainConfig.GetInstance().GetDeviceCount();
            ushort[] nVIDs = new ushort[nDeviceCount];  // 设备VID
            ushort[] nPIDs = new ushort[nDeviceCount];  // 设备PID
            for (int i = 0; i < nDeviceCount; ++i)
            {              
                MainConfig.GetInstance().GetDeviceVIDAndPIDByIndex(i, ref nVIDs[i], ref nPIDs[i]);
            }

            if (nDeviceCount >= 1)
            {
                this.textBoxDeviceNo1_VID.Text = "0x" + nVIDs[0].ToString("X4");
                this.textBoxDeviceNo1_PID.Text = "0x" + nPIDs[0].ToString("X4");
                uint nDeviceID = MainConfig.GetInstance().GenDeviceID(nVIDs[0], nPIDs[0]);
                this.textBoxDeviceNo1_Status.Text = GetDeviceStatusString(nDeviceID);
            }
            
            if (nDeviceCount >= 2)
            {
                this.textBoxDeviceNo2_VID.Text = "0x" + nVIDs[1].ToString("X4");
                this.textBoxDeviceNo2_PID.Text = "0x" + nPIDs[1].ToString("X4");
                uint nDeviceID = MainConfig.GetInstance().GenDeviceID(nVIDs[1], nPIDs[1]);
                this.textBoxDeviceNo2_Status.Text = GetDeviceStatusString(nDeviceID);
            }
            
            if (nDeviceCount >= 3)
            {
                this.textBoxDeviceNo3_VID.Text = "0x" + nVIDs[2].ToString("X4");
                this.textBoxDeviceNo3_PID.Text = "0x" + nPIDs[2].ToString("X4");
                uint nDeviceID = MainConfig.GetInstance().GenDeviceID(nVIDs[2], nPIDs[2]);
                this.textBoxDeviceNo3_Status.Text = GetDeviceStatusString(nDeviceID);
            }
        }

        /// <summary>
        /// 获取设备连接状态字符串
        /// </summary>
        /// <param name="nDeviceNo">设备编号，从0开始</param>
        /// <returns>设备状态字符串</returns>
        private string GetDeviceStatusString(uint nDeviceID)
        {
            if (null != m_DeviceManager)
            {
                DeviceStatus eDeviceStatus = m_DeviceManager.GetDeviceStatus(nDeviceID);
                switch (eDeviceStatus)
                {
                    case DeviceStatus.DISCONNECTED:
                        {
                            return "未连接";
                        }
                    case DeviceStatus.UNAVAILABLE:
                        {
                            return "不可用";
                        }
                    case DeviceStatus.CONNECTED:
                        {
                            return "连接";
                        }
                    case DeviceStatus.RUNNING:
                        {
                            return "正在运行";
                        }
                    default:
                        {
                            return "未知";
                        }
                }
            }
            
            return "未知";
        }

        /// <summary>
        /// 单击确定按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void OnDeviceStatusChanged(uint nDeviceID, DeviceStatus eDeviceStatus)
        {
            // 简单处理，直接重新初始化界面的设备信息
            InitDeviceInfo();
        }

        /// <summary>
        /// 设备关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowDeviceStatusForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (m_DeviceManager != null)
            {
                m_DeviceManager.DeviceStatusChangedEvent -= OnDeviceStatusChanged;
            }
        }
    }
}
