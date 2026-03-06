namespace OmadaPOS.Views
{
    partial class frmCierreDiario
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
            dateTimePickerFecha = new DateTimePicker();
            buttonClose = new Button();
            SuspendLayout();
            // 
            // dateTimePickerFecha
            // 
            dateTimePickerFecha.CalendarFont = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dateTimePickerFecha.Location = new Point(329, 346);
            dateTimePickerFecha.Margin = new Padding(4, 2, 4, 2);
            dateTimePickerFecha.Name = "dateTimePickerFecha";
            dateTimePickerFecha.Size = new Size(684, 39);
            dateTimePickerFecha.TabIndex = 0;
            // 
            // buttonClose
            // 
            buttonClose.Font = new Font("Segoe UI", 16F);
            buttonClose.Location = new Point(422, 804);
            buttonClose.Margin = new Padding(4, 2, 4, 2);
            buttonClose.Name = "buttonClose";
            buttonClose.Size = new Size(535, 186);
            buttonClose.TabIndex = 1;
            buttonClose.Text = "Close Day";
            buttonClose.UseVisualStyleBackColor = true;
            buttonClose.Click += buttonClose_Click;
            // 
            // frmCierreDiario
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1872, 1555);
            Controls.Add(buttonClose);
            Controls.Add(dateTimePickerFecha);
            Margin = new Padding(4, 2, 4, 2);
            Name = "frmCierreDiario";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Close Day";
            ResumeLayout(false);
        }

        #endregion

        private DateTimePicker dateTimePickerFecha;
        private Button buttonClose;
    }
}