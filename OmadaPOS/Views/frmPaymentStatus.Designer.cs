namespace OmadaPOS.Views
{
    partial class frmPaymentStatus: Form
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
            labelMsg = new Label();
            buttonClose = new Button();
            SuspendLayout();
            // 
            // labelMsg
            // 
            labelMsg.AutoSize = true;
            labelMsg.BackColor = Color.FromArgb(247, 248, 250);
            labelMsg.Dock = DockStyle.Fill;
            labelMsg.Font = new Font("Cambria", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelMsg.ForeColor = Color.FromArgb(26, 32, 44);
            labelMsg.Location = new Point(0, 0);
            labelMsg.Margin = new Padding(4, 0, 4, 0);
            labelMsg.Name = "labelMsg";
            labelMsg.Size = new Size(113, 112);
            labelMsg.TabIndex = 0;
            labelMsg.Text = "...";
            labelMsg.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // buttonClose
            // 
            buttonClose.FlatStyle = FlatStyle.Flat;
            buttonClose.Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonClose.Location = new Point(604, 809);
            buttonClose.Margin = new Padding(4, 2, 4, 2);
            buttonClose.Name = "buttonClose";
            buttonClose.Size = new Size(325, 132);
            buttonClose.TabIndex = 1;
            buttonClose.Text = "Close";
            buttonClose.UseVisualStyleBackColor = true;
            buttonClose.Click += buttonClose_Click;
            // 
            // frmPaymentStatus
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(247, 248, 250);
            ClientSize = new Size(1456, 1111);
            Controls.Add(buttonClose);
            Controls.Add(labelMsg);
            ForeColor = Color.FromArgb(26, 32, 44);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 2, 4, 2);
            Name = "frmPaymentStatus";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Payment Status";
            Load += frmPaymentStatus_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelMsg;
        private Button buttonClose;
    }
}