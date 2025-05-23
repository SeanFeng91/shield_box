namespace MCMAG30F
{
    partial class MCMAG30FForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MCMAG30FForm));
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonOpen = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonDeviceStatus = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonStart = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonConnectPLC = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDisconnectPLC = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSetting = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDisplay = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDataConvert = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabelCurveControlIndex = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBoxCurveControlIndex = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabelCurveIndex = new System.Windows.Forms.ToolStripLabel();
            this.toolStripDropDownButtonCurveIndex = new System.Windows.Forms.ToolStripDropDownButton();
            this.ShowXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ShowYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ShowZToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonHelp = new System.Windows.Forms.ToolStripButton();
            this.centerPanel = new System.Windows.Forms.Panel();
            this.labelLegend = new System.Windows.Forms.Label();
            this.backgroundWorkerReadFile = new System.ComponentModel.BackgroundWorker();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.toolStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMain
            // 
            this.toolStripMain.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonOpen,
            this.toolStripSeparator1,
            this.toolStripButtonDeviceStatus,
            this.toolStripButtonStart,
            this.toolStripButtonStop,
            this.toolStripButtonConnectPLC,
            this.toolStripButtonDisconnectPLC,
            this.toolStripButtonSetting,
            this.toolStripButtonDisplay,
            this.toolStripButtonDataConvert,
            this.toolStripLabelCurveControlIndex,
            this.toolStripComboBoxCurveControlIndex,
            this.toolStripLabelCurveIndex,
            this.toolStripDropDownButtonCurveIndex,
            this.toolStripButtonHelp});
            this.toolStripMain.Location = new System.Drawing.Point(0, 0);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.Size = new System.Drawing.Size(1418, 32);
            this.toolStripMain.TabIndex = 0;
            this.toolStripMain.Text = "主工具栏";
            // 
            // toolStripButtonOpen
            // 
            this.toolStripButtonOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonOpen.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonOpen.Image")));
            this.toolStripButtonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOpen.Name = "toolStripButtonOpen";
            this.toolStripButtonOpen.Size = new System.Drawing.Size(28, 29);
            this.toolStripButtonOpen.Text = "打开文件";
            this.toolStripButtonOpen.Click += new System.EventHandler(this.toolStripButtonOpen_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 32);
            // 
            // toolStripButtonDeviceStatus
            // 
            this.toolStripButtonDeviceStatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDeviceStatus.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDeviceStatus.Image")));
            this.toolStripButtonDeviceStatus.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDeviceStatus.Name = "toolStripButtonDeviceStatus";
            this.toolStripButtonDeviceStatus.Size = new System.Drawing.Size(28, 29);
            this.toolStripButtonDeviceStatus.Text = "设备状态";
            this.toolStripButtonDeviceStatus.Click += new System.EventHandler(this.toolStripButtonDeviceStatus_Click);
            // 
            // toolStripButtonStart
            // 
            this.toolStripButtonStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonStart.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonStart.Image")));
            this.toolStripButtonStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonStart.Name = "toolStripButtonStart";
            this.toolStripButtonStart.Size = new System.Drawing.Size(28, 29);
            this.toolStripButtonStart.Text = "开始采集";
            this.toolStripButtonStart.Click += new System.EventHandler(this.toolStripButtonStart_Click);
            // 
            // toolStripButtonStop
            // 
            this.toolStripButtonStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonStop.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonStop.Image")));
            this.toolStripButtonStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonStop.Name = "toolStripButtonStop";
            this.toolStripButtonStop.Size = new System.Drawing.Size(28, 29);
            this.toolStripButtonStop.Text = "停止采集";
            this.toolStripButtonStop.Click += new System.EventHandler(this.toolStripButtonStop_Click);
            // 
            // toolStripButtonConnectPLC
            // 
            this.toolStripButtonConnectPLC.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonConnectPLC.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonConnectPLC.Image")));
            this.toolStripButtonConnectPLC.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonConnectPLC.Name = "toolStripButtonConnectPLC";
            this.toolStripButtonConnectPLC.Size = new System.Drawing.Size(28, 29);
            this.toolStripButtonConnectPLC.Text = "连接PLC";
            this.toolStripButtonConnectPLC.Click += new System.EventHandler(this.toolStripButtonConnectPLC_Click);
            // 
            // toolStripButtonDisconnectPLC
            // 
            this.toolStripButtonDisconnectPLC.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDisconnectPLC.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDisconnectPLC.Image")));
            this.toolStripButtonDisconnectPLC.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDisconnectPLC.Name = "toolStripButtonDisconnectPLC";
            this.toolStripButtonDisconnectPLC.Size = new System.Drawing.Size(28, 29);
            this.toolStripButtonDisconnectPLC.Text = "断开PLC";
            this.toolStripButtonDisconnectPLC.Click += new System.EventHandler(this.toolStripButtonDisconnectPLC_Click);
            // 
            // toolStripButtonSetting
            // 
            this.toolStripButtonSetting.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSetting.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSetting.Image")));
            this.toolStripButtonSetting.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSetting.Name = "toolStripButtonSetting";
            this.toolStripButtonSetting.Size = new System.Drawing.Size(28, 29);
            this.toolStripButtonSetting.Text = "设置";
            this.toolStripButtonSetting.Click += new System.EventHandler(this.toolStripButtonSetting_Click);
            // 
            // toolStripButtonDisplay
            // 
            this.toolStripButtonDisplay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDisplay.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDisplay.Image")));
            this.toolStripButtonDisplay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDisplay.Name = "toolStripButtonDisplay";
            this.toolStripButtonDisplay.Size = new System.Drawing.Size(28, 29);
            this.toolStripButtonDisplay.Text = "显示";
            this.toolStripButtonDisplay.Click += new System.EventHandler(this.toolStripButtonDisplay_Click);
            // 
            // toolStripButtonDataConvert
            // 
            this.toolStripButtonDataConvert.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDataConvert.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDataConvert.Image")));
            this.toolStripButtonDataConvert.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDataConvert.Name = "toolStripButtonDataConvert";
            this.toolStripButtonDataConvert.Size = new System.Drawing.Size(28, 29);
            this.toolStripButtonDataConvert.Text = "数据转换";
            this.toolStripButtonDataConvert.Click += new System.EventHandler(this.toolStripButtonDataConvert_Click);
            // 
            // toolStripLabelCurveControlIndex
            // 
            this.toolStripLabelCurveControlIndex.Name = "toolStripLabelCurveControlIndex";
            this.toolStripLabelCurveControlIndex.Size = new System.Drawing.Size(100, 29);
            this.toolStripLabelCurveControlIndex.Text = "显示单元：";
            // 
            // toolStripComboBoxCurveControlIndex
            // 
            this.toolStripComboBoxCurveControlIndex.DropDownHeight = 500;
            this.toolStripComboBoxCurveControlIndex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBoxCurveControlIndex.DropDownWidth = 80;
            this.toolStripComboBoxCurveControlIndex.IntegralHeight = false;
            this.toolStripComboBoxCurveControlIndex.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9"});
            this.toolStripComboBoxCurveControlIndex.Name = "toolStripComboBoxCurveControlIndex";
            this.toolStripComboBoxCurveControlIndex.Size = new System.Drawing.Size(75, 32);
            // 
            // toolStripLabelCurveIndex
            // 
            this.toolStripLabelCurveIndex.Name = "toolStripLabelCurveIndex";
            this.toolStripLabelCurveIndex.Size = new System.Drawing.Size(100, 29);
            this.toolStripLabelCurveIndex.Text = "曲线索引：";
            // 
            // toolStripDropDownButtonCurveIndex
            // 
            this.toolStripDropDownButtonCurveIndex.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButtonCurveIndex.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ShowXToolStripMenuItem,
            this.ShowYToolStripMenuItem,
            this.ShowZToolStripMenuItem});
            this.toolStripDropDownButtonCurveIndex.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButtonCurveIndex.Image")));
            this.toolStripDropDownButtonCurveIndex.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButtonCurveIndex.Name = "toolStripDropDownButtonCurveIndex";
            this.toolStripDropDownButtonCurveIndex.Size = new System.Drawing.Size(42, 29);
            this.toolStripDropDownButtonCurveIndex.Text = "显示曲线";
            this.toolStripDropDownButtonCurveIndex.DropDownOpening += new System.EventHandler(this.toolStripDropDownButtonCurveIndex_DropDownOpening);
            // 
            // ShowXToolStripMenuItem
            // 
            this.ShowXToolStripMenuItem.Checked = true;
            this.ShowXToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowXToolStripMenuItem.Name = "ShowXToolStripMenuItem";
            this.ShowXToolStripMenuItem.Size = new System.Drawing.Size(140, 30);
            this.ShowXToolStripMenuItem.Text = "显示X";
            this.ShowXToolStripMenuItem.Click += new System.EventHandler(this.ShowXToolStripMenuItem_Click);
            // 
            // ShowYToolStripMenuItem
            // 
            this.ShowYToolStripMenuItem.Checked = true;
            this.ShowYToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowYToolStripMenuItem.Name = "ShowYToolStripMenuItem";
            this.ShowYToolStripMenuItem.Size = new System.Drawing.Size(140, 30);
            this.ShowYToolStripMenuItem.Text = "显示Y";
            this.ShowYToolStripMenuItem.Click += new System.EventHandler(this.ShowYToolStripMenuItem_Click);
            // 
            // ShowZToolStripMenuItem
            // 
            this.ShowZToolStripMenuItem.Checked = true;
            this.ShowZToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowZToolStripMenuItem.Name = "ShowZToolStripMenuItem";
            this.ShowZToolStripMenuItem.Size = new System.Drawing.Size(140, 30);
            this.ShowZToolStripMenuItem.Text = "显示Z";
            this.ShowZToolStripMenuItem.Click += new System.EventHandler(this.ShowZToolStripMenuItem_Click);
            // 
            // toolStripButtonHelp
            // 
            this.toolStripButtonHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonHelp.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonHelp.Image")));
            this.toolStripButtonHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonHelp.Name = "toolStripButtonHelp";
            this.toolStripButtonHelp.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripButtonHelp.Size = new System.Drawing.Size(28, 29);
            this.toolStripButtonHelp.Text = "帮助";
            // 
            // centerPanel
            // 
            this.centerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.centerPanel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.centerPanel.Location = new System.Drawing.Point(0, 32);
            this.centerPanel.Name = "centerPanel";
            this.centerPanel.Size = new System.Drawing.Size(1418, 904);
            this.centerPanel.TabIndex = 1;
            // 
            // labelLegend
            // 
            this.labelLegend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelLegend.AutoSize = true;
            this.labelLegend.BackColor = System.Drawing.SystemColors.Window;
            this.labelLegend.Cursor = System.Windows.Forms.Cursors.Default;
            this.labelLegend.Font = new System.Drawing.Font("宋体", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelLegend.Image = ((System.Drawing.Image)(resources.GetObject("labelLegend.Image")));
            this.labelLegend.Location = new System.Drawing.Point(1147, -2);
            this.labelLegend.Name = "labelLegend";
            this.labelLegend.Size = new System.Drawing.Size(271, 33);
            this.labelLegend.TabIndex = 0;
            this.labelLegend.Text = "                ";
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // MCMAG30FForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1418, 936);
            this.Controls.Add(this.labelLegend);
            this.Controls.Add(this.centerPanel);
            this.Controls.Add(this.toolStripMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MCMAG30FForm";
            this.Text = "MCMAG-30F";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Shown += new System.EventHandler(this.MCMAG30FForm_Shown);
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripButton toolStripButtonOpen;
        private System.Windows.Forms.ToolStripButton toolStripButtonDeviceStatus;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonStart;
        private System.Windows.Forms.ToolStripButton toolStripButtonStop;
        private System.Windows.Forms.ToolStripButton toolStripButtonSetting;
        private System.Windows.Forms.ToolStripButton toolStripButtonDisplay;
        private System.Windows.Forms.ToolStripButton toolStripButtonHelp;
        private System.Windows.Forms.Panel centerPanel;
        private System.Windows.Forms.Label labelLegend;
        private System.ComponentModel.BackgroundWorker backgroundWorkerReadFile;
        private System.Windows.Forms.ToolStripButton toolStripButtonDataConvert;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Windows.Forms.ToolStripLabel toolStripLabelCurveControlIndex;
        private System.Windows.Forms.ToolStripLabel toolStripLabelCurveIndex;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonCurveIndex;
        private System.Windows.Forms.ToolStripMenuItem ShowXToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ShowYToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ShowZToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxCurveControlIndex;
        private System.Windows.Forms.ToolStripButton toolStripButtonConnectPLC;
        private System.Windows.Forms.ToolStripButton toolStripButtonDisconnectPLC;
    }
}

