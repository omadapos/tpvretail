namespace OmadaPOS.Componentes;

partial class KeyPadControl
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
        buttonC = new Button();
        button0 = new Button();
        buttonLeft = new Button();
        button3 = new Button();
        button2 = new Button();
        button1 = new Button();
        button6 = new Button();
        button5 = new Button();
        button4 = new Button();
        button9 = new Button();
        button8 = new Button();
        button7 = new Button();
        labelDisplay = new Label();
        tableLayoutPanelMain = new TableLayoutPanel();
        tableLayoutPanelButton = new TableLayoutPanel();
        tableLayoutPanelMain.SuspendLayout();
        tableLayoutPanelButton.SuspendLayout();
        SuspendLayout();
        // 
        // buttonC
        // 
        buttonC.Dock = DockStyle.Fill;
        buttonC.Font = new Font("Roboto Mono", 21.75F, FontStyle.Bold);
        buttonC.Location = new Point(301, 272);
        buttonC.Margin = new Padding(5);
        buttonC.Name = "buttonC";
        buttonC.Size = new Size(140, 81);
        buttonC.TabIndex = 23;
        buttonC.Tag = "C";
        buttonC.Text = "C";
        buttonC.UseVisualStyleBackColor = true;
        buttonC.Click += btnNumber_Click;
        // 
        // button0
        // 
        button0.Dock = DockStyle.Fill;
        button0.Font = new Font("Roboto Mono", 21.75F, FontStyle.Bold);
        button0.Location = new Point(153, 272);
        button0.Margin = new Padding(5);
        button0.Name = "button0";
        button0.Size = new Size(138, 81);
        button0.TabIndex = 22;
        button0.Tag = "0";
        button0.Text = "0";
        button0.UseVisualStyleBackColor = true;
        button0.Click += btnNumber_Click;
        // 
        // buttonLeft
        // 
        buttonLeft.Dock = DockStyle.Fill;
        buttonLeft.Font = new Font("Roboto Mono", 21.75F, FontStyle.Bold);
        buttonLeft.Location = new Point(5, 272);
        buttonLeft.Margin = new Padding(5);
        buttonLeft.Name = "buttonLeft";
        buttonLeft.Size = new Size(138, 81);
        buttonLeft.TabIndex = 21;
        buttonLeft.Tag = "<-";
        buttonLeft.Text = "<<";
        buttonLeft.UseVisualStyleBackColor = false;
        buttonLeft.Click += btnNumber_Click;
        // 
        // button3
        // 
        button3.Dock = DockStyle.Fill;
        button3.Font = new Font("Roboto Mono", 21.75F, FontStyle.Bold);
        button3.Location = new Point(301, 183);
        button3.Margin = new Padding(5);
        button3.Name = "button3";
        button3.Size = new Size(140, 79);
        button3.TabIndex = 20;
        button3.Tag = "3";
        button3.Text = "3";
        button3.UseVisualStyleBackColor = true;
        button3.Click += btnNumber_Click;
        // 
        // button2
        // 
        button2.Dock = DockStyle.Fill;
        button2.Font = new Font("Roboto Mono", 21.75F, FontStyle.Bold);
        button2.Location = new Point(153, 183);
        button2.Margin = new Padding(5);
        button2.Name = "button2";
        button2.Size = new Size(138, 79);
        button2.TabIndex = 19;
        button2.Tag = "2";
        button2.Text = "2";
        button2.UseVisualStyleBackColor = true;
        button2.Click += btnNumber_Click;
        // 
        // button1
        // 
        button1.Dock = DockStyle.Fill;
        button1.Font = new Font("Roboto Mono", 21.75F, FontStyle.Bold);
        button1.Location = new Point(5, 183);
        button1.Margin = new Padding(5);
        button1.Name = "button1";
        button1.Size = new Size(138, 79);
        button1.TabIndex = 18;
        button1.Tag = "1";
        button1.Text = "1";
        button1.UseVisualStyleBackColor = false;
        button1.Click += btnNumber_Click;
        // 
        // button6
        // 
        button6.Dock = DockStyle.Fill;
        button6.Font = new Font("Roboto Mono", 21.75F, FontStyle.Bold);
        button6.Location = new Point(301, 94);
        button6.Margin = new Padding(5);
        button6.Name = "button6";
        button6.Size = new Size(140, 79);
        button6.TabIndex = 17;
        button6.Tag = "6";
        button6.Text = "6";
        button6.UseVisualStyleBackColor = true;
        button6.Click += btnNumber_Click;
        // 
        // button5
        // 
        button5.Dock = DockStyle.Fill;
        button5.Font = new Font("Roboto Mono", 21.75F, FontStyle.Bold);
        button5.Location = new Point(153, 94);
        button5.Margin = new Padding(5);
        button5.Name = "button5";
        button5.Size = new Size(138, 79);
        button5.TabIndex = 16;
        button5.Tag = "5";
        button5.Text = "5";
        button5.UseVisualStyleBackColor = true;
        button5.Click += btnNumber_Click;
        // 
        // button4
        // 
        button4.Dock = DockStyle.Fill;
        button4.Font = new Font("Roboto Mono", 21.75F, FontStyle.Bold);
        button4.Location = new Point(5, 94);
        button4.Margin = new Padding(5);
        button4.Name = "button4";
        button4.Size = new Size(138, 79);
        button4.TabIndex = 15;
        button4.Tag = "4";
        button4.Text = "4";
        button4.UseVisualStyleBackColor = true;
        button4.Click += btnNumber_Click;
        // 
        // button9
        // 
        button9.Dock = DockStyle.Fill;
        button9.Font = new Font("Roboto Mono", 21.75F, FontStyle.Bold);
        button9.Location = new Point(301, 5);
        button9.Margin = new Padding(5);
        button9.Name = "button9";
        button9.Size = new Size(140, 79);
        button9.TabIndex = 14;
        button9.Tag = "9";
        button9.Text = "9";
        button9.UseVisualStyleBackColor = true;
        button9.Click += btnNumber_Click;
        // 
        // button8
        // 
        button8.Dock = DockStyle.Fill;
        button8.Font = new Font("Roboto Mono", 21.75F, FontStyle.Bold);
        button8.Location = new Point(153, 5);
        button8.Margin = new Padding(5);
        button8.Name = "button8";
        button8.Size = new Size(138, 79);
        button8.TabIndex = 13;
        button8.Tag = "8";
        button8.Text = "8";
        button8.UseVisualStyleBackColor = true;
        button8.Click += btnNumber_Click;
        // 
        // button7
        // 
        button7.Dock = DockStyle.Fill;
        button7.Font = new Font("Roboto Mono", 21.75F, FontStyle.Bold);
        button7.Location = new Point(5, 5);
        button7.Margin = new Padding(5);
        button7.Name = "button7";
        button7.Size = new Size(138, 79);
        button7.TabIndex = 12;
        button7.Tag = "7";
        button7.Text = "7";
        button7.UseVisualStyleBackColor = true;
        button7.Click += btnNumber_Click;
        // 
        // labelDisplay
        // 
        labelDisplay.AutoSize = true;
        labelDisplay.BorderStyle = BorderStyle.Fixed3D;
        labelDisplay.Dock = DockStyle.Fill;
        labelDisplay.Font = new Font("Roboto Mono", 26.25F, FontStyle.Bold);
        labelDisplay.Location = new Point(2, 0);
        labelDisplay.Margin = new Padding(2, 0, 2, 0);
        labelDisplay.Name = "labelDisplay";
        labelDisplay.Size = new Size(446, 90);
        labelDisplay.TabIndex = 24;
        labelDisplay.Text = "0.0";
        labelDisplay.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // tableLayoutPanelMain
        // 
        tableLayoutPanelMain.AutoSize = true;
        tableLayoutPanelMain.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        tableLayoutPanelMain.BackColor = Color.FromArgb(247, 248, 250);
        tableLayoutPanelMain.ColumnCount = 1;
        tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanelMain.Controls.Add(labelDisplay, 0, 0);
        tableLayoutPanelMain.Controls.Add(tableLayoutPanelButton, 0, 1);
        tableLayoutPanelMain.Dock = DockStyle.Fill;
        tableLayoutPanelMain.Location = new Point(0, 0);
        tableLayoutPanelMain.Margin = new Padding(2, 1, 2, 1);
        tableLayoutPanelMain.Name = "tableLayoutPanelMain";
        tableLayoutPanelMain.RowCount = 2;
        tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
        tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 80F));
        tableLayoutPanelMain.Size = new Size(450, 450);
        tableLayoutPanelMain.TabIndex = 25;
        // 
        // tableLayoutPanelButton
        // 
        tableLayoutPanelButton.ColumnCount = 3;
        tableLayoutPanelButton.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
        tableLayoutPanelButton.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333359F));
        tableLayoutPanelButton.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333359F));
        tableLayoutPanelButton.Controls.Add(button7, 0, 0);
        tableLayoutPanelButton.Controls.Add(buttonC, 2, 3);
        tableLayoutPanelButton.Controls.Add(button8, 1, 0);
        tableLayoutPanelButton.Controls.Add(button0, 1, 3);
        tableLayoutPanelButton.Controls.Add(button9, 2, 0);
        tableLayoutPanelButton.Controls.Add(buttonLeft, 0, 3);
        tableLayoutPanelButton.Controls.Add(button4, 0, 1);
        tableLayoutPanelButton.Controls.Add(button3, 2, 2);
        tableLayoutPanelButton.Controls.Add(button5, 1, 1);
        tableLayoutPanelButton.Controls.Add(button2, 1, 2);
        tableLayoutPanelButton.Controls.Add(button6, 2, 1);
        tableLayoutPanelButton.Controls.Add(button1, 0, 2);
        tableLayoutPanelButton.Dock = DockStyle.Fill;
        tableLayoutPanelButton.Font = new Font("Roboto Mono", 26.25F, FontStyle.Bold);
        tableLayoutPanelButton.Location = new Point(2, 91);
        tableLayoutPanelButton.Margin = new Padding(2, 1, 2, 1);
        tableLayoutPanelButton.Name = "tableLayoutPanelButton";
        tableLayoutPanelButton.RowCount = 4;
        tableLayoutPanelButton.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
        tableLayoutPanelButton.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
        tableLayoutPanelButton.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
        tableLayoutPanelButton.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
        tableLayoutPanelButton.Size = new Size(446, 358);
        tableLayoutPanelButton.TabIndex = 25;
        // 
        // KeyPadControl
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        BackColor = Color.FromArgb(247, 248, 250);
        Controls.Add(tableLayoutPanelMain);
        Margin = new Padding(2, 1, 2, 1);
        Name = "KeyPadControl";
        Size = new Size(450, 450);
        Load += KeyPadControl_Load;
        tableLayoutPanelMain.ResumeLayout(false);
        tableLayoutPanelMain.PerformLayout();
        tableLayoutPanelButton.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Button buttonC;
    private Button button0;
    private Button buttonLeft;
    private Button button3;
    private Button button2;
    private Button button1;
    private Button button6;
    private Button button5;
    private Button button4;
    private Button button9;
    private Button button8;
    private Button button7;
    private Label labelDisplay;
    private TableLayoutPanel tableLayoutPanelMain;
    private TableLayoutPanel tableLayoutPanelButton;

}