namespace OmadaPOS.Views
{
    partial class frmProductNoExist: Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmProductNoExist));
            textBoxUPC = new TextBox();
            textBoxName = new TextBox();
            picThumb = new PictureBox();
            checkBoxTax = new CheckBox();
            checkBoxEBT = new CheckBox();
            buttonCancel = new Button();
            buttonOk = new Button();
            label1 = new Label();
            label2 = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            panel1 = new Panel();
            tableLayoutPanel2 = new TableLayoutPanel();
            keyPadMoneyControl1 = new OmadaPOS.Componentes.NumericPadControl(OmadaPOS.Componentes.NumericPadControl.PadMode.Money);
            ((System.ComponentModel.ISupportInitialize)picThumb).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            panel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // textBoxUPC
            // 
            textBoxUPC.Location = new Point(273, 58);
            textBoxUPC.Margin = new Padding(4, 2, 4, 2);
            textBoxUPC.Name = "textBoxUPC";
            textBoxUPC.PlaceholderText = "UPC";
            textBoxUPC.Size = new Size(372, 39);
            textBoxUPC.TabIndex = 0;
            // 
            // textBoxName
            // 
            textBoxName.Location = new Point(273, 205);
            textBoxName.Margin = new Padding(4, 2, 4, 2);
            textBoxName.Name = "textBoxName";
            textBoxName.PlaceholderText = "Name";
            textBoxName.Size = new Size(372, 39);
            textBoxName.TabIndex = 1;
            // 
            // picThumb
            // 
            picThumb.Dock = DockStyle.Bottom;
            picThumb.Location = new Point(0, 496);
            picThumb.Margin = new Padding(4, 2, 4, 2);
            picThumb.Name = "picThumb";
            picThumb.Size = new Size(924, 425);
            picThumb.TabIndex = 2;
            picThumb.TabStop = false;
            // 
            // checkBoxTax
            // 
            checkBoxTax.AutoSize = true;
            checkBoxTax.Location = new Point(568, 365);
            checkBoxTax.Margin = new Padding(4, 2, 4, 2);
            checkBoxTax.Name = "checkBoxTax";
            checkBoxTax.Size = new Size(79, 36);
            checkBoxTax.TabIndex = 4;
            checkBoxTax.Text = "Tax";
            checkBoxTax.UseVisualStyleBackColor = true;
            // 
            // checkBoxEBT
            // 
            checkBoxEBT.AutoSize = true;
            checkBoxEBT.Location = new Point(71, 365);
            checkBoxEBT.Margin = new Padding(4, 2, 4, 2);
            checkBoxEBT.Name = "checkBoxEBT";
            checkBoxEBT.Size = new Size(84, 36);
            checkBoxEBT.TabIndex = 5;
            checkBoxEBT.Text = "EBT";
            checkBoxEBT.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            buttonCancel.Dock = DockStyle.Fill;
            buttonCancel.Location = new Point(4, 2);
            buttonCancel.Margin = new Padding(4, 2, 4, 2);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(454, 139);
            buttonCancel.TabIndex = 6;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // buttonOk
            // 
            buttonOk.Dock = DockStyle.Fill;
            buttonOk.Location = new Point(466, 2);
            buttonOk.Margin = new Padding(4, 2, 4, 2);
            buttonOk.Name = "buttonOk";
            buttonOk.Size = new Size(454, 139);
            buttonOk.TabIndex = 7;
            buttonOk.Text = "OK";
            buttonOk.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(87, 64);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(58, 32);
            label1.TabIndex = 8;
            label1.Text = "UPC";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(71, 211);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(78, 32);
            label2.TabIndex = 9;
            label2.Text = "Name";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(panel1, 0, 0);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 1, 1);
            tableLayoutPanel1.Controls.Add(keyPadMoneyControl1, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(6, 6, 6, 6);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            tableLayoutPanel1.Size = new Size(1872, 1555);
            tableLayoutPanel1.TabIndex = 10;
            // 
            // panel1
            // 
            panel1.Controls.Add(textBoxUPC);
            panel1.Controls.Add(checkBoxTax);
            panel1.Controls.Add(checkBoxEBT);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(picThumb);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(textBoxName);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(6, 6);
            panel1.Margin = new Padding(6, 6, 6, 6);
            panel1.Name = "panel1";
            panel1.Size = new Size(924, 921);
            panel1.TabIndex = 8;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(buttonOk, 1, 0);
            tableLayoutPanel2.Controls.Add(buttonCancel, 0, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(942, 939);
            tableLayoutPanel2.Margin = new Padding(6, 6, 6, 6);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Size = new Size(924, 143);
            tableLayoutPanel2.TabIndex = 9;
            // 
            // keyPadMoneyControl1
            // 
            keyPadMoneyControl1.Dock     = DockStyle.Fill;
            keyPadMoneyControl1.Name     = "keyPadMoneyControl1";
            keyPadMoneyControl1.TabIndex = 10;
            // 
            // frmProductNoExist
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1872, 1555);
            Controls.Add(tableLayoutPanel1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 2, 4, 2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmProductNoExist";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Product No Exist";
            Load += frmProductNoExist_Load;
            ((System.ComponentModel.ISupportInitialize)picThumb).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TextBox textBoxUPC;
        private TextBox textBoxName;
        private PictureBox picThumb;
        private CheckBox checkBoxTax;
        private CheckBox checkBoxEBT;
        private Button buttonCancel;
        private Button buttonOk;
        private Label label1;
        private Label label2;
        private TableLayoutPanel tableLayoutPanel1;
        private Panel panel1;
        private TableLayoutPanel tableLayoutPanel2;
        private Componentes.NumericPadControl keyPadMoneyControl1;
    }
}