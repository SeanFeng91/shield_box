namespace MCMAG30F
{
    partial class SetDisplaySettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetDisplaySettingForm));
            this.radioButtonDisplayWave = new System.Windows.Forms.RadioButton();
            this.radioButtonDisplayData = new System.Windows.Forms.RadioButton();
            this.radioButtonDisplayFFT = new System.Windows.Forms.RadioButton();
            this.groupBoxDisplayMode = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataGridView_CurveDeviceChannelMap = new System.Windows.Forms.DataGridView();
            this.CurveIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChannelIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelCurveConfig = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelUnit = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.labelYAxis = new System.Windows.Forms.Label();
            this.radioButtonYAxis_uT = new System.Windows.Forms.RadioButton();
            this.radioButtonYAxis_nT = new System.Windows.Forms.RadioButton();
            this.flowLayoutPanelXAxis = new System.Windows.Forms.FlowLayoutPanel();
            this.labelXAxis = new System.Windows.Forms.Label();
            this.radioButtonXAxis_S = new System.Windows.Forms.RadioButton();
            this.flowLayoutPanelShowChannelCount = new System.Windows.Forms.FlowLayoutPanel();
            this.labelShowChannelCount = new System.Windows.Forms.Label();
            this.comboBoxShowChannelCount = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.buttonSwitchShowChannel = new System.Windows.Forms.Button();
            this.labelShowDesc1 = new System.Windows.Forms.Label();
            this.labelShowDesc2 = new System.Windows.Forms.Label();
            this.textBoxDeviceIndex = new System.Windows.Forms.TextBox();
            this.groupBoxDisplayMode.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_CurveDeviceChannelMap)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanelCurveConfig.SuspendLayout();
            this.tableLayoutPanelUnit.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanelXAxis.SuspendLayout();
            this.flowLayoutPanelShowChannelCount.SuspendLayout();
            this.tableLayoutPanelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioButtonDisplayWave
            // 
            this.radioButtonDisplayWave.AutoSize = true;
            this.radioButtonDisplayWave.Location = new System.Drawing.Point(2, 2);
            this.radioButtonDisplayWave.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.radioButtonDisplayWave.Name = "radioButtonDisplayWave";
            this.radioButtonDisplayWave.Size = new System.Drawing.Size(47, 16);
            this.radioButtonDisplayWave.TabIndex = 0;
            this.radioButtonDisplayWave.TabStop = true;
            this.radioButtonDisplayWave.Text = "波形";
            this.radioButtonDisplayWave.UseVisualStyleBackColor = true;
            // 
            // radioButtonDisplayData
            // 
            this.radioButtonDisplayData.AutoSize = true;
            this.radioButtonDisplayData.Location = new System.Drawing.Point(139, 2);
            this.radioButtonDisplayData.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.radioButtonDisplayData.Name = "radioButtonDisplayData";
            this.radioButtonDisplayData.Size = new System.Drawing.Size(47, 16);
            this.radioButtonDisplayData.TabIndex = 1;
            this.radioButtonDisplayData.TabStop = true;
            this.radioButtonDisplayData.Text = "数据";
            this.radioButtonDisplayData.UseVisualStyleBackColor = true;
            // 
            // radioButtonDisplayFFT
            // 
            this.radioButtonDisplayFFT.AutoSize = true;
            this.radioButtonDisplayFFT.Location = new System.Drawing.Point(276, 2);
            this.radioButtonDisplayFFT.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.radioButtonDisplayFFT.Name = "radioButtonDisplayFFT";
            this.radioButtonDisplayFFT.Size = new System.Drawing.Size(41, 16);
            this.radioButtonDisplayFFT.TabIndex = 2;
            this.radioButtonDisplayFFT.TabStop = true;
            this.radioButtonDisplayFFT.Text = "FFT";
            this.radioButtonDisplayFFT.UseVisualStyleBackColor = true;
            // 
            // groupBoxDisplayMode
            // 
            this.groupBoxDisplayMode.Controls.Add(this.tableLayoutPanel1);
            this.groupBoxDisplayMode.Location = new System.Drawing.Point(2, 2);
            this.groupBoxDisplayMode.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBoxDisplayMode.Name = "groupBoxDisplayMode";
            this.groupBoxDisplayMode.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBoxDisplayMode.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.groupBoxDisplayMode.Size = new System.Drawing.Size(421, 51);
            this.groupBoxDisplayMode.TabIndex = 3;
            this.groupBoxDisplayMode.TabStop = false;
            this.groupBoxDisplayMode.Text = "显示方式";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.tableLayoutPanel1.Controls.Add(this.radioButtonDisplayFFT, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonDisplayData, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonDisplayWave, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 16);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(417, 33);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(451, 6);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(50, 20);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "确定";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(512, 6);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(50, 20);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "取消";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Controls.Add(this.buttonOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(2, 638);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(573, 31);
            this.panel1.TabIndex = 6;
            // 
            // dataGridView_CurveDeviceChannelMap
            // 
            this.dataGridView_CurveDeviceChannelMap.AllowUserToAddRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.Format = "N0";
            dataGridViewCellStyle1.NullValue = "0";
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_CurveDeviceChannelMap.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView_CurveDeviceChannelMap.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_CurveDeviceChannelMap.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CurveIndex,
            this.ChannelIndex});
            this.dataGridView_CurveDeviceChannelMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_CurveDeviceChannelMap.Location = new System.Drawing.Point(2, 79);
            this.dataGridView_CurveDeviceChannelMap.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dataGridView_CurveDeviceChannelMap.Name = "dataGridView_CurveDeviceChannelMap";
            this.dataGridView_CurveDeviceChannelMap.RowHeadersVisible = false;
            this.dataGridView_CurveDeviceChannelMap.RowTemplate.Height = 30;
            this.dataGridView_CurveDeviceChannelMap.Size = new System.Drawing.Size(565, 478);
            this.dataGridView_CurveDeviceChannelMap.TabIndex = 7;
            this.dataGridView_CurveDeviceChannelMap.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridView_CurveDeviceChannelMap_CellValidating);
            this.dataGridView_CurveDeviceChannelMap.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CurveDeviceChannelMap_CellValueChanged);
            // 
            // CurveIndex
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Format = "N0";
            dataGridViewCellStyle2.NullValue = "0";
            this.CurveIndex.DefaultCellStyle = dataGridViewCellStyle2;
            this.CurveIndex.HeaderText = "波形索引";
            this.CurveIndex.Name = "CurveIndex";
            this.CurveIndex.ReadOnly = true;
            // 
            // ChannelIndex
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.Format = "N0";
            dataGridViewCellStyle3.NullValue = "0";
            this.ChannelIndex.DefaultCellStyle = dataGridViewCellStyle3;
            this.ChannelIndex.HeaderText = "通道索引";
            this.ChannelIndex.Name = "ChannelIndex";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.tableLayoutPanelCurveConfig);
            this.groupBox1.Location = new System.Drawing.Point(2, 57);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Size = new System.Drawing.Size(573, 577);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "曲线配置";
            // 
            // tableLayoutPanelCurveConfig
            // 
            this.tableLayoutPanelCurveConfig.ColumnCount = 1;
            this.tableLayoutPanelCurveConfig.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelCurveConfig.Controls.Add(this.tableLayoutPanelUnit, 0, 1);
            this.tableLayoutPanelCurveConfig.Controls.Add(this.flowLayoutPanelShowChannelCount, 0, 0);
            this.tableLayoutPanelCurveConfig.Controls.Add(this.dataGridView_CurveDeviceChannelMap, 0, 2);
            this.tableLayoutPanelCurveConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelCurveConfig.Location = new System.Drawing.Point(2, 16);
            this.tableLayoutPanelCurveConfig.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanelCurveConfig.Name = "tableLayoutPanelCurveConfig";
            this.tableLayoutPanelCurveConfig.RowCount = 3;
            this.tableLayoutPanelCurveConfig.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelCurveConfig.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelCurveConfig.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelCurveConfig.Size = new System.Drawing.Size(569, 559);
            this.tableLayoutPanelCurveConfig.TabIndex = 10;
            // 
            // tableLayoutPanelUnit
            // 
            this.tableLayoutPanelUnit.ColumnCount = 2;
            this.tableLayoutPanelUnit.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelUnit.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelUnit.Controls.Add(this.flowLayoutPanel1, 1, 0);
            this.tableLayoutPanelUnit.Controls.Add(this.flowLayoutPanelXAxis, 0, 0);
            this.tableLayoutPanelUnit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelUnit.Location = new System.Drawing.Point(2, 35);
            this.tableLayoutPanelUnit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanelUnit.Name = "tableLayoutPanelUnit";
            this.tableLayoutPanelUnit.RowCount = 1;
            this.tableLayoutPanelUnit.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelUnit.Size = new System.Drawing.Size(565, 40);
            this.tableLayoutPanelUnit.TabIndex = 10;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.labelYAxis);
            this.flowLayoutPanel1.Controls.Add(this.radioButtonYAxis_uT);
            this.flowLayoutPanel1.Controls.Add(this.radioButtonYAxis_nT);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(284, 2);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(279, 36);
            this.flowLayoutPanel1.TabIndex = 10;
            // 
            // labelYAxis
            // 
            this.labelYAxis.AutoSize = true;
            this.labelYAxis.Location = new System.Drawing.Point(2, 2);
            this.labelYAxis.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.labelYAxis.MinimumSize = new System.Drawing.Size(0, 20);
            this.labelYAxis.Name = "labelYAxis";
            this.labelYAxis.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.labelYAxis.Size = new System.Drawing.Size(59, 20);
            this.labelYAxis.TabIndex = 10;
            this.labelYAxis.Text = "Y轴单位：";
            this.labelYAxis.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // radioButtonYAxis_uT
            // 
            this.radioButtonYAxis_uT.AutoSize = true;
            this.radioButtonYAxis_uT.Location = new System.Drawing.Point(65, 2);
            this.radioButtonYAxis_uT.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.radioButtonYAxis_uT.MinimumSize = new System.Drawing.Size(53, 20);
            this.radioButtonYAxis_uT.Name = "radioButtonYAxis_uT";
            this.radioButtonYAxis_uT.Size = new System.Drawing.Size(53, 20);
            this.radioButtonYAxis_uT.TabIndex = 12;
            this.radioButtonYAxis_uT.TabStop = true;
            this.radioButtonYAxis_uT.Text = "uT";
            this.radioButtonYAxis_uT.UseVisualStyleBackColor = true;
            // 
            // radioButtonYAxis_nT
            // 
            this.radioButtonYAxis_nT.AutoSize = true;
            this.radioButtonYAxis_nT.Location = new System.Drawing.Point(122, 2);
            this.radioButtonYAxis_nT.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.radioButtonYAxis_nT.MinimumSize = new System.Drawing.Size(53, 20);
            this.radioButtonYAxis_nT.Name = "radioButtonYAxis_nT";
            this.radioButtonYAxis_nT.Size = new System.Drawing.Size(53, 20);
            this.radioButtonYAxis_nT.TabIndex = 11;
            this.radioButtonYAxis_nT.TabStop = true;
            this.radioButtonYAxis_nT.Text = "nT";
            this.radioButtonYAxis_nT.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanelXAxis
            // 
            this.flowLayoutPanelXAxis.Controls.Add(this.labelXAxis);
            this.flowLayoutPanelXAxis.Controls.Add(this.radioButtonXAxis_S);
            this.flowLayoutPanelXAxis.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelXAxis.Location = new System.Drawing.Point(2, 2);
            this.flowLayoutPanelXAxis.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.flowLayoutPanelXAxis.Name = "flowLayoutPanelXAxis";
            this.flowLayoutPanelXAxis.Size = new System.Drawing.Size(278, 36);
            this.flowLayoutPanelXAxis.TabIndex = 15;
            // 
            // labelXAxis
            // 
            this.labelXAxis.AutoSize = true;
            this.labelXAxis.Location = new System.Drawing.Point(2, 2);
            this.labelXAxis.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.labelXAxis.MinimumSize = new System.Drawing.Size(0, 20);
            this.labelXAxis.Name = "labelXAxis";
            this.labelXAxis.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.labelXAxis.Size = new System.Drawing.Size(59, 20);
            this.labelXAxis.TabIndex = 13;
            this.labelXAxis.Text = "X轴单位：";
            this.labelXAxis.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelXAxis.Click += new System.EventHandler(this.labelXAxis_Click);
            // 
            // radioButtonXAxis_S
            // 
            this.radioButtonXAxis_S.AutoSize = true;
            this.radioButtonXAxis_S.Location = new System.Drawing.Point(65, 2);
            this.radioButtonXAxis_S.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.radioButtonXAxis_S.MinimumSize = new System.Drawing.Size(53, 20);
            this.radioButtonXAxis_S.Name = "radioButtonXAxis_S";
            this.radioButtonXAxis_S.Size = new System.Drawing.Size(53, 20);
            this.radioButtonXAxis_S.TabIndex = 14;
            this.radioButtonXAxis_S.TabStop = true;
            this.radioButtonXAxis_S.Text = "秒";
            this.radioButtonXAxis_S.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanelShowChannelCount
            // 
            this.flowLayoutPanelShowChannelCount.Controls.Add(this.labelShowChannelCount);
            this.flowLayoutPanelShowChannelCount.Controls.Add(this.comboBoxShowChannelCount);
            this.flowLayoutPanelShowChannelCount.Controls.Add(this.labelShowDesc1);
            this.flowLayoutPanelShowChannelCount.Controls.Add(this.textBoxDeviceIndex);
            this.flowLayoutPanelShowChannelCount.Controls.Add(this.labelShowDesc2);
            this.flowLayoutPanelShowChannelCount.Controls.Add(this.buttonSwitchShowChannel);
            this.flowLayoutPanelShowChannelCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelShowChannelCount.Location = new System.Drawing.Point(2, 2);
            this.flowLayoutPanelShowChannelCount.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.flowLayoutPanelShowChannelCount.Name = "flowLayoutPanelShowChannelCount";
            this.flowLayoutPanelShowChannelCount.Size = new System.Drawing.Size(565, 29);
            this.flowLayoutPanelShowChannelCount.TabIndex = 9;
            // 
            // labelShowChannelCount
            // 
            this.labelShowChannelCount.AutoSize = true;
            this.labelShowChannelCount.Location = new System.Drawing.Point(2, 2);
            this.labelShowChannelCount.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.labelShowChannelCount.Name = "labelShowChannelCount";
            this.labelShowChannelCount.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.labelShowChannelCount.Size = new System.Drawing.Size(77, 15);
            this.labelShowChannelCount.TabIndex = 8;
            this.labelShowChannelCount.Text = "显示通道数：";
            // 
            // comboBoxShowChannelCount
            // 
            this.comboBoxShowChannelCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxShowChannelCount.FormattingEnabled = true;
            this.comboBoxShowChannelCount.Items.AddRange(new object[] {
            "3",
            "6",
            "9",
            "12",
            "15",
            "18",
            "21",
            "24",
            "27",
            "30"});
            this.comboBoxShowChannelCount.Location = new System.Drawing.Point(83, 2);
            this.comboBoxShowChannelCount.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboBoxShowChannelCount.Name = "comboBoxShowChannelCount";
            this.comboBoxShowChannelCount.Size = new System.Drawing.Size(83, 20);
            this.comboBoxShowChannelCount.TabIndex = 9;
            this.comboBoxShowChannelCount.SelectedIndexChanged += new System.EventHandler(this.comboBoxShowChannelCount_SelectedIndexChanged);
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 1;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.Controls.Add(this.groupBox1, 0, 1);
            this.tableLayoutPanelMain.Controls.Add(this.panel1, 0, 2);
            this.tableLayoutPanelMain.Controls.Add(this.groupBoxDisplayMode, 0, 0);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 3;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(577, 671);
            this.tableLayoutPanelMain.TabIndex = 9;
            this.tableLayoutPanelMain.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanelMain_Paint);
            // 
            // buttonSwitchShowChannel
            // 
            this.buttonSwitchShowChannel.Location = new System.Drawing.Point(436, 2);
            this.buttonSwitchShowChannel.Margin = new System.Windows.Forms.Padding(2);
            this.buttonSwitchShowChannel.Name = "buttonSwitchShowChannel";
            this.buttonSwitchShowChannel.Size = new System.Drawing.Size(75, 23);
            this.buttonSwitchShowChannel.TabIndex = 10;
            this.buttonSwitchShowChannel.Text = "切换";
            this.buttonSwitchShowChannel.UseVisualStyleBackColor = true;
            this.buttonSwitchShowChannel.Click += new System.EventHandler(this.buttonSwitchShowChannel_Click);
            // 
            // labelShowDesc1
            // 
            this.labelShowDesc1.AutoSize = true;
            this.labelShowDesc1.Location = new System.Drawing.Point(170, 2);
            this.labelShowDesc1.Margin = new System.Windows.Forms.Padding(2);
            this.labelShowDesc1.Name = "labelShowDesc1";
            this.labelShowDesc1.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.labelShowDesc1.Size = new System.Drawing.Size(41, 15);
            this.labelShowDesc1.TabIndex = 11;
            this.labelShowDesc1.Text = "显示第";
            this.labelShowDesc1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelShowDesc2
            // 
            this.labelShowDesc2.AutoSize = true;
            this.labelShowDesc2.Location = new System.Drawing.Point(319, 2);
            this.labelShowDesc2.Margin = new System.Windows.Forms.Padding(2);
            this.labelShowDesc2.Name = "labelShowDesc2";
            this.labelShowDesc2.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.labelShowDesc2.Size = new System.Drawing.Size(113, 15);
            this.labelShowDesc2.TabIndex = 12;
            this.labelShowDesc2.Text = "组采集器的通道数据";
            this.labelShowDesc2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxDeviceIndex
            // 
            this.textBoxDeviceIndex.Location = new System.Drawing.Point(215, 2);
            this.textBoxDeviceIndex.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxDeviceIndex.Name = "textBoxDeviceIndex";
            this.textBoxDeviceIndex.Size = new System.Drawing.Size(100, 21);
            this.textBoxDeviceIndex.TabIndex = 13;
            // 
            // SetDisplaySettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(577, 671);
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "SetDisplaySettingForm";
            this.Text = "显示设置";
            this.Load += new System.EventHandler(this.SetDisplaySettingForm_Load);
            this.SizeChanged += new System.EventHandler(this.SetDisplaySettingForm_SizeChanged);
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.SetDisplaySettingForm_Layout);
            this.groupBoxDisplayMode.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_CurveDeviceChannelMap)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanelCurveConfig.ResumeLayout(false);
            this.tableLayoutPanelUnit.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanelXAxis.ResumeLayout(false);
            this.flowLayoutPanelXAxis.PerformLayout();
            this.flowLayoutPanelShowChannelCount.ResumeLayout(false);
            this.flowLayoutPanelShowChannelCount.PerformLayout();
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonDisplayWave;
        private System.Windows.Forms.RadioButton radioButtonDisplayData;
        private System.Windows.Forms.RadioButton radioButtonDisplayFFT;
        private System.Windows.Forms.GroupBox groupBoxDisplayMode;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dataGridView_CurveDeviceChannelMap;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBoxShowChannelCount;
        private System.Windows.Forms.Label labelShowChannelCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn CurveIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn ChannelIndex;
        private System.Windows.Forms.RadioButton radioButtonYAxis_uT;
        private System.Windows.Forms.RadioButton radioButtonYAxis_nT;
        private System.Windows.Forms.Label labelYAxis;
        private System.Windows.Forms.RadioButton radioButtonXAxis_S;
        private System.Windows.Forms.Label labelXAxis;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelXAxis;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelCurveConfig;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelShowChannelCount;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelUnit;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label labelShowDesc1;
        private System.Windows.Forms.TextBox textBoxDeviceIndex;
        private System.Windows.Forms.Label labelShowDesc2;
        private System.Windows.Forms.Button buttonSwitchShowChannel;
    }
}