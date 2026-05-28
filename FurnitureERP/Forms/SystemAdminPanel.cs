using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Sales_user.Controllers;
using FurnitureERP.Helpers;

namespace FurnitureERP.Forms
{
    public class SystemAdminPanel : Panel
    {
        private readonly ProductController _productCtrl = new ProductController();
        private readonly SystemDictionaryController _dictCtrl = new SystemDictionaryController();
        private readonly RawMaterialController _rawMaterialCtrl = new RawMaterialController();

        public SystemAdminPanel(string module = "System Admin")
        {
            Dock = DockStyle.Fill;
            BackColor = UITheme.Background;
            BuildUI();
        }

        private void BuildUI()
        {
            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(0)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            // System Dictionary
            Panel dictCard = UITheme.CreateCard("System Dictionary");
            DataGridView dictGrid = GridHelper.CreateStyledGrid();
            dictGrid.Dock = DockStyle.Fill;
            try { dictGrid.DataSource = _dictCtrl.GetAllDictionaries(); GridHelper.StyleGrid(dictGrid); } catch { }
            Button addDictBtn = UITheme.CreatePrimaryButton("+ Add Entry");
            addDictBtn.Dock = DockStyle.Bottom;
            addDictBtn.Height = 32;
            addDictBtn.Click += AddDictEntry_Click;
            dictCard.Controls.Add(dictGrid);
            dictCard.Controls.Add(addDictBtn);
            layout.Controls.Add(dictCard, 0, 0);

            // Product Catalog
            Panel productCard = UITheme.CreateCard("Product Catalog");
            DataGridView productGrid = GridHelper.CreateStyledGrid();
            productGrid.Dock = DockStyle.Fill;
            try { productGrid.DataSource = _productCtrl.GetAllProducts(); GridHelper.StyleGrid(productGrid); } catch { }
            Button addProductBtn = UITheme.CreatePrimaryButton("+ Add Product");
            addProductBtn.Dock = DockStyle.Bottom;
            addProductBtn.Height = 32;
            addProductBtn.Click += AddProduct_Click;
            productCard.Controls.Add(productGrid);
            productCard.Controls.Add(addProductBtn);
            layout.Controls.Add(productCard, 1, 0);

            // Supplier Raw Material Quotes
            Panel sqCard = UITheme.CreateCard("Supplier Raw Material Quotes");
            DataGridView sqGrid = GridHelper.CreateStyledGrid();
            sqGrid.Dock = DockStyle.Fill;
            try { sqGrid.DataSource = _rawMaterialCtrl.GetAllSupplierQuotes(); GridHelper.StyleGrid(sqGrid); } catch { }
            sqCard.Controls.Add(sqGrid);
            layout.Controls.Add(sqCard, 0, 1);

            // System Information
            Panel infoCard = UITheme.CreateCard("System Information");
            Label infoLabel = new Label
            {
                Text = "Premium Living Furniture ERP\n\nVersion: 2.0\nFramework: .NET 4.8\nDatabase: MySQL\n\n" +
                       "Modules:\n• Sales & Quotations\n• Production Management\n• Warehouse & Inventory\n" +
                       "• Procurement & GRN\n• Finance & Invoicing\n• System Administration",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = UITheme.TextDark,
                Padding = new Padding(12)
            };
            infoCard.Controls.Add(infoLabel);
            layout.Controls.Add(infoCard, 1, 1);

            Controls.Add(layout);
        }

        private void AddDictEntry_Click(object sender, EventArgs e)
        {
            using (var dlg = UITheme.BuildInputDialog("New Dictionary Entry",
                new[] { "Category *", "Code Value *", "Display Name *", "Sort Order" }))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    var vals = UITheme.GetDialogValues(dlg);
                    try
                    {
                        _dictCtrl.Insert(new Sales_user.Models.SystemDictionary
                        {
                            Category = vals[0],
                            CodeValue = string.IsNullOrEmpty(vals[1]) ? 0 : int.Parse(vals[1]),
                            DisplayNameEnglish = vals[2],
                            SortOrder = string.IsNullOrEmpty(vals[3]) ? 0 : int.Parse(vals[3])
                        });
                        MessageBox.Show("Entry added.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
        }

        private void AddProduct_Click(object sender, EventArgs e)
        {
            using (var dlg = UITheme.BuildInputDialog("New Product",
                new[] { "Product Code *", "Category", "Style Number", "Unit", "Status" }))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    var vals = UITheme.GetDialogValues(dlg);
                    try
                    {
                        _productCtrl.Insert(new Sales_user.Models.Product
                        {
                            ProductCode = vals[0],
                            Category = vals[1],
                            StyleNumber = vals[2],
                            Unit = vals[3],
                            Status = string.IsNullOrEmpty(vals[4]) ? 1 : int.Parse(vals[4])
                        });
                        MessageBox.Show("Product added.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
        }
    }
}
