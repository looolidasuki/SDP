using System.Drawing;

namespace FurnitureERP
{
    public static class Theme
    {
        // Primary palette
        public static readonly Color Primary      = Color.FromArgb(30, 58, 138);   // deep navy
        public static readonly Color PrimaryLight = Color.FromArgb(59, 130, 246);  // bright blue
        public static readonly Color Accent       = Color.FromArgb(245, 158, 11);  // amber
        public static readonly Color Success      = Color.FromArgb(16, 185, 129);  // emerald
        public static readonly Color Danger       = Color.FromArgb(239, 68, 68);   // red
        public static readonly Color Warning      = Color.FromArgb(251, 191, 36);  // yellow

        // Surfaces
        public static readonly Color Background   = Color.FromArgb(248, 250, 252);
        public static readonly Color Surface      = Color.White;
        public static readonly Color SidebarBg    = Color.FromArgb(15, 23, 42);
        public static readonly Color SidebarHover = Color.FromArgb(30, 41, 59);
        public static readonly Color HeaderBg     = Color.White;
        public static readonly Color BorderColor  = Color.FromArgb(226, 232, 240);
        public static readonly Color TextPrimary  = Color.FromArgb(15, 23, 42);
        public static readonly Color TextMuted    = Color.FromArgb(100, 116, 139);

        // Fonts
        public static readonly Font FontTitle    = new Font("Segoe UI", 13f, FontStyle.Bold);
        public static readonly Font FontHeading  = new Font("Segoe UI", 11f, FontStyle.Bold);
        public static readonly Font FontBody     = new Font("Segoe UI", 9.5f);
        public static readonly Font FontSmall    = new Font("Segoe UI", 8.5f);
        public static readonly Font FontMono     = new Font("Consolas", 9f);

        public static void StyleGrid(System.Windows.Forms.DataGridView grid)
        {
            grid.BackgroundColor      = Surface;
            grid.BorderStyle          = System.Windows.Forms.BorderStyle.None;
            grid.CellBorderStyle      = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor            = BorderColor;
            grid.RowHeadersVisible    = false;
            grid.AllowUserToAddRows   = false;
            grid.SelectionMode        = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            grid.ReadOnly             = true;
            grid.AutoSizeColumnsMode  = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            grid.Font                 = FontBody;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            grid.DefaultCellStyle.SelectionBackColor       = Color.FromArgb(219, 234, 254);
            grid.DefaultCellStyle.SelectionForeColor       = TextPrimary;
            grid.ColumnHeadersDefaultCellStyle.BackColor   = Color.FromArgb(241, 245, 249);
            grid.ColumnHeadersDefaultCellStyle.ForeColor   = TextPrimary;
            grid.ColumnHeadersDefaultCellStyle.Font        = new Font("Segoe UI", 9f, FontStyle.Bold);
            grid.ColumnHeadersHeight       = 38;
            grid.RowTemplate.Height        = 34;
            grid.EnableHeadersVisualStyles = false;
        }

        public static System.Windows.Forms.Button PrimaryButton(string text, int width = 130, int height = 36)
        {
            var btn = new System.Windows.Forms.Button
            {
                Text      = text,
                Width     = width,
                Height    = height,
                FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                BackColor = Primary,
                ForeColor = Color.White,
                Font      = FontBody,
                Cursor    = System.Windows.Forms.Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        public static System.Windows.Forms.Button SecondaryButton(string text, int width = 130, int height = 36)
        {
            var btn = new System.Windows.Forms.Button
            {
                Text      = text,
                Width     = width,
                Height    = height,
                FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = TextPrimary,
                Font      = FontBody,
                Cursor    = System.Windows.Forms.Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = BorderColor;
            btn.FlatAppearance.BorderSize  = 1;
            return btn;
        }

        public static System.Windows.Forms.Button DangerButton(string text, int width = 130, int height = 36)
        {
            var btn = new System.Windows.Forms.Button
            {
                Text      = text,
                Width     = width,
                Height    = height,
                FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                BackColor = Danger,
                ForeColor = Color.White,
                Font      = FontBody,
                Cursor    = System.Windows.Forms.Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }
    }
}
