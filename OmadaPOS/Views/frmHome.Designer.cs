namespace OmadaPOS.Views
{
    partial class frmHome
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
            tableLayoutPanelMain = new TableLayoutPanel();
            tableLayoutPanelCart = new TableLayoutPanel();
            listViewCart = new ListView();
            tableLayoutPanelButtonCart = new TableLayoutPanel();
            buttonHold = new Button();
            buttonDeleteItem = new Button();
            buttonChangeQuantity = new Button();
            buttonCancelOrder = new Button();
            tableLayoutPanelCategoria = new TableLayoutPanel();
            tabControlMenuCategories = new TabControl();
            tableLayoutPanelPayment = new TableLayoutPanel();
            textBoxUPC = new TextBox();
            buttonInvoice = new Button();
            buttonClose = new Button();
            labelCashier = new Button();
            ButtonSettings = new Button();
            tableLayoutPanelTotal = new TableLayoutPanel();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            labelSubTotal = new Label();
            labelTotalTax = new Label();
            labelTotalValue = new Label();
            labelProductName = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            MaintableLayout = new TableLayoutPanel();
            pnlProductStrip = new Panel();
            tableLayoutPanelMain.SuspendLayout();
            tableLayoutPanelCart.SuspendLayout();
            tableLayoutPanelButtonCart.SuspendLayout();
            tableLayoutPanelCategoria.SuspendLayout();
            tableLayoutPanelTotal.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            MaintableLayout.SuspendLayout();
            pnlProductStrip.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanelMain
            // 
            tableLayoutPanelMain.BackColor = Color.WhiteSmoke;
            tableLayoutPanelMain.ColumnCount = 3;
            tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanelMain.Controls.Add(tableLayoutPanelCart, 0, 0);
            tableLayoutPanelMain.Controls.Add(tableLayoutPanelCategoria, 1, 0);
            tableLayoutPanelMain.Controls.Add(tableLayoutPanelPayment, 2, 0);
            tableLayoutPanelMain.Dock = DockStyle.Fill;
            tableLayoutPanelMain.Location = new Point(3, 102);
            tableLayoutPanelMain.Margin = new Padding(3, 2, 3, 2);
            tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            tableLayoutPanelMain.RowCount = 1;
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelMain.Size = new Size(2070, 772);
            tableLayoutPanelMain.TabIndex = 0;
            // 
            // tableLayoutPanelCart
            // 
            tableLayoutPanelCart.BackColor = Color.WhiteSmoke;
            tableLayoutPanelCart.ColumnCount = 1;
            tableLayoutPanelCart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelCart.Controls.Add(listViewCart, 0, 0);
            tableLayoutPanelCart.Controls.Add(tableLayoutPanelButtonCart, 0, 2);
            tableLayoutPanelCart.Dock = DockStyle.Fill;
            tableLayoutPanelCart.Location = new Point(0, 0);
            tableLayoutPanelCart.Margin = new Padding(0);
            tableLayoutPanelCart.Name = "tableLayoutPanelCart";
            tableLayoutPanelCart.RowCount = 3;
            tableLayoutPanelCart.RowStyles.Add(new RowStyle(SizeType.Percent, 73.68421F));
            tableLayoutPanelCart.RowStyles.Add(new RowStyle(SizeType.Percent, 15.7894735F));
            tableLayoutPanelCart.RowStyles.Add(new RowStyle(SizeType.Percent, 10.5263157F));
            tableLayoutPanelCart.Size = new Size(621, 772);
            tableLayoutPanelCart.TabIndex = 0;
            // 
            // listViewCart
            // 
            listViewCart.BackColor = Color.WhiteSmoke;
            listViewCart.Dock = DockStyle.Fill;
            listViewCart.Location = new Point(3, 3);
            listViewCart.Name = "listViewCart";
            listViewCart.Size = new Size(615, 562);
            listViewCart.TabIndex = 4;
            listViewCart.UseCompatibleStateImageBehavior = false;
            // 
            // tableLayoutPanelButtonCart
            // 
            tableLayoutPanelButtonCart.BackColor = Color.WhiteSmoke;
            tableLayoutPanelButtonCart.ColumnCount = 4;
            tableLayoutPanelButtonCart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanelButtonCart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanelButtonCart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanelButtonCart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanelButtonCart.Controls.Add(buttonHold, 3, 0);
            tableLayoutPanelButtonCart.Controls.Add(buttonDeleteItem, 2, 0);
            tableLayoutPanelButtonCart.Controls.Add(buttonChangeQuantity, 1, 0);
            tableLayoutPanelButtonCart.Controls.Add(buttonCancelOrder, 0, 0);
            tableLayoutPanelButtonCart.Dock = DockStyle.Fill;
            tableLayoutPanelButtonCart.Location = new Point(3, 691);
            tableLayoutPanelButtonCart.Margin = new Padding(3, 2, 3, 2);
            tableLayoutPanelButtonCart.Name = "tableLayoutPanelButtonCart";
            tableLayoutPanelButtonCart.RowCount = 1;
            tableLayoutPanelButtonCart.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelButtonCart.Size = new Size(615, 79);
            tableLayoutPanelButtonCart.TabIndex = 3;
            // 
            // buttonHold
            // 
            buttonHold.Dock = DockStyle.Fill;
            buttonHold.Font = new Font("Microsoft Sans Serif", 8.25F);
            buttonHold.Location = new Point(462, 3);
            buttonHold.Name = "buttonHold";
            buttonHold.Size = new Size(150, 73);
            buttonHold.TabIndex = 3;
            buttonHold.Text = "HOLD";
            buttonHold.UseVisualStyleBackColor = true;
            buttonHold.Click += buttonHold_Click;
            // 
            // buttonDeleteItem
            // 
            buttonDeleteItem.Dock = DockStyle.Fill;
            buttonDeleteItem.Font = new Font("Microsoft Sans Serif", 8.25F);
            buttonDeleteItem.Location = new Point(309, 3);
            buttonDeleteItem.Name = "buttonDeleteItem";
            buttonDeleteItem.Size = new Size(147, 73);
            buttonDeleteItem.TabIndex = 2;
            buttonDeleteItem.Text = "Delete";
            buttonDeleteItem.UseVisualStyleBackColor = true;
            buttonDeleteItem.Click += buttonDeleteItem_Click;
            // 
            // buttonChangeQuantity
            // 
            buttonChangeQuantity.Dock = DockStyle.Fill;
            buttonChangeQuantity.Font = new Font("Microsoft Sans Serif", 8.25F);
            buttonChangeQuantity.Location = new Point(156, 3);
            buttonChangeQuantity.Name = "buttonChangeQuantity";
            buttonChangeQuantity.Size = new Size(147, 73);
            buttonChangeQuantity.TabIndex = 1;
            buttonChangeQuantity.Text = "Change";
            buttonChangeQuantity.UseVisualStyleBackColor = true;
            buttonChangeQuantity.Click += buttonChangeQuantity_Click;
            // 
            // buttonCancelOrder
            // 
            buttonCancelOrder.Dock = DockStyle.Fill;
            buttonCancelOrder.Font = new Font("Microsoft Sans Serif", 8.25F);
            buttonCancelOrder.Location = new Point(3, 3);
            buttonCancelOrder.Name = "buttonCancelOrder";
            buttonCancelOrder.Size = new Size(147, 73);
            buttonCancelOrder.TabIndex = 0;
            buttonCancelOrder.Text = "Cancel Order";
            buttonCancelOrder.UseVisualStyleBackColor = true;
            buttonCancelOrder.Click += buttonCancelOrder_Click;
            // 
            // tableLayoutPanelCategoria
            // 
            tableLayoutPanelCategoria.ColumnCount = 1;
            tableLayoutPanelCategoria.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelCategoria.Controls.Add(tabControlMenuCategories, 0, 0);
            tableLayoutPanelCategoria.Dock = DockStyle.Fill;
            tableLayoutPanelCategoria.Location = new Point(621, 0);
            tableLayoutPanelCategoria.Margin = new Padding(0);
            tableLayoutPanelCategoria.Name = "tableLayoutPanelCategoria";
            tableLayoutPanelCategoria.RowCount = 2;
            tableLayoutPanelCategoria.RowStyles.Add(new RowStyle(SizeType.Percent, 88.32117F));
            tableLayoutPanelCategoria.RowStyles.Add(new RowStyle(SizeType.Percent, 11.6788321F));
            tableLayoutPanelCategoria.Size = new Size(828, 772);
            tableLayoutPanelCategoria.TabIndex = 1;
            // 
            // tabControlMenuCategories
            // 
            tabControlMenuCategories.Alignment = TabAlignment.Bottom;
            tabControlMenuCategories.Dock = DockStyle.Fill;
            tabControlMenuCategories.Font = new Font("Microsoft Sans Serif", 35.9999962F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tabControlMenuCategories.Location = new Point(3, 2);
            tabControlMenuCategories.Margin = new Padding(3, 2, 3, 2);
            tabControlMenuCategories.Name = "tabControlMenuCategories";
            tabControlMenuCategories.SelectedIndex = 0;
            tabControlMenuCategories.Size = new Size(822, 677);
            tabControlMenuCategories.TabIndex = 0;
            tabControlMenuCategories.SelectedIndexChanged += tabControlMenuCategories_SelectedIndexChanged;
            // 
            // tableLayoutPanelPayment — minimal placeholder; replaced at runtime by
            // PaymentPanelControl.Attach() which uses GetPositionFromControl to locate
            // it in tableLayoutPanelMain, then removes and disposes it.
            // 
            tableLayoutPanelPayment.Dock     = DockStyle.Fill;
            tableLayoutPanelPayment.Margin   = new Padding(2);
            tableLayoutPanelPayment.Name     = "tableLayoutPanelPayment";
            tableLayoutPanelPayment.TabIndex = 2;
            // 
            // textBoxUPC
            // 
            textBoxUPC.BackColor = Color.WhiteSmoke;
            textBoxUPC.BorderStyle = BorderStyle.None;
            textBoxUPC.Dock = DockStyle.Fill;
            textBoxUPC.Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            textBoxUPC.ForeColor = Color.DarkOliveGreen;
            textBoxUPC.Location = new Point(447, 3);
            textBoxUPC.MaxLength = 32000;
            textBoxUPC.Name = "textBoxUPC";
            textBoxUPC.PlaceholderText = "UPC";
            textBoxUPC.Size = new Size(163, 46);
            textBoxUPC.TabIndex = 1;
            textBoxUPC.TextAlign = HorizontalAlignment.Center;
            textBoxUPC.TextChanged += textBoxUPC_TextChanged;
            // 
            // buttonInvoice
            // 
            buttonInvoice.Location = new Point(1353, 2);
            buttonInvoice.Margin = new Padding(2);
            buttonInvoice.Name = "buttonInvoice";
            buttonInvoice.Size = new Size(51, 54);
            buttonInvoice.TabIndex = 9;
            buttonInvoice.Text = "Print Invoice";
            buttonInvoice.UseVisualStyleBackColor = true;
            buttonInvoice.Click += buttonInvoice_Click;
            // 
            // buttonClose
            // 
            buttonClose.BackColor = Color.IndianRed;
            buttonClose.Location = new Point(623, 2);
            buttonClose.Margin = new Padding(2);
            buttonClose.Name = "buttonClose";
            buttonClose.Size = new Size(128, 54);
            buttonClose.TabIndex = 6;
            buttonClose.Text = "Close";
            buttonClose.UseVisualStyleBackColor = false;
            buttonClose.Click += buttonLogout_Click;
            // 
            // labelCashier
            // 
            labelCashier.BackColor = Color.Transparent;
            labelCashier.FlatAppearance.BorderColor = Color.YellowGreen;
            labelCashier.FlatStyle = FlatStyle.Flat;
            labelCashier.Font = new Font("Microsoft Sans Serif", 12F);
            labelCashier.ForeColor = Color.FromArgb(59, 130, 246);
            labelCashier.Location = new Point(994, 3);
            labelCashier.Name = "labelCashier";
            labelCashier.Size = new Size(89, 52);
            labelCashier.TabIndex = 3;
            labelCashier.Text = "Username";
            labelCashier.UseVisualStyleBackColor = false;
            labelCashier.Click += labelCashier_Click;
            // 
            // ButtonSettings
            // 
            ButtonSettings.Location = new Point(0, 0);
            ButtonSettings.Name = "ButtonSettings";
            ButtonSettings.Size = new Size(75, 23);
            ButtonSettings.TabIndex = 0;
            // 
            // tableLayoutPanelTotal
            // 
            tableLayoutPanelTotal.BackColor = Color.Transparent;
            tableLayoutPanelTotal.ColumnCount = 2;
            tableLayoutPanelTotal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelTotal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelTotal.Controls.Add(label1, 0, 0);
            tableLayoutPanelTotal.Controls.Add(label2, 0, 1);
            tableLayoutPanelTotal.Controls.Add(label3, 0, 2);
            tableLayoutPanelTotal.Controls.Add(labelSubTotal, 1, 0);
            tableLayoutPanelTotal.Controls.Add(labelTotalTax, 1, 1);
            tableLayoutPanelTotal.Controls.Add(labelTotalValue, 1, 2);
            tableLayoutPanelTotal.Dock = DockStyle.Fill;
            tableLayoutPanelTotal.Location = new Point(20, 20);
            tableLayoutPanelTotal.Margin = new Padding(3, 2, 3, 2);
            tableLayoutPanelTotal.Name = "tableLayoutPanelTotal";
            tableLayoutPanelTotal.RowCount = 3;
            tableLayoutPanelTotal.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanelTotal.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanelTotal.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanelTotal.Size = new Size(575, 75);
            tableLayoutPanelTotal.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Microsoft Sans Serif", 20.1F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(281, 25);
            label1.TabIndex = 0;
            label1.Text = "Subtotal";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Font = new Font("Microsoft Sans Serif", 20.1F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(3, 25);
            label2.Name = "label2";
            label2.Size = new Size(281, 25);
            label2.TabIndex = 1;
            label2.Text = "Tax";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Fill;
            label3.FlatStyle = FlatStyle.Flat;
            label3.Font = new Font("Microsoft Sans Serif", 20.1F, FontStyle.Bold);
            label3.Location = new Point(3, 50);
            label3.Name = "label3";
            label3.Size = new Size(281, 25);
            label3.TabIndex = 2;
            label3.Text = "Total";
            label3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // labelSubTotal
            // 
            labelSubTotal.AutoSize = true;
            labelSubTotal.Dock = DockStyle.Fill;
            labelSubTotal.Font = new Font("Cascadia Mono", 15.75F, FontStyle.Bold);
            labelSubTotal.Location = new Point(290, 0);
            labelSubTotal.Name = "labelSubTotal";
            labelSubTotal.Size = new Size(282, 25);
            labelSubTotal.TabIndex = 3;
            labelSubTotal.Text = "0.00";
            labelSubTotal.TextAlign = ContentAlignment.MiddleRight;
            // 
            // labelTotalTax
            // 
            labelTotalTax.AutoSize = true;
            labelTotalTax.BackColor = Color.Transparent;
            labelTotalTax.Dock = DockStyle.Fill;
            labelTotalTax.Font = new Font("Cascadia Mono", 15.75F, FontStyle.Bold);
            labelTotalTax.Location = new Point(290, 25);
            labelTotalTax.Name = "labelTotalTax";
            labelTotalTax.Size = new Size(282, 25);
            labelTotalTax.TabIndex = 4;
            labelTotalTax.Text = "0.00";
            labelTotalTax.TextAlign = ContentAlignment.MiddleRight;
            // 
            // labelTotalValue
            // 
            labelTotalValue.AutoSize = true;
            labelTotalValue.Dock = DockStyle.Fill;
            labelTotalValue.Font = new Font("Cascadia Mono", 15.75F, FontStyle.Bold);
            labelTotalValue.Location = new Point(290, 50);
            labelTotalValue.Name = "labelTotalValue";
            labelTotalValue.Size = new Size(282, 25);
            labelTotalValue.TabIndex = 5;
            labelTotalValue.Text = "0.00";
            labelTotalValue.TextAlign = ContentAlignment.MiddleRight;
            // 
            // labelProductName
            // 
            labelProductName.Font = new Font("Segoe UI", 12F);
            labelProductName.ForeColor = Color.FromArgb(71, 85, 105);
            labelProductName.Location = new Point(0, 0);
            labelProductName.Margin = new Padding(0);
            labelProductName.Name = "labelProductName";
            labelProductName.Padding = new Padding(16, 0, 0, 0);
            labelProductName.Size = new Size(621, 36);
            labelProductName.TabIndex = 2;
            labelProductName.Text = "Ready to scan...";
            labelProductName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.BackColor = Color.FromArgb(15, 23, 42);
            tableLayoutPanel1.ColumnCount = 5;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 370F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 360F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 317F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 402F));
            tableLayoutPanel1.Controls.Add(buttonInvoice, 3, 0);
            tableLayoutPanel1.Controls.Add(buttonClose, 1, 0);
            tableLayoutPanel1.Controls.Add(labelCashier, 2, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(3, 2);
            tableLayoutPanel1.Margin = new Padding(3, 2, 3, 2);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(2070, 60);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // MaintableLayout
            // 
            MaintableLayout.BackColor = Color.White;
            MaintableLayout.ColumnCount = 1;
            MaintableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            MaintableLayout.Controls.Add(tableLayoutPanel1, 0, 0);
            MaintableLayout.Controls.Add(pnlProductStrip, 0, 1);
            MaintableLayout.Controls.Add(tableLayoutPanelMain, 0, 2);
            MaintableLayout.Dock = DockStyle.Fill;
            MaintableLayout.Location = new Point(0, 0);
            MaintableLayout.Margin = new Padding(0);
            MaintableLayout.Name = "MaintableLayout";
            MaintableLayout.RowCount = 3;
            MaintableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));
            MaintableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            MaintableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            MaintableLayout.Size = new Size(2076, 876);
            MaintableLayout.TabIndex = 1;
            // 
            // pnlProductStrip
            // 
            pnlProductStrip.BackColor = Color.FromArgb(248, 250, 252);
            pnlProductStrip.Controls.Add(labelProductName);
            pnlProductStrip.Location = new Point(0, 64);
            pnlProductStrip.Margin = new Padding(0);
            pnlProductStrip.Name = "pnlProductStrip";
            pnlProductStrip.Size = new Size(624, 36);
            pnlProductStrip.TabIndex = 2;
            // 
            // frmHome
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScroll = true;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(2076, 876);
            Controls.Add(MaintableLayout);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(3, 2, 3, 2);
            MaximizeBox = false;
            Name = "frmHome";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "OmadaPOS";
            WindowState = FormWindowState.Maximized;
            FormClosing += frmHome_FormClosing;
            Load += frmHome_Load;
            tableLayoutPanelMain.ResumeLayout(false);
            tableLayoutPanelCart.ResumeLayout(false);
            tableLayoutPanelButtonCart.ResumeLayout(false);
            tableLayoutPanelCategoria.ResumeLayout(false);
            tableLayoutPanelTotal.ResumeLayout(false);
            tableLayoutPanelTotal.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            MaintableLayout.ResumeLayout(false);
            pnlProductStrip.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanelMain;
        private TableLayoutPanel tableLayoutPanelCart;
        private Label labelProductName;
        private TableLayoutPanel tableLayoutPanelTotal;
        private Label label1;
        private Label label2;
        private Label label3;
        private TableLayoutPanel tableLayoutPanelButtonCart;
        private Button buttonHold;
        private Button buttonDeleteItem;
        private Button buttonChangeQuantity;
        private Button buttonCancelOrder;
        private Label labelSubTotal;
        private Label labelTotalTax;
        private Label labelTotalValue;
        private TableLayoutPanel tableLayoutPanelCategoria;
        private TabControl tabControlMenuCategories;
        private TableLayoutPanel tableLayoutPanelPayment; // placeholder — disposed by PaymentPanelControl.Attach()
        private TableLayoutPanel tableLayoutPanel1;
        private Button labelCashier;
        private Componentes.AbecedarioControl abecedarioControl1;
        private TableLayoutPanel MaintableLayout;
        private TextBox textBoxUPC;
        private Panel pnlProductStrip;
        private Button buttonClose;
        private ListView listViewCart;
        private Componentes.RoundedPanel roundedPanel1;
        private Button buttonInvoice;
        private Button ButtonSettings;
    }
}
