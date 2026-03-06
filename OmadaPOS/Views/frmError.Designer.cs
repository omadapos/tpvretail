namespace OmadaPOS.Views
{
    partial class frmError:Form
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
            labelError = new Label();
            buttonOK = new Button();
            SuspendLayout();
            // 
            // labelError
            // 
            labelError.AutoSize = true;
            labelError.Font = new Font("Roboto Mono Medium", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelError.Location = new Point(443, 421);
            labelError.Margin = new Padding(4, 0, 4, 0);
            labelError.Name = "labelError";
            labelError.Size = new Size(82, 28);
            labelError.TabIndex = 0;
            labelError.Text = "Error";
            labelError.Click += labelError_Click;
            // 
            // buttonOK
            // 
            buttonOK.FlatStyle = FlatStyle.Flat;
            buttonOK.Font = new Font("Roboto Mono", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            buttonOK.Location = new Point(298, 567);
            buttonOK.Margin = new Padding(4, 2, 4, 2);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new Size(373, 118);
            buttonOK.TabIndex = 1;
            buttonOK.Text = "Close";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += buttonOK_Click;
            // 
            // frmError
            // 
            AutoScaleDimensions = new SizeF(13F, 28F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1008, 729);
            Controls.Add(buttonOK);
            Controls.Add(labelError);
            Font = new Font("Consolas", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 2, 4, 2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmError";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Error";
            Load += frmError_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelError;
        private Button buttonOK;
    }
}