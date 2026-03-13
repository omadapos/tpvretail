namespace OmadaPOS.Views
{
    partial class frmSignIn : Form
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pnlHeader     = new Panel();
            lblLogo       = new Label();
            lblClock      = new Label();
            pnlFooter     = new Panel();
            labelId       = new Label();
            pnlBackground = new Panel();
            pnlCard       = new Panel();
            pnlCardTop    = new Panel();
            pnlPinDots    = new Panel();
            lblError      = new Label();
            tlpKeypad     = new TableLayoutPanel();
            button7       = new Button();
            button8       = new Button();
            button9       = new Button();
            button4       = new Button();
            button5       = new Button();
            button6       = new Button();
            button1       = new Button();
            button2       = new Button();
            button3       = new Button();
            buttonClear   = new Button();
            button0       = new Button();
            buttonLogin   = new Button();

            pnlHeader.SuspendLayout();
            pnlFooter.SuspendLayout();
            pnlBackground.SuspendLayout();
            pnlCard.SuspendLayout();
            pnlCardTop.SuspendLayout();
            tlpKeypad.SuspendLayout();
            SuspendLayout();

            // ════════════════════════════════════════════════════════════════
            // HEADER  — dark strip 56 px
            // ════════════════════════════════════════════════════════════════
            pnlHeader.Dock      = DockStyle.Top;
            pnlHeader.Height    = 56;
            pnlHeader.BackColor = Color.FromArgb(53, 122, 189);   // AccentBlueDark
            pnlHeader.Controls.Add(lblClock);
            pnlHeader.Controls.Add(lblLogo);
            pnlHeader.Name      = "pnlHeader";
            pnlHeader.Paint    += PnlHeader_Paint;

            lblLogo.Text      = "OMADA  POS";
            lblLogo.Font      = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblLogo.ForeColor = Color.FromArgb(248, 250, 252);
            lblLogo.AutoSize  = true;
            lblLogo.Location  = new Point(28, 17);
            lblLogo.BackColor = Color.Transparent;
            lblLogo.Name      = "lblLogo";

            lblClock.Font      = new Font("Consolas", 14F, FontStyle.Regular);
            lblClock.ForeColor = Color.FromArgb(52, 211, 153);
            lblClock.AutoSize  = true;
            lblClock.Anchor    = AnchorStyles.Top | AnchorStyles.Right;
            lblClock.Location  = new Point(1230, 18);
            lblClock.BackColor = Color.Transparent;
            lblClock.Name      = "lblClock";
            lblClock.Text      = "00:00 AM";

            // ════════════════════════════════════════════════════════════════
            // FOOTER  — terminal ID 34 px
            // ════════════════════════════════════════════════════════════════
            pnlFooter.Dock      = DockStyle.Bottom;
            pnlFooter.Height    = 34;
            pnlFooter.BackColor = Color.FromArgb(53, 122, 189);   // AccentBlueDark
            pnlFooter.Controls.Add(labelId);
            pnlFooter.Name      = "pnlFooter";

            labelId.Dock      = DockStyle.Fill;
            labelId.Font      = new Font("Consolas", 9F, FontStyle.Regular);
            labelId.ForeColor = Color.FromArgb(45, 55, 72);
            labelId.TextAlign = ContentAlignment.MiddleCenter;
            labelId.BackColor = Color.Transparent;
            labelId.Name      = "labelId";

            // ════════════════════════════════════════════════════════════════
            // BACKGROUND  — deep navy fill, hosts the centered card
            // ════════════════════════════════════════════════════════════════
            pnlBackground.Dock      = DockStyle.Fill;
            pnlBackground.BackColor = Color.FromArgb(74, 144, 226);   // AccentBlue #4A90E2
            pnlBackground.Controls.Add(pnlCard);
            pnlBackground.Name      = "pnlBackground";
            pnlBackground.Resize   += PnlBackground_Resize;

            // ════════════════════════════════════════════════════════════════
            // CARD  — dark slate, 420 × 500, centered, rounded via Region
            // ════════════════════════════════════════════════════════════════
            pnlCard.Size      = new Size(420, 500);
            pnlCard.BackColor = Color.FromArgb(30, 41, 59);    // NavyBase #1E293B
            pnlCard.Controls.Add(tlpKeypad);
            pnlCard.Controls.Add(pnlCardTop);
            pnlCard.Name      = "pnlCard";
            pnlCard.Paint    += PnlCard_Paint;

            // ── PIN area (top 96 px): 6 dots + error
            pnlCardTop.Dock      = DockStyle.Top;
            pnlCardTop.Height    = 96;
            pnlCardTop.BackColor = Color.Transparent;
            pnlCardTop.Controls.Add(lblError);
            pnlCardTop.Controls.Add(pnlPinDots);
            pnlCardTop.Name      = "pnlCardTop";

            pnlPinDots.Dock      = DockStyle.Top;
            pnlPinDots.Height    = 56;
            pnlPinDots.BackColor = Color.Transparent;
            pnlPinDots.Name      = "pnlPinDots";
            pnlPinDots.Paint    += PnlPinDots_Paint;

            lblError.Dock      = DockStyle.Top;
            lblError.Height    = 40;
            lblError.Font      = new Font("Segoe UI", 10F, FontStyle.Regular);
            lblError.ForeColor = Color.FromArgb(252, 165, 165);
            lblError.TextAlign = ContentAlignment.MiddleCenter;
            lblError.BackColor = Color.Transparent;
            lblError.Visible   = false;
            lblError.Name      = "lblError";

            // ════════════════════════════════════════════════════════════════
            // KEYPAD  — 3 × 4 grid, fills remaining card height
            // tlpKeypad BackColor shows as thin gap between buttons
            // ════════════════════════════════════════════════════════════════
            tlpKeypad.Dock        = DockStyle.Fill;
            tlpKeypad.ColumnCount = 3;
            tlpKeypad.RowCount    = 4;
            tlpKeypad.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tlpKeypad.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tlpKeypad.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
            tlpKeypad.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tlpKeypad.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tlpKeypad.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tlpKeypad.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tlpKeypad.Padding   = new Padding(14, 4, 14, 16);
            tlpKeypad.BackColor = Color.FromArgb(30, 41, 59);    // dark gaps between buttons
            tlpKeypad.Name      = "tlpKeypad";
            tlpKeypad.Controls.Add(button7, 0, 0);
            tlpKeypad.Controls.Add(button8, 1, 0);
            tlpKeypad.Controls.Add(button9, 2, 0);
            tlpKeypad.Controls.Add(button4, 0, 1);
            tlpKeypad.Controls.Add(button5, 1, 1);
            tlpKeypad.Controls.Add(button6, 2, 1);
            tlpKeypad.Controls.Add(button1, 0, 2);
            tlpKeypad.Controls.Add(button2, 1, 2);
            tlpKeypad.Controls.Add(button3, 2, 2);
            tlpKeypad.Controls.Add(buttonClear, 0, 3);
            tlpKeypad.Controls.Add(button0,     1, 3);
            tlpKeypad.Controls.Add(buttonLogin,  2, 3);

            // ── Number buttons
            ConfigureNumButton(button7, "7", 0);
            ConfigureNumButton(button8, "8", 1);
            ConfigureNumButton(button9, "9", 2);
            ConfigureNumButton(button4, "4", 3);
            ConfigureNumButton(button5, "5", 4);
            ConfigureNumButton(button6, "6", 5);
            ConfigureNumButton(button1, "1", 6);
            ConfigureNumButton(button2, "2", 7);
            ConfigureNumButton(button3, "3", 8);
            ConfigureNumButton(button0, "0", 9);

            // ── Clear — amber
            buttonClear.Dock      = DockStyle.Fill;
            buttonClear.Margin    = new Padding(3);
            buttonClear.Text      = "⌫";
            buttonClear.Font      = new Font("Segoe UI", 20F, FontStyle.Regular);
            buttonClear.ForeColor = Color.White;
            buttonClear.BackColor = Color.FromArgb(146, 64, 14);
            buttonClear.FlatStyle = FlatStyle.Flat;
            buttonClear.FlatAppearance.BorderSize         = 0;
            buttonClear.FlatAppearance.MouseOverBackColor = Color.FromArgb(146, 64, 14);
            buttonClear.FlatAppearance.MouseDownBackColor = Color.FromArgb(146, 64, 14);
            buttonClear.Cursor    = Cursors.Default;
            buttonClear.TabIndex  = 10;
            buttonClear.Name      = "buttonClear";
            buttonClear.Click    += ButtonClear_Click;

            // ── Login — emerald
            buttonLogin.Dock      = DockStyle.Fill;
            buttonLogin.Margin    = new Padding(3);
            buttonLogin.Text      = "✓";
            buttonLogin.Font      = new Font("Segoe UI", 20F, FontStyle.Bold);
            buttonLogin.ForeColor = Color.White;
            buttonLogin.BackColor = Color.FromArgb(4, 120, 87);
            buttonLogin.FlatStyle = FlatStyle.Flat;
            buttonLogin.FlatAppearance.BorderSize         = 0;
            buttonLogin.FlatAppearance.MouseOverBackColor = Color.FromArgb(4, 120, 87);
            buttonLogin.FlatAppearance.MouseDownBackColor = Color.FromArgb(4, 120, 87);
            buttonLogin.Cursor    = Cursors.Default;
            buttonLogin.TabIndex  = 11;
            buttonLogin.Name      = "buttonLogin";
            buttonLogin.Click    += ButtonLogin_Click;

            // ════════════════════════════════════════════════════════════════
            // FORM
            // ════════════════════════════════════════════════════════════════
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode       = AutoScaleMode.Dpi;
            BackColor           = Color.FromArgb(74, 144, 226);   // AccentBlue
            ClientSize          = new Size(1366, 768);
            FormBorderStyle     = FormBorderStyle.None;
            WindowState         = FormWindowState.Maximized;
            Name                = "frmSignIn";
            Text                = "Omada POS — Sign In";
            Controls.Add(pnlBackground);
            Controls.Add(pnlFooter);
            Controls.Add(pnlHeader);
            Load += FrmSignIn_Load;

            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlFooter.ResumeLayout(false);
            pnlBackground.ResumeLayout(false);
            pnlCard.ResumeLayout(false);
            pnlCardTop.ResumeLayout(false);
            tlpKeypad.ResumeLayout(false);
            ResumeLayout(false);
        }

        // ── Number button factory
        private static void ConfigureNumButton(Button b, string digit, int tabIndex)
        {
            b.Dock      = DockStyle.Fill;
            b.Margin    = new Padding(3);
            b.Text      = digit;
            b.Tag       = digit;
            b.Font      = new Font("Segoe UI", 26F, FontStyle.Bold);
            b.ForeColor = Color.FromArgb(248, 250, 252);
            b.BackColor = Color.FromArgb(22, 34, 54);           // slightly lighter than gap → visible cells
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize         = 0;
            b.FlatAppearance.MouseOverBackColor = Color.FromArgb(22, 34, 54);  // same — no hover
            b.FlatAppearance.MouseDownBackColor = Color.FromArgb(22, 34, 54);  // same — no press
            b.Cursor    = Cursors.Default;
            b.TabIndex  = tabIndex;
            b.Name      = $"button{digit}";
            b.Click    += (s, _) =>
            {
                if (s is Button btn && btn.FindForm() is frmSignIn frm)
                    frm.AppendDigit(digit);
            };
        }

        #region Fields
        private Panel            pnlHeader;
        private Label            lblLogo;
        private Label            lblClock;
        private Panel            pnlFooter;
        private Label            labelId;
        private Panel            pnlBackground;
        private Panel            pnlCard;
        private Panel            pnlCardTop;
        private Label            lblTitle;      // unused — kept to avoid breaking partial class
        private Panel            pnlBrand;      // unused — kept to avoid breaking partial class
        private Panel            pnlPinDots;
        private Label            lblError;
        private TableLayoutPanel tlpKeypad;
        private Button           button1;
        private Button           button0;
        private Button           button9;
        private Button           button8;
        private Button           button7;
        private Button           button6;
        private Button           button5;
        private Button           button4;
        private Button           button3;
        private Button           button2;
        private Button           buttonClear;
        private Button           buttonLogin;
        #endregion
    }
}
