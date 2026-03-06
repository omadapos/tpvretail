namespace OmadaPOS.Views
{
    partial class frmPopupQuantity: Form
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
            buttonOK = new Button();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            buttonCancel = new Button();
            keyPadControl1 = new OmadaPOS.Componentes.KeyPadControl();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // buttonOK
            // 
            buttonOK.Dock = DockStyle.Fill;
            buttonOK.ForeColor = Color.Black;
            buttonOK.Location = new Point(202, 1);
            buttonOK.Margin = new Padding(2, 1, 2, 1);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new Size(196, 78);
            buttonOK.TabIndex = 9;
            buttonOK.Text = "OK";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += buttonOK_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.BackColor = Color.FromArgb(247, 248, 250);
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 1);
            tableLayoutPanel1.Controls.Add(keyPadControl1, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 80F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.Size = new Size(1008, 729);
            tableLayoutPanel1.TabIndex = 11;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.Anchor = AnchorStyles.None;
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(buttonOK, 1, 0);
            tableLayoutPanel2.Controls.Add(buttonCancel, 0, 0);
            tableLayoutPanel2.Location = new Point(304, 616);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Size = new Size(400, 80);
            tableLayoutPanel2.TabIndex = 10;
            // 
            // buttonCancel
            // 
            buttonCancel.Dock = DockStyle.Fill;
            buttonCancel.Font = new Font("Segoe UI", 9F);
            buttonCancel.ForeColor = Color.Black;
            buttonCancel.Location = new Point(3, 3);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(194, 74);
            buttonCancel.TabIndex = 10;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonClose_Click;
            // 
            // keyPadControl1
            // 
            keyPadControl1.Anchor = AnchorStyles.None;
            keyPadControl1.AutoSize = true;
            keyPadControl1.BackColor = Color.FromArgb(247, 248, 250);
            keyPadControl1.ForeColor = Color.FromArgb(26, 32, 44);
            keyPadControl1.Location = new Point(21, 19);
            keyPadControl1.Margin = new Padding(2, 1, 2, 1);
            keyPadControl1.Name = "keyPadControl1";
            keyPadControl1.Size = new Size(965, 545);
            keyPadControl1.TabIndex = 11;
            // 
            // frmPopupQuantity
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(247, 248, 250);
            ClientSize = new Size(1008, 729);
            Controls.Add(tableLayoutPanel1);
            Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(2, 1, 2, 1);
            Name = "frmPopupQuantity";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "CHANGE QTY";
            Load += frmPopupQuantity_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Button buttonOK;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private Button buttonCancel;
        private Componentes.KeyPadControl keyPadControl1;
    }
}