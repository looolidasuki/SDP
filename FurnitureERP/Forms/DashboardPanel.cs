using System;
using System.Drawing;
using System.Windows.Forms;
using Sales_user.Controllers;
using FurnitureERP.Helpers;

namespace FurnitureERP.Forms
{
    public class DashboardPanel : Panel
    {
        private readonly CustomerController _customerCtrl = new CustomerController();
        private readonly SalesOrderController _salesOrderCtrl = new SalesOrderController();
        private readonly InvoiceController _invoiceCtrl = new InvoiceController();
        private readonly ProductController _productCtrl = new ProductController();

        public DashboardPanel()
        {
            Dock = DockStyle.Fill;
            Build();
        }

        private void Build()
        {
            SuspendLayout();

            // KPI Cards
            Panel cardsRow = new Panel { Dock = DockStyle.Top, Height = 120, Padding = new Padding(0, 0, 0, 12) };
            TableLayoutPanel cardTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1
            };
            for (int i = 0; i < 4; i++)
                cardTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            try
            {
                var customers = _customerCtrl.GetAllCustomers();
                var orders = _salesOrderCtrl.GetAllSalesOrders();
                var invoices = _invoiceCtrl.GetAllInvoices();
                var products = _productCtrl.GetAllProducts();

                cardTable.Controls.Add(CreateKpiCard("Total Customers", customers?.Rows.Count.ToString() ?? "—", "👥", UITheme.Primary), 0, 0);
                cardTable.Controls.Add(CreateKpiCard("Sales Orders", orders?.Rows.Count.ToString() ?? "—", "🛒", Color.FromArgb(0, 168, 120)), 1, 0);
                cardTable.Controls.Add(CreateKpiCard("Invoices", invoices?.Rows.Count.ToString() ?? "—", "🧾", Color.FromArgb(230, 120, 20)), 2, 0);
                cardTable.Controls.Add(CreateKpiCard("Products", products?.Rows.Count.ToString() ?? "—", "📦", Color.FromArgb(160, 40, 180)), 3, 0);
            }
            catch
            {
                cardTable.Controls.Add(CreateKpiCard("Customers", "—", "👥", UITheme.Primary), 0, 0);
                cardTable.Controls.Add(CreateKpiCard("Sales Orders", "—", "🛒", Color.FromArgb(0, 168, 120)), 1, 0);
                cardTable.Controls.Add(CreateKpiCard("Invoices", "—", "🧾", Color.FromArgb(230, 120, 20)), 2, 0);
                cardTable.Controls.Add(CreateKpiCard("Products", "—", "📦", Color.FromArgb(160, 40, 180)), 3, 0);
            }
            cardsRow.Controls.Add(cardTable);

            // Info panel
            Panel infoPanel = CreateCard("System Overview");
            Label infoLabel = new Label
            {
                Text = "Welcome to Premium Living Furniture ERP System\n\nUse the sidebar to navigate between modules.\n\nModules available:\n• Sales: Customers, Quotations, Sales Orders\n• Production: Production Orders, Raw Materials\n• Procurement: Purchase Orders, Goods Received, Suppliers\n• Warehouse: Warehouse Management\n• Logistics: Delivery Notes\n• Finance: Invoices, Refunds\n• System: Staff, System Administration",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = UITheme.TextDark,
                Padding = new Padding(10)
            };
            infoPanel.Dock = DockStyle.Fill;
            infoPanel.Controls.Add(infoLabel);

            Controls.Add(infoPanel);
            Controls.Add(cardsRow);

            ResumeLayout();
        }

        private Panel CreateKpiCard(string title, string value, string icon, Color accentColor)
        {
            Panel card = new Panel { Margin = new Padding(6), BackColor = Color.White };
            card.Paint += (s, e) =>
            {
                using (var brush = new System.Drawing.SolidBrush(accentColor))
                    e.Graphics.FillRectangle(brush, 0, 0, 5, card.Height);
                using (var pen = new System.Drawing.Pen(Color.FromArgb(230, 235, 245)))
                    e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
            };
            Label iconLbl = new Label { Text = icon, Font = new Font("Segoe UI Emoji", 20), AutoSize = true, Location = new Point(14, 14) };
            Label valueLbl = new Label { Text = value, Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = UITheme.TextDark, AutoSize = true, Location = new Point(58, 12) };
            Label titleLbl = new Label { Text = title, Font = new Font("Segoe UI", 8), ForeColor = UITheme.TextGray, AutoSize = true, Location = new Point(58, 46) };
            card.Controls.Add(iconLbl);
            card.Controls.Add(valueLbl);
            card.Controls.Add(titleLbl);
            return card;
        }

        private Panel CreateCard(string title)
        {
            Panel card = new Panel { Margin = new Padding(6), BackColor = Color.White, Padding = new Padding(12) };
            card.Paint += (s, e) =>
            {
                using (var pen = new System.Drawing.Pen(Color.FromArgb(225, 230, 240)))
                    e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
            };
            Label titleLbl = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = UITheme.TextDark,
                Dock = DockStyle.Top,
                Height = 30
            };
            card.Controls.Add(titleLbl);
            return card;
        }
    }
}
