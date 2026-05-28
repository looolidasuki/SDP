using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sales_user.Controllers;
using Sales_user.Models;
using FurnitureERP.Helpers;

namespace FurnitureERP.Forms
{
    public class WarehousePanel : UserControl
    {
        private readonly WarehouseController _warehouseCtrl = new WarehouseController();
        private readonly RawMaterialController _rawMaterialCtrl = new RawMaterialController();
        private DataGridView _grid;

        private TabControl _tabs;
        private DataGridView _deliveryGrid;
        private readonly DeliveryNoteController _deliveryCtrl = new DeliveryNoteController();

        public WarehousePanel(string module = "Warehouse")
        {
            Dock = DockStyle.Fill;
            BackColor = UITheme.Background;
            BuildTabUI();
            if (module == "Delivery Notes" && _tabs != null) _tabs.SelectedIndex = 1;
        }

        private void BuildTabUI()
        {
            _tabs = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9) };

            // Warehouses Tab
            var warehouseTab = new TabPage("🏭 Warehouses") { BackColor = UITheme.Background };

            var toolbar = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = UITheme.Background };
            var btnNew = UITheme.CreatePrimaryButton("+ New Warehouse");
            btnNew.Location = new Point(0, 8);
            btnNew.Click += (s, e) => ShowCreateDialog();
            var btnRefresh = UITheme.CreateSecondaryButton("↻ Refresh");
            btnRefresh.Location = new Point(btnNew.Width + 10, 8);
            btnRefresh.Click += (s, e) => LoadData();
            toolbar.Controls.Add(btnNew);
            toolbar.Controls.Add(btnRefresh);

            _grid = GridHelper.CreateStyledGrid();
            _grid.CellDoubleClick += Grid_CellDoubleClick;

            var warehouseContent = new Panel { Dock = DockStyle.Fill };
            warehouseContent.Controls.Add(_grid);
            warehouseContent.Controls.Add(toolbar);
            warehouseTab.Controls.Add(warehouseContent);

            // Delivery Notes Tab
            var deliveryTab = new TabPage("🚚 Delivery Notes") { BackColor = UITheme.Background };

            var deliveryToolbar = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = UITheme.Background };
            var btnNewDelivery = UITheme.CreatePrimaryButton("+ New Delivery Note");
            btnNewDelivery.Location = new Point(0, 8);
            btnNewDelivery.Click += (s, e) => ShowCreateDeliveryDialog();
            var btnRefreshDelivery = UITheme.CreateSecondaryButton("↻ Refresh");
            btnRefreshDelivery.Location = new Point(btnNewDelivery.Width + 10, 8);
            btnRefreshDelivery.Click += (s, e) => LoadDeliveryNotes();
            deliveryToolbar.Controls.Add(btnNewDelivery);
            deliveryToolbar.Controls.Add(btnRefreshDelivery);

            _deliveryGrid = GridHelper.CreateStyledGrid();

            var deliveryContent = new Panel { Dock = DockStyle.Fill };
            deliveryContent.Controls.Add(_deliveryGrid);
            deliveryContent.Controls.Add(deliveryToolbar);
            deliveryTab.Controls.Add(deliveryContent);

            _tabs.TabPages.Add(warehouseTab);
            _tabs.TabPages.Add(deliveryTab);

            Controls.Add(_tabs);

            LoadData();
            LoadDeliveryNotes();
        }

        private void LoadData()
        {
            try { _grid.DataSource = _warehouseCtrl.GetAllWarehouses(); GridHelper.StyleGrid(_grid); }
            catch { }
        }

        private void LoadDeliveryNotes()
        {
            try { _deliveryGrid.DataSource = _deliveryCtrl.GetAllDeliveryNotes(); GridHelper.StyleGrid(_deliveryGrid); }
            catch { }
        }

        private void Grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = _grid.Rows[e.RowIndex];
            if (row.Cells[0].Value == null) return;
            long id = Convert.ToInt64(row.Cells[0].Value);
            ShowEditDialog(id);
        }

        private void ShowCreateDialog()
        {
            using (var dlg = new Form())
            {
                dlg.Text = "New Warehouse";
                dlg.Size = new Size(460, 240);
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MaximizeBox = false;
                dlg.BackColor = UITheme.Background;

                var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3, Padding = new Padding(16) };
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

                var txtName = new TextBox();
                var txtAddr = new TextBox();

                UITheme.AddFormRow(layout, 0, "Warehouse Name *", txtName);
                UITheme.AddFormRow(layout, 1, "Address", txtAddr);

                var btnSave = UITheme.CreatePrimaryButton("Save");
                var btnCancel = UITheme.CreateSecondaryButton("Cancel");
                btnCancel.Click += (s, e) => dlg.Close();
                btnSave.Click += (s, e) =>
                {
                    if (string.IsNullOrWhiteSpace(txtName.Text)) { MessageBox.Show("Warehouse Name is required.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                    try
                    {
                        _warehouseCtrl.Insert(new Warehouse { WarehouseName = txtName.Text.Trim(), WarehouseAddress = txtAddr.Text.Trim() });
                        MessageBox.Show("Warehouse created successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dlg.DialogResult = DialogResult.OK; dlg.Close();
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                };

                var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 50, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) };
                btnPanel.Controls.Add(btnSave); btnPanel.Controls.Add(btnCancel);

                dlg.Controls.Add(layout); dlg.Controls.Add(btnPanel);
                if (dlg.ShowDialog(this) == DialogResult.OK) LoadData();
            }
        }

        private void ShowEditDialog(long id)
        {
            var wh = _warehouseCtrl.GetById(id);
            if (wh == null) { MessageBox.Show("Warehouse not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            using (var dlg = new Form())
            {
                dlg.Text = $"Edit Warehouse — {wh.WarehouseName}";
                dlg.Size = new Size(460, 240);
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MaximizeBox = false;
                dlg.BackColor = UITheme.Background;

                var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3, Padding = new Padding(16) };
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

                var txtName = new TextBox { Text = wh.WarehouseName };
                var txtAddr = new TextBox { Text = wh.WarehouseAddress };

                UITheme.AddFormRow(layout, 0, "Warehouse Name *", txtName);
                UITheme.AddFormRow(layout, 1, "Address", txtAddr);

                var btnSave = UITheme.CreatePrimaryButton("Update");
                var btnCancel = UITheme.CreateSecondaryButton("Cancel");
                btnCancel.Click += (s, e) => dlg.Close();
                btnSave.Click += (s, e) =>
                {
                    if (string.IsNullOrWhiteSpace(txtName.Text)) { MessageBox.Show("Warehouse Name is required.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                    try
                    {
                        wh.WarehouseName = txtName.Text.Trim();
                        wh.WarehouseAddress = txtAddr.Text.Trim();
                        _warehouseCtrl.Update(wh);
                        MessageBox.Show("Warehouse updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dlg.DialogResult = DialogResult.OK; dlg.Close();
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                };

                var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 50, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) };
                btnPanel.Controls.Add(btnSave); btnPanel.Controls.Add(btnCancel);

                dlg.Controls.Add(layout); dlg.Controls.Add(btnPanel);
                if (dlg.ShowDialog(this) == DialogResult.OK) LoadData();
            }
        }

        private void ShowCreateDeliveryDialog()
        {
            using (var dlg = new Form())
            {
                dlg.Text = "New Delivery Note";
                dlg.Size = new Size(480, 320);
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MaximizeBox = false;
                dlg.BackColor = UITheme.Background;

                var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 5, Padding = new Padding(16) };
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

                var txtCustomerId = new TextBox();
                var txtSalesOrderId = new TextBox();
                var txtStaffId = new TextBox();
                var cmbStatus = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
                cmbStatus.Items.AddRange(new object[] { "0 - Draft", "1 - Dispatched", "2 - Delivered" });
                cmbStatus.SelectedIndex = 0;
                var txtRemark = new TextBox();

                UITheme.AddFormRow(layout, 0, "Customer ID *", txtCustomerId);
                UITheme.AddFormRow(layout, 1, "Sales Order ID *", txtSalesOrderId);
                UITheme.AddFormRow(layout, 2, "Staff ID *", txtStaffId);
                UITheme.AddFormRow(layout, 3, "Status", cmbStatus);
                UITheme.AddFormRow(layout, 4, "Remark", txtRemark);

                var btnSave = UITheme.CreatePrimaryButton("Save");
                var btnCancel = UITheme.CreateSecondaryButton("Cancel");
                btnCancel.Click += (s, e) => dlg.Close();
                btnSave.Click += (s, e) =>
                {
                    if (!long.TryParse(txtCustomerId.Text, out long custId) || !long.TryParse(txtSalesOrderId.Text, out long soId) || !long.TryParse(txtStaffId.Text, out long staffId))
                    { MessageBox.Show("Valid Customer ID, Sales Order ID and Staff ID are required.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                    try
                    {
                        var dn = new DeliveryNote
                        {
                            DeliveryNoteCode = "DN-TEMP",
                            CustomerID = custId,
                            SalesOrderID = soId,
                            StaffID = staffId,
                            Status = cmbStatus.SelectedIndex,
                            Remark = txtRemark.Text.Trim()
                        };
                        long newId = _deliveryCtrl.Insert(dn);
                        _deliveryCtrl.UpdateCodeAfterInsert(newId);
                        MessageBox.Show($"Delivery Note DN-{newId} created.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dlg.DialogResult = DialogResult.OK; dlg.Close();
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                };

                var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 50, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) };
                btnPanel.Controls.Add(btnSave); btnPanel.Controls.Add(btnCancel);
                dlg.Controls.Add(layout); dlg.Controls.Add(btnPanel);
                if (dlg.ShowDialog(this) == DialogResult.OK) LoadDeliveryNotes();
            }
        }
    }
}
