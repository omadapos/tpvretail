namespace OmadaPOS.Views
{
    partial class frmProductNew: Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmProductNew));
            tableLayoutPanel1 = new TableLayoutPanel();
            buttonCancel = new Button();
            buttonOk = new Button();
            tableLayoutPanel2 = new TableLayoutPanel();
            keyPadMoneyControl1 = new OmadaPOS.Componentes.KeyPadMoneyControl();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.None;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(buttonCancel, 0, 0);
            tableLayoutPanel1.Controls.Add(buttonOk, 1, 0);
            tableLayoutPanel1.Location = new Point(564, 1314);
            tableLayoutPanel1.Margin = new Padding(4);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(743, 171);
            tableLayoutPanel1.TabIndex = 14;
            // 
            // buttonCancel
            // 
            buttonCancel.Dock = DockStyle.Fill;
            buttonCancel.Location = new Point(4, 4);
            buttonCancel.Margin = new Padding(4);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(363, 163);
            buttonCancel.TabIndex = 0;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonClose_Click;
            // 
            // buttonOk
            // 
            buttonOk.Dock = DockStyle.Fill;
            buttonOk.Location = new Point(375, 4);
            buttonOk.Margin = new Padding(4);
            buttonOk.Name = "buttonOk";
            buttonOk.Size = new Size(364, 163);
            buttonOk.TabIndex = 1;
            buttonOk.Text = "ok";
            buttonOk.UseVisualStyleBackColor = true;
            buttonOk.Click += buttonOK_Click;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(tableLayoutPanel1, 0, 1);
            tableLayoutPanel2.Controls.Add(keyPadMoneyControl1, 0, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(0, 0);
            tableLayoutPanel2.Margin = new Padding(6);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 80F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel2.Size = new Size(1872, 1555);
            tableLayoutPanel2.TabIndex = 15;
            // 
            // keyPadMoneyControl1
            // 
            keyPadMoneyControl1.Dock = DockStyle.Fill;
            keyPadMoneyControl1.Location = new Point(4, 2);
            keyPadMoneyControl1.Margin = new Padding(4, 2, 4, 2);
            keyPadMoneyControl1.Name = "keyPadMoneyControl1";
            keyPadMoneyControl1.Size = new Size(1864, 1240);
            keyPadMoneyControl1.TabIndex = 15;
            // 
            // frmProductNew
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1872, 1555);
            Controls.Add(tableLayoutPanel2);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 2, 4, 2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmProductNew";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Product New";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private TableLayoutPanel tableLayoutPanel1;
        private Button buttonCancel;
        private Button buttonOk;
        private TableLayoutPanel tableLayoutPanel2;
        private Componentes.KeyPadMoneyControl keyPadMoneyControl1;
    }
}