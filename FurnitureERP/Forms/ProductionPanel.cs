using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Sales_user.Controllers;
using Sales_user.Models;
using FurnitureERP.Helpers;

namespace FurnitureERP.Forms
{
    public class ProductionPanel : UserControl
    {
        private readonly ProductionOrderController _productionCtrl = new ProductionOrderController();
        private readonly RawMaterialController _rawMaterialCtrl = new RawMaterialController();
        private readonly RawMaterialRequestNoteController _rmrnCtrl = new RawMaterialRequestNoteController();
        private DataGridView _grid;
        private TextBox _searchBox;

        private TabControl _tabs;

        public ProductionPanel(string module = "Production")
        {
            Dock = DockStyle.Fill;
            BackColor = UITheme.Background;
            BuildUI();
            LoadData();
            // Raw Materials view handled inline
        }

        private void BuildUI()
        {
            // Toolbar
            Panel toolbar = new Panel { Dock = DockStyle.Top, Height = 52, Padding = new Padding(0, 8, 0, 8) };

            Button btnNew = UITheme.CreatePrimaryButton("+ New Production Order");
            btnNew.Location = new Point(0, 9);
            btnNew.Click += (s, e) => ShowCreateDialog();

            Button btnRefresh = UITheme.CreateSecondaryButton("↻ Refresh");
            btnRefresh.Location = new Point(btnNew.Width + 10, 9);
            btnRefresh.Click += (s, e) => LoadData();

            _searchBox = new TextBox { Width = 220, Height = 28, Location = new Point(btnNew.Width + btnRefresh.Width + 24, 12) };
            _searchBox.TextChanged += (s, e) => LoadData(_searchBox.Text.Trim());

            // Raw Material Request button
            Button btnRMRN = UITheme.CreateSecondaryButton("📋 RM Requests");
            btnRMRN.Location = new Point(_searchBox.Right + 10, 9);
            btnRMRN.Click += (s, e) => ShowRawMaterialRequestsPanel();

            toolbar.Controls.AddRange(new Control[] { btnNew, btnRefresh, _searchBox, btnRMRN });

            // Grid
            _grid = GridHelper.CreateStyledGrid();
            _grid.CellDoubleClick += Grid_CellDoubleClick;

            Controls.Add(_grid);
            Controls.Add(toolbar);
        }

        private void LoadData(string keyword = null)
        {
            try
            {
                DataTable dt;
                if (string.IsNullOrEmpty(keyword))
                    dt = _productionCtrl.GetAllProductionOrders();
                else
                    dt = _productionCtrl.Search(new SearchFilterCriteria { Keyword = keyword });

                if (dt != null)
                {
                    // Add status labels
                    dt.Columns.Add("Status Label", typeof(string));
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["Status"] != DBNull.Value)
                        {
                            int status = Convert.ToInt32(row["Status"]);
                            string label;
                            if (status == 0) label = "Pending";
                            else if (status == 1) label = "In Progress";
                            else if (status == 2) label = "Completed";
                            else if (status == 3) label = "Cancelled";
                            else label = status.ToString();
                            row["Status Label"] = label;
                        }
                    }
                }
                _grid.DataSource = dt;
                GridHelper.StyleGrid(_grid);
            }
            catch (Exception ex)
            {
                // Silently handle no connection
            }
        }

        private void Grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = _grid.Rows[e.RowIndex];
            if (row.Cells[0].Value == null) return;
            long id = Convert.ToInt64(row.Cells[0].Value);
            ShowDetailDialog(id);
        }

        private void ShowCreateDialog()
        {
            using (var dlg = new Form())
            {
                dlg.Text = "New Production Order";
                dlg.Size = new Size(480, 360);
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MaximizeBox = false;
                dlg.BackColor = UITheme.Background;

                var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 6, Padding = new Padding(16) };
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

                var txtSalesOrderId = new TextBox();
                var txtStaffId = new TextBox();
                var dtpFinishDate = new DateTimePicker { Format = DateTimePickerFormat.Short };
                var cmbStatus = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
                cmbStatus.Items.AddRange(new[] { "0 - Pending", "1 - In Progress", "2 - Completed", "3 - Cancelled" });
                cmbStatus.SelectedIndex = 0;
                var txtRemark = new TextBox { Multiline = true, Height = 60 };

                UITheme.AddFormField(layout, 0, "Sales Order ID *", txtSalesOrderId);
                UITheme.AddFormField(layout, 1, "Staff ID *", txtStaffId);
                UITheme.AddFormField(layout, 2, "Est. Finish Date *", dtpFinishDate);
                UITheme.AddFormField(layout, 3, "Status", cmbStatus);
                UITheme.AddFormField(layout, 4, "Remark", txtRemark);

                var btnSave = UITheme.CreatePrimaryButton("Save");
                var btnCancel = UITheme.CreateSecondaryButton("Cancel");
                btnCancel.Click += (s, e) => dlg.Close();
                btnSave.Click += (s, e) =>
                {
                    if (string.IsNullOrWhiteSpace(txtSalesOrderId.Text) || string.IsNullOrWhiteSpace(txtStaffId.Text))
                    { MessageBox.Show("Sales Order ID and Staff ID are required.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                    try
                    {
                        var po = new ProductionOrder
                        {
                            ProductionOrderCode = "PO-TEMP",
                            SalesOrderID = long.Parse(txtSalesOrderId.Text.Trim()),
                            StaffID = long.Parse(txtStaffId.Text.Trim()),
                            EstFinishDate = dtpFinishDate.Value,
                            Status = cmbStatus.SelectedIndex,
                            Remark = txtRemark.Text.Trim()
                        };
                        long id = _productionCtrl.Insert(po);
                        _productionCtrl.UpdateCodeAfterInsert(id);
                        MessageBox.Show($"Production Order PO-{id} created.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dlg.DialogResult = DialogResult.OK;
                        dlg.Close();
                        LoadData();
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                };

                var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 50, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) };
                btnPanel.Controls.Add(btnSave); btnPanel.Controls.Add(btnCancel);
                dlg.Controls.Add(layout); dlg.Controls.Add(btnPanel);
                dlg.ShowDialog(this);
            }
        }

        private void ShowDetailDialog(long id)
        {
            var po = _productionCtrl.GetById(id);
            if (po == null) { MessageBox.Show("Record not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            using (var dlg = new Form())
            {
                dlg.Text = $"Production Order - {po.ProductionOrderCode}";
                dlg.Size = new Size(560, 450);
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MaximizeBox = false;
                dlg.BackColor = UITheme.Background;

                var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 5, Padding = new Padding(16) };
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

                var dtpFinishDate = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = po.EstFinishDate };
                var cmbStatus = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
                cmbStatus.Items.AddRange(new[] { "0 - Pending", "1 - In Progress", "2 - Completed", "3 - Cancelled" });
                cmbStatus.SelectedIndex = Math.Max(0, Math.Min(po.Status, 3));
                var txtRemark = new TextBox { Text = po.Remark ?? "", Multiline = true, Height = 60 };

                UITheme.AddFormField(layout, 0, "Order Code", new Label { Text = po.ProductionOrderCode, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
                UITheme.AddFormField(layout, 1, "Est. Finish Date", dtpFinishDate);
                UITheme.AddFormField(layout, 2, "Status", cmbStatus);
                UITheme.AddFormField(layout, 3, "Remark", txtRemark);

                var btnUpdate = UITheme.CreatePrimaryButton("Update");
                var btnClose = UITheme.CreateSecondaryButton("Close");
                btnClose.Click += (s, e) => dlg.Close();
                btnUpdate.Click += (s, e) =>
                {
                    try
                    {
                        po.EstFinishDate = dtpFinishDate.Value;
                        po.Status = cmbStatus.SelectedIndex;
                        po.Remark = txtRemark.Text.Trim();
                        _productionCtrl.Update(po);
                        MessageBox.Show("Updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dlg.Close();
                        LoadData();
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                };

                var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 50, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) };
                btnPanel.Controls.Add(btnUpdate); btnPanel.Controls.Add(btnClose);
                dlg.Controls.Add(layout); dlg.Controls.Add(btnPanel);
                dlg.ShowDialog(this);
            }
        }

        private void ShowRawMaterialRequestsPanel()
        {
            using (var dlg = new Form())
            {
                dlg.Text = "Raw Material Request Notes";
                dlg.Size = new Size(800, 500);
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.BackColor = UITheme.Background;

                var grid = GridHelper.CreateStyledGrid();
                grid.Dock = DockStyle.Fill;
                try { grid.DataSource = _rmrnCtrl.GetAllRequestNotes(); GridHelper.StyleGrid(grid); } catch { }

                var btnClose = UITheme.CreateSecondaryButton("Close");
                btnClose.Dock = DockStyle.Bottom;
                btnClose.Click += (s, e) => dlg.Close();

                dlg.Controls.Add(grid);
                dlg.Controls.Add(btnClose);
                dlg.ShowDialog(this);
            }
        }
    }
}
