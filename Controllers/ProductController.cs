using MySql.Data.MySqlClient;
using Sales_user.Models;
using System.Data;

namespace Sales_user.Controllers
{
    public class ProductController
    {
        public DataTable GetAllProducts()
        {
            string sql = @"SELECT productID AS 'Product ID',
                                  productCode AS 'Product Code',
                                  category AS 'Category',
                                  styleNumber AS 'Style Number',
                                  size AS 'Size',
                                  color AS 'Color',
                                  basePriceByCurrency AS 'Base Price',
                                  unit AS 'Unit',
                                  status AS 'Status'
                           FROM Product
                           ORDER BY createDate DESC";
            return DatabaseConnect.ExecuteQuery(sql);
        }

        public DataTable GetProductsForPicker()
        {
            return GetAllProducts();
        }

        public long Insert(Product product)
        {
            string sql = @"INSERT INTO Product
                (productCode, category, sequenceNumber, styleNumber, size, color,
                 basePriceByCurrency, currencyID, staffID, unit, status, remark)
                VALUES (@code, @category, @seq, @style, @size, @color,
                        @price, @currencyID, @staffID, @unit, @status, @remark)";
            return DatabaseConnect.ExecuteInsertReturnId(sql, new[] {
                new MySqlParameter("@code", product.ProductCode),
                new MySqlParameter("@category", product.Category),
                new MySqlParameter("@seq", product.SequenceNumber ?? (object)System.DBNull.Value),
                new MySqlParameter("@style", product.StyleNumber),
                new MySqlParameter("@size", product.Size),
                new MySqlParameter("@color", product.Color),
                new MySqlParameter("@price", product.BasePriceByCurrency),
                new MySqlParameter("@currencyID", product.CurrencyID),
                new MySqlParameter("@staffID", product.StaffID),
                new MySqlParameter("@unit", product.Unit),
                new MySqlParameter("@status", product.Status),
                new MySqlParameter("@remark", product.Remark ?? (object)System.DBNull.Value)
            });
        }

        public DataTable GetBomLines(long productId)
        {
            string sql = @"SELECT rm.rawMaterialCode AS 'Raw Material',
                                  prml.rawMaterialNeedQty AS 'Need Qty'
                           FROM ProductRawMaterialLine prml
                           INNER JOIN RawMaterial rm ON prml.rawMaterialID = rm.rawMaterialID
                           WHERE prml.productID = @id";
            return DatabaseConnect.ExecuteQuery(sql, new[] { new MySqlParameter("@id", productId) });
        }
    }
}
