namespace OmadaPOS.Views
{
    partial class frmHold: Form
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
            tableLayoutPanelButton = new TableLayoutPanel();
            buttonHold = new Button();
            buttonCancel = new Button();
            listBoxHold = new ListBox();
            tableLayoutPanelMain.SuspendLayout();
            tableLayoutPanelButton.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanelMain
            // 
            tableLayoutPanelMain.BackColor = Color.FromArgb(247, 248, 250);
            tableLayoutPanelMain.ColumnCount = 1;
            tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelMain.Controls.Add(tableLayoutPanelButton, 0, 1);
            tableLayoutPanelMain.Controls.Add(listBoxHold, 0, 0);
            tableLayoutPanelMain.Dock = DockStyle.Fill;
            tableLayoutPanelMain.Location = new Point(0, 0);
            tableLayoutPanelMain.Margin = new Padding(2, 1, 2, 1);
            tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            tableLayoutPanelMain.RowCount = 2;
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 80F));
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanelMain.Size = new Size(796, 549);
            tableLayoutPanelMain.TabIndex = 0;
            // 
            // tableLayoutPanelButton
            // 
            tableLayoutPanelButton.Anchor = AnchorStyles.None;
            tableLayoutPanelButton.BackColor = Color.FromArgb(247, 248, 250);
            tableLayoutPanelButton.ColumnCount = 2;
            tableLayoutPanelButton.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelButton.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelButton.Controls.Add(buttonHold, 1, 0);
            tableLayoutPanelButton.Controls.Add(buttonCancel, 0, 0);
            tableLayoutPanelButton.Location = new Point(154, 450);
            tableLayoutPanelButton.Margin = new Padding(2, 1, 2, 1);
            tableLayoutPanelButton.Name = "tableLayoutPanelButton";
            tableLayoutPanelButton.RowCount = 1;
            tableLayoutPanelButton.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanelButton.Size = new Size(487, 88);
            tableLayoutPanelButton.TabIndex = 1;
            // 
            // buttonHold
            // 
            buttonHold.Dock = DockStyle.Fill;
            buttonHold.FlatStyle = FlatStyle.Flat;
            buttonHold.Font = new Font("Segoe UI", 36F);
            buttonHold.Location = new Point(248, 5);
            buttonHold.Margin = new Padding(5, 5, 5, 5);
            buttonHold.Name = "buttonHold";
            buttonHold.Size = new Size(234, 78);
            buttonHold.TabIndex = 1;
            buttonHold.Text = "HOLD IT";
            buttonHold.UseVisualStyleBackColor = true;
            buttonHold.Click += buttonAdd_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.BackColor = Color.FromArgb(247, 248, 250);
            buttonCancel.Dock = DockStyle.Fill;
            buttonCancel.FlatStyle = FlatStyle.Flat;
            buttonCancel.Font = new Font("Segoe UI", 36F);
            buttonCancel.Location = new Point(5, 5);
            buttonCancel.Margin = new Padding(5, 5, 5, 5);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(233, 78);
            buttonCancel.TabIndex = 0;
            buttonCancel.Text = "CANCEL";
            buttonCancel.UseVisualStyleBackColor = false;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // listBoxHold
            // 
            listBoxHold.Dock = DockStyle.Fill;
            listBoxHold.Font = new Font("Segoe UI", 36F);
            listBoxHold.FormattingEnabled = true;
            listBoxHold.Location = new Point(2, 1);
            listBoxHold.Margin = new Padding(2, 1, 2, 1);
            listBoxHold.Name = "listBoxHold";
            listBoxHold.Size = new Size(792, 437);
            listBoxHold.TabIndex = 0;
            listBoxHold.SelectedIndexChanged += listBoxHold_SelectedIndexChanged;
            // 
            // frmHold
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(796, 549);
            Controls.Add(tableLayoutPanelMain);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(2, 1, 2, 1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmHold";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Hold Cart";
            Load += frmHold_Load;
            tableLayoutPanelMain.ResumeLayout(false);
            tableLayoutPanelButton.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanelMain;
        private ListBox listBoxHold;
        private TableLayoutPanel tableLayoutPanelButton;
        private Button buttonCancel;
        private Button buttonHold;
    }
}