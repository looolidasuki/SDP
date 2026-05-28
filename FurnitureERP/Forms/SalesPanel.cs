using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Sales_user.Controllers;
using Sales_user.Models;
using FurnitureERP.Helpers;

namespace FurnitureERP.Forms
{
    public class SalesPanel : UserControl
    {
        private readonly CustomerController _customerCtrl = new CustomerController();
        private readonly SalesOrderController _salesOrderCtrl = new SalesOrderController();
        private readonly QuotationController _quotationCtrl = new QuotationController();

        private TabControl _tabs;

        public SalesPanel(string module = "Customers")
        {
            Dock = DockStyle.Fill;
            BackColor = UITheme.Background;
            BuildUI();
            if (module == "Quotations") _tabs.SelectedIndex = 1;
            else if (module == "Sales Orders") _tabs.SelectedIndex = 2;
        }

        private void BuildUI()
        {
            _tabs = new TabControl { Dock = DockStyle.Fill };

            _tabs.TabPages.Add(BuildCustomerTab());
            _tabs.TabPages.Add(BuildQuotationTab());
            _tabs.TabPages.Add(BuildSalesOrderTab());

            Controls.Add(_tabs);
        }

        private TabPage BuildCustomerTab()
        {
            var page = new TabPage("Customers");
            page.Controls.Add(BuildCrudPanel("Customer",
                () => _customerCtrl.GetAllCustomers(),
                ShowCreateCustomerDialog));
            return page;
        }

        private TabPage BuildQuotationTab()
        {
            var page = new TabPage("Quotations");
            page.Controls.Add(BuildCrudPanel("Quotation",
                () => _quotationCtrl.GetAllQuotations(),
                ShowCreateQuotationDialog));
            return page;
        }

        private TabPage BuildSalesOrderTab()
        {
            var page = new TabPage("Sales Orders");
            page.Controls.Add(BuildCrudPanel("Sales Order",
                () => _salesOrderCtrl.GetAllSalesOrders(),
                ShowCreateSalesOrderDialog));
            return page;
        }

        private Panel BuildCrudPanel(string entity, Func<DataTable> loadData, Action onCreate)
        {
            Panel panel = new Panel { Dock = DockStyle.Fill };

            Panel toolbar = new Panel { Dock = DockStyle.Top, Height = 50 };
            Button btnNew = UITheme.CreatePrimaryButton($"+ New {entity}");
            btnNew.Location = new Point(8, 8);
            btnNew.Click += (s, e) => onCreate();
            Button btnRefresh = UITheme.CreateSecondaryButton("↻ Refresh");
            btnRefresh.Location = new Point(btnNew.Width + 18, 8);

            DataGridView grid = GridHelper.CreateStyledGrid();
            btnRefresh.Click += (s, e) => {
                try { grid.DataSource = loadData(); GridHelper.StyleGrid(grid); } catch { }
            };

            toolbar.Controls.Add(btnNew);
            toolbar.Controls.Add(btnRefresh);

            try { grid.DataSource = loadData(); GridHelper.StyleGrid(grid); } catch { }

            panel.Controls.Add(grid);
            panel.Controls.Add(toolbar);
            return panel;
        }

        private void ShowCreateCustomerDialog()
        {
            using (var dlg = new Form())
            {
                dlg.Text = "New Customer";
                dlg.Size = new Size(480, 280);
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MaximizeBox = false;
                dlg.BackColor = UITheme.Background;

                var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 4, Padding = new Padding(16) };
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

                var txtName = new TextBox(); var txtAddr = new TextBox(); var txtTerm = new TextBox();
                UITheme.AddFormRow(layout, 0, "Customer Name *", txtName);
                UITheme.AddFormRow(layout, 1, "Billing Address", txtAddr);
                UITheme.AddFormRow(layout, 2, "Payment Term", txtTerm);

                var btnSave = UITheme.CreatePrimaryButton("Save");
                var btnCancel = UITheme.CreateSecondaryButton("Cancel");
                btnCancel.Click += (s, e) => dlg.Close();
                btnSave.Click += (s, e) =>
                {
                    if (string.IsNullOrWhiteSpace(txtName.Text)) { MessageBox.Show("Customer Name is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                    try
                    {
                        _customerCtrl.Insert(new Customer { CustomerName = txtName.Text.Trim(), BillingAddress = txtAddr.Text.Trim(), PaymentTerm = txtTerm.Text.Trim() });
                        MessageBox.Show("Customer created successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dlg.DialogResult = DialogResult.OK; dlg.Close();
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                };

                var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 50, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) };
                btnPanel.Controls.Add(btnSave); btnPanel.Controls.Add(btnCancel);
                dlg.Controls.Add(layout); dlg.Controls.Add(btnPanel);
                dlg.ShowDialog(this);
            }
        }

        private void ShowCreateQuotationDialog()
        {
            using (var dlg = UITheme.BuildInputDialog("New Quotation",
                new[] { "Customer ID *", "Staff ID *", "Currency ID", "Status (0-3)", "Remark" }))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        var vals = UITheme.GetDialogValues(dlg);
                        var q = new Quotation
                        {
                            QuotationCode = "QT-TEMP",
                            CustomerID = long.Parse(vals[0]),
                            StaffID = long.Parse(vals[1]),
                            CurrencyID = string.IsNullOrEmpty(vals[2]) ? 1 : long.Parse(vals[2]),
                            Status = string.IsNullOrEmpty(vals[3]) ? 0 : int.Parse(vals[3]),
                            Remark = vals[4],
                            SequenceNumber = 1
                        };
                        long id = _quotationCtrl.Insert(q);
                        _quotationCtrl.UpdateCodeAfterInsert(id);
                        MessageBox.Show($"Quotation QT-{id} created.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
        }

        private void ShowCreateSalesOrderDialog()
        {
            using (var dlg = UITheme.BuildInputDialog("New Sales Order",
                new[] { "Customer ID *", "Staff ID *", "Currency ID", "Delivery Address *", "Discount", "Status", "Remark" }))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        var vals = UITheme.GetDialogValues(dlg);
                        var so = new SalesOrder
                        {
                            SalesOrderCode = "SO-TEMP",
                            CustomerID = long.Parse(vals[0]),
                            StaffID = long.Parse(vals[1]),
                            CurrencyCurrencyID = string.IsNullOrEmpty(vals[2]) ? 1 : long.Parse(vals[2]),
                            DeliveryAddress = vals[3],
                            Discount = string.IsNullOrEmpty(vals[4]) ? 0 : decimal.Parse(vals[4]),
                            Status = string.IsNullOrEmpty(vals[5]) ? 0 : int.Parse(vals[5]),
                            Remark = vals[6]
                        };
                        long id = _salesOrderCtrl.Insert(so);
                        _salesOrderCtrl.UpdateCodeAfterInsert(id);
                        MessageBox.Show($"Sales Order SO-{id} created.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
        }
    }
}
