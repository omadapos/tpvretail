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
            tableLayoutPanelTotal = new TableLayoutPanel();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            labelSubTotal = new Label();
            labelTotalTax = new Label();
            labelTotalValue = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            MaintableLayout = new TableLayoutPanel();
            tableLayoutPanelMain.SuspendLayout();
            tableLayoutPanelCart.SuspendLayout();
            tableLayoutPanelButtonCart.SuspendLayout();
            tableLayoutPanelCategoria.SuspendLayout();
            tableLayoutPanelTotal.SuspendLayout();
            MaintableLayout.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanelMain
            // 
            tableLayoutPanelMain.BackColor = Color.FromArgb(15, 23, 42);
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
            tableLayoutPanelCart.BackColor = Color.FromArgb(15, 23, 42);
            tableLayoutPanelCart.ColumnCount = 1;
            tableLayoutPanelCart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelCart.Controls.Add(listViewCart, 0, 0);
            tableLayoutPanelCart.Controls.Add(tableLayoutPanelButtonCart, 0, 2);
            tableLayoutPanelCart.Dock = DockStyle.Fill;
            tableLayoutPanelCart.Location = new Point(0, 0);
            tableLayoutPanelCart.Margin = new Padding(0);
            tableLayoutPanelCart.Name = "tableLayoutPanelCart";
            tableLayoutPanelCart.RowCount = 3;
            tableLayoutPanelCart.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));   // Row 0: list (fills remaining height)
            tableLayoutPanelCart.RowStyles.Add(new RowStyle(SizeType.Absolute, 170F));  // Row 1: totals card (fixed)
            tableLayoutPanelCart.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));   // Row 2: action buttons (fixed)
            tableLayoutPanelCart.Size = new Size(621, 772);
            tableLayoutPanelCart.TabIndex = 0;
            // 
            // listViewCart
            // 
            listViewCart.BackColor = Color.FromArgb(30, 41, 59);
            listViewCart.Dock = DockStyle.Fill;
            listViewCart.Location = new Point(3, 3);
            listViewCart.Name = "listViewCart";
            listViewCart.Size = new Size(615, 562);
            listViewCart.TabIndex = 4;
            listViewCart.UseCompatibleStateImageBehavior = false;
            // 
            // tableLayoutPanelButtonCart
            //
            tableLayoutPanelButtonCart.BackColor = Color.FromArgb(15, 23, 42); // NavyDark — matches overall theme
            tableLayoutPanelButtonCart.ColumnCount = 4;
            tableLayoutPanelButtonCart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanelButtonCart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanelButtonCart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanelButtonCart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanelButtonCart.Controls.Add(buttonCancelOrder,    0, 0);
            tableLayoutPanelButtonCart.Controls.Add(buttonChangeQuantity, 1, 0);
            tableLayoutPanelButtonCart.Controls.Add(buttonDeleteItem,     2, 0);
            tableLayoutPanelButtonCart.Controls.Add(buttonHold,           3, 0);
            tableLayoutPanelButtonCart.Dock     = DockStyle.Fill;
            tableLayoutPanelButtonCart.Margin   = new Padding(0);
            tableLayoutPanelButtonCart.Name     = "tableLayoutPanelButtonCart";
            tableLayoutPanelButtonCart.RowCount = 1;
            tableLayoutPanelButtonCart.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelButtonCart.TabIndex = 3;
            tableLayoutPanelButtonCart.Padding  = new Padding(4, 6, 4, 6);
            //
            // buttonCancelOrder
            //
            buttonCancelOrder.Dock    = DockStyle.Fill;
            buttonCancelOrder.Margin  = new Padding(3, 0, 3, 0);
            buttonCancelOrder.Name    = "buttonCancelOrder";
            buttonCancelOrder.TabIndex = 0;
            buttonCancelOrder.Text    = "✕  CANCEL";
            buttonCancelOrder.Click  += buttonCancelOrder_Click;
            //
            // buttonChangeQuantity
            //
            buttonChangeQuantity.Dock    = DockStyle.Fill;
            buttonChangeQuantity.Margin  = new Padding(3, 0, 3, 0);
            buttonChangeQuantity.Name    = "buttonChangeQuantity";
            buttonChangeQuantity.TabIndex = 1;
            buttonChangeQuantity.Text    = "✏  QTY";
            buttonChangeQuantity.Click  += buttonChangeQuantity_Click;
            //
            // buttonDeleteItem
            //
            buttonDeleteItem.Dock    = DockStyle.Fill;
            buttonDeleteItem.Margin  = new Padding(3, 0, 3, 0);
            buttonDeleteItem.Name    = "buttonDeleteItem";
            buttonDeleteItem.TabIndex = 2;
            buttonDeleteItem.Text    = "🗑  DELETE";
            buttonDeleteItem.Click  += buttonDeleteItem_Click;
            //
            // buttonHold
            //
            buttonHold.Dock    = DockStyle.Fill;
            buttonHold.Margin  = new Padding(3, 0, 3, 0);
            buttonHold.Name    = "buttonHold";
            buttonHold.TabIndex = 3;
            buttonHold.Text    = "⏸  HOLD";
            buttonHold.Click  += buttonHold_Click;
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
            tableLayoutPanelCategoria.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // Row 0: products (fills remaining space)
            tableLayoutPanelCategoria.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));  // Row 1: abecedario (fixed height, stable on resize)
            tableLayoutPanelCategoria.Size = new Size(828, 772);
            tableLayoutPanelCategoria.TabIndex = 1;
            // 
            // tabControlMenuCategories
            // 
            tabControlMenuCategories.Alignment = TabAlignment.Bottom;
            tabControlMenuCategories.Dock = DockStyle.Fill;
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
            textBoxUPC.BackColor = Color.FromArgb(15, 23, 42);
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
            tableLayoutPanelTotal.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            tableLayoutPanelTotal.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            tableLayoutPanelTotal.RowStyles.Add(new RowStyle(SizeType.Percent, 33.34F));
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
            // tableLayoutPanel1 — placeholder; disposed and replaced by POSHeaderControl.Attach()
            // 
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.TabIndex = 1;
            // 
            // MaintableLayout
            // 
            MaintableLayout.BackColor = Color.FromArgb(15, 23, 42);
            MaintableLayout.ColumnCount = 1;
            MaintableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            MaintableLayout.Controls.Add(tableLayoutPanel1, 0, 0);
            MaintableLayout.Controls.Add(tableLayoutPanelMain, 0, 1);
            MaintableLayout.Dock = DockStyle.Fill;
            MaintableLayout.Location = new Point(0, 0);
            MaintableLayout.Margin = new Padding(0);
            MaintableLayout.Name = "MaintableLayout";
            MaintableLayout.RowCount = 2;
            MaintableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
            MaintableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            MaintableLayout.Size = new Size(2076, 876);
            MaintableLayout.TabIndex = 1;
            // 
            // frmHome
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScroll = true;
            BackColor = Color.FromArgb(15, 23, 42);
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
            MaintableLayout.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanelMain;
        private TableLayoutPanel tableLayoutPanelCart;
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
        private TableLayoutPanel tableLayoutPanel1;       // placeholder — disposed by POSHeaderControl.Attach()
        private Componentes.AbecedarioControl abecedarioControl1;
        private TableLayoutPanel MaintableLayout;
        private TextBox textBoxUPC;
        private ListView listViewCart;
        private Componentes.RoundedPanel roundedPanel1;
    }
}
