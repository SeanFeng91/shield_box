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
    public partial class ProgressForm : Form
    {
        private BackgroundWorker m_backgroundWorker;

        public ProgressForm(BackgroundWorker bgWork)
        {
            InitializeComponent();
			
            this.m_backgroundWorker = bgWork;
        }
		
		/// <summary>
        /// 设置对话框标题
        /// </summary>
        /// <param name="strTitle"></param>
        public void SetTitleText(string strTitle)
        {
            this.Text = strTitle;
        }

        /// <summary>
        /// 进度条初始化
        /// </summary>
        public void Init()
        {
            this.progressBar.Value = 0;
            this.labelDesc.Text = "进度：0%";
            this.buttonCancel.Enabled = true;
        }

        /// <summary>
        /// 进度条改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar.Value = e.ProgressPercentage;
            this.labelDesc.Text = "进度：" + e.ProgressPercentage.ToString() + "%";
        }

        /// <summary>
        /// 后台操作完成，取消，异常时事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Hide();
            //this.Close();   // 执行完成之后，直接关闭页面
        }

        /// <summary>
        /// 取消按钮单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.m_backgroundWorker.CancelAsync();
            this.buttonCancel.Enabled = false;
            // this.Close();
        }
    }
}
