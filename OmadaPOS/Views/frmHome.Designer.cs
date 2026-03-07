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
            tableLayoutPanelButtonCart = new TableLayoutPanel();
            buttonHold = new Button();
            buttonDeleteItem = new Button();
            buttonChangeQuantity = new Button();
            buttonCancelOrder = new Button();
            listViewCart = new ListView();
            roundedPanel1 = new OmadaPOS.Componentes.RoundedPanel();
            tableLayoutPanelTotal = new TableLayoutPanel();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            labelSubTotal = new Label();
            labelTotalTax = new Label();
            labelTotalValue = new Label();
            tableLayoutPanelCategoria = new TableLayoutPanel();
            tabControlMenuCategories = new TabControl();
            tabPage2 = new TabPage();
            abecedarioControl1 = new OmadaPOS.Componentes.AbecedarioControl();
            tableLayoutPanelPayment = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            keyPaymentControl1 = new OmadaPOS.Componentes.KeyPaymentControl();
            roundedPanel2 = new OmadaPOS.Componentes.RoundedPanel();
            tableLayoutPanel4 = new TableLayoutPanel();
            buttonQsale = new Button();
            buttonLookup = new Button();
            label5 = new Label();
            labelInputValue = new Label();
            label4 = new Label();
            labelChangeValue = new Label();
            splitContainerScale = new SplitContainer();
            tableLayoutPanelPesado = new TableLayoutPanel();
            labelWeight = new Label();
            labelPesaProduct = new Label();
            lblScalStatusDesc = new Label();
            pictureBoxPesado = new PictureBox();
            tableLayoutPanel3 = new TableLayoutPanel();
            buttonEBTFood = new Button();
            buttonPayDebitCard = new Button();
            buttonPayCreditCard = new Button();
            buttonEBTBalance = new Button();
            buttonOpenDrawer = new Button();
            buttonGiftCard = new Button();
            buttonPayCash = new Button();
            buttonSplit = new Button();
            labelCashier = new Button();
            buttonClose = new Button();
            labelProductName = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            ButtonSettings = new Button();
            buttonInvoice = new Button();
            textBoxUPC = new TextBox();
            MaintableLayout = new TableLayoutPanel();
            pnlProductStrip = new Panel();
            tableLayoutPanelMain.SuspendLayout();
            tableLayoutPanelCart.SuspendLayout();
            tableLayoutPanelButtonCart.SuspendLayout();
            roundedPanel1.SuspendLayout();
            tableLayoutPanelTotal.SuspendLayout();
            tableLayoutPanelCategoria.SuspendLayout();
            tabControlMenuCategories.SuspendLayout();
            tableLayoutPanelPayment.SuspendLayout();
            roundedPanel2.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerScale).BeginInit();
            splitContainerScale.Panel1.SuspendLayout();
            splitContainerScale.Panel2.SuspendLayout();
            splitContainerScale.SuspendLayout();
            tableLayoutPanelPesado.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPesado).BeginInit();
            tableLayoutPanel3.SuspendLayout();
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
            tableLayoutPanelMain.Location = new Point(3, 53);
            tableLayoutPanelMain.Margin = new Padding(3, 2, 3, 2);
            tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            tableLayoutPanelMain.RowCount = 1;
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelMain.Size = new Size(2070, 821);
            tableLayoutPanelMain.TabIndex = 0;
            // 
            // tableLayoutPanelCart
            // 
            tableLayoutPanelCart.BackColor = Color.WhiteSmoke;
            tableLayoutPanelCart.ColumnCount = 1;
            tableLayoutPanelCart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelCart.Controls.Add(tableLayoutPanelButtonCart, 0, 2);
            tableLayoutPanelCart.Controls.Add(listViewCart, 0, 0);
            tableLayoutPanelCart.Controls.Add(roundedPanel1, 0, 1);
            tableLayoutPanelCart.Dock = DockStyle.Fill;
            tableLayoutPanelCart.Location = new Point(0, 0);
            tableLayoutPanelCart.Margin = new Padding(0);
            tableLayoutPanelCart.Name = "tableLayoutPanelCart";
            tableLayoutPanelCart.RowCount = 3;
            tableLayoutPanelCart.RowStyles.Add(new RowStyle(SizeType.Percent, 73.68421F));
            tableLayoutPanelCart.RowStyles.Add(new RowStyle(SizeType.Percent, 15.7894735F));
            tableLayoutPanelCart.RowStyles.Add(new RowStyle(SizeType.Percent, 10.5263157F));
            tableLayoutPanelCart.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanelCart.Size = new Size(621, 821);
            tableLayoutPanelCart.TabIndex = 0;
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
            tableLayoutPanelButtonCart.Location = new Point(3, 735);
            tableLayoutPanelButtonCart.Margin = new Padding(3, 2, 3, 2);
            tableLayoutPanelButtonCart.Name = "tableLayoutPanelButtonCart";
            tableLayoutPanelButtonCart.RowCount = 1;
            tableLayoutPanelButtonCart.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelButtonCart.Size = new Size(615, 84);
            tableLayoutPanelButtonCart.TabIndex = 3;
            // 
            // buttonHold
            // 
            buttonHold.Dock = DockStyle.Fill;
            buttonHold.Font = new Font("Microsoft Sans Serif", 8.25F);
            buttonHold.Location = new Point(462, 3);
            buttonHold.Name = "buttonHold";
            buttonHold.Size = new Size(150, 78);
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
            buttonDeleteItem.Size = new Size(147, 78);
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
            buttonChangeQuantity.Size = new Size(147, 78);
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
            buttonCancelOrder.Size = new Size(147, 78);
            buttonCancelOrder.TabIndex = 0;
            buttonCancelOrder.Text = "Cancel Order";
            buttonCancelOrder.UseVisualStyleBackColor = true;
            buttonCancelOrder.Click += buttonCancelOrder_Click;
            // 
            // listViewCart
            // 
            listViewCart.BackColor = Color.WhiteSmoke;
            listViewCart.Dock = DockStyle.Fill;
            listViewCart.Location = new Point(3, 3);
            listViewCart.Name = "listViewCart";
            listViewCart.Size = new Size(615, 598);
            listViewCart.TabIndex = 4;
            listViewCart.UseCompatibleStateImageBehavior = false;
            // 
            // roundedPanel1
            // 
            roundedPanel1.BackColor = Color.Transparent;
            roundedPanel1.Controls.Add(tableLayoutPanelTotal);
            roundedPanel1.Dock = DockStyle.Fill;
            roundedPanel1.Location = new Point(3, 606);
            roundedPanel1.Margin = new Padding(3, 2, 3, 2);
            roundedPanel1.Name = "roundedPanel1";
            roundedPanel1.Padding = new Padding(20);
            roundedPanel1.Size = new Size(615, 125);
            roundedPanel1.TabIndex = 5;
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
            tableLayoutPanelTotal.Size = new Size(575, 85);
            tableLayoutPanelTotal.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Microsoft Sans Serif", 20.1F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(281, 28);
            label1.TabIndex = 0;
            label1.Text = "Subtotal";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Font = new Font("Microsoft Sans Serif", 20.1F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(3, 28);
            label2.Name = "label2";
            label2.Size = new Size(281, 28);
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
            label3.Location = new Point(3, 56);
            label3.Name = "label3";
            label3.Size = new Size(281, 29);
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
            labelSubTotal.Size = new Size(282, 28);
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
            labelTotalTax.Location = new Point(290, 28);
            labelTotalTax.Name = "labelTotalTax";
            labelTotalTax.Size = new Size(282, 28);
            labelTotalTax.TabIndex = 4;
            labelTotalTax.Text = "0.00";
            labelTotalTax.TextAlign = ContentAlignment.MiddleRight;
            // 
            // labelTotalValue
            // 
            labelTotalValue.AutoSize = true;
            labelTotalValue.Dock = DockStyle.Fill;
            labelTotalValue.Font = new Font("Cascadia Mono", 15.75F, FontStyle.Bold);
            labelTotalValue.Location = new Point(290, 56);
            labelTotalValue.Name = "labelTotalValue";
            labelTotalValue.Size = new Size(282, 29);
            labelTotalValue.TabIndex = 5;
            labelTotalValue.Text = "0.00";
            labelTotalValue.TextAlign = ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanelCategoria
            // 
            tableLayoutPanelCategoria.ColumnCount = 1;
            tableLayoutPanelCategoria.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelCategoria.Controls.Add(tabControlMenuCategories, 0, 0);
            tableLayoutPanelCategoria.Controls.Add(abecedarioControl1, 0, 1);
            tableLayoutPanelCategoria.Dock = DockStyle.Fill;
            tableLayoutPanelCategoria.Location = new Point(621, 0);
            tableLayoutPanelCategoria.Margin = new Padding(0);
            tableLayoutPanelCategoria.Name = "tableLayoutPanelCategoria";
            tableLayoutPanelCategoria.RowCount = 2;
            tableLayoutPanelCategoria.RowStyles.Add(new RowStyle(SizeType.Percent, 88.32117F));
            tableLayoutPanelCategoria.RowStyles.Add(new RowStyle(SizeType.Percent, 11.6788321F));
            tableLayoutPanelCategoria.RowStyles.Add(new RowStyle(SizeType.Absolute, 14F));
            tableLayoutPanelCategoria.Size = new Size(828, 821);
            tableLayoutPanelCategoria.TabIndex = 1;
            // 
            // tabControlMenuCategories
            // 
            tabControlMenuCategories.Alignment = TabAlignment.Bottom;
            tabControlMenuCategories.Controls.Add(tabPage2);
            tabControlMenuCategories.Dock = DockStyle.Fill;
            tabControlMenuCategories.Font = new Font("Microsoft Sans Serif", 35.9999962F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tabControlMenuCategories.Location = new Point(3, 2);
            tabControlMenuCategories.Margin = new Padding(3, 2, 3, 2);
            tabControlMenuCategories.Name = "tabControlMenuCategories";
            tabControlMenuCategories.SelectedIndex = 0;
            tabControlMenuCategories.Size = new Size(822, 721);
            tabControlMenuCategories.TabIndex = 0;
            tabControlMenuCategories.SelectedIndexChanged += tabControlMenuCategories_SelectedIndexChanged;
            // 
            // tabPage2
            // 
            tabPage2.BackColor = Color.WhiteSmoke;
            tabPage2.BackgroundImageLayout = ImageLayout.None;
            tabPage2.Font = new Font("Segoe UI Black", 48F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tabPage2.Location = new Point(4, 4);
            tabPage2.Margin = new Padding(0);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(22);
            tabPage2.Size = new Size(814, 626);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "tabPage2";
            // 
            // abecedarioControl1
            // 
            abecedarioControl1.BackColor = Color.FromArgb(15, 23, 42);
            abecedarioControl1.Dock = DockStyle.Fill;
            abecedarioControl1.Location = new Point(3, 727);
            abecedarioControl1.Margin = new Padding(3, 2, 3, 2);
            abecedarioControl1.Name = "abecedarioControl1";
            abecedarioControl1.Size = new Size(822, 92);
            abecedarioControl1.TabIndex = 1;
            abecedarioControl1.LetraClicked += AbecedarioControl1_LetraClicked;
            // 
            // tableLayoutPanelPayment
            // 
            tableLayoutPanelPayment.BackColor = Color.WhiteSmoke;
            tableLayoutPanelPayment.ColumnCount = 1;
            tableLayoutPanelPayment.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelPayment.Controls.Add(tableLayoutPanel2, 0, 0);
            tableLayoutPanelPayment.Controls.Add(keyPaymentControl1, 0, 1);
            tableLayoutPanelPayment.Controls.Add(roundedPanel2, 0, 2);
            tableLayoutPanelPayment.Controls.Add(splitContainerScale, 0, 3);
            tableLayoutPanelPayment.Controls.Add(tableLayoutPanel3, 0, 4);
            tableLayoutPanelPayment.Dock = DockStyle.Fill;
            tableLayoutPanelPayment.Location = new Point(1451, 2);
            tableLayoutPanelPayment.Margin = new Padding(2);
            tableLayoutPanelPayment.Name = "tableLayoutPanelPayment";
            tableLayoutPanelPayment.RowCount = 5;
            tableLayoutPanelPayment.RowStyles.Add(new RowStyle(SizeType.Percent, 5F));
            tableLayoutPanelPayment.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanelPayment.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tableLayoutPanelPayment.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tableLayoutPanelPayment.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tableLayoutPanelPayment.Size = new Size(617, 817);
            tableLayoutPanelPayment.TabIndex = 2;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.BackColor = Color.WhiteSmoke;
            tableLayoutPanel2.ColumnCount = 4;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 21.9482517F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22.5484657F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22.1699524F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(2, 2);
            tableLayoutPanel2.Margin = new Padding(2);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(613, 36);
            tableLayoutPanel2.TabIndex = 11;
            // 
            // keyPaymentControl1
            // 
            keyPaymentControl1.BackColor = Color.FromArgb(248, 249, 252);
            keyPaymentControl1.Dock = DockStyle.Fill;
            keyPaymentControl1.Location = new Point(3, 42);
            keyPaymentControl1.Margin = new Padding(3, 2, 3, 2);
            keyPaymentControl1.Name = "keyPaymentControl1";
            keyPaymentControl1.Size = new Size(611, 404);
            keyPaymentControl1.TabIndex = 10;
            keyPaymentControl1.KeyPaymentClicked += KeyPaymentControl1_KeyPaymentClicked;
            // 
            // roundedPanel2
            // 
            roundedPanel2.BackColor = Color.Transparent;
            roundedPanel2.Controls.Add(tableLayoutPanel4);
            roundedPanel2.Dock = DockStyle.Fill;
            roundedPanel2.Location = new Point(3, 450);
            roundedPanel2.Margin = new Padding(3, 2, 3, 2);
            roundedPanel2.Name = "roundedPanel2";
            roundedPanel2.Padding = new Padding(20);
            roundedPanel2.Size = new Size(611, 118);
            roundedPanel2.TabIndex = 6;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 3;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel4.Controls.Add(buttonQsale, 0, 0);
            tableLayoutPanel4.Controls.Add(buttonLookup, 0, 1);
            tableLayoutPanel4.Controls.Add(label5, 1, 1);
            tableLayoutPanel4.Controls.Add(labelInputValue, 2, 0);
            tableLayoutPanel4.Controls.Add(label4, 1, 0);
            tableLayoutPanel4.Controls.Add(labelChangeValue, 2, 1);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.Location = new Point(20, 20);
            tableLayoutPanel4.Margin = new Padding(2);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.Padding = new Padding(3);
            tableLayoutPanel4.RowCount = 2;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.Size = new Size(571, 78);
            tableLayoutPanel4.TabIndex = 0;
            // 
            // buttonQsale
            // 
            buttonQsale.Dock = DockStyle.Fill;
            buttonQsale.Location = new Point(5, 5);
            buttonQsale.Margin = new Padding(2);
            buttonQsale.Name = "buttonQsale";
            buttonQsale.Size = new Size(222, 32);
            buttonQsale.TabIndex = 7;
            buttonQsale.Text = "Quick Sale";
            buttonQsale.UseVisualStyleBackColor = true;
            buttonQsale.Click += buttonProductNoTax_Click;
            // 
            // buttonLookup
            // 
            buttonLookup.Dock = DockStyle.Fill;
            buttonLookup.Location = new Point(5, 41);
            buttonLookup.Margin = new Padding(2);
            buttonLookup.Name = "buttonLookup";
            buttonLookup.Size = new Size(222, 32);
            buttonLookup.TabIndex = 5;
            buttonLookup.Text = "LookUp UPC";
            buttonLookup.UseVisualStyleBackColor = false;
            buttonLookup.Click += buttonLookup_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Dock = DockStyle.Fill;
            label5.FlatStyle = FlatStyle.Flat;
            label5.Font = new Font("Consolas", 27.75F, FontStyle.Bold);
            label5.Location = new Point(232, 39);
            label5.Name = "label5";
            label5.Size = new Size(163, 36);
            label5.TabIndex = 2;
            label5.Text = "Due";
            label5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelInputValue
            // 
            labelInputValue.AutoSize = true;
            labelInputValue.Dock = DockStyle.Fill;
            labelInputValue.Font = new Font("Consolas", 27.75F, FontStyle.Bold);
            labelInputValue.Location = new Point(406, 11);
            labelInputValue.Margin = new Padding(8);
            labelInputValue.Name = "labelInputValue";
            labelInputValue.Size = new Size(154, 20);
            labelInputValue.TabIndex = 1;
            labelInputValue.Text = "0.00";
            labelInputValue.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = DockStyle.Fill;
            label4.FlatStyle = FlatStyle.Flat;
            label4.Font = new Font("Microsoft Sans Serif", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(231, 5);
            label4.Margin = new Padding(2);
            label4.Name = "label4";
            label4.Size = new Size(165, 32);
            label4.TabIndex = 0;
            label4.Text = "Tender";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelChangeValue
            // 
            labelChangeValue.AutoSize = true;
            labelChangeValue.Dock = DockStyle.Fill;
            labelChangeValue.Font = new Font("Consolas", 27.75F, FontStyle.Bold);
            labelChangeValue.ForeColor = Color.DarkBlue;
            labelChangeValue.Location = new Point(401, 39);
            labelChangeValue.Name = "labelChangeValue";
            labelChangeValue.Size = new Size(164, 36);
            labelChangeValue.TabIndex = 3;
            labelChangeValue.Text = "0.00";
            labelChangeValue.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // splitContainerScale
            // 
            splitContainerScale.Dock = DockStyle.Fill;
            splitContainerScale.Font = new Font("Verdana", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            splitContainerScale.ForeColor = Color.Black;
            splitContainerScale.IsSplitterFixed = true;
            splitContainerScale.Location = new Point(8, 578);
            splitContainerScale.Margin = new Padding(8);
            splitContainerScale.Name = "splitContainerScale";
            // 
            // splitContainerScale.Panel1
            // 
            splitContainerScale.Panel1.Controls.Add(tableLayoutPanelPesado);
            // 
            // splitContainerScale.Panel2
            // 
            splitContainerScale.Panel2.Controls.Add(pictureBoxPesado);
            splitContainerScale.Size = new Size(601, 106);
            splitContainerScale.SplitterDistance = 319;
            splitContainerScale.SplitterWidth = 6;
            splitContainerScale.TabIndex = 5;
            // 
            // tableLayoutPanelPesado
            // 
            tableLayoutPanelPesado.BackColor = Color.White;
            tableLayoutPanelPesado.ColumnCount = 1;
            tableLayoutPanelPesado.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 87.0646744F));
            tableLayoutPanelPesado.Controls.Add(labelWeight, 0, 0);
            tableLayoutPanelPesado.Controls.Add(labelPesaProduct, 0, 1);
            tableLayoutPanelPesado.Controls.Add(lblScalStatusDesc, 0, 2);
            tableLayoutPanelPesado.Dock = DockStyle.Fill;
            tableLayoutPanelPesado.Location = new Point(0, 0);
            tableLayoutPanelPesado.Margin = new Padding(3, 2, 3, 2);
            tableLayoutPanelPesado.Name = "tableLayoutPanelPesado";
            tableLayoutPanelPesado.RowCount = 3;
            tableLayoutPanelPesado.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelPesado.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));
            tableLayoutPanelPesado.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanelPesado.Size = new Size(319, 106);
            tableLayoutPanelPesado.TabIndex = 5;
            // 
            // labelWeight
            // 
            labelWeight.AutoSize = true;
            labelWeight.BackColor = Color.Transparent;
            labelWeight.Dock = DockStyle.Fill;
            labelWeight.Font = new Font("Microsoft Sans Serif", 21.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelWeight.ForeColor = Color.FromArgb(251, 140, 0);
            labelWeight.Location = new Point(3, 0);
            labelWeight.Name = "labelWeight";
            labelWeight.Size = new Size(313, 30);
            labelWeight.TabIndex = 1;
            labelWeight.Text = "Weight";
            labelWeight.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelPesaProduct
            // 
            labelPesaProduct.AutoSize = true;
            labelPesaProduct.BackColor = Color.Transparent;
            labelPesaProduct.Dock = DockStyle.Top;
            labelPesaProduct.Font = new Font("Cascadia Mono", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelPesaProduct.Location = new Point(3, 30);
            labelPesaProduct.Name = "labelPesaProduct";
            labelPesaProduct.Size = new Size(313, 43);
            labelPesaProduct.TabIndex = 0;
            labelPesaProduct.Text = "Product";
            labelPesaProduct.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblScalStatusDesc
            // 
            lblScalStatusDesc.AutoSize = true;
            lblScalStatusDesc.Dock = DockStyle.Bottom;
            lblScalStatusDesc.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblScalStatusDesc.ForeColor = Color.Red;
            lblScalStatusDesc.Location = new Point(3, 85);
            lblScalStatusDesc.Name = "lblScalStatusDesc";
            lblScalStatusDesc.Size = new Size(313, 21);
            lblScalStatusDesc.TabIndex = 2;
            lblScalStatusDesc.Text = "Status";
            lblScalStatusDesc.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pictureBoxPesado
            // 
            pictureBoxPesado.BackColor = Color.White;
            pictureBoxPesado.Dock = DockStyle.Fill;
            pictureBoxPesado.Location = new Point(0, 0);
            pictureBoxPesado.Margin = new Padding(3, 2, 3, 2);
            pictureBoxPesado.Name = "pictureBoxPesado";
            pictureBoxPesado.Size = new Size(276, 106);
            pictureBoxPesado.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxPesado.TabIndex = 1;
            pictureBoxPesado.TabStop = false;
            pictureBoxPesado.Click += pictureBoxPesado_Click;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 4;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel3.Controls.Add(buttonEBTFood, 1, 1);
            tableLayoutPanel3.Controls.Add(buttonPayDebitCard, 1, 0);
            tableLayoutPanel3.Controls.Add(buttonPayCreditCard, 0, 0);
            tableLayoutPanel3.Controls.Add(buttonEBTBalance, 0, 1);
            tableLayoutPanel3.Controls.Add(buttonOpenDrawer, 2, 1);
            tableLayoutPanel3.Controls.Add(buttonGiftCard, 3, 1);
            tableLayoutPanel3.Controls.Add(buttonPayCash, 3, 0);
            tableLayoutPanel3.Controls.Add(buttonSplit, 2, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(3, 694);
            tableLayoutPanel3.Margin = new Padding(3, 2, 3, 2);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 2;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.Size = new Size(611, 121);
            tableLayoutPanel3.TabIndex = 4;
            // 
            // buttonEBTFood
            // 
            buttonEBTFood.Dock = DockStyle.Fill;
            buttonEBTFood.FlatStyle = FlatStyle.System;
            buttonEBTFood.Font = new Font("Microsoft Sans Serif", 8.25F);
            buttonEBTFood.ForeColor = Color.DarkCyan;
            buttonEBTFood.Location = new Point(155, 63);
            buttonEBTFood.Name = "buttonEBTFood";
            buttonEBTFood.Size = new Size(146, 55);
            buttonEBTFood.TabIndex = 2;
            buttonEBTFood.Text = "EBT";
            buttonEBTFood.UseVisualStyleBackColor = true;
            buttonEBTFood.Click += buttonEBTFood_Click;
            // 
            // buttonPayDebitCard
            // 
            buttonPayDebitCard.BackColor = Color.RoyalBlue;
            buttonPayDebitCard.Dock = DockStyle.Fill;
            buttonPayDebitCard.FlatStyle = FlatStyle.System;
            buttonPayDebitCard.Font = new Font("Microsoft Sans Serif", 8.25F);
            buttonPayDebitCard.ForeColor = SystemColors.ButtonHighlight;
            buttonPayDebitCard.Location = new Point(155, 3);
            buttonPayDebitCard.Name = "buttonPayDebitCard";
            buttonPayDebitCard.Size = new Size(146, 54);
            buttonPayDebitCard.TabIndex = 1;
            buttonPayDebitCard.Text = "Debit Card";
            buttonPayDebitCard.UseVisualStyleBackColor = false;
            buttonPayDebitCard.Click += buttonPayDebitCard_Click;
            // 
            // buttonPayCreditCard
            // 
            buttonPayCreditCard.BackColor = Color.YellowGreen;
            buttonPayCreditCard.Dock = DockStyle.Fill;
            buttonPayCreditCard.FlatStyle = FlatStyle.System;
            buttonPayCreditCard.Font = new Font("Microsoft Sans Serif", 8.25F);
            buttonPayCreditCard.ForeColor = SystemColors.ControlLightLight;
            buttonPayCreditCard.Location = new Point(3, 3);
            buttonPayCreditCard.Name = "buttonPayCreditCard";
            buttonPayCreditCard.Size = new Size(146, 54);
            buttonPayCreditCard.TabIndex = 0;
            buttonPayCreditCard.Text = "Credit Card";
            buttonPayCreditCard.UseVisualStyleBackColor = false;
            buttonPayCreditCard.Click += buttonPayCreditCard_Click;
            // 
            // buttonEBTBalance
            // 
            buttonEBTBalance.Dock = DockStyle.Fill;
            buttonEBTBalance.FlatStyle = FlatStyle.System;
            buttonEBTBalance.Font = new Font("Microsoft Sans Serif", 8.25F);
            buttonEBTBalance.Location = new Point(3, 63);
            buttonEBTBalance.Name = "buttonEBTBalance";
            buttonEBTBalance.Size = new Size(146, 55);
            buttonEBTBalance.TabIndex = 9;
            buttonEBTBalance.Text = "EBT Balance";
            buttonEBTBalance.UseVisualStyleBackColor = true;
            buttonEBTBalance.Click += buttonEBTBalance_Click;
            // 
            // buttonOpenDrawer
            // 
            buttonOpenDrawer.Dock = DockStyle.Fill;
            buttonOpenDrawer.FlatStyle = FlatStyle.System;
            buttonOpenDrawer.Font = new Font("Microsoft Sans Serif", 8.25F);
            buttonOpenDrawer.Location = new Point(307, 63);
            buttonOpenDrawer.Name = "buttonOpenDrawer";
            buttonOpenDrawer.Size = new Size(146, 55);
            buttonOpenDrawer.TabIndex = 7;
            buttonOpenDrawer.Text = "Open Drawer";
            buttonOpenDrawer.UseVisualStyleBackColor = true;
            buttonOpenDrawer.Click += buttonOpenDrawer_Click;
            // 
            // buttonGiftCard
            // 
            buttonGiftCard.BackColor = Color.WhiteSmoke;
            buttonGiftCard.Dock = DockStyle.Fill;
            buttonGiftCard.FlatStyle = FlatStyle.System;
            buttonGiftCard.Font = new Font("Microsoft Sans Serif", 8.25F);
            buttonGiftCard.ForeColor = Color.HotPink;
            buttonGiftCard.Location = new Point(459, 63);
            buttonGiftCard.Name = "buttonGiftCard";
            buttonGiftCard.Size = new Size(149, 55);
            buttonGiftCard.TabIndex = 3;
            buttonGiftCard.Text = "Gift Card";
            buttonGiftCard.UseVisualStyleBackColor = false;
            buttonGiftCard.Click += buttonGiftCard_Click;
            // 
            // buttonPayCash
            // 
            buttonPayCash.Dock = DockStyle.Fill;
            buttonPayCash.FlatStyle = FlatStyle.System;
            buttonPayCash.Font = new Font("Microsoft Sans Serif", 8.25F);
            buttonPayCash.ForeColor = Color.White;
            buttonPayCash.Location = new Point(459, 3);
            buttonPayCash.Name = "buttonPayCash";
            buttonPayCash.Size = new Size(149, 54);
            buttonPayCash.TabIndex = 4;
            buttonPayCash.Text = "Pay Cash";
            buttonPayCash.UseVisualStyleBackColor = true;
            buttonPayCash.Click += buttonPayCash_Click;
            // 
            // buttonSplit
            // 
            buttonSplit.Dock = DockStyle.Fill;
            buttonSplit.FlatStyle = FlatStyle.Flat;
            buttonSplit.Font = new Font("Microsoft Sans Serif", 8.25F);
            buttonSplit.Location = new Point(307, 3);
            buttonSplit.Name = "buttonSplit";
            buttonSplit.Size = new Size(146, 54);
            buttonSplit.TabIndex = 8;
            buttonSplit.Text = "Split Pay";
            buttonSplit.UseVisualStyleBackColor = true;
            buttonSplit.Click += buttonSplit_Click;
            // 
            // labelCashier
            // 
            labelCashier.BackColor = Color.Transparent;
            labelCashier.Dock = DockStyle.Fill;
            labelCashier.FlatAppearance.BorderColor = Color.YellowGreen;
            labelCashier.FlatStyle = FlatStyle.Flat;
            labelCashier.Font = new Font("Microsoft Sans Serif", 12F);
            labelCashier.ForeColor = Color.FromArgb(59, 130, 246);
            labelCashier.Location = new Point(1668, 3);
            labelCashier.Name = "labelCashier";
            labelCashier.Size = new Size(181, 41);
            labelCashier.TabIndex = 3;
            labelCashier.Text = "Username";
            labelCashier.UseVisualStyleBackColor = false;
            labelCashier.Click += labelCashier_Click;
            // 
            // buttonClose
            // 
            buttonClose.Dock = DockStyle.Fill;
            buttonClose.Location = new Point(1854, 2);
            buttonClose.Margin = new Padding(2);
            buttonClose.Name = "buttonClose";
            buttonClose.Size = new Size(214, 43);
            buttonClose.TabIndex = 6;
            buttonClose.Text = "Close";
            buttonClose.UseVisualStyleBackColor = true;
            buttonClose.Click += buttonLogout_Click;
            // 
            // pnlProductStrip
            // 
            pnlProductStrip.BackColor = Color.FromArgb(248, 250, 252);
            pnlProductStrip.Controls.Add(labelProductName);
            pnlProductStrip.Dock = DockStyle.Fill;
            pnlProductStrip.Margin = new Padding(0);
            pnlProductStrip.Name = "pnlProductStrip";
            pnlProductStrip.Padding = new Padding(0);
            // 
            // labelProductName
            // 
            labelProductName.Dock = DockStyle.Fill;
            labelProductName.Font = new Font("Segoe UI", 12F, FontStyle.Regular);
            labelProductName.ForeColor = Color.FromArgb(71, 85, 105);
            labelProductName.Margin = new Padding(0);
            labelProductName.Name = "labelProductName";
            labelProductName.Padding = new Padding(16, 0, 0, 0);
            labelProductName.TabIndex = 2;
            labelProductName.Text = "Ready to scan...";
            labelProductName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.BackColor = Color.FromArgb(15, 23, 42);
            tableLayoutPanel1.ColumnCount = 6;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            tableLayoutPanel1.Controls.Add(textBoxUPC, 1, 0);
            tableLayoutPanel1.Controls.Add(buttonInvoice, 2, 0);
            tableLayoutPanel1.Controls.Add(ButtonSettings, 3, 0);
            tableLayoutPanel1.Controls.Add(labelCashier, 4, 0);
            tableLayoutPanel1.Controls.Add(buttonClose, 5, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(3, 2);
            tableLayoutPanel1.Margin = new Padding(3, 2, 3, 2);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(2070, 47);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // ButtonSettings
            // 
            ButtonSettings.Dock = DockStyle.Fill;
            ButtonSettings.Location = new Point(1481, 2);
            ButtonSettings.Margin = new Padding(2);
            ButtonSettings.Name = "ButtonSettings";
            ButtonSettings.Size = new Size(182, 43);
            ButtonSettings.TabIndex = 11;
            ButtonSettings.Text = "Settings";
            ButtonSettings.UseVisualStyleBackColor = true;
            ButtonSettings.Click += ButtonSettings_Click;
            // 
            // buttonInvoice
            // 
            buttonInvoice.Dock = DockStyle.Fill;
            buttonInvoice.Location = new Point(1114, 2);
            buttonInvoice.Margin = new Padding(2);
            buttonInvoice.Name = "buttonInvoice";
            buttonInvoice.Size = new Size(181, 43);
            buttonInvoice.TabIndex = 9;
            buttonInvoice.Text = "Print Invoice";
            buttonInvoice.UseVisualStyleBackColor = true;
            buttonInvoice.Click += buttonInvoice_Click;
            // 
            // textBoxUPC
            // 
            textBoxUPC.Anchor = AnchorStyles.None;
            textBoxUPC.BackColor = Color.WhiteSmoke;
            textBoxUPC.BorderStyle = BorderStyle.None;
            textBoxUPC.Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            textBoxUPC.ForeColor = Color.DarkOliveGreen;
            textBoxUPC.Location = new Point(618, 3);
            textBoxUPC.MaxLength = 32000;
            textBoxUPC.Name = "textBoxUPC";
            textBoxUPC.PlaceholderText = "UPC";
            textBoxUPC.Size = new Size(306, 46);
            textBoxUPC.TabIndex = 1;
            textBoxUPC.TextAlign = HorizontalAlignment.Center;
            textBoxUPC.TextChanged += textBoxUPC_TextChanged;
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
            roundedPanel1.ResumeLayout(false);
            tableLayoutPanelTotal.ResumeLayout(false);
            tableLayoutPanelTotal.PerformLayout();
            tableLayoutPanelCategoria.ResumeLayout(false);
            tabControlMenuCategories.ResumeLayout(false);
            tableLayoutPanelPayment.ResumeLayout(false);
            roundedPanel2.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel4.PerformLayout();
            splitContainerScale.Panel1.ResumeLayout(false);
            splitContainerScale.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerScale).EndInit();
            splitContainerScale.ResumeLayout(false);
            tableLayoutPanelPesado.ResumeLayout(false);
            tableLayoutPanelPesado.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPesado).EndInit();
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            pnlProductStrip.ResumeLayout(false);
            MaintableLayout.ResumeLayout(false);
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
        private TabPage tabPage2;
        private TableLayoutPanel tableLayoutPanelPayment;
        private Label label4;
        private Label labelInputValue;
        private Label labelChangeValue;
        private Label label5;
        private TableLayoutPanel tableLayoutPanel1;
        private Button buttonEBTBalance;
        private Button buttonSplit;
        private Button buttonOpenDrawer;
        private Button labelCashier;
        private Componentes.AbecedarioControl abecedarioControl1;
        private TableLayoutPanel tableLayoutPanel3;
        private Button buttonPayCash;
        private Button buttonGiftCard;
        private Button buttonEBTFood;
        private Button buttonPayDebitCard;
        private Button buttonPayCreditCard;
        private TableLayoutPanel tableLayoutPanelPesado;
        private PictureBox pictureBoxPesado;
        private Label labelPesaProduct;
        private Label labelWeight;
        private Label lblScalStatusDesc;
        private SplitContainer splitContainerScale;
        private TableLayoutPanel MaintableLayout;
        private Componentes.KeyPaymentControl keyPaymentControl1;
        private TextBox textBoxUPC;
        private TableLayoutPanel tableLayoutPanel2;
        private Button buttonInvoice;
        private Button ButtonSettings;
        private Panel pnlProductStrip;
        private Button buttonLookup;
        private Button buttonClose;
        private Button buttonQsale;
        private ListView listViewCart;
        private Componentes.RoundedPanel roundedPanel1;
        private Componentes.RoundedPanel roundedPanel2;
        private TableLayoutPanel tableLayoutPanel4;
    }
}