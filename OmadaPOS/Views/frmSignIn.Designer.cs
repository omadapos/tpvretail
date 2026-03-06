namespace OmadaPOS.Views
{
    partial class frmSignIn: Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSignIn));
            tableLayoutPanelMain = new TableLayoutPanel();
            tableLayoutPanel1 = new TableLayoutPanel();
            button9 = new Button();
            button6 = new Button();
            button1 = new Button();
            button2 = new Button();
            button8 = new Button();
            button5 = new Button();
            button4 = new Button();
            button3 = new Button();
            button0 = new Button();
            button7 = new Button();
            buttonClear = new Button();
            buttonLogin = new Button();
            textBoxPhone = new TextBox();
            labelId = new Label();
            watermarkOmadapos1 = new OmadaPOS.Controles.WatermarkOmadaPOS();
            tableLayoutPanelMain.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanelMain
            // 
            tableLayoutPanelMain.BackColor = Color.Transparent;
            tableLayoutPanelMain.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
            tableLayoutPanelMain.ColumnCount = 3;
            tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 31.9277115F));
            tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 39.2493057F));
            tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28.8693237F));
            tableLayoutPanelMain.Controls.Add(tableLayoutPanel1, 1, 1);
            tableLayoutPanelMain.Controls.Add(textBoxPhone, 1, 0);
            tableLayoutPanelMain.Controls.Add(labelId, 2, 3);
            tableLayoutPanelMain.Controls.Add(watermarkOmadapos1, 0, 2);
            tableLayoutPanelMain.Dock = DockStyle.Fill;
            tableLayoutPanelMain.Location = new Point(0, 0);
            tableLayoutPanelMain.Margin = new Padding(2, 1, 2, 1);
            tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            tableLayoutPanelMain.RowCount = 4;
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 21.0912914F));
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 78.90871F));
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 107F));
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
            tableLayoutPanelMain.Size = new Size(1008, 729);
            tableLayoutPanelMain.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.Controls.Add(button9, 2, 0);
            tableLayoutPanel1.Controls.Add(button6, 2, 1);
            tableLayoutPanel1.Controls.Add(button1, 0, 2);
            tableLayoutPanel1.Controls.Add(button2, 1, 2);
            tableLayoutPanel1.Controls.Add(button8, 1, 0);
            tableLayoutPanel1.Controls.Add(button5, 1, 1);
            tableLayoutPanel1.Controls.Add(button4, 0, 1);
            tableLayoutPanel1.Controls.Add(button3, 2, 2);
            tableLayoutPanel1.Controls.Add(button0, 1, 3);
            tableLayoutPanel1.Controls.Add(button7, 0, 0);
            tableLayoutPanel1.Controls.Add(buttonClear, 0, 3);
            tableLayoutPanel1.Controls.Add(buttonLogin, 2, 3);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(323, 120);
            tableLayoutPanel1.Margin = new Padding(2, 1, 2, 1);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.Size = new Size(391, 444);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // button9
            // 
            button9.Dock = DockStyle.Fill;
            button9.Font = new Font("Microsoft Sans Serif", 8.1F);
            button9.ForeColor = SystemColors.ActiveCaptionText;
            button9.Location = new Point(262, 2);
            button9.Margin = new Padding(2);
            button9.Name = "button9";
            button9.Size = new Size(127, 107);
            button9.TabIndex = 8;
            button9.Tag = "9";
            button9.Text = "9";
            button9.UseVisualStyleBackColor = true;
            button9.Click += buttonKey_Click;
            // 
            // button6
            // 
            button6.Dock = DockStyle.Fill;
            button6.Font = new Font("Microsoft Sans Serif", 8.1F);
            button6.ForeColor = SystemColors.ActiveCaptionText;
            button6.Location = new Point(262, 113);
            button6.Margin = new Padding(2);
            button6.Name = "button6";
            button6.Size = new Size(127, 107);
            button6.TabIndex = 5;
            button6.Tag = "6";
            button6.Text = "6";
            button6.UseVisualStyleBackColor = true;
            button6.Click += buttonKey_Click;
            // 
            // button1
            // 
            button1.Dock = DockStyle.Fill;
            button1.Font = new Font("Microsoft Sans Serif", 8.1F);
            button1.ForeColor = SystemColors.ActiveCaptionText;
            button1.Location = new Point(2, 224);
            button1.Margin = new Padding(2);
            button1.Name = "button1";
            button1.Size = new Size(126, 107);
            button1.TabIndex = 0;
            button1.Tag = "1";
            button1.Text = "1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += buttonKey_Click;
            // 
            // button2
            // 
            button2.Dock = DockStyle.Fill;
            button2.Font = new Font("Microsoft Sans Serif", 8.1F);
            button2.ForeColor = SystemColors.ActiveCaptionText;
            button2.Location = new Point(132, 224);
            button2.Margin = new Padding(2);
            button2.Name = "button2";
            button2.Size = new Size(126, 107);
            button2.TabIndex = 1;
            button2.Tag = "2";
            button2.Text = "2";
            button2.UseVisualStyleBackColor = true;
            button2.Click += buttonKey_Click;
            // 
            // button8
            // 
            button8.Dock = DockStyle.Fill;
            button8.Font = new Font("Microsoft Sans Serif", 8.1F);
            button8.ForeColor = SystemColors.ActiveCaptionText;
            button8.Location = new Point(132, 2);
            button8.Margin = new Padding(2);
            button8.Name = "button8";
            button8.Size = new Size(126, 107);
            button8.TabIndex = 7;
            button8.Tag = "8";
            button8.Text = "8";
            button8.UseVisualStyleBackColor = true;
            button8.Click += buttonKey_Click;
            // 
            // button5
            // 
            button5.Dock = DockStyle.Fill;
            button5.Font = new Font("Microsoft Sans Serif", 8.1F);
            button5.ForeColor = SystemColors.ActiveCaptionText;
            button5.Location = new Point(132, 113);
            button5.Margin = new Padding(2);
            button5.Name = "button5";
            button5.Size = new Size(126, 107);
            button5.TabIndex = 4;
            button5.Tag = "5";
            button5.Text = "5";
            button5.UseVisualStyleBackColor = true;
            button5.Click += buttonKey_Click;
            // 
            // button4
            // 
            button4.Dock = DockStyle.Fill;
            button4.Font = new Font("Microsoft Sans Serif", 8.1F);
            button4.ForeColor = SystemColors.ActiveCaptionText;
            button4.Location = new Point(2, 113);
            button4.Margin = new Padding(2);
            button4.Name = "button4";
            button4.Size = new Size(126, 107);
            button4.TabIndex = 3;
            button4.Tag = "4";
            button4.Text = "4";
            button4.UseVisualStyleBackColor = true;
            button4.Click += buttonKey_Click;
            // 
            // button3
            // 
            button3.Dock = DockStyle.Fill;
            button3.Font = new Font("Microsoft Sans Serif", 8.1F);
            button3.ForeColor = SystemColors.ActiveCaptionText;
            button3.Location = new Point(262, 224);
            button3.Margin = new Padding(2);
            button3.Name = "button3";
            button3.Size = new Size(127, 107);
            button3.TabIndex = 2;
            button3.Tag = "3";
            button3.Text = "3";
            button3.UseVisualStyleBackColor = true;
            button3.Click += buttonKey_Click;
            // 
            // button0
            // 
            button0.Dock = DockStyle.Fill;
            button0.Location = new Point(132, 335);
            button0.Margin = new Padding(2);
            button0.Name = "button0";
            button0.Size = new Size(126, 107);
            button0.TabIndex = 9;
            button0.Tag = "0";
            button0.Text = "0";
            button0.UseVisualStyleBackColor = true;
            button0.Click += buttonKey_Click;
            // 
            // button7
            // 
            button7.Dock = DockStyle.Fill;
            button7.Font = new Font("Microsoft Sans Serif", 8.1F);
            button7.ForeColor = SystemColors.ActiveCaptionText;
            button7.Location = new Point(2, 2);
            button7.Margin = new Padding(2);
            button7.Name = "button7";
            button7.Size = new Size(126, 107);
            button7.TabIndex = 6;
            button7.Tag = "7";
            button7.Text = "7";
            button7.UseVisualStyleBackColor = true;
            button7.Click += buttonKey_Click;
            // 
            // buttonClear
            // 
            buttonClear.Dock = DockStyle.Fill;
            buttonClear.Location = new Point(3, 336);
            buttonClear.Name = "buttonClear";
            buttonClear.Size = new Size(124, 105);
            buttonClear.TabIndex = 12;
            buttonClear.Text = "Clear";
            buttonClear.UseVisualStyleBackColor = true;
            buttonClear.Click += buttonClear_Click;
            // 
            // buttonLogin
            // 
            buttonLogin.Dock = DockStyle.Fill;
            buttonLogin.Location = new Point(263, 336);
            buttonLogin.Name = "buttonLogin";
            buttonLogin.Size = new Size(125, 105);
            buttonLogin.TabIndex = 13;
            buttonLogin.Text = "Login";
            buttonLogin.UseVisualStyleBackColor = true;
            buttonLogin.Click += buttonLogin_Click;
            // 
            // textBoxPhone
            // 
            textBoxPhone.BackColor = Color.FromArgb(13, 31, 45);
            textBoxPhone.BorderStyle = BorderStyle.None;
            textBoxPhone.Dock = DockStyle.Bottom;
            textBoxPhone.Font = new Font("Calibri", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            textBoxPhone.ForeColor = Color.FromArgb(0, 166, 80);
            textBoxPhone.ImeMode = ImeMode.NoControl;
            textBoxPhone.Location = new Point(326, 55);
            textBoxPhone.Margin = new Padding(5);
            textBoxPhone.MaxLength = 20;
            textBoxPhone.Name = "textBoxPhone";
            textBoxPhone.PasswordChar = '*';
            textBoxPhone.PlaceholderText = "ENTER PIN";
            textBoxPhone.ReadOnly = true;
            textBoxPhone.Size = new Size(385, 59);
            textBoxPhone.TabIndex = 1;
            textBoxPhone.TextAlign = HorizontalAlignment.Center;
            // 
            // labelId
            // 
            labelId.AutoSize = true;
            labelId.BackColor = Color.Transparent;
            labelId.Dock = DockStyle.Fill;
            labelId.Font = new Font("Consolas", 20.1F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelId.ForeColor = Color.FromArgb(113, 128, 150);
            labelId.Location = new Point(718, 672);
            labelId.Margin = new Padding(2, 0, 2, 0);
            labelId.Name = "labelId";
            labelId.Size = new Size(288, 57);
            labelId.TabIndex = 3;
            labelId.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // watermarkOmadapos1
            // 
            watermarkOmadapos1.BackColor = Color.Transparent;
            watermarkOmadapos1.Dock = DockStyle.Fill;
            watermarkOmadapos1.Enabled = false;
            watermarkOmadapos1.Font = new Font("Nirmala UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            watermarkOmadapos1.Location = new Point(3, 568);
            watermarkOmadapos1.Name = "watermarkOmadapos1";
            watermarkOmadapos1.Size = new Size(315, 101);
            watermarkOmadapos1.TabIndex = 4;
            watermarkOmadapos1.Text = "watermarkOmadapos1";
            // 
            // frmSignIn
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(13, 31, 45);
            ClientSize = new Size(1008, 729);
            Controls.Add(tableLayoutPanelMain);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2, 1, 2, 1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmSignIn";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login OmadaPOS ";
            WindowState = FormWindowState.Maximized;
            Load += frmSignIn_Load;
            tableLayoutPanelMain.ResumeLayout(false);
            tableLayoutPanelMain.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanelMain;
        private TableLayoutPanel tableLayoutPanel1;
       
        private TextBox textBoxPhone;
        private Button button1;
        private Button button0;
        private Button button9;
        private Button button8;
        private Button button7;
        private Button button6;
        private Button button5;
        private Button button4;
        private Button button3;
        private Button button2;
        private Label labelId;
        private Controles.WatermarkOmadaPOS watermarkOmadapos1;
        private Button buttonClear;
        private Button buttonLogin;
    }
}