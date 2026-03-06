namespace OmadaPOS.Views
{
    partial class frmCheckPrice
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
            textUPC = new TextBox();
            labelName = new Label();
            labelPrice = new Label();
            buttonOK = new Button();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(textUPC, 0, 0);
            tableLayoutPanel1.Controls.Add(labelName, 0, 1);
            tableLayoutPanel1.Controls.Add(labelPrice, 0, 2);
            tableLayoutPanel1.Controls.Add(buttonOK, 0, 3);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(4, 2, 4, 2);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.Size = new Size(1872, 1555);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // textUPC
            // 
            textUPC.Dock = DockStyle.Fill;
            textUPC.Location = new Point(4, 2);
            textUPC.Margin = new Padding(4, 2, 4, 2);
            textUPC.Name = "textUPC";
            textUPC.Size = new Size(1864, 39);
            textUPC.TabIndex = 0;
            textUPC.TextChanged += textUPC_TextChanged;
            // 
            // labelName
            // 
            labelName.AutoSize = true;
            labelName.Dock = DockStyle.Fill;
            labelName.Location = new Point(4, 388);
            labelName.Margin = new Padding(4, 0, 4, 0);
            labelName.Name = "labelName";
            labelName.Size = new Size(1864, 388);
            labelName.TabIndex = 1;
            labelName.Text = "Name";
            labelName.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelPrice
            // 
            labelPrice.AutoSize = true;
            labelPrice.Dock = DockStyle.Fill;
            labelPrice.Location = new Point(4, 776);
            labelPrice.Margin = new Padding(4, 0, 4, 0);
            labelPrice.Name = "labelPrice";
            labelPrice.Size = new Size(1864, 388);
            labelPrice.TabIndex = 2;
            labelPrice.Text = "Price";
            labelPrice.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // buttonOK
            // 
            buttonOK.Dock = DockStyle.Fill;
            buttonOK.Location = new Point(4, 1166);
            buttonOK.Margin = new Padding(4, 2, 4, 2);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new Size(1864, 387);
            buttonOK.TabIndex = 3;
            buttonOK.Text = "OK";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += buttonOK_Click;
            // 
            // frmCheckPrice
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1872, 1555);
            Controls.Add(tableLayoutPanel1);
            Margin = new Padding(4, 2, 4, 2);
            Name = "frmCheckPrice";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "CheckPrice";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TextBox textUPC;
        private Label labelName;
        private Label labelPrice;
        private Button buttonOK;
    }
}