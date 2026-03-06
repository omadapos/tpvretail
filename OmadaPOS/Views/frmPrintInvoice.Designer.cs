namespace OmadaPOS.Views
{
    partial class frmPrintInvoice
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPrintInvoice));
            tableLayoutPanel1 = new TableLayoutPanel();
            listViewInvoices = new ListView();
            tableLayoutPanel2 = new TableLayoutPanel();
            dateTimePicker1 = new DateTimePicker();
            dateTimePicker2 = new DateTimePicker();
            buttonSearch = new Button();
            label1 = new Label();
            listViewProducts = new ListView();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableLayoutPanel1.Controls.Add(listViewInvoices, 0, 1);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 0);
            tableLayoutPanel1.Controls.Add(label1, 1, 0);
            tableLayoutPanel1.Controls.Add(listViewProducts, 1, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(2, 1, 2, 1);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 5.76368856F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 94.23631F));
            tableLayoutPanel1.Size = new Size(1904, 1041);
            tableLayoutPanel1.TabIndex = 2;
            // 
            // listViewInvoices
            // 
            listViewInvoices.Alignment = ListViewAlignment.Default;
            listViewInvoices.BackColor = Color.White;
            listViewInvoices.BorderStyle = BorderStyle.FixedSingle;
            listViewInvoices.Dock = DockStyle.Fill;
            listViewInvoices.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            listViewInvoices.ForeColor = Color.Black;
            listViewInvoices.Location = new Point(2, 62);
            listViewInvoices.Margin = new Padding(2);
            listViewInvoices.Name = "listViewInvoices";
            listViewInvoices.Size = new Size(1138, 977);
            listViewInvoices.TabIndex = 4;
            listViewInvoices.UseCompatibleStateImageBehavior = false;
            listViewInvoices.SelectedIndexChanged += listViewInvoices_SelectedIndexChanged;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableLayoutPanel2.Controls.Add(dateTimePicker1, 0, 0);
            tableLayoutPanel2.Controls.Add(dateTimePicker2, 1, 0);
            tableLayoutPanel2.Controls.Add(buttonSearch, 2, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(2, 1);
            tableLayoutPanel2.Margin = new Padding(2, 1, 2, 1);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(1138, 58);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Dock = DockStyle.Fill;
            dateTimePicker1.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dateTimePicker1.Location = new Point(20, 10);
            dateTimePicker1.Margin = new Padding(20, 10, 10, 10);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(311, 31);
            dateTimePicker1.TabIndex = 2;
            // 
            // dateTimePicker2
            // 
            dateTimePicker2.Dock = DockStyle.Fill;
            dateTimePicker2.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold);
            dateTimePicker2.Location = new Point(361, 10);
            dateTimePicker2.Margin = new Padding(20, 10, 10, 10);
            dateTimePicker2.Name = "dateTimePicker2";
            dateTimePicker2.Size = new Size(311, 31);
            dateTimePicker2.TabIndex = 3;
            // 
            // buttonSearch
            // 
            buttonSearch.Dock = DockStyle.Fill;
            buttonSearch.FlatAppearance.BorderColor = Color.FromArgb(0, 192, 0);
            buttonSearch.FlatStyle = FlatStyle.System;
            buttonSearch.Font = new Font("Microsoft Sans Serif", 24F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonSearch.Location = new Point(684, 2);
            buttonSearch.Margin = new Padding(2);
            buttonSearch.Name = "buttonSearch";
            buttonSearch.Size = new Size(452, 54);
            buttonSearch.TabIndex = 4;
            buttonSearch.Text = "Search";
            buttonSearch.UseVisualStyleBackColor = false;
            buttonSearch.Click += buttonSearch_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(1145, 0);
            label1.Name = "label1";
            label1.Size = new Size(756, 60);
            label1.TabIndex = 5;
            label1.Text = "Invoice Details ";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // listViewProducts
            // 
            listViewProducts.Dock = DockStyle.Fill;
            listViewProducts.Location = new Point(1144, 61);
            listViewProducts.Margin = new Padding(2, 1, 2, 1);
            listViewProducts.Name = "listViewProducts";
            listViewProducts.Size = new Size(758, 979);
            listViewProducts.TabIndex = 6;
            listViewProducts.UseCompatibleStateImageBehavior = false;
            // 
            // frmPrintInvoice
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1904, 1041);
            Controls.Add(tableLayoutPanel1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2, 1, 2, 1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmPrintInvoice";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PRINT INVOICE";
            Load += frmPrintInvoice_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private TableLayoutPanel tableLayoutPanel1;
        private ListView listViewInvoices;
        private TableLayoutPanel tableLayoutPanel2;
        private Button buttonSearch;
        private DateTimePicker dateTimePicker1;
        private DateTimePicker dateTimePicker2;
        private Label label1;
        private ListView listViewProducts;
    }
}