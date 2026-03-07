namespace OmadaPOS.Views
{
    partial class frmKeyLookup: Form
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
            keyPadControl1 = new OmadaPOS.Componentes.NumericPadControl(OmadaPOS.Componentes.NumericPadControl.PadMode.Integer);
            buttonOK = new Button();
            buttonCancel = new Button();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // keyPadControl1
            // 
            keyPadControl1.Dock     = DockStyle.Fill;
            keyPadControl1.Name     = "keyPadControl1";
            keyPadControl1.TabIndex = 0;
            // 
            // buttonOK
            // 
            buttonOK.BackColor = Color.White;
            buttonOK.Dock = DockStyle.Fill;
            buttonOK.FlatStyle = FlatStyle.Flat;
            buttonOK.Font = new Font("Segoe UI", 9F);
            buttonOK.ForeColor = Color.Black;
            buttonOK.Location = new Point(201, 1);
            buttonOK.Margin = new Padding(1);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new Size(198, 78);
            buttonOK.TabIndex = 11;
            buttonOK.Text = "OK";
            buttonOK.UseVisualStyleBackColor = false;
            buttonOK.Click += buttonOK_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.BackColor = Color.WhiteSmoke;
            buttonCancel.Dock = DockStyle.Fill;
            buttonCancel.FlatStyle = FlatStyle.Flat;
            buttonCancel.Font = new Font("Segoe UI", 9F);
            buttonCancel.ForeColor = Color.Black;
            buttonCancel.Location = new Point(1, 1);
            buttonCancel.Margin = new Padding(1);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(198, 78);
            buttonCancel.TabIndex = 10;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = false;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 1);
            tableLayoutPanel1.Controls.Add(keyPadControl1, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(2);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 80F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.Size = new Size(1008, 729);
            tableLayoutPanel1.TabIndex = 13;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.Anchor = AnchorStyles.None;
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(buttonCancel, 0, 0);
            tableLayoutPanel2.Controls.Add(buttonOK, 1, 0);
            tableLayoutPanel2.Location = new Point(304, 616);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Size = new Size(400, 80);
            tableLayoutPanel2.TabIndex = 12;
            // 
            // frmKeyLookup
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1008, 729);
            Controls.Add(tableLayoutPanel1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(2, 1, 2, 1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmKeyLookup";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "UPC LOOKUP";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Componentes.NumericPadControl keyPadControl1;
        private Button buttonOK;
        private Button buttonCancel;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
    }
}