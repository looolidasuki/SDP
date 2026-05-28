using System;
using System.Drawing;
using System.Windows.Forms;
using Sales_user.Controllers;
using Sales_user.Models;
using FurnitureERP.Helpers;

namespace FurnitureERP.Forms
{
    public partial class MainForm : Form
    {
        public Staff CurrentUser { get; set; }

        private Panel _sidebar;
        private Panel _contentPanel;
        private Panel _headerPanel;
        private Button _activeNavButton;

        public MainForm()
        {
            InitializeComponent();
            BuildLayout();
        }

        private void BuildLayout()
        {
            SuspendLayout();

            _sidebar = new Panel { Dock = DockStyle.Left, Width = 220, BackColor = UITheme.NavDark };
            BuildSidebar();

            _headerPanel = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.White };
            _headerPanel.Paint += (s, e) => e.Graphics.DrawLine(new System.Drawing.Pen(Color.FromArgb(220, 225, 235), 1), 0, 59, _headerPanel.Width, 59);
            BuildHeader();

            _contentPanel = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.Background, Padding = new Padding(20) };

            Controls.Add(_contentPanel);
            Controls.Add(_headerPanel);
            Controls.Add(_sidebar);

            ResumeLayout();
            LoadModule("Dashboard");
        }

        private void BuildSidebar()
        {
            Panel logoPanel = new Panel { Dock = DockStyle.Top, Height = 72, BackColor = UITheme.NavDarkest };
            Label logoLabel = new Label { Text = "PREMIUM LIVING", ForeColor = Color.White, Font = new Font("Segoe UI", 11, FontStyle.Bold), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            Label subLabel = new Label { Text = "ERP System", ForeColor = Color.FromArgb(160, 190, 230), Font = new Font("Segoe UI", 8), Dock = DockStyle.Bottom, Height = 20, TextAlign = ContentAlignment.MiddleCenter };
            logoPanel.Controls.Add(logoLabel);
            logoPanel.Controls.Add(subLabel);
            _sidebar.Controls.Add(logoPanel);

            var navItems = new[] {
                ("Dashboard"), ("Customers"), ("Quotations"), ("Sales Orders"),
                ("Production"), ("Raw Materials"), ("Purchase Orders"), ("Goods Received"),
                ("Warehouse"), ("Delivery Notes"), ("Invoices"), ("Refunds"),
                ("Suppliers"), ("Staff"), ("System Admin")
            };

            Panel navContainer = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = UITheme.NavDark };
            int yPos = 8;
            foreach (var name in navItems)
            {
                Button btn = new Button
                {
                    Text = "  " + name, Height = 40, Width = 220,
                    FlatStyle = FlatStyle.Flat, ForeColor = Color.FromArgb(180, 210, 255),
                    BackColor = UITheme.NavDark, Font = new Font("Segoe UI", 9),
                    TextAlign = ContentAlignment.MiddleLeft, Cursor = Cursors.Hand, Tag = name,
                    Location = new Point(0, yPos)
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = UITheme.NavHover;
                btn.Click += NavButton_Click;
                navContainer.Controls.Add(btn);
                yPos += 44;
            }
            _sidebar.Controls.Add(navContainer);

            Button logoutBtn = new Button
            {
                Text = "Logout", Dock = DockStyle.Bottom, Height = 46,
                FlatStyle = FlatStyle.Flat, ForeColor = Color.FromArgb(255, 120, 120),
                BackColor = Color.FromArgb(60, 20, 20), Font = new Font("Segoe UI", 9, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter, Cursor = Cursors.Hand
            };
            logoutBtn.FlatAppearance.BorderSize = 0;
            logoutBtn.Click += (s, e) => {
                if (MessageBox.Show("Logout from the system?", "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                { new FurnitureERP.LoginForm().Show(); Close(); }
            };
            _sidebar.Controls.Add(logoutBtn);
        }

        private void BuildHeader()
        {
            Label moduleTitle = new Label { Name = "lblModuleTitle", Text = "Dashboard", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = UITheme.Primary, AutoSize = true, Location = new Point(20, 17) };
            Label userLabel = new Label { Name = "lblUser", AutoSize = true, ForeColor = UITheme.TextDark, Font = new Font("Segoe UI", 9) };
            _headerPanel.Controls.Add(moduleTitle);
            _headerPanel.Controls.Add(userLabel);
            _headerPanel.Resize += (s, e) => {
                if (_headerPanel.Controls["lblUser"] is Label ul)
                    ul.Location = new Point(_headerPanel.Width - ul.PreferredWidth - 20, (_headerPanel.Height - ul.PreferredHeight) / 2);
            };
        }

        private void NavButton_Click(object sender, EventArgs e)
        {
            if (!(sender is Button btn)) return;
            if (_activeNavButton != null) { _activeNavButton.BackColor = UITheme.NavDark; _activeNavButton.ForeColor = Color.FromArgb(180, 210, 255); _activeNavButton.Font = new Font("Segoe UI", 9); }
            btn.BackColor = UITheme.NavActive; btn.ForeColor = Color.White; btn.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            _activeNavButton = btn;
            LoadModule(btn.Tag?.ToString() ?? "Dashboard");
        }

        public void LoadModule(string module)
        {
            if (_headerPanel.Controls["lblModuleTitle"] is Label t) t.Text = module;
            _contentPanel.Controls.Clear();
            _contentPanel.SuspendLayout();
            try
            {
                Control panel = null;
                switch (module)
                {
                    case "Dashboard":       panel = new DashboardPanel(); break;
                    case "Customers":
                    case "Quotations":
                    case "Sales Orders":    panel = new SalesPanel(module); break;
                    case "Production":
                    case "Raw Materials":   panel = new ProductionPanel(module); break;
                    case "Purchase Orders":
                    case "Goods Received":
                    case "Suppliers":       panel = new ProcurementPanel(module); break;
                    case "Warehouse":
                    case "Delivery Notes":  panel = new WarehousePanel(module); break;
                    case "Invoices":
                    case "Refunds":         panel = new FinancePanel(module); break;
                    case "Staff":
                    case "System Admin":    panel = new SystemAdminPanel(module); break;
                }
                if (panel != null)
                {
                    panel.Dock = DockStyle.Fill;
                    _contentPanel.Controls.Add(panel);
                }
            }
            catch (Exception ex)
            {
                _contentPanel.Controls.Add(new Label { Text = "Error loading module: " + ex.Message, ForeColor = Color.Red, AutoSize = true, Location = new Point(20, 20) });
            }
            _contentPanel.ResumeLayout();
        }

        public void SetCurrentUser(Staff user)
        {
            CurrentUser = user;
            if (_headerPanel.Controls["lblUser"] is Label ul && user != null)
            {
                ul.Text = user.FullName + " | " + user.Department;
                ul.Location = new Point(_headerPanel.Width - ul.PreferredWidth - 20, (_headerPanel.Height - ul.PreferredHeight) / 2);
            }
        }
    }
}
