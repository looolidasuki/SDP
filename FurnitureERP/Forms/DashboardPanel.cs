using System;
using System.Drawing;
using System.Windows.Forms;
using Sales_user.Controllers;
using FurnitureERP.Helpers;

namespace FurnitureERP.Forms
{
    /// <summary>
    /// DashboardPanel displays high-level system metrics and overview.
    /// Inheritance: UserControl provides better stability for UI components.
    /// </summary>
    public class DashboardPanel : UserControl
    {
        private readonly CustomerController _customerCtrl = new CustomerController();
        private readonly SalesOrderController _salesOrderCtrl = new SalesOrderController();
        private readonly InvoiceController _invoiceCtrl = new InvoiceController();
        private readonly ProductController _productCtrl = new ProductController();

        public DashboardPanel()
        {
            Dock = DockStyle.Fill;
            BackColor = UITheme.Background;
            Build();
        }

        private void Build()
        {
            // 1. 初始化與佈局參數設定
            this.SuspendLayout();
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.WhiteSmoke;

            // 2. 建立 KPI 區域 (Top)
            Panel cardsRow = new Panel { Dock = DockStyle.Top, Height = 150, BackColor = Color.Transparent };

            // 3. 建立 TableLayout (使用 25% 強制均分)
            TableLayoutPanel cardTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                Padding = new Padding(0), // 必須為 0 以避免計算偏移
                Margin = new Padding(0)
            };

            for (int i = 0; i < 4; i++)
                cardTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            // 4. 定義數據 (確保資料完整)
            string[] titles = { "Customers", "Sales Orders", "Invoices", "Products" };
            string[] values = {
        _customerCtrl.GetCount().ToString(),
        _salesOrderCtrl.GetCount().ToString(),
        _invoiceCtrl.GetCount().ToString(),
        _productCtrl.GetCount().ToString()
    };
            string[] icons = { "👥", "📝", "📄", "📦" };
            Color[] colors = { Color.DodgerBlue, Color.Orange, Color.Green, Color.Purple };

            // 5. 強制加入卡片
            for (int i = 0; i < 4; i++)
            {
                // 直接建立並加入 Table，不使用額外 Container，減少佈局計算誤差
                Panel kpiCard = CreateKpiCard(titles[i], values[i], icons[i], colors[i]);
                kpiCard.Dock = DockStyle.Fill;
                kpiCard.Margin = new Padding(8); // 卡片間距
                cardTable.Controls.Add(kpiCard, i, 0);
            }

            cardsRow.Controls.Add(cardTable);

            // 6. 建立填充區域 (Fill)
            Panel fillPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };

            // 7. 重要：Dock 佈局規則 — Top 必須在 Fill 之前加入
            // WinForms 按 Controls 倒序處理 Dock，Fill 必須最後加入
            this.Controls.Add(cardsRow);   // Top — 先加
            this.Controls.Add(fillPanel);  // Fill — 後加（最後）

            // 8. 強制 UI 更新計算，修復 "向上移" 或渲染異常
            this.PerformLayout();
            this.ResumeLayout(true);
        }


        private Panel CreateCard(string title)
        {
            Panel card = new Panel { BackColor = Color.White, Padding = new Padding(15) };

            // 繪製卡片邊框
            card.Paint += (s, e) => {
                using (var pen = new System.Drawing.Pen(Color.FromArgb(225, 230, 240)))
                    e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
            };

            // 建立標題 Label
            Label titleLbl = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = UITheme.TextDark,
                Dock = DockStyle.Top,
                Height = 35
            };

            card.Controls.Add(titleLbl);
            return card;
        }

        private Panel CreateKpiCard(string title, string value, string icon, Color accentColor)
        {
            Panel card = new Panel { Dock = DockStyle.Fill, Margin = new Padding(8), BackColor = Color.White };

            // 1. 繪製邊框與側邊強調色 (Paint 事件保持不變)
            card.Paint += (s, e) => {
                using (var brush = new SolidBrush(accentColor))
                    e.Graphics.FillRectangle(brush, 0, 0, 6, card.Height);
                using (var pen = new Pen(Color.FromArgb(220, 225, 230)))
                    e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
            };

            // 2. 使用一個容器來承載內部文字，避免使用 Location
            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(15, 10, 10, 10)
            };
            layout.RowCount = 3;
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 30));

            // 3. 建立 Label
            Label iconLbl = new Label { Text = icon, Font = new Font("Segoe UI Emoji", 16), AutoSize = true, Anchor = AnchorStyles.Left };
            Label valueLbl = new Label { Text = value, Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = UITheme.TextDark, AutoSize = true, Anchor = AnchorStyles.Left };
            Label titleLbl = new Label { Text = title, Font = new Font("Segoe UI", 9), ForeColor = UITheme.TextGray, AutoSize = true, Anchor = AnchorStyles.Left };

            // 將控件放入表格，不再依賴硬編碼的 Point
            layout.Controls.Add(iconLbl, 0, 0);
            layout.Controls.Add(valueLbl, 0, 1);
            layout.Controls.Add(titleLbl, 0, 2);

            card.Controls.Add(layout);
            return card;
        }

        private Panel CreateDetailCard(string title)
        {
            Panel card = new Panel { BackColor = Color.White, Padding = new Padding(15) };
            card.Paint += (s, e) => {
                using (var pen = new Pen(Color.FromArgb(225, 230, 240)))
                    e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
            };
            Label lbl = new Label { Text = title, Font = new Font("Segoe UI", 11, FontStyle.Bold), Dock = DockStyle.Top, Height = 35 };
            card.Controls.Add(lbl);
            return card;
        }
    }
}