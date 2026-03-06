namespace OmadaPOS.Views
{
    partial class frmGiftCard: OmadaPOS.Estilos.EstiloFormularioPOS
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
            labelTotal = new Label();
            labelSaldo = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            textBoxCode = new TextBox();
            tableLayoutPanel2 = new TableLayoutPanel();
            buttonCancel = new Button();
            buttonPay = new Button();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // labelTotal
            // 
            labelTotal.AutoSize = true;
            labelTotal.Dock = DockStyle.Fill;
            labelTotal.Font = new Font("Segoe UI", 30F);
            labelTotal.Location = new Point(6, 365);
            labelTotal.Margin = new Padding(6, 0, 6, 0);
            labelTotal.Name = "labelTotal";
            labelTotal.Size = new Size(2036, 365);
            labelTotal.TabIndex = 0;
            labelTotal.Text = "0.0";
            labelTotal.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelSaldo
            // 
            labelSaldo.AutoSize = true;
            labelSaldo.Dock = DockStyle.Fill;
            labelSaldo.Font = new Font("Segoe UI", 27.75F);
            labelSaldo.Location = new Point(6, 730);
            labelSaldo.Margin = new Padding(6, 0, 6, 0);
            labelSaldo.Name = "labelSaldo";
            labelSaldo.Size = new Size(2036, 365);
            labelSaldo.TabIndex = 1;
            labelSaldo.Text = "Saldo";
            labelSaldo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.BackColor = Color.White;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(labelTotal, 0, 1);
            tableLayoutPanel1.Controls.Add(labelSaldo, 0, 2);
            tableLayoutPanel1.Controls.Add(textBoxCode, 0, 0);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 3);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(6, 2, 6, 2);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 22.2222214F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 22.2222233F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 22.2222233F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333359F));
            tableLayoutPanel1.Size = new Size(2048, 1646);
            tableLayoutPanel1.TabIndex = 2;
            // 
            // textBoxCode
            // 
            textBoxCode.Dock = DockStyle.Fill;
            textBoxCode.Location = new Point(6, 2);
            textBoxCode.Margin = new Padding(6, 2, 6, 2);
            textBoxCode.Name = "textBoxCode";
            textBoxCode.Size = new Size(2036, 50);
            textBoxCode.TabIndex = 2;
            textBoxCode.TextChanged += textBoxCode_TextChanged;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.BackColor = Color.White;
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(buttonCancel, 0, 0);
            tableLayoutPanel2.Controls.Add(buttonPay, 1, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(6, 1097);
            tableLayoutPanel2.Margin = new Padding(6, 2, 6, 2);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Size = new Size(2036, 547);
            tableLayoutPanel2.TabIndex = 3;
            // 
            // buttonCancel
            // 
            buttonCancel.Dock = DockStyle.Fill;
            buttonCancel.Font = new Font("Segoe UI", 27F);
            buttonCancel.Location = new Point(6, 2);
            buttonCancel.Margin = new Padding(6, 2, 6, 2);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(1006, 543);
            buttonCancel.TabIndex = 0;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // buttonPay
            // 
            buttonPay.Dock = DockStyle.Fill;
            buttonPay.Font = new Font("Segoe UI", 27F);
            buttonPay.Location = new Point(1024, 2);
            buttonPay.Margin = new Padding(6, 2, 6, 2);
            buttonPay.Name = "buttonPay";
            buttonPay.Size = new Size(1006, 543);
            buttonPay.TabIndex = 1;
            buttonPay.Text = "PAY";
            buttonPay.UseVisualStyleBackColor = true;
            buttonPay.Click += buttonPay_Click;
            // 
            // frmGiftCard
            // 
            AutoScaleDimensions = new SizeF(18F, 45F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(2048, 1646);
            Controls.Add(tableLayoutPanel1);
            Location = new Point(0, 0);
            Margin = new Padding(6, 2, 6, 2);
            Name = "frmGiftCard";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "GiftCard";
            Load += frmGiftCard_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Label labelTotal;
        private Label labelSaldo;
        private TableLayoutPanel tableLayoutPanel1;
        private TextBox textBoxCode;
        private TableLayoutPanel tableLayoutPanel2;
        private Button buttonCancel;
        private Button buttonPay;
    }
}