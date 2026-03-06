namespace OmadaPOS.Views
{
    partial class frmSetting: Form
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
            tableLayoutPanelMain = new TableLayoutPanel();
            textBoxPrinterName = new TextBox();
            label4 = new Label();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            textBoxIP = new TextBox();
            textBoxPort = new TextBox();
            textBoxTerminal = new TextBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            buttonCancel = new Button();
            buttonSave = new Button();
            labelWindowsId = new Label();
            tableLayoutPanelMain.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanelMain
            // 
            tableLayoutPanelMain.Anchor = AnchorStyles.None;
            tableLayoutPanelMain.BackColor = Color.White;
            tableLayoutPanelMain.ColumnCount = 1;
            tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelMain.Controls.Add(textBoxPrinterName, 0, 7);
            tableLayoutPanelMain.Controls.Add(label4, 0, 6);
            tableLayoutPanelMain.Controls.Add(label1, 0, 0);
            tableLayoutPanelMain.Controls.Add(label2, 0, 2);
            tableLayoutPanelMain.Controls.Add(label3, 0, 4);
            tableLayoutPanelMain.Controls.Add(textBoxIP, 0, 1);
            tableLayoutPanelMain.Controls.Add(textBoxPort, 0, 3);
            tableLayoutPanelMain.Controls.Add(textBoxTerminal, 0, 5);
            tableLayoutPanelMain.Controls.Add(tableLayoutPanel1, 0, 8);
            tableLayoutPanelMain.Controls.Add(labelWindowsId, 0, 9);
            tableLayoutPanelMain.Location = new Point(171, 181);
            tableLayoutPanelMain.Margin = new Padding(4, 2, 4, 2);
            tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            tableLayoutPanelMain.RowCount = 10;
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 15.2603235F));
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 5.745063F));
            tableLayoutPanelMain.Size = new Size(1460, 1188);
            tableLayoutPanelMain.TabIndex = 0;
            // 
            // textBoxPrinterName
            // 
            textBoxPrinterName.Dock = DockStyle.Fill;
            textBoxPrinterName.Location = new Point(4, 821);
            textBoxPrinterName.Margin = new Padding(4, 2, 4, 2);
            textBoxPrinterName.Name = "textBoxPrinterName";
            textBoxPrinterName.PlaceholderText = "RONGTA";
            textBoxPrinterName.Size = new Size(1452, 39);
            textBoxPrinterName.TabIndex = 8;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.White;
            label4.Dock = DockStyle.Fill;
            label4.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.ForeColor = Color.Black;
            label4.Location = new Point(4, 702);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(1452, 117);
            label4.TabIndex = 7;
            label4.Text = "Printer name";
            label4.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.White;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold);
            label1.Location = new Point(4, 0);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(1452, 117);
            label1.TabIndex = 0;
            label1.Text = "IP Address :";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold);
            label2.Location = new Point(4, 234);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(1452, 117);
            label2.TabIndex = 1;
            label2.Text = "Port : 10009";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Fill;
            label3.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold);
            label3.Location = new Point(4, 468);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(1452, 117);
            label3.TabIndex = 2;
            label3.Text = "Terminal POS";
            label3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // textBoxIP
            // 
            textBoxIP.Dock = DockStyle.Fill;
            textBoxIP.Location = new Point(4, 119);
            textBoxIP.Margin = new Padding(4, 2, 4, 2);
            textBoxIP.Name = "textBoxIP";
            textBoxIP.PlaceholderText = "192.168.1.0";
            textBoxIP.Size = new Size(1452, 39);
            textBoxIP.TabIndex = 4;
            // 
            // textBoxPort
            // 
            textBoxPort.Dock = DockStyle.Fill;
            textBoxPort.Location = new Point(4, 353);
            textBoxPort.Margin = new Padding(4, 2, 4, 2);
            textBoxPort.Name = "textBoxPort";
            textBoxPort.PlaceholderText = "10009";
            textBoxPort.Size = new Size(1452, 39);
            textBoxPort.TabIndex = 5;
            // 
            // textBoxTerminal
            // 
            textBoxTerminal.Dock = DockStyle.Fill;
            textBoxTerminal.Location = new Point(4, 587);
            textBoxTerminal.Margin = new Padding(4, 2, 4, 2);
            textBoxTerminal.Name = "textBoxTerminal";
            textBoxTerminal.PlaceholderText = "POS";
            textBoxTerminal.Size = new Size(1452, 39);
            textBoxTerminal.TabIndex = 6;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.None;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(buttonCancel, 0, 0);
            tableLayoutPanel1.Controls.Add(buttonSave, 1, 0);
            tableLayoutPanel1.Location = new Point(358, 940);
            tableLayoutPanel1.Margin = new Padding(4, 2, 4, 2);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(743, 171);
            tableLayoutPanel1.TabIndex = 9;
            // 
            // buttonCancel
            // 
            buttonCancel.Dock = DockStyle.Fill;
            buttonCancel.FlatStyle = FlatStyle.Flat;
            buttonCancel.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold);
            buttonCancel.Location = new Point(4, 2);
            buttonCancel.Margin = new Padding(4, 2, 4, 2);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(363, 167);
            buttonCancel.TabIndex = 0;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // buttonSave
            // 
            buttonSave.Dock = DockStyle.Fill;
            buttonSave.FlatStyle = FlatStyle.Flat;
            buttonSave.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold);
            buttonSave.Location = new Point(375, 2);
            buttonSave.Margin = new Padding(4, 2, 4, 2);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(364, 167);
            buttonSave.TabIndex = 1;
            buttonSave.Text = "Save";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += buttonSave_Click;
            // 
            // labelWindowsId
            // 
            labelWindowsId.Anchor = AnchorStyles.None;
            labelWindowsId.AutoSize = true;
            labelWindowsId.Font = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelWindowsId.Location = new Point(623, 1129);
            labelWindowsId.Margin = new Padding(4, 0, 4, 0);
            labelWindowsId.Name = "labelWindowsId";
            labelWindowsId.Size = new Size(213, 44);
            labelWindowsId.TabIndex = 10;
            labelWindowsId.Text = "WindowsId";
            labelWindowsId.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // frmSetting
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1872, 1555);
            ControlBox = false;
            Controls.Add(tableLayoutPanelMain);
            Margin = new Padding(4, 2, 4, 2);
            Name = "frmSetting";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Settings";
            Load += frmSetting_Load;
            tableLayoutPanelMain.ResumeLayout(false);
            tableLayoutPanelMain.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanelMain;
        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox textBoxIP;
        private TextBox textBoxPort;
        private TextBox textBoxTerminal;
        private TextBox textBoxPrinterName;
        private Label label4;
        private TableLayoutPanel tableLayoutPanel1;
        private Button buttonCancel;
        private Button buttonSave;
        private Label labelWindowsId;
    }
}