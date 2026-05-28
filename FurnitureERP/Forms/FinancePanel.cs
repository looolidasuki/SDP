using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Sales_user.Controllers;
using Sales_user.Models;
using FurnitureERP.Helpers;

namespace FurnitureERP.Forms
{
    public class FinancePanel : UserControl
    {
        private readonly InvoiceController _invoiceCtrl = new InvoiceController();
        private readonly RefundRequestController _refundCtrl = new RefundRequestController();

        private TabControl _tabControl;
        private DataGridView _invoiceGrid;
        private DataGridView _refundGrid;

        public FinancePanel(string module = "Invoices")
        {
            Dock = DockStyle.Fill;
            BackColor = UITheme.Background;
            BuildUI();
            LoadData();
            if (module == "Refunds") _tabControl.SelectedIndex = 1;
        }

        private void BuildUI()
        {
            _tabControl = new TabControl { Dock = DockStyle.Fill };

            // Invoices Tab
            TabPage invoiceTab = new TabPage("🧾 Invoices");
            invoiceTab.BackColor = UITheme.Background;

            Panel invoiceToolbar = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(0, 8, 0, 8) };
            Button btnNewInvoice = UITheme.CreatePrimaryButton("+ New Invoice");
            btnNewInvoice.Location = new Point(0, 8);
            btnNewInvoice.Click += (s, e) => ShowCreateInvoiceDialog();
            Button btnRefreshInvoice = UITheme.CreateSecondaryButton("↻ Refresh");
            btnRefreshInvoice.Location = new Point(btnNewInvoice.Width + 10, 8);
            btnRefreshInvoice.Click += (s, e) => LoadInvoices();
            invoiceToolbar.Controls.Add(btnNewInvoice);
            invoiceToolbar.Controls.Add(btnRefreshInvoice);

            _invoiceGrid = GridHelper.CreateStyledGrid();
            _invoiceGrid.CellDoubleClick += InvoiceGrid_CellDoubleClick;

            Panel invoicePanel = new Panel { Dock = DockStyle.Fill };
            invoicePanel.Controls.Add(_invoiceGrid);
            invoicePanel.Controls.Add(invoiceToolbar);
            invoiceTab.Controls.Add(invoicePanel);

            // Refunds Tab
            TabPage refundTab = new TabPage("💰 Refund Requests");
            refundTab.BackColor = UITheme.Background;

            Panel refundToolbar = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(0, 8, 0, 8) };
            Button btnNewRefund = UITheme.CreatePrimaryButton("+ New Refund");
            btnNewRefund.Location = new Point(0, 8);
            btnNewRefund.Click += (s, e) => ShowCreateRefundDialog();
            Button btnRefreshRefund = UITheme.CreateSecondaryButton("↻ Refresh");
            btnRefreshRefund.Location = new Point(btnNewRefund.Width + 10, 8);
            btnRefreshRefund.Click += (s, e) => LoadRefunds();
            refundToolbar.Controls.Add(btnNewRefund);
            refundToolbar.Controls.Add(btnRefreshRefund);

            _refundGrid = GridHelper.CreateStyledGrid();

            Panel refundPanel = new Panel { Dock = DockStyle.Fill };
            refundPanel.Controls.Add(_refundGrid);
            refundPanel.Controls.Add(refundToolbar);
            refundTab.Controls.Add(refundPanel);

            _tabControl.TabPages.Add(invoiceTab);
            _tabControl.TabPages.Add(refundTab);
            Controls.Add(_tabControl);
        }

        private void LoadData()
        {
            LoadInvoices();
            LoadRefunds();
        }

        private void LoadInvoices()
        {
            try
            {
                _invoiceGrid.DataSource = _invoiceCtrl.GetAllInvoices();
                GridHelper.StyleGrid(_invoiceGrid);
            }
            catch { }
        }

        private void LoadRefunds()
        {
            try
            {
                _refundGrid.DataSource = _refundCtrl.GetAllRefundRequests();
                GridHelper.StyleGrid(_refundGrid);
            }
            catch { }
        }

        private void InvoiceGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = _invoiceGrid.Rows[e.RowIndex];
            long invoiceId = Convert.ToInt64(row.Cells[0].Value);
            ShowInvoiceDetails(invoiceId);
        }

        private void ShowInvoiceDetails(long invoiceId)
        {
            using (var dlg = new Form())
            {
                dlg.Text = $"Invoice Details — ID: {invoiceId}";
                dlg.Size = new Size(700, 500);
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.BackColor = UITheme.Background;

                var grid = GridHelper.CreateStyledGrid();
                try
                {
                    // Show invoice product lines if available
                    grid.DataSource = _invoiceCtrl.GetInvoiceLines(invoiceId);
                    GridHelper.StyleGrid(grid);
                }
                catch { }

                dlg.Controls.Add(grid);
                dlg.ShowDialog(this);
            }
        }

        private void ShowCreateInvoiceDialog()
        {
            using (var dlg = new Form())
            {
                dlg.Text = "New Invoice";
                dlg.Size = new Size(480, 400);
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MaximizeBox = false;
                dlg.BackColor = UITheme.Background;

                var layout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 7,
                    Padding = new Padding(16)
                };
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

                var txtCustomerId = new TextBox();
                var txtSalesOrderId = new TextBox();
                var txtStaffId = new TextBox();
                var cmbType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
                cmbType.Items.AddRange(new[] { "1 - Standard", "2 - Proforma", "3 - Credit Note" });
                cmbType.SelectedIndex = 0;
                var cmbStatus = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
                cmbStatus.Items.AddRange(new[] { "0 - Draft", "1 - Sent", "2 - Paid", "3 - Overdue", "4 - Cancelled" });
                cmbStatus.SelectedIndex = 0;
                var txtRemark = new TextBox();

                UITheme.AddFormField(layout, 0, "Customer ID *", txtCustomerId);
                UITheme.AddFormField(layout, 1, "Sales Order ID *", txtSalesOrderId);
                UITheme.AddFormField(layout, 2, "Staff ID *", txtStaffId);
                UITheme.AddFormField(layout, 3, "Invoice Type", cmbType);
                UITheme.AddFormField(layout, 4, "Status", cmbStatus);
                UITheme.AddFormField(layout, 5, "Remark", txtRemark);

                var btnSave = UITheme.CreatePrimaryButton("Save");
                var btnCancel = UITheme.CreateSecondaryButton("Cancel");
                btnCancel.Click += (s, e) => dlg.Close();
                btnSave.Click += (s, e) =>
                {
                    if (string.IsNullOrWhiteSpace(txtCustomerId.Text) ||
                        string.IsNullOrWhiteSpace(txtSalesOrderId.Text) ||
                        string.IsNullOrWhiteSpace(txtStaffId.Text))
                    {
                        MessageBox.Show("Customer ID, Sales Order ID and Staff ID are required.", "Validation",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    try
                    {
                        var inv = new Invoice
                        {
                            InvoiceCode = "INV-TEMP",
                            CustomerID = long.Parse(txtCustomerId.Text.Trim()),
                            SalesOrderID = long.Parse(txtSalesOrderId.Text.Trim()),
                            StaffID = long.Parse(txtStaffId.Text.Trim()),
                            InvoiceType = cmbType.SelectedIndex + 1,
                            Status = cmbStatus.SelectedIndex,
                            Remark = txtRemark.Text.Trim()
                        };
                        long id = _invoiceCtrl.Insert(inv);
                        _invoiceCtrl.UpdateCodeAfterInsert(id);
                        MessageBox.Show($"Invoice INV-{id} created successfully.", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dlg.DialogResult = DialogResult.OK;
                        dlg.Close();
                        LoadInvoices();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                var btnPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Bottom, Height = 50,
                    FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8)
                };
                btnPanel.Controls.Add(btnSave);
                btnPanel.Controls.Add(btnCancel);

                dlg.Controls.Add(layout);
                dlg.Controls.Add(btnPanel);
                dlg.ShowDialog(this);
            }
        }

        private void ShowCreateRefundDialog()
        {
            using (var dlg = new Form())
            {
                dlg.Text = "New Refund Request";
                dlg.Size = new Size(480, 420);
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MaximizeBox = false;
                dlg.BackColor = UITheme.Background;

                var layout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 7,
                    Padding = new Padding(16)
                };
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

                var txtInvoiceId = new TextBox();
                var txtStaffId = new TextBox();
                var txtAmount = new TextBox();
                var cmbMethod = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
                cmbMethod.Items.AddRange(new[] { "Bank Transfer", "Credit Card", "Cash", "Store Credit" });
                cmbMethod.SelectedIndex = 0;
                var txtReason = new TextBox();
                var txtRemark = new TextBox();

                UITheme.AddFormField(layout, 0, "Invoice ID *", txtInvoiceId);
                UITheme.AddFormField(layout, 1, "Staff ID *", txtStaffId);
                UITheme.AddFormField(layout, 2, "Refund Amount *", txtAmount);
                UITheme.AddFormField(layout, 3, "Refund Method *", cmbMethod);
                UITheme.AddFormField(layout, 4, "Reason *", txtReason);
                UITheme.AddFormField(layout, 5, "Remark", txtRemark);

                var btnSave = UITheme.CreatePrimaryButton("Save");
                var btnCancel = UITheme.CreateSecondaryButton("Cancel");
                btnCancel.Click += (s, e) => dlg.Close();
                btnSave.Click += (s, e) =>
                {
                    if (string.IsNullOrWhiteSpace(txtInvoiceId.Text) ||
                        string.IsNullOrWhiteSpace(txtStaffId.Text) ||
                        string.IsNullOrWhiteSpace(txtAmount.Text) ||
                        string.IsNullOrWhiteSpace(txtReason.Text))
                    {
                        MessageBox.Show("Invoice ID, Staff ID, Amount and Reason are required.", "Validation",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    try
                    {
                        var rr = new RefundRequest
                        {
                            RefundRequestCode = "RF-TEMP",
                            InvoiceID = long.Parse(txtInvoiceId.Text.Trim()),
                            StaffID = long.Parse(txtStaffId.Text.Trim()),
                            RefundAmount = decimal.Parse(txtAmount.Text.Trim()),
                            RefundMethod = cmbMethod.SelectedIndex,
                            RefundReason = txtReason.Text.Trim(),
                            Remark = txtRemark.Text.Trim(),
                            Status = 0
                        };
                        _refundCtrl.CreateRefundRequest(rr);
                        MessageBox.Show("Refund Request created successfully.", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dlg.DialogResult = DialogResult.OK;
                        dlg.Close();
                        LoadRefunds();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                var btnPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Bottom, Height = 50,
                    FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8)
                };
                btnPanel.Controls.Add(btnSave);
                btnPanel.Controls.Add(btnCancel);

                dlg.Controls.Add(layout);
                dlg.Controls.Add(btnPanel);
                dlg.ShowDialog(this);
            }
        }
    }
}
