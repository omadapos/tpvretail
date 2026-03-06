namespace OmadaPOS.Views
{
    partial class frmCustomerScreen
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
            components = new System.ComponentModel.Container();
            tableLayoutPanelMain = new TableLayoutPanel();
            tableLayoutPanelLeft = new TableLayoutPanel();
            listViewCart = new ListView();
            panel1 = new Panel();
            tableLayoutPanelTotal = new TableLayoutPanel();
            labelWeight = new Label();
            label3 = new Label();
            label2 = new Label();
            labelTotal = new Label();
            label1 = new Label();
            panel3 = new Panel();
            labelHour = new Label();
            tableLayoutPanel2 = new TableLayoutPanel();
            pictureBoxBanner = new PictureBox();
            timer1 = new System.Windows.Forms.Timer(components);
            tableLayoutPanelMain.SuspendLayout();
            tableLayoutPanelLeft.SuspendLayout();
            panel1.SuspendLayout();
            tableLayoutPanelTotal.SuspendLayout();
            panel3.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxBanner).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanelMain
            // 
            tableLayoutPanelMain.BackColor = Color.White;
            tableLayoutPanelMain.ColumnCount = 2;
            tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelMain.Controls.Add(tableLayoutPanelLeft, 0, 1);
            tableLayoutPanelMain.Controls.Add(panel1, 0, 2);
            tableLayoutPanelMain.Controls.Add(label1, 0, 0);
            tableLayoutPanelMain.Controls.Add(panel3, 1, 2);
            tableLayoutPanelMain.Controls.Add(tableLayoutPanel2, 1, 1);
            tableLayoutPanelMain.Dock = DockStyle.Fill;
            tableLayoutPanelMain.Location = new Point(0, 0);
            tableLayoutPanelMain.Margin = new Padding(4, 2, 4, 2);
            tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            tableLayoutPanelMain.RowCount = 3;
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 115F));
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 75.7510757F));
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 24.2489262F));
            tableLayoutPanelMain.Size = new Size(1902, 1638);
            tableLayoutPanelMain.TabIndex = 0;
            // 
            // tableLayoutPanelLeft
            // 
            tableLayoutPanelLeft.ColumnCount = 1;
            tableLayoutPanelLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelLeft.Controls.Add(listViewCart, 0, 0);
            tableLayoutPanelLeft.Dock = DockStyle.Fill;
            tableLayoutPanelLeft.Location = new Point(4, 117);
            tableLayoutPanelLeft.Margin = new Padding(4, 2, 4, 2);
            tableLayoutPanelLeft.Name = "tableLayoutPanelLeft";
            tableLayoutPanelLeft.RowCount = 1;
            tableLayoutPanelLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanelLeft.Size = new Size(943, 1149);
            tableLayoutPanelLeft.TabIndex = 1;
            // 
            // listViewCart
            // 
            listViewCart.Dock = DockStyle.Fill;
            listViewCart.Location = new Point(4, 2);
            listViewCart.Margin = new Padding(4, 2, 4, 2);
            listViewCart.Name = "listViewCart";
            listViewCart.Size = new Size(935, 1145);
            listViewCart.TabIndex = 0;
            listViewCart.UseCompatibleStateImageBehavior = false;
            // 
            // panel1
            // 
            panel1.Controls.Add(tableLayoutPanelTotal);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(6, 1274);
            panel1.Margin = new Padding(6, 6, 6, 6);
            panel1.Name = "panel1";
            panel1.Size = new Size(939, 358);
            panel1.TabIndex = 3;
            panel1.Tag = "";
            panel1.Paint += panel1_Paint;
            // 
            // tableLayoutPanelTotal
            // 
            tableLayoutPanelTotal.BackColor = Color.Transparent;
            tableLayoutPanelTotal.ColumnCount = 2;
            tableLayoutPanelTotal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableLayoutPanelTotal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            tableLayoutPanelTotal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 39.8981323F));
            tableLayoutPanelTotal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60.1018677F));
            tableLayoutPanelTotal.Controls.Add(labelWeight, 1, 1);
            tableLayoutPanelTotal.Controls.Add(label3, 0, 1);
            tableLayoutPanelTotal.Controls.Add(label2, 0, 0);
            tableLayoutPanelTotal.Controls.Add(labelTotal, 1, 0);
            tableLayoutPanelTotal.Dock = DockStyle.Fill;
            tableLayoutPanelTotal.Location = new Point(0, 0);
            tableLayoutPanelTotal.Margin = new Padding(37, 43, 37, 43);
            tableLayoutPanelTotal.Name = "tableLayoutPanelTotal";
            tableLayoutPanelTotal.Padding = new Padding(93, 0, 0, 0);
            tableLayoutPanelTotal.RowCount = 3;
            tableLayoutPanelTotal.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            tableLayoutPanelTotal.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            tableLayoutPanelTotal.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanelTotal.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanelTotal.RowStyles.Add(new RowStyle(SizeType.Absolute, 96F));
            tableLayoutPanelTotal.RowStyles.Add(new RowStyle(SizeType.Absolute, 43F));
            tableLayoutPanelTotal.Size = new Size(939, 358);
            tableLayoutPanelTotal.TabIndex = 2;
            // 
            // labelWeight
            // 
            labelWeight.AutoSize = true;
            labelWeight.Dock = DockStyle.Fill;
            labelWeight.Font = new Font("Microsoft Sans Serif", 26.25F, FontStyle.Bold);
            labelWeight.ForeColor = Color.Black;
            labelWeight.Location = new Point(435, 143);
            labelWeight.Margin = new Padding(4, 0, 4, 0);
            labelWeight.Name = "labelWeight";
            labelWeight.Size = new Size(500, 143);
            labelWeight.TabIndex = 2;
            labelWeight.Text = "0.0lb";
            labelWeight.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("Microsoft Sans Serif", 26.25F, FontStyle.Bold);
            label3.ForeColor = Color.Coral;
            label3.ImageAlign = ContentAlignment.MiddleRight;
            label3.Location = new Point(97, 143);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(258, 79);
            label3.TabIndex = 1;
            label3.Text = "Weight";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Dock = DockStyle.Fill;
            label2.FlatStyle = FlatStyle.Flat;
            label2.Font = new Font("Microsoft Sans Serif", 26.25F, FontStyle.Bold);
            label2.ForeColor = Color.Black;
            label2.ImageAlign = ContentAlignment.MiddleRight;
            label2.Location = new Point(97, 0);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(330, 143);
            label2.TabIndex = 0;
            label2.Text = "Total";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // labelTotal
            // 
            labelTotal.AutoSize = true;
            labelTotal.Dock = DockStyle.Fill;
            labelTotal.Font = new Font("Microsoft Sans Serif", 26.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelTotal.ForeColor = Color.Black;
            labelTotal.Location = new Point(435, 0);
            labelTotal.Margin = new Padding(4, 0, 4, 0);
            labelTotal.Name = "labelTotal";
            labelTotal.Size = new Size(500, 143);
            labelTotal.TabIndex = 1;
            labelTotal.Text = "0.00$";
            labelTotal.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.FlatStyle = FlatStyle.Popup;
            label1.Font = new Font("Microsoft Sans Serif", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.RoyalBlue;
            label1.Location = new Point(6, 0);
            label1.Margin = new Padding(6, 0, 6, 0);
            label1.Name = "label1";
            label1.Size = new Size(939, 115);
            label1.TabIndex = 2;
            label1.Tag = "OmadaPOS";
            label1.Text = "Supermarket";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel3
            // 
            panel3.BackColor = Color.White;
            panel3.Controls.Add(labelHour);
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new Point(957, 1274);
            panel3.Margin = new Padding(6, 6, 6, 6);
            panel3.Name = "panel3";
            panel3.Size = new Size(939, 358);
            panel3.TabIndex = 6;
            // 
            // labelHour
            // 
            labelHour.AutoSize = true;
            labelHour.BackColor = Color.Transparent;
            labelHour.Dock = DockStyle.Fill;
            labelHour.FlatStyle = FlatStyle.Flat;
            labelHour.Font = new Font("Microsoft Sans Serif", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelHour.ForeColor = Color.Black;
            labelHour.Location = new Point(0, 0);
            labelHour.Margin = new Padding(4, 0, 4, 0);
            labelHour.Name = "labelHour";
            labelHour.Size = new Size(290, 108);
            labelHour.TabIndex = 0;
            labelHour.Text = "Time:";
            labelHour.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(pictureBoxBanner, 0, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(957, 121);
            tableLayoutPanel2.Margin = new Padding(6, 6, 6, 6);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 54.00517F));
            tableLayoutPanel2.Size = new Size(939, 1141);
            tableLayoutPanel2.TabIndex = 7;
            // 
            // pictureBoxBanner
            // 
            pictureBoxBanner.BackColor = Color.Transparent;
            pictureBoxBanner.BackgroundImageLayout = ImageLayout.None;
            pictureBoxBanner.Dock = DockStyle.Fill;
            pictureBoxBanner.Location = new Point(0, 0);
            pictureBoxBanner.Margin = new Padding(0);
            pictureBoxBanner.Name = "pictureBoxBanner";
            pictureBoxBanner.Size = new Size(939, 1141);
            pictureBoxBanner.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxBanner.TabIndex = 1;
            pictureBoxBanner.TabStop = false;
            // 
            // frmCustomerScreen
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1902, 1638);
            Controls.Add(tableLayoutPanelMain);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4, 2, 4, 2);
            Name = "frmCustomerScreen";
            Text = "Customer Screen";
            TopMost = true;
            WindowState = FormWindowState.Maximized;
            tableLayoutPanelMain.ResumeLayout(false);
            tableLayoutPanelMain.PerformLayout();
            tableLayoutPanelLeft.ResumeLayout(false);
            panel1.ResumeLayout(false);
            tableLayoutPanelTotal.ResumeLayout(false);
            tableLayoutPanelTotal.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxBanner).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanelMain;
        private PictureBox pictureBoxBanner;
        private TableLayoutPanel tableLayoutPanelLeft;
        private Label labelHour;
        private System.Windows.Forms.Timer timer1;
        private TableLayoutPanel tableLayoutPanel2;
        private Label label1;
        private ListView listViewCart;
        private Panel panel1;
        private TableLayoutPanel tableLayoutPanelTotal;
        private Label labelWeight;
        private Label label3;
        private Label label2;
        private Label labelTotal;
        private Panel panel3;
    }
}