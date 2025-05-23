namespace MCMAG30F
{
    partial class SetParametersForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetParametersForm));
            this.dataGridView_SensorParams = new System.Windows.Forms.DataGridView();
            this.ChannelIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Scaling = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelDataSavePath = new System.Windows.Forms.Label();
            this.textBoxDataSavePath = new System.Windows.Forms.TextBox();
            this.buttonSelectDataSavePath = new System.Windows.Forms.Button();
            this.labelADC = new System.Windows.Forms.Label();
            this.comboBoxADC = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelChannelCount = new System.Windows.Forms.Label();
            this.comboBoxChannelCount = new System.Windows.Forms.ComboBox();
            this.radioButtonACQModeSingle = new System.Windows.Forms.RadioButton();
            this.radioButtonACQModeContinuous = new System.Windows.Forms.RadioButton();
            this.labelSamplingFreq = new System.Windows.Forms.Label();
            this.comboBoxSamplingFreq = new System.Windows.Forms.ComboBox();
            this.labelSamplingTime = new System.Windows.Forms.Label();
            this.textBoxSamplingDuration = new System.Windows.Forms.TextBox();
            this.labelSamplingTimeUnit = new System.Windows.Forms.Label();
            this.groupBoxSampling = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBoxNeedSaveData = new System.Windows.Forms.CheckBox();
            this.groupBoxFilter = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.checkBoxLowPass = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel5 = new System.Windows.Forms.FlowLayoutPanel();
            this.labelPassDB = new System.Windows.Forms.Label();
            this.textBoxPassDB = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.flowLayoutPanel6 = new System.Windows.Forms.FlowLayoutPanel();
            this.labelPassFreq = new System.Windows.Forms.Label();
            this.textBoxPassFreq = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.flowLayoutPanel7 = new System.Windows.Forms.FlowLayoutPanel();
            this.labelStopFreq = new System.Windows.Forms.Label();
            this.textBoxStopFreq = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBoxTiming = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel8 = new System.Windows.Forms.FlowLayoutPanel();
            this.labelStopDB = new System.Windows.Forms.Label();
            this.textBoxStopDB = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.labelTiming = new System.Windows.Forms.Label();
            this.comboBoxTiming = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_SensorParams)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBoxSampling.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBoxFilter.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.flowLayoutPanel5.SuspendLayout();
            this.flowLayoutPanel6.SuspendLayout();
            this.flowLayoutPanel7.SuspendLayout();
            this.groupBoxTiming.SuspendLayout();
            this.flowLayoutPanel8.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView_SensorParams
            // 
            this.dataGridView_SensorParams.AllowUserToAddRows = false;
            this.dataGridView_SensorParams.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_SensorParams.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView_SensorParams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_SensorParams.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ChannelIndex,
            this.Scaling});
            this.dataGridView_SensorParams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_SensorParams.Location = new System.Drawing.Point(3, 3);
            this.dataGridView_SensorParams.Name = "dataGridView_SensorParams";
            this.dataGridView_SensorParams.RowHeadersVisible = false;
            this.dataGridView_SensorParams.RowTemplate.Height = 30;
            this.dataGridView_SensorParams.Size = new System.Drawing.Size(593, 881);
            this.dataGridView_SensorParams.TabIndex = 1;
            this.dataGridView_SensorParams.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridView_SensorParams_CellValidating);
            this.dataGridView_SensorParams.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_SensorParams_CellValueChanged);
            // 
            // ChannelIndex
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.NullValue = "0";
            this.ChannelIndex.DefaultCellStyle = dataGridViewCellStyle2;
            this.ChannelIndex.HeaderText = "通道索引";
            this.ChannelIndex.Name = "ChannelIndex";
            this.ChannelIndex.ReadOnly = true;
            this.ChannelIndex.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ChannelIndex.Width = 150;
            // 
            // Scaling
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.Format = "N6";
            dataGridViewCellStyle3.NullValue = "0.00";
            this.Scaling.DefaultCellStyle = dataGridViewCellStyle3;
            this.Scaling.HeaderText = "比例系数(uT/10V)";
            this.Scaling.Name = "Scaling";
            this.Scaling.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Scaling.Width = 150;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(407, 3);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 30);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "确定";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(500, 3);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 30);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "取消";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // labelDataSavePath
            // 
            this.labelDataSavePath.AutoSize = true;
            this.labelDataSavePath.Location = new System.Drawing.Point(3, 53);
            this.labelDataSavePath.Margin = new System.Windows.Forms.Padding(3);
            this.labelDataSavePath.Name = "labelDataSavePath";
            this.labelDataSavePath.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.labelDataSavePath.Size = new System.Drawing.Size(134, 22);
            this.labelDataSavePath.TabIndex = 4;
            this.labelDataSavePath.Text = "数据保存路径：";
            // 
            // textBoxDataSavePath
            // 
            this.textBoxDataSavePath.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDataSavePath.Location = new System.Drawing.Point(3, 3);
            this.textBoxDataSavePath.Name = "textBoxDataSavePath";
            this.textBoxDataSavePath.ReadOnly = true;
            this.textBoxDataSavePath.Size = new System.Drawing.Size(300, 28);
            this.textBoxDataSavePath.TabIndex = 5;
            // 
            // buttonSelectDataSavePath
            // 
            this.buttonSelectDataSavePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSelectDataSavePath.Location = new System.Drawing.Point(309, 3);
            this.buttonSelectDataSavePath.Name = "buttonSelectDataSavePath";
            this.buttonSelectDataSavePath.Size = new System.Drawing.Size(45, 28);
            this.buttonSelectDataSavePath.TabIndex = 6;
            this.buttonSelectDataSavePath.Text = "...";
            this.buttonSelectDataSavePath.UseVisualStyleBackColor = true;
            this.buttonSelectDataSavePath.Click += new System.EventHandler(this.buttonSelectDataSavePath_Click);
            // 
            // labelADC
            // 
            this.labelADC.AutoSize = true;
            this.labelADC.Location = new System.Drawing.Point(3, 3);
            this.labelADC.Margin = new System.Windows.Forms.Padding(3);
            this.labelADC.Name = "labelADC";
            this.labelADC.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.labelADC.Size = new System.Drawing.Size(71, 22);
            this.labelADC.TabIndex = 7;
            this.labelADC.Text = "AC/DC：";
            // 
            // comboBoxADC
            // 
            this.comboBoxADC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBoxADC.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxADC.FormattingEnabled = true;
            this.comboBoxADC.Items.AddRange(new object[] {
            "AC",
            "DC"});
            this.comboBoxADC.Location = new System.Drawing.Point(179, 3);
            this.comboBoxADC.Name = "comboBoxADC";
            this.comboBoxADC.Size = new System.Drawing.Size(406, 26);
            this.comboBoxADC.TabIndex = 8;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Controls.Add(this.buttonOK);
            this.panel1.Location = new System.Drawing.Point(604, 890);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(592, 51);
            this.panel1.TabIndex = 11;
            // 
            // labelChannelCount
            // 
            this.labelChannelCount.AutoSize = true;
            this.labelChannelCount.Location = new System.Drawing.Point(3, 3);
            this.labelChannelCount.Margin = new System.Windows.Forms.Padding(3);
            this.labelChannelCount.Name = "labelChannelCount";
            this.labelChannelCount.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.labelChannelCount.Size = new System.Drawing.Size(116, 22);
            this.labelChannelCount.TabIndex = 12;
            this.labelChannelCount.Text = "使用通道数：";
            // 
            // comboBoxChannelCount
            // 
            this.comboBoxChannelCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBoxChannelCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxChannelCount.FormattingEnabled = true;
            this.comboBoxChannelCount.Location = new System.Drawing.Point(179, 3);
            this.comboBoxChannelCount.Name = "comboBoxChannelCount";
            this.comboBoxChannelCount.Size = new System.Drawing.Size(406, 26);
            this.comboBoxChannelCount.TabIndex = 13;
            // 
            // radioButtonACQModeSingle
            // 
            this.radioButtonACQModeSingle.AutoSize = true;
            this.radioButtonACQModeSingle.Location = new System.Drawing.Point(3, 3);
            this.radioButtonACQModeSingle.Name = "radioButtonACQModeSingle";
            this.radioButtonACQModeSingle.Size = new System.Drawing.Size(69, 22);
            this.radioButtonACQModeSingle.TabIndex = 14;
            this.radioButtonACQModeSingle.TabStop = true;
            this.radioButtonACQModeSingle.Text = "单次";
            this.radioButtonACQModeSingle.UseVisualStyleBackColor = true;
            // 
            // radioButtonACQModeContinuous
            // 
            this.radioButtonACQModeContinuous.AutoSize = true;
            this.radioButtonACQModeContinuous.Location = new System.Drawing.Point(78, 3);
            this.radioButtonACQModeContinuous.Name = "radioButtonACQModeContinuous";
            this.radioButtonACQModeContinuous.Size = new System.Drawing.Size(69, 22);
            this.radioButtonACQModeContinuous.TabIndex = 15;
            this.radioButtonACQModeContinuous.TabStop = true;
            this.radioButtonACQModeContinuous.Text = "连续";
            this.radioButtonACQModeContinuous.UseVisualStyleBackColor = true;
            // 
            // labelSamplingFreq
            // 
            this.labelSamplingFreq.AutoSize = true;
            this.labelSamplingFreq.Location = new System.Drawing.Point(3, 3);
            this.labelSamplingFreq.Margin = new System.Windows.Forms.Padding(3);
            this.labelSamplingFreq.Name = "labelSamplingFreq";
            this.labelSamplingFreq.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.labelSamplingFreq.Size = new System.Drawing.Size(80, 22);
            this.labelSamplingFreq.TabIndex = 16;
            this.labelSamplingFreq.Text = "采样率：";
            this.labelSamplingFreq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // comboBoxSamplingFreq
            // 
            this.comboBoxSamplingFreq.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSamplingFreq.FormattingEnabled = true;
            this.comboBoxSamplingFreq.Items.AddRange(new object[] {
            "1Hz",
            "10Hz",
            "250Hz",
            "500Hz",
            "1000Hz",
            "4000Hz"});
            this.comboBoxSamplingFreq.Location = new System.Drawing.Point(89, 3);
            this.comboBoxSamplingFreq.Name = "comboBoxSamplingFreq";
            this.comboBoxSamplingFreq.Size = new System.Drawing.Size(121, 26);
            this.comboBoxSamplingFreq.TabIndex = 17;
            // 
            // labelSamplingTime
            // 
            this.labelSamplingTime.AutoSize = true;
            this.labelSamplingTime.Location = new System.Drawing.Point(216, 3);
            this.labelSamplingTime.Margin = new System.Windows.Forms.Padding(3);
            this.labelSamplingTime.Name = "labelSamplingTime";
            this.labelSamplingTime.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.labelSamplingTime.Size = new System.Drawing.Size(98, 22);
            this.labelSamplingTime.TabIndex = 18;
            this.labelSamplingTime.Text = "采样时间：";
            this.labelSamplingTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBoxSamplingDuration
            // 
            this.textBoxSamplingDuration.Location = new System.Drawing.Point(320, 3);
            this.textBoxSamplingDuration.Name = "textBoxSamplingDuration";
            this.textBoxSamplingDuration.Size = new System.Drawing.Size(100, 28);
            this.textBoxSamplingDuration.TabIndex = 19;
            // 
            // labelSamplingTimeUnit
            // 
            this.labelSamplingTimeUnit.AutoSize = true;
            this.labelSamplingTimeUnit.Location = new System.Drawing.Point(426, 3);
            this.labelSamplingTimeUnit.Margin = new System.Windows.Forms.Padding(3);
            this.labelSamplingTimeUnit.Name = "labelSamplingTimeUnit";
            this.labelSamplingTimeUnit.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.labelSamplingTimeUnit.Size = new System.Drawing.Size(26, 22);
            this.labelSamplingTimeUnit.TabIndex = 20;
            this.labelSamplingTimeUnit.Text = "秒";
            this.labelSamplingTimeUnit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBoxSampling
            // 
            this.groupBoxSampling.Controls.Add(this.tableLayoutPanel5);
            this.groupBoxSampling.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxSampling.Location = new System.Drawing.Point(3, 235);
            this.groupBoxSampling.Name = "groupBoxSampling";
            this.groupBoxSampling.Size = new System.Drawing.Size(588, 168);
            this.groupBoxSampling.TabIndex = 21;
            this.groupBoxSampling.TabStop = false;
            this.groupBoxSampling.Text = "采样设置";
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.Controls.Add(this.flowLayoutPanel2, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 24);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(582, 141);
            this.tableLayoutPanel5.TabIndex = 30;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.radioButtonACQModeSingle);
            this.flowLayoutPanel2.Controls.Add(this.radioButtonACQModeContinuous);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(576, 64);
            this.flowLayoutPanel2.TabIndex = 29;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.labelSamplingFreq);
            this.flowLayoutPanel1.Controls.Add(this.comboBoxSamplingFreq);
            this.flowLayoutPanel1.Controls.Add(this.labelSamplingTime);
            this.flowLayoutPanel1.Controls.Add(this.textBoxSamplingDuration);
            this.flowLayoutPanel1.Controls.Add(this.labelSamplingTimeUnit);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 73);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(576, 65);
            this.flowLayoutPanel1.TabIndex = 28;
            // 
            // checkBoxNeedSaveData
            // 
            this.checkBoxNeedSaveData.AutoSize = true;
            this.checkBoxNeedSaveData.Location = new System.Drawing.Point(3, 3);
            this.checkBoxNeedSaveData.Name = "checkBoxNeedSaveData";
            this.checkBoxNeedSaveData.Size = new System.Drawing.Size(142, 22);
            this.checkBoxNeedSaveData.TabIndex = 22;
            this.checkBoxNeedSaveData.Text = "是否保存数据";
            this.checkBoxNeedSaveData.UseVisualStyleBackColor = true;
            this.checkBoxNeedSaveData.CheckedChanged += new System.EventHandler(this.checkBoxNeedSaveData_CheckedChanged);
            // 
            // groupBoxFilter
            // 
            this.groupBoxFilter.Controls.Add(this.tableLayoutPanel8);
            this.groupBoxFilter.Location = new System.Drawing.Point(3, 409);
            this.groupBoxFilter.Name = "groupBoxFilter";
            this.groupBoxFilter.Size = new System.Drawing.Size(586, 184);
            this.groupBoxFilter.TabIndex = 23;
            this.groupBoxFilter.TabStop = false;
            this.groupBoxFilter.Text = "滤波";
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.ColumnCount = 2;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.04014F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.95986F));
            this.tableLayoutPanel8.Controls.Add(this.checkBoxLowPass, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.flowLayoutPanel5, 1, 1);
            this.tableLayoutPanel8.Controls.Add(this.flowLayoutPanel6, 0, 1);
            this.tableLayoutPanel8.Controls.Add(this.flowLayoutPanel7, 0, 2);
            this.tableLayoutPanel8.Controls.Add(this.flowLayoutPanel8, 1, 2);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(3, 24);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 3;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.Size = new System.Drawing.Size(580, 157);
            this.tableLayoutPanel8.TabIndex = 1;
            // 
            // checkBoxLowPass
            // 
            this.checkBoxLowPass.AutoSize = true;
            this.checkBoxLowPass.Location = new System.Drawing.Point(3, 3);
            this.checkBoxLowPass.Name = "checkBoxLowPass";
            this.checkBoxLowPass.Size = new System.Drawing.Size(106, 22);
            this.checkBoxLowPass.TabIndex = 0;
            this.checkBoxLowPass.Text = "低通滤波";
            this.checkBoxLowPass.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel5
            // 
            this.flowLayoutPanel5.Controls.Add(this.labelPassDB);
            this.flowLayoutPanel5.Controls.Add(this.textBoxPassDB);
            this.flowLayoutPanel5.Controls.Add(this.label3);
            this.flowLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel5.Location = new System.Drawing.Point(287, 31);
            this.flowLayoutPanel5.Name = "flowLayoutPanel5";
            this.flowLayoutPanel5.Size = new System.Drawing.Size(290, 43);
            this.flowLayoutPanel5.TabIndex = 10;
            // 
            // labelPassDB
            // 
            this.labelPassDB.AutoSize = true;
            this.labelPassDB.Location = new System.Drawing.Point(3, 3);
            this.labelPassDB.Margin = new System.Windows.Forms.Padding(3);
            this.labelPassDB.Name = "labelPassDB";
            this.labelPassDB.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.labelPassDB.Size = new System.Drawing.Size(98, 22);
            this.labelPassDB.TabIndex = 5;
            this.labelPassDB.Text = "通带衰减：";
            // 
            // textBoxPassDB
            // 
            this.textBoxPassDB.Location = new System.Drawing.Point(107, 3);
            this.textBoxPassDB.Name = "textBoxPassDB";
            this.textBoxPassDB.Size = new System.Drawing.Size(100, 28);
            this.textBoxPassDB.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(213, 3);
            this.label3.Margin = new System.Windows.Forms.Padding(3);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.label3.Size = new System.Drawing.Size(26, 22);
            this.label3.TabIndex = 8;
            this.label3.Text = "dB";
            // 
            // flowLayoutPanel6
            // 
            this.flowLayoutPanel6.Controls.Add(this.labelPassFreq);
            this.flowLayoutPanel6.Controls.Add(this.textBoxPassFreq);
            this.flowLayoutPanel6.Controls.Add(this.label1);
            this.flowLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel6.Location = new System.Drawing.Point(3, 31);
            this.flowLayoutPanel6.Name = "flowLayoutPanel6";
            this.flowLayoutPanel6.Size = new System.Drawing.Size(278, 43);
            this.flowLayoutPanel6.TabIndex = 11;
            // 
            // labelPassFreq
            // 
            this.labelPassFreq.AutoSize = true;
            this.labelPassFreq.Location = new System.Drawing.Point(3, 3);
            this.labelPassFreq.Margin = new System.Windows.Forms.Padding(3);
            this.labelPassFreq.Name = "labelPassFreq";
            this.labelPassFreq.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.labelPassFreq.Size = new System.Drawing.Size(98, 22);
            this.labelPassFreq.TabIndex = 1;
            this.labelPassFreq.Text = "通带频率：";
            // 
            // textBoxPassFreq
            // 
            this.textBoxPassFreq.Location = new System.Drawing.Point(107, 3);
            this.textBoxPassFreq.Name = "textBoxPassFreq";
            this.textBoxPassFreq.Size = new System.Drawing.Size(98, 28);
            this.textBoxPassFreq.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(211, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.label1.Size = new System.Drawing.Size(26, 22);
            this.label1.TabIndex = 9;
            this.label1.Text = "Hz";
            // 
            // flowLayoutPanel7
            // 
            this.flowLayoutPanel7.Controls.Add(this.labelStopFreq);
            this.flowLayoutPanel7.Controls.Add(this.textBoxStopFreq);
            this.flowLayoutPanel7.Controls.Add(this.label2);
            this.flowLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel7.Location = new System.Drawing.Point(3, 80);
            this.flowLayoutPanel7.Name = "flowLayoutPanel7";
            this.flowLayoutPanel7.Size = new System.Drawing.Size(278, 84);
            this.flowLayoutPanel7.TabIndex = 12;
            // 
            // labelStopFreq
            // 
            this.labelStopFreq.AutoSize = true;
            this.labelStopFreq.Location = new System.Drawing.Point(3, 3);
            this.labelStopFreq.Margin = new System.Windows.Forms.Padding(3);
            this.labelStopFreq.Name = "labelStopFreq";
            this.labelStopFreq.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.labelStopFreq.Size = new System.Drawing.Size(98, 22);
            this.labelStopFreq.TabIndex = 2;
            this.labelStopFreq.Text = "阻带频率：";
            // 
            // textBoxStopFreq
            // 
            this.textBoxStopFreq.Location = new System.Drawing.Point(107, 3);
            this.textBoxStopFreq.Name = "textBoxStopFreq";
            this.textBoxStopFreq.Size = new System.Drawing.Size(100, 28);
            this.textBoxStopFreq.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(213, 3);
            this.label2.Margin = new System.Windows.Forms.Padding(3);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.label2.Size = new System.Drawing.Size(26, 22);
            this.label2.TabIndex = 5;
            this.label2.Text = "Hz";
            // 
            // groupBoxTiming
            // 
            this.groupBoxTiming.Controls.Add(this.flowLayoutPanel3);
            this.groupBoxTiming.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxTiming.Location = new System.Drawing.Point(3, 599);
            this.groupBoxTiming.Name = "groupBoxTiming";
            this.groupBoxTiming.Size = new System.Drawing.Size(588, 100);
            this.groupBoxTiming.TabIndex = 2;
            this.groupBoxTiming.TabStop = false;
            this.groupBoxTiming.Text = "授时";
            // 
            // flowLayoutPanel8
            // 
            this.flowLayoutPanel8.Controls.Add(this.labelStopDB);
            this.flowLayoutPanel8.Controls.Add(this.textBoxStopDB);
            this.flowLayoutPanel8.Controls.Add(this.label4);
            this.flowLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel8.Location = new System.Drawing.Point(287, 80);
            this.flowLayoutPanel8.Name = "flowLayoutPanel8";
            this.flowLayoutPanel8.Size = new System.Drawing.Size(290, 84);
            this.flowLayoutPanel8.TabIndex = 13;
            // 
            // labelStopDB
            // 
            this.labelStopDB.AutoSize = true;
            this.labelStopDB.Location = new System.Drawing.Point(3, 3);
            this.labelStopDB.Margin = new System.Windows.Forms.Padding(3);
            this.labelStopDB.Name = "labelStopDB";
            this.labelStopDB.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.labelStopDB.Size = new System.Drawing.Size(98, 22);
            this.labelStopDB.TabIndex = 6;
            this.labelStopDB.Text = "阻带衰减：";
            // 
            // textBoxStopDB
            // 
            this.textBoxStopDB.Location = new System.Drawing.Point(107, 3);
            this.textBoxStopDB.Name = "textBoxStopDB";
            this.textBoxStopDB.Size = new System.Drawing.Size(100, 28);
            this.textBoxStopDB.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(213, 3);
            this.label4.Margin = new System.Windows.Forms.Padding(3);
            this.label4.Name = "label4";
            this.label4.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.label4.Size = new System.Drawing.Size(26, 22);
            this.label4.TabIndex = 9;
            this.label4.Text = "dB";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel1.Controls.Add(this.labelADC, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxADC, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 53F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(588, 53);
            this.tableLayoutPanel1.TabIndex = 24;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel3.Controls.Add(this.checkBoxNeedSaveData, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.labelDataSavePath, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.flowLayoutPanel4, 1, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 62);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(588, 100);
            this.tableLayoutPanel3.TabIndex = 26;
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.Controls.Add(this.textBoxDataSavePath);
            this.flowLayoutPanel4.Controls.Add(this.buttonSelectDataSavePath);
            this.flowLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel4.Location = new System.Drawing.Point(179, 53);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(406, 44);
            this.flowLayoutPanel4.TabIndex = 30;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel4.Controls.Add(this.labelChannelCount, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.comboBoxChannelCount, 1, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 168);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 61F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(588, 61);
            this.tableLayoutPanel4.TabIndex = 27;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 1;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.groupBoxFilter, 0, 5);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel4, 0, 3);
            this.tableLayoutPanel6.Controls.Add(this.groupBoxSampling, 0, 4);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel3, 0, 2);
            this.tableLayoutPanel6.Controls.Add(this.groupBoxTiming, 0, 6);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(602, 3);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 6;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(594, 881);
            this.tableLayoutPanel6.TabIndex = 28;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 2;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.Controls.Add(this.dataGridView_SensorParams, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.panel1, 1, 1);
            this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel6, 1, 0);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel7.Margin = new System.Windows.Forms.Padding(5);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 2;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel7.Size = new System.Drawing.Size(1199, 944);
            this.tableLayoutPanel7.TabIndex = 29;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.labelTiming);
            this.flowLayoutPanel3.Controls.Add(this.comboBoxTiming);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 24);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(582, 73);
            this.flowLayoutPanel3.TabIndex = 0;
            // 
            // labelTiming
            // 
            this.labelTiming.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelTiming.AutoSize = true;
            this.labelTiming.Location = new System.Drawing.Point(3, 7);
            this.labelTiming.Name = "labelTiming";
            this.labelTiming.Size = new System.Drawing.Size(62, 18);
            this.labelTiming.TabIndex = 0;
            this.labelTiming.Text = "授时：";
            // 
            // comboBoxTiming
            // 
            this.comboBoxTiming.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.comboBoxTiming.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTiming.FormattingEnabled = true;
            this.comboBoxTiming.Items.AddRange(new object[] {
            "PC授时",
            "GPS授时"});
            this.comboBoxTiming.Location = new System.Drawing.Point(71, 3);
            this.comboBoxTiming.Name = "comboBoxTiming";
            this.comboBoxTiming.Size = new System.Drawing.Size(121, 26);
            this.comboBoxTiming.TabIndex = 1;
            this.comboBoxTiming.SelectedIndexChanged += new System.EventHandler(this.comboBoxTiming_SelectedIndexChanged);
            // 
            // SetParametersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1199, 944);
            this.Controls.Add(this.tableLayoutPanel7);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetParametersForm";
            this.Text = "参数设置";
            this.Load += new System.EventHandler(this.SetParametersForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_SensorParams)).EndInit();
            this.panel1.ResumeLayout(false);
            this.groupBoxSampling.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.groupBoxFilter.ResumeLayout(false);
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            this.flowLayoutPanel5.ResumeLayout(false);
            this.flowLayoutPanel5.PerformLayout();
            this.flowLayoutPanel6.ResumeLayout(false);
            this.flowLayoutPanel6.PerformLayout();
            this.flowLayoutPanel7.ResumeLayout(false);
            this.flowLayoutPanel7.PerformLayout();
            this.groupBoxTiming.ResumeLayout(false);
            this.flowLayoutPanel8.ResumeLayout(false);
            this.flowLayoutPanel8.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.flowLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel4.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView_SensorParams;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelDataSavePath;
        private System.Windows.Forms.TextBox textBoxDataSavePath;
        private System.Windows.Forms.Button buttonSelectDataSavePath;
        private System.Windows.Forms.Label labelADC;
        private System.Windows.Forms.ComboBox comboBoxADC;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelChannelCount;
        private System.Windows.Forms.ComboBox comboBoxChannelCount;
        private System.Windows.Forms.RadioButton radioButtonACQModeSingle;
        private System.Windows.Forms.RadioButton radioButtonACQModeContinuous;
        private System.Windows.Forms.Label labelSamplingFreq;
        private System.Windows.Forms.ComboBox comboBoxSamplingFreq;
        private System.Windows.Forms.Label labelSamplingTime;
        private System.Windows.Forms.TextBox textBoxSamplingDuration;
        private System.Windows.Forms.Label labelSamplingTimeUnit;
        private System.Windows.Forms.GroupBox groupBoxSampling;
        private System.Windows.Forms.DataGridViewTextBoxColumn ChannelIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn Scaling;
        private System.Windows.Forms.CheckBox checkBoxNeedSaveData;
        private System.Windows.Forms.GroupBox groupBoxFilter;
        private System.Windows.Forms.CheckBox checkBoxLowPass;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private System.Windows.Forms.Label labelStopFreq;
        private System.Windows.Forms.TextBox textBoxPassFreq;
        private System.Windows.Forms.TextBox textBoxStopFreq;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel5;
        private System.Windows.Forms.TextBox textBoxPassDB;
        private System.Windows.Forms.Label labelPassDB;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel6;
        private System.Windows.Forms.Label labelPassFreq;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel7;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel8;
        private System.Windows.Forms.Label labelStopDB;
        private System.Windows.Forms.TextBox textBoxStopDB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBoxTiming;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Label labelTiming;
        private System.Windows.Forms.ComboBox comboBoxTiming;
    }
}