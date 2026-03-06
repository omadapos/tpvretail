namespace OmadaPOS.Views
{
    partial class frmPopupCashPayment: Form
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
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            label1 = new Label();
            labelDevuelta = new Label();
            labelInvoice = new Label();
            tableLayoutPanel3 = new TableLayoutPanel();
            buttonClose = new Button();
            buttonPrint = new Button();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.BackColor = Color.White;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 0);
            tableLayoutPanel1.Controls.Add(labelInvoice, 0, 1);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel3, 0, 2);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(10);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.Size = new Size(1008, 729);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.Anchor = AnchorStyles.None;
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(label1, 0, 0);
            tableLayoutPanel2.Controls.Add(labelDevuelta, 1, 0);
            tableLayoutPanel2.Location = new Point(257, 95);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Size = new Size(493, 100);
            tableLayoutPanel2.TabIndex = 4;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.None;
            label1.AutoSize = true;
            label1.Font = new Font("Consolas", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(20, 22);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(206, 56);
            label1.TabIndex = 0;
            label1.Text = "Change:";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelDevuelta
            // 
            labelDevuelta.Anchor = AnchorStyles.None;
            labelDevuelta.AutoSize = true;
            labelDevuelta.Font = new Font("Consolas", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelDevuelta.Location = new Point(305, 22);
            labelDevuelta.Margin = new Padding(2, 0, 2, 0);
            labelDevuelta.Name = "labelDevuelta";
            labelDevuelta.Size = new Size(128, 56);
            labelDevuelta.TabIndex = 1;
            labelDevuelta.Text = "0.00";
            labelDevuelta.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelInvoice
            // 
            labelInvoice.Anchor = AnchorStyles.None;
            labelInvoice.AutoSize = true;
            labelInvoice.Font = new Font("Consolas", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelInvoice.Location = new Point(479, 408);
            labelInvoice.Margin = new Padding(2, 0, 2, 0);
            labelInvoice.Name = "labelInvoice";
            labelInvoice.Size = new Size(50, 56);
            labelInvoice.TabIndex = 3;
            labelInvoice.Text = "0";
            labelInvoice.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.Anchor = AnchorStyles.None;
            tableLayoutPanel3.ColumnCount = 2;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.Controls.Add(buttonClose, 0, 0);
            tableLayoutPanel3.Controls.Add(buttonPrint, 1, 0);
            tableLayoutPanel3.Location = new Point(243, 614);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.Size = new Size(521, 82);
            tableLayoutPanel3.TabIndex = 5;
            // 
            // buttonClose
            // 
            buttonClose.Dock = DockStyle.Fill;
            buttonClose.Location = new Point(3, 3);
            buttonClose.Name = "buttonClose";
            buttonClose.Size = new Size(254, 76);
            buttonClose.TabIndex = 0;
            buttonClose.Text = "Close";
            buttonClose.UseVisualStyleBackColor = true;
            buttonClose.Click += buttonClose_Click;
            // 
            // buttonPrint
            // 
            buttonPrint.Dock = DockStyle.Fill;
            buttonPrint.Location = new Point(263, 3);
            buttonPrint.Name = "buttonPrint";
            buttonPrint.Size = new Size(255, 76);
            buttonPrint.TabIndex = 1;
            buttonPrint.Text = "Print";
            buttonPrint.UseVisualStyleBackColor = true;
            buttonPrint.Click += buttonPrint_Click;
            // 
            // frmPopupCashPayment
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1008, 729);
            Controls.Add(tableLayoutPanel1);
            Margin = new Padding(2, 1, 2, 1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmPopupCashPayment";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Info Payment";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tableLayoutPanel3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Label label1;
        private Label labelInvoice;
        private Label labelDevuelta;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel3;
        private Button buttonClose;
        private Button buttonPrint;
    }
}