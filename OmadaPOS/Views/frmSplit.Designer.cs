namespace OmadaPOS.Views
{
    partial class frmSplit
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
            tableLayoutSplit = new TableLayoutPanel();
            tableLayoutCart = new TableLayoutPanel();
            listViewCart = new ListView();
            listViewPayments = new ListView();
            tableLayoutPanel2 = new TableLayoutPanel();
            buttonDebit = new Button();
            buttonEbt = new Button();
            buttonCash = new Button();
            buttonCredit = new Button();
            buttonClose = new Button();
            tableLayoutPanel3 = new TableLayoutPanel();
            tableLayoutPanel4 = new TableLayoutPanel();
            button7 = new Button();
            button8 = new Button();
            button9 = new Button();
            button4 = new Button();
            button5 = new Button();
            button6 = new Button();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button00 = new Button();
            button0 = new Button();
            buttonClear = new Button();
            tableLayoutPanel5 = new TableLayoutPanel();
            buttonPrintBill = new Button();
            buttonCalculateEBT = new Button();
            roundedPanel1 = new OmadaPOS.Componentes.RoundedPanel();
            tableLayoutPanel6 = new TableLayoutPanel();
            labelDueValue = new Label();
            labelTotal = new Label();
            labelcharged = new Label();
            labelTotalValue = new Label();
            labelChargedValue = new Label();
            label1 = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            buttonRemain = new Button();
            button20usd = new Button();
            button10usd = new Button();
            button50usd = new Button();
            button100usd = new Button();
            buttonEbtBalance = new Button();
            tableLayoutSplit.SuspendLayout();
            tableLayoutCart.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            roundedPanel1.SuspendLayout();
            tableLayoutPanel6.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutSplit
            // 
            tableLayoutSplit.BackColor = Color.WhiteSmoke;
            tableLayoutSplit.ColumnCount = 4;
            tableLayoutSplit.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 29.9782944F));
            tableLayoutSplit.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40.0353737F));
            tableLayoutSplit.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11.3353167F));
            tableLayoutSplit.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18.6510181F));
            tableLayoutSplit.Controls.Add(tableLayoutCart, 0, 1);
            tableLayoutSplit.Controls.Add(tableLayoutPanel2, 3, 1);
            tableLayoutSplit.Controls.Add(tableLayoutPanel3, 1, 1);
            tableLayoutSplit.Controls.Add(tableLayoutPanel1, 2, 1);
            tableLayoutSplit.Dock = DockStyle.Fill;
            tableLayoutSplit.Location = new Point(0, 0);
            tableLayoutSplit.Margin = new Padding(93, 107, 93, 107);
            tableLayoutSplit.Name = "tableLayoutSplit";
            tableLayoutSplit.RowCount = 3;
            tableLayoutSplit.RowStyles.Add(new RowStyle(SizeType.Absolute, 98F));
            tableLayoutSplit.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutSplit.RowStyles.Add(new RowStyle(SizeType.Absolute, 68F));
            tableLayoutSplit.Size = new Size(2308, 1421);
            tableLayoutSplit.TabIndex = 0;
            // 
            // tableLayoutCart
            // 
            tableLayoutCart.BackColor = Color.WhiteSmoke;
            tableLayoutCart.ColumnCount = 1;
            tableLayoutCart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutCart.Controls.Add(listViewCart, 0, 0);
            tableLayoutCart.Controls.Add(listViewPayments, 0, 1);
            tableLayoutCart.Dock = DockStyle.Fill;
            tableLayoutCart.Location = new Point(6, 104);
            tableLayoutCart.Margin = new Padding(6, 6, 6, 6);
            tableLayoutCart.Name = "tableLayoutCart";
            tableLayoutCart.RowCount = 2;
            tableLayoutCart.RowStyles.Add(new RowStyle(SizeType.Percent, 84.5705948F));
            tableLayoutCart.RowStyles.Add(new RowStyle(SizeType.Percent, 15.4294033F));
            tableLayoutCart.Size = new Size(679, 1243);
            tableLayoutCart.TabIndex = 0;
            // 
            // listViewCart
            // 
            listViewCart.BackColor = Color.White;
            listViewCart.Dock = DockStyle.Fill;
            listViewCart.Location = new Point(6, 6);
            listViewCart.Margin = new Padding(6, 6, 6, 6);
            listViewCart.Name = "listViewCart";
            listViewCart.Size = new Size(667, 1039);
            listViewCart.TabIndex = 0;
            listViewCart.UseCompatibleStateImageBehavior = false;
            // 
            // listViewPayments
            // 
            listViewPayments.BackColor = Color.White;
            listViewPayments.Dock = DockStyle.Fill;
            listViewPayments.Location = new Point(6, 1057);
            listViewPayments.Margin = new Padding(6, 6, 6, 6);
            listViewPayments.Name = "listViewPayments";
            listViewPayments.Size = new Size(667, 180);
            listViewPayments.TabIndex = 1;
            listViewPayments.UseCompatibleStateImageBehavior = false;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.BackColor = Color.WhiteSmoke;
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(buttonDebit, 0, 1);
            tableLayoutPanel2.Controls.Add(buttonEbt, 0, 2);
            tableLayoutPanel2.Controls.Add(buttonCash, 0, 4);
            tableLayoutPanel2.Controls.Add(buttonCredit, 0, 3);
            tableLayoutPanel2.Controls.Add(buttonClose, 0, 5);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(1882, 104);
            tableLayoutPanel2.Margin = new Padding(6, 6, 6, 6);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 6;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tableLayoutPanel2.Size = new Size(420, 1243);
            tableLayoutPanel2.TabIndex = 2;
            // 
            // buttonDebit
            // 
            buttonDebit.Dock = DockStyle.Fill;
            buttonDebit.Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Bold);
            buttonDebit.Location = new Point(6, 316);
            buttonDebit.Margin = new Padding(6, 6, 6, 6);
            buttonDebit.Name = "buttonDebit";
            buttonDebit.Size = new Size(408, 174);
            buttonDebit.TabIndex = 1;
            buttonDebit.Tag = "DEBIT";
            buttonDebit.Text = "Debit Card";
            buttonDebit.UseVisualStyleBackColor = true;
            buttonDebit.Click += PaymentButton_Click;
            // 
            // buttonEbt
            // 
            buttonEbt.Dock = DockStyle.Fill;
            buttonEbt.Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Bold);
            buttonEbt.Location = new Point(6, 502);
            buttonEbt.Margin = new Padding(6, 6, 6, 6);
            buttonEbt.Name = "buttonEbt";
            buttonEbt.Size = new Size(408, 174);
            buttonEbt.TabIndex = 2;
            buttonEbt.Tag = "EBT";
            buttonEbt.Text = "EBT Card";
            buttonEbt.UseVisualStyleBackColor = true;
            buttonEbt.Click += PaymentButton_Click;
            // 
            // buttonCash
            // 
            buttonCash.Dock = DockStyle.Fill;
            buttonCash.Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Bold);
            buttonCash.Location = new Point(6, 874);
            buttonCash.Margin = new Padding(6, 6, 6, 6);
            buttonCash.Name = "buttonCash";
            buttonCash.Size = new Size(408, 174);
            buttonCash.TabIndex = 3;
            buttonCash.Tag = "CASH";
            buttonCash.Text = "Cash Pay";
            buttonCash.UseVisualStyleBackColor = true;
            buttonCash.Click += PaymentButton_Click;
            // 
            // buttonCredit
            // 
            buttonCredit.Dock = DockStyle.Fill;
            buttonCredit.Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Bold);
            buttonCredit.Location = new Point(6, 688);
            buttonCredit.Margin = new Padding(6, 6, 6, 6);
            buttonCredit.Name = "buttonCredit";
            buttonCredit.Size = new Size(408, 174);
            buttonCredit.TabIndex = 0;
            buttonCredit.Tag = "CREDIT";
            buttonCredit.Text = "Credit Card";
            buttonCredit.UseVisualStyleBackColor = true;
            buttonCredit.Click += PaymentButton_Click;
            // 
            // buttonClose
            // 
            buttonClose.Dock = DockStyle.Fill;
            buttonClose.Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Bold);
            buttonClose.Location = new Point(6, 1060);
            buttonClose.Margin = new Padding(6, 6, 6, 6);
            buttonClose.Name = "buttonClose";
            buttonClose.Size = new Size(408, 177);
            buttonClose.TabIndex = 4;
            buttonClose.Text = "Close";
            buttonClose.UseVisualStyleBackColor = true;
            buttonClose.Click += buttonClose_Click;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.BackColor = Color.WhiteSmoke;
            tableLayoutPanel3.ColumnCount = 1;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Controls.Add(tableLayoutPanel4, 0, 1);
            tableLayoutPanel3.Controls.Add(tableLayoutPanel5, 0, 2);
            tableLayoutPanel3.Controls.Add(roundedPanel1, 0, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(697, 104);
            tableLayoutPanel3.Margin = new Padding(6, 6, 6, 6);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 3;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel3.Size = new Size(912, 1243);
            tableLayoutPanel3.TabIndex = 3;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.BackColor = Color.WhiteSmoke;
            tableLayoutPanel4.ColumnCount = 3;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel4.Controls.Add(button7, 0, 0);
            tableLayoutPanel4.Controls.Add(button8, 1, 0);
            tableLayoutPanel4.Controls.Add(button9, 2, 0);
            tableLayoutPanel4.Controls.Add(button4, 0, 1);
            tableLayoutPanel4.Controls.Add(button5, 1, 1);
            tableLayoutPanel4.Controls.Add(button6, 2, 1);
            tableLayoutPanel4.Controls.Add(button1, 0, 2);
            tableLayoutPanel4.Controls.Add(button2, 1, 2);
            tableLayoutPanel4.Controls.Add(button3, 2, 2);
            tableLayoutPanel4.Controls.Add(button00, 0, 3);
            tableLayoutPanel4.Controls.Add(button0, 1, 3);
            tableLayoutPanel4.Controls.Add(buttonClear, 2, 3);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.Location = new Point(6, 254);
            tableLayoutPanel4.Margin = new Padding(6, 6, 6, 6);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 4;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel4.Size = new Size(900, 858);
            tableLayoutPanel4.TabIndex = 0;
            // 
            // button7
            // 
            button7.Dock = DockStyle.Fill;
            button7.Location = new Point(6, 6);
            button7.Margin = new Padding(6, 6, 6, 6);
            button7.Name = "button7";
            button7.Size = new Size(287, 202);
            button7.TabIndex = 0;
            button7.Tag = "7";
            button7.Text = "7";
            button7.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            button8.Dock = DockStyle.Fill;
            button8.Location = new Point(305, 6);
            button8.Margin = new Padding(6, 6, 6, 6);
            button8.Name = "button8";
            button8.Size = new Size(287, 202);
            button8.TabIndex = 1;
            button8.Tag = "8";
            button8.Text = "8";
            button8.UseVisualStyleBackColor = true;
            // 
            // button9
            // 
            button9.Dock = DockStyle.Fill;
            button9.Location = new Point(604, 6);
            button9.Margin = new Padding(6, 6, 6, 6);
            button9.Name = "button9";
            button9.Size = new Size(290, 202);
            button9.TabIndex = 2;
            button9.Tag = "9";
            button9.Text = "9";
            button9.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Dock = DockStyle.Fill;
            button4.Location = new Point(6, 220);
            button4.Margin = new Padding(6, 6, 6, 6);
            button4.Name = "button4";
            button4.Size = new Size(287, 202);
            button4.TabIndex = 3;
            button4.Tag = "4";
            button4.Text = "4";
            button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            button5.Dock = DockStyle.Fill;
            button5.Location = new Point(305, 220);
            button5.Margin = new Padding(6, 6, 6, 6);
            button5.Name = "button5";
            button5.Size = new Size(287, 202);
            button5.TabIndex = 4;
            button5.Tag = "5";
            button5.Text = "5";
            button5.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            button6.Dock = DockStyle.Fill;
            button6.Location = new Point(604, 220);
            button6.Margin = new Padding(6, 6, 6, 6);
            button6.Name = "button6";
            button6.Size = new Size(290, 202);
            button6.TabIndex = 5;
            button6.Tag = "6";
            button6.Text = "6";
            button6.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Dock = DockStyle.Fill;
            button1.Location = new Point(6, 434);
            button1.Margin = new Padding(6, 6, 6, 6);
            button1.Name = "button1";
            button1.Size = new Size(287, 202);
            button1.TabIndex = 6;
            button1.Tag = "1";
            button1.Text = "1";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Dock = DockStyle.Fill;
            button2.Location = new Point(305, 434);
            button2.Margin = new Padding(6, 6, 6, 6);
            button2.Name = "button2";
            button2.Size = new Size(287, 202);
            button2.TabIndex = 7;
            button2.Tag = "2";
            button2.Text = "2";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Dock = DockStyle.Fill;
            button3.Location = new Point(604, 434);
            button3.Margin = new Padding(6, 6, 6, 6);
            button3.Name = "button3";
            button3.Size = new Size(290, 202);
            button3.TabIndex = 8;
            button3.Tag = "3";
            button3.Text = "3";
            button3.UseVisualStyleBackColor = true;
            // 
            // button00
            // 
            button00.Dock = DockStyle.Fill;
            button00.Location = new Point(6, 648);
            button00.Margin = new Padding(6, 6, 6, 6);
            button00.Name = "button00";
            button00.Size = new Size(287, 204);
            button00.TabIndex = 9;
            button00.Tag = "00";
            button00.Text = "00";
            button00.UseVisualStyleBackColor = true;
            // 
            // button0
            // 
            button0.Dock = DockStyle.Fill;
            button0.Location = new Point(305, 648);
            button0.Margin = new Padding(6, 6, 6, 6);
            button0.Name = "button0";
            button0.Size = new Size(287, 204);
            button0.TabIndex = 10;
            button0.Tag = "0";
            button0.Text = "0";
            button0.UseVisualStyleBackColor = true;
            // 
            // buttonClear
            // 
            buttonClear.Dock = DockStyle.Fill;
            buttonClear.Location = new Point(604, 648);
            buttonClear.Margin = new Padding(6, 6, 6, 6);
            buttonClear.Name = "buttonClear";
            buttonClear.Size = new Size(290, 204);
            buttonClear.TabIndex = 11;
            buttonClear.Tag = "Clear";
            buttonClear.Text = "Clear";
            buttonClear.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.BackColor = Color.WhiteSmoke;
            tableLayoutPanel5.ColumnCount = 2;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.Controls.Add(buttonPrintBill, 1, 0);
            tableLayoutPanel5.Controls.Add(buttonCalculateEBT, 0, 0);
            tableLayoutPanel5.Dock = DockStyle.Fill;
            tableLayoutPanel5.Location = new Point(6, 1124);
            tableLayoutPanel5.Margin = new Padding(6, 6, 6, 6);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 1;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.Size = new Size(900, 113);
            tableLayoutPanel5.TabIndex = 1;
            // 
            // buttonPrintBill
            // 
            buttonPrintBill.Dock = DockStyle.Fill;
            buttonPrintBill.Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Bold);
            buttonPrintBill.Location = new Point(456, 6);
            buttonPrintBill.Margin = new Padding(6, 6, 6, 6);
            buttonPrintBill.Name = "buttonPrintBill";
            buttonPrintBill.Size = new Size(438, 101);
            buttonPrintBill.TabIndex = 0;
            buttonPrintBill.Text = "Print Bill";
            buttonPrintBill.UseVisualStyleBackColor = true;
            buttonPrintBill.Click += buttonPrintBill_Click;
            // 
            // buttonCalculateEBT
            // 
            buttonCalculateEBT.Dock = DockStyle.Fill;
            buttonCalculateEBT.Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Bold);
            buttonCalculateEBT.Location = new Point(6, 6);
            buttonCalculateEBT.Margin = new Padding(6, 6, 6, 6);
            buttonCalculateEBT.Name = "buttonCalculateEBT";
            buttonCalculateEBT.Size = new Size(438, 101);
            buttonCalculateEBT.TabIndex = 1;
            buttonCalculateEBT.Text = "Calculate EBT";
            buttonCalculateEBT.UseVisualStyleBackColor = true;
            buttonCalculateEBT.Click += buttonCalculateEBT_Click;
            // 
            // roundedPanel1
            // 
            roundedPanel1.BackColor = Color.Transparent;
            roundedPanel1.Controls.Add(tableLayoutPanel6);
            roundedPanel1.Dock = DockStyle.Fill;
            roundedPanel1.Location = new Point(6, 6);
            roundedPanel1.Margin = new Padding(6, 6, 6, 6);
            roundedPanel1.Name = "roundedPanel1";
            roundedPanel1.Padding = new Padding(37, 43, 37, 43);
            roundedPanel1.Size = new Size(900, 236);
            roundedPanel1.TabIndex = 2;
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.ColumnCount = 2;
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            tableLayoutPanel6.Controls.Add(labelDueValue, 1, 2);
            tableLayoutPanel6.Controls.Add(labelTotal, 0, 0);
            tableLayoutPanel6.Controls.Add(labelcharged, 0, 1);
            tableLayoutPanel6.Controls.Add(labelTotalValue, 1, 0);
            tableLayoutPanel6.Controls.Add(labelChargedValue, 1, 1);
            tableLayoutPanel6.Controls.Add(label1, 0, 2);
            tableLayoutPanel6.Dock = DockStyle.Fill;
            tableLayoutPanel6.Location = new Point(37, 43);
            tableLayoutPanel6.Margin = new Padding(6, 6, 6, 6);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            tableLayoutPanel6.RowCount = 3;
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            tableLayoutPanel6.Size = new Size(826, 150);
            tableLayoutPanel6.TabIndex = 2;
            // 
            // labelDueValue
            // 
            labelDueValue.AutoSize = true;
            labelDueValue.Dock = DockStyle.Fill;
            labelDueValue.Font = new Font("Microsoft Sans Serif", 18F, FontStyle.Bold | FontStyle.Italic);
            labelDueValue.Location = new Point(251, 90);
            labelDueValue.Margin = new Padding(4, 0, 4, 0);
            labelDueValue.Name = "labelDueValue";
            labelDueValue.Size = new Size(571, 60);
            labelDueValue.TabIndex = 5;
            labelDueValue.Text = "0.0";
            labelDueValue.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelTotal
            // 
            labelTotal.AutoSize = true;
            labelTotal.Dock = DockStyle.Fill;
            labelTotal.Font = new Font("Microsoft Sans Serif", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelTotal.Location = new Point(6, 0);
            labelTotal.Margin = new Padding(6, 0, 6, 0);
            labelTotal.Name = "labelTotal";
            labelTotal.Size = new Size(235, 45);
            labelTotal.TabIndex = 0;
            labelTotal.Text = "Total";
            // 
            // labelcharged
            // 
            labelcharged.AutoSize = true;
            labelcharged.Dock = DockStyle.Fill;
            labelcharged.Font = new Font("Microsoft Sans Serif", 18F, FontStyle.Bold);
            labelcharged.Location = new Point(6, 45);
            labelcharged.Margin = new Padding(6, 0, 6, 0);
            labelcharged.Name = "labelcharged";
            labelcharged.Size = new Size(235, 45);
            labelcharged.TabIndex = 1;
            labelcharged.Text = "Charged";
            // 
            // labelTotalValue
            // 
            labelTotalValue.AutoSize = true;
            labelTotalValue.Dock = DockStyle.Fill;
            labelTotalValue.Font = new Font("Microsoft Sans Serif", 18F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            labelTotalValue.Location = new Point(251, 0);
            labelTotalValue.Margin = new Padding(4, 0, 4, 0);
            labelTotalValue.Name = "labelTotalValue";
            labelTotalValue.Size = new Size(571, 45);
            labelTotalValue.TabIndex = 2;
            labelTotalValue.Text = "0.0";
            labelTotalValue.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelChargedValue
            // 
            labelChargedValue.AutoSize = true;
            labelChargedValue.Dock = DockStyle.Fill;
            labelChargedValue.Font = new Font("Microsoft Sans Serif", 18F, FontStyle.Bold | FontStyle.Italic);
            labelChargedValue.Location = new Point(251, 45);
            labelChargedValue.Margin = new Padding(4, 0, 4, 0);
            labelChargedValue.Name = "labelChargedValue";
            labelChargedValue.Size = new Size(571, 45);
            labelChargedValue.TabIndex = 3;
            labelChargedValue.Text = "0.0";
            labelChargedValue.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Microsoft Sans Serif", 18F, FontStyle.Bold);
            label1.Location = new Point(6, 90);
            label1.Margin = new Padding(6, 0, 6, 0);
            label1.Name = "label1";
            label1.Size = new Size(235, 60);
            label1.TabIndex = 4;
            label1.Text = "Due";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.BackColor = Color.WhiteSmoke;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(buttonRemain, 0, 0);
            tableLayoutPanel1.Controls.Add(button20usd, 0, 2);
            tableLayoutPanel1.Controls.Add(button10usd, 0, 1);
            tableLayoutPanel1.Controls.Add(button50usd, 0, 3);
            tableLayoutPanel1.Controls.Add(button100usd, 0, 4);
            tableLayoutPanel1.Controls.Add(buttonEbtBalance, 0, 5);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(1621, 104);
            tableLayoutPanel1.Margin = new Padding(6, 6, 6, 6);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 6;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tableLayoutPanel1.Size = new Size(249, 1243);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // buttonRemain
            // 
            buttonRemain.Dock = DockStyle.Fill;
            buttonRemain.Font = new Font("Microsoft Sans Serif", 18F, FontStyle.Bold | FontStyle.Italic);
            buttonRemain.Location = new Point(6, 6);
            buttonRemain.Margin = new Padding(6, 6, 6, 6);
            buttonRemain.Name = "buttonRemain";
            buttonRemain.Size = new Size(237, 298);
            buttonRemain.TabIndex = 4;
            buttonRemain.Text = "84.96";
            buttonRemain.UseVisualStyleBackColor = true;
            buttonRemain.Click += buttonRemain_Click;
            // 
            // button20usd
            // 
            button20usd.Dock = DockStyle.Fill;
            button20usd.Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Bold);
            button20usd.Location = new Point(6, 502);
            button20usd.Margin = new Padding(6, 6, 6, 6);
            button20usd.Name = "button20usd";
            button20usd.Size = new Size(237, 174);
            button20usd.TabIndex = 1;
            button20usd.Tag = "2000";
            button20usd.Text = "$ 20";
            button20usd.UseVisualStyleBackColor = true;
            // 
            // button10usd
            // 
            button10usd.Dock = DockStyle.Fill;
            button10usd.Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Bold);
            button10usd.Location = new Point(6, 316);
            button10usd.Margin = new Padding(6, 6, 6, 6);
            button10usd.Name = "button10usd";
            button10usd.Size = new Size(237, 174);
            button10usd.TabIndex = 5;
            button10usd.Tag = "1000";
            button10usd.Text = "$ 10";
            button10usd.UseVisualStyleBackColor = true;
            // 
            // button50usd
            // 
            button50usd.Dock = DockStyle.Fill;
            button50usd.Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Bold);
            button50usd.Location = new Point(6, 688);
            button50usd.Margin = new Padding(6, 6, 6, 6);
            button50usd.Name = "button50usd";
            button50usd.Size = new Size(237, 174);
            button50usd.TabIndex = 2;
            button50usd.Tag = "5000";
            button50usd.Text = "$ 50";
            button50usd.UseVisualStyleBackColor = true;
            // 
            // button100usd
            // 
            button100usd.Dock = DockStyle.Fill;
            button100usd.Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Bold);
            button100usd.Location = new Point(6, 874);
            button100usd.Margin = new Padding(6, 6, 6, 6);
            button100usd.Name = "button100usd";
            button100usd.Size = new Size(237, 174);
            button100usd.TabIndex = 3;
            button100usd.Tag = "10000";
            button100usd.Text = "$ 100";
            button100usd.UseVisualStyleBackColor = true;
            // 
            // buttonEbtBalance
            // 
            buttonEbtBalance.Dock = DockStyle.Fill;
            buttonEbtBalance.Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Bold);
            buttonEbtBalance.Location = new Point(6, 1060);
            buttonEbtBalance.Margin = new Padding(6, 6, 6, 6);
            buttonEbtBalance.Name = "buttonEbtBalance";
            buttonEbtBalance.Size = new Size(237, 177);
            buttonEbtBalance.TabIndex = 5;
            buttonEbtBalance.Tag = "EBT BALANCE";
            buttonEbtBalance.Text = "EBT Balance";
            buttonEbtBalance.UseVisualStyleBackColor = true;
            buttonEbtBalance.Click += PaymentButton_Click;
            // 
            // frmSplit
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(2308, 1421);
            Controls.Add(tableLayoutSplit);
            Margin = new Padding(6, 6, 6, 6);
            Name = "frmSplit";
            Text = "frmSplit";
            tableLayoutSplit.ResumeLayout(false);
            tableLayoutCart.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel5.ResumeLayout(false);
            roundedPanel1.ResumeLayout(false);
            tableLayoutPanel6.ResumeLayout(false);
            tableLayoutPanel6.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutSplit;
        private TableLayoutPanel tableLayoutCart;
        private ListView listViewCart;
        private ListView listViewPayments;
        private TableLayoutPanel tableLayoutPanel1;

        private Button button10usd;

        private Button button20usd;
        private Button button50usd;
        private Button button100usd;
        private Button buttonRemain;
        private TableLayoutPanel tableLayoutPanel2;
        private Button buttonCredit;
        private Button buttonDebit;
        private Button buttonEbt;
        private Button buttonCash;
        private Button buttonClose;
        private TableLayoutPanel tableLayoutPanel3;
        private TableLayoutPanel tableLayoutPanel4;
        private Button button7;
        private Button button8;
        private Button button9;
        private Button button4;
        private Button button5;

        private TableLayoutPanel tableLayoutPanel;

   

        private Button button6;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button00;
        private Button button0;
        private Button buttonClear;
        private TableLayoutPanel tableLayoutPanel5;
        private Button buttonPrintBill;
        private Button buttonCalculateEBT;
        private TableLayoutPanel tableLayoutPanel6;
        private Label labelTotal;
        private Label labelcharged;
        private Label labelTotalValue;
        private Label labelChargedValue;
        private Componentes.RoundedPanel roundedPanel1;
        private Label label1;
        private Label labelDueValue;
        private Button buttonEbtBalance;


        //      private Button button10;

    }
}
