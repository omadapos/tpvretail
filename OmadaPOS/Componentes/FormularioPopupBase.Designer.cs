namespace OmadaPOS.Componentes
{
    partial class FormularioPopupBase
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            panelHeader = new System.Windows.Forms.Panel();
            SuspendLayout();

            // panelHeader — 30% Navy (banda superior)
            panelHeader.BackColor = AppColors.NavyDark;
            panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            panelHeader.Location = new System.Drawing.Point(0, 0);
            panelHeader.Name = "panelHeader";
            panelHeader.Size = new System.Drawing.Size(584, 54);
            panelHeader.TabIndex = 0;

            // FormularioPopupBase
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = AppColors.BackgroundPrimary;
            ClientSize = new System.Drawing.Size(584, 561);
            Controls.Add(panelHeader);
            Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            Name = "FormularioPopupBase";
            Text = "FormularioPopupBase";
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelHeader;
    }
}
