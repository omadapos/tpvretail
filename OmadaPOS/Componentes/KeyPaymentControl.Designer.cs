namespace OmadaPOS.Componentes;

partial class KeyPaymentControl
{
    /// <summary> 
    /// Variable del diseñador necesaria.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Limpiar los recursos que se estén usando.
    /// </summary>
    /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Código generado por el Diseñador de componentes

    /// <summary> 
    /// Método necesario para admitir el Diseñador. No se puede modificar
    /// el contenido de este método con el editor de código.
    /// </summary>
    private void InitializeComponent()
    {
        tableLayoutPanelMoney = new TableLayoutPanel();
        button100 = new Button();
        buttonClear = new Button();
        button0 = new Button();
        button00 = new Button();
        button50 = new Button();
        button3 = new Button();
        button2 = new Button();
        button1 = new Button();
        button20 = new Button();
        button6 = new Button();
        button5 = new Button();
        button4 = new Button();
        button10 = new Button();
        button9 = new Button();
        button8 = new Button();
        button7 = new Button();
        tableLayoutPanelMoney.SuspendLayout();
        SuspendLayout();
        // 
        // tableLayoutPanelMoney
        // 
        tableLayoutPanelMoney.ColumnCount = 4;
        tableLayoutPanelMoney.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tableLayoutPanelMoney.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tableLayoutPanelMoney.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tableLayoutPanelMoney.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tableLayoutPanelMoney.Controls.Add(button100, 3, 3);
        tableLayoutPanelMoney.Controls.Add(buttonClear, 2, 3);
        tableLayoutPanelMoney.Controls.Add(button0, 1, 3);
        tableLayoutPanelMoney.Controls.Add(button00, 0, 3);
        tableLayoutPanelMoney.Controls.Add(button50, 3, 2);
        tableLayoutPanelMoney.Controls.Add(button3, 2, 2);
        tableLayoutPanelMoney.Controls.Add(button2, 1, 2);
        tableLayoutPanelMoney.Controls.Add(button1, 0, 2);
        tableLayoutPanelMoney.Controls.Add(button20, 3, 1);
        tableLayoutPanelMoney.Controls.Add(button6, 2, 1);
        tableLayoutPanelMoney.Controls.Add(button5, 1, 1);
        tableLayoutPanelMoney.Controls.Add(button4, 0, 1);
        tableLayoutPanelMoney.Controls.Add(button10, 3, 0);
        tableLayoutPanelMoney.Controls.Add(button9, 2, 0);
        tableLayoutPanelMoney.Controls.Add(button8, 1, 0);
        tableLayoutPanelMoney.Controls.Add(button7, 0, 0);
        tableLayoutPanelMoney.Dock = DockStyle.Fill;
        tableLayoutPanelMoney.Location = new Point(0, 0);
        tableLayoutPanelMoney.Margin = new Padding(5, 3, 5, 3);
        tableLayoutPanelMoney.Name = "tableLayoutPanelMoney";
        tableLayoutPanelMoney.RowCount = 4;
        tableLayoutPanelMoney.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
        tableLayoutPanelMoney.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
        tableLayoutPanelMoney.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
        tableLayoutPanelMoney.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
        tableLayoutPanelMoney.Size = new Size(916, 897);
        tableLayoutPanelMoney.TabIndex = 0;
        // 
        // button100
        // 
        button100.Dock = DockStyle.Fill;
        button100.Font = new Font("Microsoft Sans Serif", 21.75F);
        button100.Location = new Point(690, 675);
        button100.Name = "button100";
        button100.Size = new Size(223, 219);
        button100.TabIndex = 15;
        button100.Tag = "10000";
        button100.Text = "$100";
        button100.UseVisualStyleBackColor = true;
        button100.Click += buttonKeyScan_Click;
        // 
        // buttonClear
        // 
        buttonClear.Dock = DockStyle.Fill;
        buttonClear.Font = new Font("Microsoft Sans Serif", 21.75F);
        buttonClear.Location = new Point(461, 675);
        buttonClear.Name = "buttonClear";
        buttonClear.Size = new Size(223, 219);
        buttonClear.TabIndex = 14;
        buttonClear.Tag = "*";
        buttonClear.Text = "Clear";
        buttonClear.UseVisualStyleBackColor = true;
        buttonClear.Click += buttonKeyScan_Click;
        // 
        // button0
        // 
        button0.Dock = DockStyle.Fill;
        button0.Font = new Font("Microsoft Sans Serif", 21.75F);
        button0.Location = new Point(232, 675);
        button0.Name = "button0";
        button0.Size = new Size(223, 219);
        button0.TabIndex = 13;
        button0.Tag = "0";
        button0.Text = "0";
        button0.UseVisualStyleBackColor = true;
        button0.Click += buttonKeyScan_Click;
        // 
        // button00
        // 
        button00.Dock = DockStyle.Fill;
        button00.Font = new Font("Microsoft Sans Serif", 21.75F);
        button00.Location = new Point(3, 675);
        button00.Name = "button00";
        button00.Size = new Size(223, 219);
        button00.TabIndex = 12;
        button00.Tag = "00";
        button00.Text = "00";
        button00.UseVisualStyleBackColor = true;
        button00.Click += buttonKeyScan_Click;
        // 
        // button50
        // 
        button50.Dock = DockStyle.Fill;
        button50.Font = new Font("Microsoft Sans Serif", 21.75F);
        button50.Location = new Point(690, 451);
        button50.Name = "button50";
        button50.Size = new Size(223, 218);
        button50.TabIndex = 11;
        button50.Tag = "5000";
        button50.Text = "$50";
        button50.UseVisualStyleBackColor = true;
        button50.Click += buttonKeyScan_Click;
        // 
        // button3
        // 
        button3.Dock = DockStyle.Fill;
        button3.Font = new Font("Microsoft Sans Serif", 21.75F);
        button3.Location = new Point(461, 451);
        button3.Name = "button3";
        button3.Size = new Size(223, 218);
        button3.TabIndex = 10;
        button3.Tag = "3";
        button3.Text = "3";
        button3.UseVisualStyleBackColor = true;
        button3.Click += buttonKeyScan_Click;
        // 
        // button2
        // 
        button2.Dock = DockStyle.Fill;
        button2.Font = new Font("Microsoft Sans Serif", 21.75F);
        button2.Location = new Point(232, 451);
        button2.Name = "button2";
        button2.Size = new Size(223, 218);
        button2.TabIndex = 9;
        button2.Tag = "2";
        button2.Text = "2";
        button2.UseVisualStyleBackColor = true;
        button2.Click += buttonKeyScan_Click;
        // 
        // button1
        // 
        button1.Dock = DockStyle.Fill;
        button1.Font = new Font("Microsoft Sans Serif", 21.75F);
        button1.Location = new Point(3, 451);
        button1.Name = "button1";
        button1.Size = new Size(223, 218);
        button1.TabIndex = 8;
        button1.Tag = "1";
        button1.Text = "1";
        button1.UseVisualStyleBackColor = true;
        button1.Click += buttonKeyScan_Click;
        // 
        // button20
        // 
        button20.Dock = DockStyle.Fill;
        button20.Font = new Font("Microsoft Sans Serif", 21.75F);
        button20.Location = new Point(690, 227);
        button20.Name = "button20";
        button20.Size = new Size(223, 218);
        button20.TabIndex = 7;
        button20.Tag = "2000";
        button20.Text = "$20";
        button20.UseVisualStyleBackColor = true;
        button20.Click += buttonKeyScan_Click;
        // 
        // button6
        // 
        button6.Dock = DockStyle.Fill;
        button6.Font = new Font("Microsoft Sans Serif", 21.75F);
        button6.Location = new Point(461, 227);
        button6.Name = "button6";
        button6.Size = new Size(223, 218);
        button6.TabIndex = 6;
        button6.Tag = "6";
        button6.Text = "6";
        button6.UseVisualStyleBackColor = true;
        button6.Click += buttonKeyScan_Click;
        // 
        // button5
        // 
        button5.Dock = DockStyle.Fill;
        button5.Font = new Font("Microsoft Sans Serif", 21.75F);
        button5.Location = new Point(232, 227);
        button5.Name = "button5";
        button5.Size = new Size(223, 218);
        button5.TabIndex = 5;
        button5.Tag = "5";
        button5.Text = "5";
        button5.UseVisualStyleBackColor = true;
        button5.Click += buttonKeyScan_Click;
        // 
        // button4
        // 
        button4.Dock = DockStyle.Fill;
        button4.Font = new Font("Microsoft Sans Serif", 21.75F);
        button4.Location = new Point(3, 227);
        button4.Name = "button4";
        button4.Size = new Size(223, 218);
        button4.TabIndex = 4;
        button4.Tag = "4";
        button4.Text = "4";
        button4.UseVisualStyleBackColor = true;
        button4.Click += buttonKeyScan_Click;
        // 
        // button10
        // 
        button10.Dock = DockStyle.Fill;
        button10.Font = new Font("Microsoft Sans Serif", 21.75F);
        button10.Location = new Point(690, 3);
        button10.Name = "button10";
        button10.Size = new Size(223, 218);
        button10.TabIndex = 3;
        button10.Tag = "1000";
        button10.Text = "$10";
        button10.UseVisualStyleBackColor = true;
        button10.Click += buttonKeyScan_Click;
        // 
        // button9
        // 
        button9.Dock = DockStyle.Fill;
        button9.Font = new Font("Microsoft Sans Serif", 21.75F);
        button9.Location = new Point(461, 3);
        button9.Name = "button9";
        button9.Size = new Size(223, 218);
        button9.TabIndex = 2;
        button9.Tag = "9";
        button9.Text = "9";
        button9.UseVisualStyleBackColor = true;
        button9.Click += buttonKeyScan_Click;
        // 
        // button8
        // 
        button8.Dock = DockStyle.Fill;
        button8.Font = new Font("Microsoft Sans Serif", 21.75F);
        button8.Location = new Point(232, 3);
        button8.Name = "button8";
        button8.Size = new Size(223, 218);
        button8.TabIndex = 1;
        button8.Tag = "8";
        button8.Text = "8";
        button8.UseVisualStyleBackColor = true;
        button8.Click += buttonKeyScan_Click;
        // 
        // button7
        // 
        button7.Dock = DockStyle.Fill;
        button7.Font = new Font("Microsoft Sans Serif", 21.75F);
        button7.Location = new Point(3, 3);
        button7.Name = "button7";
        button7.Size = new Size(223, 218);
        button7.TabIndex = 0;
        button7.Tag = "7";
        button7.Text = "7";
        button7.UseVisualStyleBackColor = true;
        button7.Click += buttonKeyScan_Click;
        // 
        // KeyPaymentControl
        // 
        AutoScaleDimensions = new SizeF(17F, 41F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(tableLayoutPanelMoney);
        Margin = new Padding(5, 3, 5, 3);
        Name = "KeyPaymentControl";
        Size = new Size(916, 897);
        tableLayoutPanelMoney.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel tableLayoutPanelMoney;
    private Button button7;
    private Button button10;
    private Button button9;
    private Button button8;
    private Button button20;
    private Button button6;
    private Button button5;
    private Button button4;
    private Button button100;
    private Button buttonClear;
    private Button button0;
    private Button button00;
    private Button button50;
    private Button button3;
    private Button button2;
    private Button button1;
}
