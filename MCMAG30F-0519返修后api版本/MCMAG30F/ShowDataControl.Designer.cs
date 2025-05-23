namespace MCMAG30F
{
    partial class ShowDataControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timerRefreshData = new System.Windows.Forms.Timer(this.components);
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.setZerobutton = new System.Windows.Forms.Button();
            this.cancelZerobutton = new System.Windows.Forms.Button();
            this.saveDatabutton = new System.Windows.Forms.Button();
            this.buttonSetAbnormalValueRange = new System.Windows.Forms.Button();
            this.textBoxAbnormalValueRange = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.folderBrowserDialog2 = new System.Windows.Forms.FolderBrowserDialog();
            this.labelPLCStatus = new System.Windows.Forms.Label();
            this.listViewDevice1 = new MCMAG30F.DoubleBufferListView();
            this.columnHeaderNo1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderIndex1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderX1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderY1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderZ1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderT1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listViewDevice2 = new MCMAG30F.DoubleBufferListView();
            this.columnHeaderNo2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderTimestamp2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSensor2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderChannel2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderMagValue2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerRefreshData
            // 
            this.timerRefreshData.Interval = 200;
            this.timerRefreshData.Tick += new System.EventHandler(this.timerRefreshData_Tick);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listViewDevice2);
            this.tabPage2.Location = new System.Drawing.Point(4, 62);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1644, 784);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "异常数据";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.listViewDevice1);
            this.tabPage1.Controls.Add(this.flowLayoutPanel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 62);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1644, 784);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "磁场数据";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.setZerobutton);
            this.flowLayoutPanel1.Controls.Add(this.cancelZerobutton);
            this.flowLayoutPanel1.Controls.Add(this.saveDatabutton);
            this.flowLayoutPanel1.Controls.Add(this.buttonSetAbnormalValueRange);
            this.flowLayoutPanel1.Controls.Add(this.textBoxAbnormalValueRange);
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.labelPLCStatus);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 708);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1638, 73);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // setZerobutton
            // 
            this.setZerobutton.AutoSize = true;
            this.setZerobutton.Location = new System.Drawing.Point(3, 3);
            this.setZerobutton.Name = "setZerobutton";
            this.setZerobutton.Size = new System.Drawing.Size(170, 67);
            this.setZerobutton.TabIndex = 2;
            this.setZerobutton.Text = "归零";
            this.setZerobutton.UseVisualStyleBackColor = true;
            this.setZerobutton.Click += new System.EventHandler(this.setZerobutton_Click);
            // 
            // cancelZerobutton
            // 
            this.cancelZerobutton.AutoSize = true;
            this.cancelZerobutton.Location = new System.Drawing.Point(179, 3);
            this.cancelZerobutton.Name = "cancelZerobutton";
            this.cancelZerobutton.Size = new System.Drawing.Size(270, 67);
            this.cancelZerobutton.TabIndex = 2;
            this.cancelZerobutton.Text = "取消归零";
            this.cancelZerobutton.UseVisualStyleBackColor = true;
            this.cancelZerobutton.Click += new System.EventHandler(this.cancelZerobutton_Click);
            // 
            // saveDatabutton
            // 
            this.saveDatabutton.AutoSize = true;
            this.saveDatabutton.Location = new System.Drawing.Point(455, 3);
            this.saveDatabutton.Name = "saveDatabutton";
            this.saveDatabutton.Size = new System.Drawing.Size(270, 67);
            this.saveDatabutton.TabIndex = 2;
            this.saveDatabutton.Text = "数据快照";
            this.saveDatabutton.UseVisualStyleBackColor = true;
            this.saveDatabutton.Click += new System.EventHandler(this.saveDatabutton_Click);
            // 
            // buttonSetAbnormalValueRange
            // 
            this.buttonSetAbnormalValueRange.Location = new System.Drawing.Point(731, 3);
            this.buttonSetAbnormalValueRange.Name = "buttonSetAbnormalValueRange";
            this.buttonSetAbnormalValueRange.Size = new System.Drawing.Size(360, 67);
            this.buttonSetAbnormalValueRange.TabIndex = 4;
            this.buttonSetAbnormalValueRange.Text = "设置异常阈值";
            this.buttonSetAbnormalValueRange.UseVisualStyleBackColor = true;
            this.buttonSetAbnormalValueRange.Click += new System.EventHandler(this.buttonSetAbnormalValueRange_Click);
            // 
            // textBoxAbnormalValueRange
            // 
            this.textBoxAbnormalValueRange.Location = new System.Drawing.Point(1097, 3);
            this.textBoxAbnormalValueRange.Name = "textBoxAbnormalValueRange";
            this.textBoxAbnormalValueRange.Size = new System.Drawing.Size(180, 67);
            this.textBoxAbnormalValueRange.TabIndex = 3;
            this.textBoxAbnormalValueRange.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxAbnormalValueRange.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxAbnormalValueRange_KeyPress);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(1283, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 67);
            this.label1.TabIndex = 5;
            this.label1.Text = "nT";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("宋体", 26F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1652, 850);
            this.tabControl1.TabIndex = 3;
            // 
            // labelPLCStatus
            // 
            this.labelPLCStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelPLCStatus.AutoSize = true;
            this.labelPLCStatus.Location = new System.Drawing.Point(1393, 0);
            this.labelPLCStatus.Name = "labelPLCStatus";
            this.labelPLCStatus.Size = new System.Drawing.Size(179, 73);
            this.labelPLCStatus.TabIndex = 7;
            this.labelPLCStatus.Text = "label2";
            // 
            // listViewDevice1
            // 
            this.listViewDevice1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewDevice1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderNo1,
            this.columnHeaderIndex1,
            this.columnHeaderX1,
            this.columnHeaderY1,
            this.columnHeaderZ1,
            this.columnHeaderT1});
            this.listViewDevice1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewDevice1.Font = new System.Drawing.Font("宋体", 26F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listViewDevice1.FullRowSelect = true;
            this.listViewDevice1.GridLines = true;
            this.listViewDevice1.Location = new System.Drawing.Point(3, 3);
            this.listViewDevice1.Name = "listViewDevice1";
            this.listViewDevice1.OwnerDraw = true;
            this.listViewDevice1.Size = new System.Drawing.Size(1638, 705);
            this.listViewDevice1.TabIndex = 1;
            this.listViewDevice1.UseCompatibleStateImageBehavior = false;
            this.listViewDevice1.View = System.Windows.Forms.View.Details;
            this.listViewDevice1.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.listViewDevice1_ColumnWidthChanged);
            this.listViewDevice1.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.listViewDevice1_ColumnWidthChanging);
            // 
            // columnHeaderNo1
            // 
            this.columnHeaderNo1.Text = "传感器";
            this.columnHeaderNo1.Width = 0;
            // 
            // columnHeaderIndex1
            // 
            this.columnHeaderIndex1.Text = "传感器";
            this.columnHeaderIndex1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderIndex1.Width = 200;
            // 
            // columnHeaderX1
            // 
            this.columnHeaderX1.Text = "X(nT)";
            this.columnHeaderX1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderX1.Width = 200;
            // 
            // columnHeaderY1
            // 
            this.columnHeaderY1.Text = "Y(nT)";
            this.columnHeaderY1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderY1.Width = 200;
            // 
            // columnHeaderZ1
            // 
            this.columnHeaderZ1.Text = "Z(nT)";
            this.columnHeaderZ1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderZ1.Width = 200;
            // 
            // columnHeaderT1
            // 
            this.columnHeaderT1.Text = "总场(nT)";
            this.columnHeaderT1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderT1.Width = 244;
            // 
            // listViewDevice2
            // 
            this.listViewDevice2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewDevice2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderNo2,
            this.columnHeaderTimestamp2,
            this.columnHeaderSensor2,
            this.columnHeaderChannel2,
            this.columnHeaderMagValue2});
            this.listViewDevice2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewDevice2.Font = new System.Drawing.Font("宋体", 26F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listViewDevice2.FullRowSelect = true;
            this.listViewDevice2.GridLines = true;
            this.listViewDevice2.Location = new System.Drawing.Point(3, 3);
            this.listViewDevice2.Name = "listViewDevice2";
            this.listViewDevice2.OwnerDraw = true;
            this.listViewDevice2.Size = new System.Drawing.Size(1638, 778);
            this.listViewDevice2.TabIndex = 1;
            this.listViewDevice2.UseCompatibleStateImageBehavior = false;
            this.listViewDevice2.View = System.Windows.Forms.View.Details;
            this.listViewDevice2.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.listViewDevice2_ColumnWidthChanged);
            this.listViewDevice2.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.listViewDevice2_ColumnWidthChanging);
            // 
            // columnHeaderNo2
            // 
            this.columnHeaderNo2.Text = "序号";
            this.columnHeaderNo2.Width = 0;
            // 
            // columnHeaderTimestamp2
            // 
            this.columnHeaderTimestamp2.Text = "时间";
            this.columnHeaderTimestamp2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderTimestamp2.Width = 200;
            // 
            // columnHeaderSensor2
            // 
            this.columnHeaderSensor2.Text = "传感器";
            this.columnHeaderSensor2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderSensor2.Width = 200;
            // 
            // columnHeaderChannel2
            // 
            this.columnHeaderChannel2.Text = "通道";
            this.columnHeaderChannel2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderChannel2.Width = 200;
            // 
            // columnHeaderMagValue2
            // 
            this.columnHeaderMagValue2.Text = "磁场值(nT)";
            this.columnHeaderMagValue2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderMagValue2.Width = 508;
            // 
            // ShowDataControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "ShowDataControl";
            this.Size = new System.Drawing.Size(1652, 850);
            this.SizeChanged += new System.EventHandler(this.ShowDataControl_SizeChanged);
            this.tabPage2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timerRefreshData;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TabPage tabPage2;
        private DoubleBufferListView listViewDevice2;
        private System.Windows.Forms.ColumnHeader columnHeaderNo2;
        private System.Windows.Forms.ColumnHeader columnHeaderTimestamp2;
        private System.Windows.Forms.ColumnHeader columnHeaderSensor2;
        private System.Windows.Forms.ColumnHeader columnHeaderChannel2;
        private System.Windows.Forms.ColumnHeader columnHeaderMagValue2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button setZerobutton;
        private System.Windows.Forms.Button cancelZerobutton;
        private System.Windows.Forms.Button saveDatabutton;
        private DoubleBufferListView listViewDevice1;
        private System.Windows.Forms.ColumnHeader columnHeaderNo1;
        private System.Windows.Forms.ColumnHeader columnHeaderIndex1;
        private System.Windows.Forms.ColumnHeader columnHeaderX1;
        private System.Windows.Forms.ColumnHeader columnHeaderY1;
        private System.Windows.Forms.ColumnHeader columnHeaderZ1;
        private System.Windows.Forms.ColumnHeader columnHeaderT1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog2;
        private System.Windows.Forms.TextBox textBoxAbnormalValueRange;
        private System.Windows.Forms.Button buttonSetAbnormalValueRange;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelPLCStatus;
    }
}
