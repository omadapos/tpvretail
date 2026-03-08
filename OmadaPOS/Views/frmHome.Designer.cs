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
            tableLayoutPanel2 = new TableLayoutPanel();
            textBoxUPC = new TextBox();
            buttonInvoice = new Button();
            buttonClose = new Button();
            labelCashier = new Button();
            ButtonSettings = new Button();
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
            tableLayoutPanelTotal = new TableLayoutPanel();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            labelSubTotal = new Label();
            labelTotalTax = new Label();
            labelTotalValue = new Label();
            tableLayoutPanel4 = new TableLayoutPanel();
            buttonQsale = new Button();
            buttonLookup = new Button();
            label5 = new Label();
            labelInputValue = new Label();
            label4 = new Label();
            labelChangeValue = new Label();
            labelProductName = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            MaintableLayout = new TableLayoutPanel();
            pnlProductStrip = new Panel();
            tableLayoutPanelMain.SuspendLayout();
            tableLayoutPanelCart.SuspendLayout();
            tableLayoutPanelButtonCart.SuspendLayout();
            tableLayoutPanelCategoria.SuspendLayout();
            tableLayoutPanelPayment.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerScale).BeginInit();
            splitContainerScale.Panel1.SuspendLayout();
            splitContainerScale.Panel2.SuspendLayout();
            splitContainerScale.SuspendLayout();
            tableLayoutPanelPesado.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPesado).BeginInit();
            tableLayoutPanel3.SuspendLayout();
            tableLayoutPanelTotal.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
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
            // tableLayoutPanelPayment
            // 
            tableLayoutPanelPayment.BackColor = Color.WhiteSmoke;
            tableLayoutPanelPayment.ColumnCount = 1;
            tableLayoutPanelPayment.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelPayment.Controls.Add(tableLayoutPanel2, 0, 0);
            tableLayoutPanelPayment.Controls.Add(splitContainerScale, 0, 3);
            tableLayoutPanelPayment.Controls.Add(tableLayoutPanel3, 0, 4);
            tableLayoutPanelPayment.Dock = DockStyle.Fill;
            tableLayoutPanelPayment.Location = new Point(1451, 2);
            tableLayoutPanelPayment.Margin = new Padding(2);
            tableLayoutPanelPayment.Name = "tableLayoutPanelPayment";
            tableLayoutPanelPayment.RowCount = 5;
            tableLayoutPanelPayment.RowStyles.Add(new RowStyle(SizeType.Percent, 8.072917F));
            tableLayoutPanelPayment.RowStyles.Add(new RowStyle(SizeType.Percent, 46.875F));
            tableLayoutPanelPayment.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tableLayoutPanelPayment.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tableLayoutPanelPayment.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tableLayoutPanelPayment.Size = new Size(617, 768);
            tableLayoutPanelPayment.TabIndex = 2;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.BackColor = Color.WhiteSmoke;
            tableLayoutPanel2.ColumnCount = 5;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.3980427F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 13.5399675F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 21.37031F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52.69168F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 168F));
            tableLayoutPanel2.Controls.Add(textBoxUPC, 4, 0);
            tableLayoutPanel2.Controls.Add(buttonInvoice, 0, 0);
            tableLayoutPanel2.Controls.Add(buttonClose, 3, 0);
            tableLayoutPanel2.Controls.Add(labelCashier, 2, 0);
            tableLayoutPanel2.Controls.Add(ButtonSettings, 1, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(2, 2);
            tableLayoutPanel2.Margin = new Padding(2);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(613, 58);
            tableLayoutPanel2.TabIndex = 11;
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
            buttonInvoice.Dock = DockStyle.Fill;
            buttonInvoice.Location = new Point(2, 2);
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
            buttonClose.Dock = DockStyle.Fill;
            buttonClose.Location = new Point(212, 2);
            buttonClose.Margin = new Padding(2);
            buttonClose.Name = "buttonClose";
            buttonClose.Size = new Size(230, 54);
            buttonClose.TabIndex = 6;
            buttonClose.Text = "Close";
            buttonClose.UseVisualStyleBackColor = true;
            buttonClose.Click += buttonLogout_Click;
            // 
            // labelCashier
            // 
            labelCashier.BackColor = Color.Transparent;
            labelCashier.Dock = DockStyle.Fill;
            labelCashier.FlatAppearance.BorderColor = Color.YellowGreen;
            labelCashier.FlatStyle = FlatStyle.Flat;
            labelCashier.Font = new Font("Microsoft Sans Serif", 12F);
            labelCashier.ForeColor = Color.FromArgb(59, 130, 246);
            labelCashier.Location = new Point(118, 3);
            labelCashier.Name = "labelCashier";
            labelCashier.Size = new Size(89, 52);
            labelCashier.TabIndex = 3;
            labelCashier.Text = "Username";
            labelCashier.UseVisualStyleBackColor = false;
            labelCashier.Click += labelCashier_Click;
            // 
            // ButtonSettings
            // 
            ButtonSettings.Dock = DockStyle.Fill;
            ButtonSettings.Location = new Point(57, 2);
            ButtonSettings.Margin = new Padding(2);
            ButtonSettings.Name = "ButtonSettings";
            ButtonSettings.Size = new Size(56, 54);
            ButtonSettings.TabIndex = 11;
            ButtonSettings.Text = "Settings";
            ButtonSettings.UseVisualStyleBackColor = true;
            ButtonSettings.Click += ButtonSettings_Click;
            // 
            // splitContainerScale
            // 
            splitContainerScale.Dock = DockStyle.Fill;
            splitContainerScale.Font = new Font("Verdana", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            splitContainerScale.ForeColor = Color.Black;
            splitContainerScale.IsSplitterFixed = true;
            splitContainerScale.Location = new Point(8, 545);
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
            splitContainerScale.Size = new Size(601, 99);
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
            tableLayoutPanelPesado.Size = new Size(319, 99);
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
            labelWeight.Size = new Size(313, 23);
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
            labelPesaProduct.Location = new Point(3, 23);
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
            lblScalStatusDesc.Location = new Point(3, 78);
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
            pictureBoxPesado.Size = new Size(276, 99);
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
            tableLayoutPanel3.Location = new Point(3, 654);
            tableLayoutPanel3.Margin = new Padding(3, 2, 3, 2);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 2;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.Size = new Size(611, 112);
            tableLayoutPanel3.TabIndex = 4;
            // 
            // buttonEBTFood
            // 
            buttonEBTFood.Dock = DockStyle.Fill;
            buttonEBTFood.FlatStyle = FlatStyle.System;
            buttonEBTFood.Font = new Font("Microsoft Sans Serif", 8.25F);
            buttonEBTFood.Location = new Point(155, 59);
            buttonEBTFood.Name = "buttonEBTFood";
            buttonEBTFood.Size = new Size(146, 50);
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
            buttonPayDebitCard.Location = new Point(155, 3);
            buttonPayDebitCard.Name = "buttonPayDebitCard";
            buttonPayDebitCard.Size = new Size(146, 50);
            buttonPayDebitCard.TabIndex = 1;
            buttonPayDebitCard.Text = "Debit Card";
            buttonPayDebitCard.UseVisualStyleBackColor = false;
            buttonPayDebitCard.Click += buttonPayDebitCard_Click;
            // 
            // buttonPayCreditCard
            // 
            buttonPayCreditCard.Dock = DockStyle.Fill;
            buttonPayCreditCard.FlatStyle = FlatStyle.System;
            buttonPayCreditCard.Font = new Font("Microsoft Sans Serif", 8.25F);
            buttonPayCreditCard.Location = new Point(3, 3);
            buttonPayCreditCard.Name = "buttonPayCreditCard";
            buttonPayCreditCard.Size = new Size(146, 50);
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
            buttonEBTBalance.Location = new Point(3, 59);
            buttonEBTBalance.Name = "buttonEBTBalance";
            buttonEBTBalance.Size = new Size(146, 50);
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
            buttonOpenDrawer.Location = new Point(307, 59);
            buttonOpenDrawer.Name = "buttonOpenDrawer";
            buttonOpenDrawer.Size = new Size(146, 50);
            buttonOpenDrawer.TabIndex = 7;
            buttonOpenDrawer.Text = "Open Drawer";
            buttonOpenDrawer.UseVisualStyleBackColor = true;
            buttonOpenDrawer.Click += buttonOpenDrawer_Click;
            // 
            // buttonGiftCard
            // 
            buttonGiftCard.Dock = DockStyle.Fill;
            buttonGiftCard.FlatStyle = FlatStyle.System;
            buttonGiftCard.Font = new Font("Microsoft Sans Serif", 8.25F);
            buttonGiftCard.Location = new Point(459, 59);
            buttonGiftCard.Name = "buttonGiftCard";
            buttonGiftCard.Size = new Size(149, 50);
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
            buttonPayCash.Location = new Point(459, 3);
            buttonPayCash.Name = "buttonPayCash";
            buttonPayCash.Size = new Size(149, 50);
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
            buttonSplit.Size = new Size(146, 50);
            buttonSplit.TabIndex = 8;
            buttonSplit.Text = "Split Pay";
            buttonSplit.UseVisualStyleBackColor = true;
            buttonSplit.Click += buttonSplit_Click;
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
            tableLayoutPanel4.Size = new Size(571, 69);
            tableLayoutPanel4.TabIndex = 0;
            // 
            // buttonQsale
            // 
            buttonQsale.Dock = DockStyle.Fill;
            buttonQsale.Location = new Point(5, 5);
            buttonQsale.Margin = new Padding(2);
            buttonQsale.Name = "buttonQsale";
            buttonQsale.Size = new Size(222, 27);
            buttonQsale.TabIndex = 7;
            buttonQsale.Text = "Quick Sale";
            buttonQsale.UseVisualStyleBackColor = true;
            buttonQsale.Click += buttonProductNoTax_Click;
            // 
            // buttonLookup
            // 
            buttonLookup.Dock = DockStyle.Fill;
            buttonLookup.Location = new Point(5, 36);
            buttonLookup.Margin = new Padding(2);
            buttonLookup.Name = "buttonLookup";
            buttonLookup.Size = new Size(222, 28);
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
            label5.Location = new Point(232, 34);
            label5.Name = "label5";
            label5.Size = new Size(163, 32);
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
            labelInputValue.Size = new Size(154, 15);
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
            label4.Size = new Size(165, 27);
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
            labelChangeValue.Location = new Point(401, 34);
            labelChangeValue.Name = "labelChangeValue";
            labelChangeValue.Size = new Size(164, 32);
            labelChangeValue.TabIndex = 3;
            labelChangeValue.Text = "0.00";
            labelChangeValue.TextAlign = ContentAlignment.MiddleCenter;
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
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
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
            tableLayoutPanelPayment.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            splitContainerScale.Panel1.ResumeLayout(false);
            splitContainerScale.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerScale).EndInit();
            splitContainerScale.ResumeLayout(false);
            tableLayoutPanelPesado.ResumeLayout(false);
            tableLayoutPanelPesado.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPesado).EndInit();
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanelTotal.ResumeLayout(false);
            tableLayoutPanelTotal.PerformLayout();
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel4.PerformLayout();
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
        private TableLayoutPanel tableLayoutPanelPayment;
        private TableLayoutPanel tableLayoutPanel2;
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
        private TextBox textBoxUPC;
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
