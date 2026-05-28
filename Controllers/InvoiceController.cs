using MySql.Data.MySqlClient;
using Sales_user.Models;
using System.Data;

namespace Sales_user.Controllers
{
    public class InvoiceController
    {
        public DataTable GetAllInvoices()
        {
            string sql = @"SELECT invoiceID AS 'Invoice ID',
                                  invoiceCode AS 'Invoice Code',
                                  customerID AS 'Customer ID',
                                  salesOrderID AS 'Sales Order ID',
                                  invoiceType AS 'Invoice Type',
                                  createDate AS 'Create Date',
                                  status AS 'Status'
                           FROM Invoice
                           ORDER BY createDate DESC";
            return DatabaseConnect.ExecuteQuery(sql);
        }

        public long Insert(Invoice invoice)
        {
            string sql = @"INSERT INTO Invoice
                (invoiceCode, customerID, salesOrderID, staffID, invoiceType, status, remark)
                VALUES (@code, @customerID, @soID, @staffID, @type, @status, @remark)";
            return DatabaseConnect.ExecuteInsertReturnId(sql, new[] {
                new MySqlParameter("@code", invoice.InvoiceCode),
                new MySqlParameter("@customerID", invoice.CustomerID),
                new MySqlParameter("@soID", invoice.SalesOrderID),
                new MySqlParameter("@staffID", invoice.StaffID),
                new MySqlParameter("@type", invoice.InvoiceType),
                new MySqlParameter("@status", invoice.Status),
                new MySqlParameter("@remark", invoice.Remark ?? (object)System.DBNull.Value)
            });
        }

        public void UpdateCodeAfterInsert(long invoiceId)
        {
            DatabaseConnect.ExecuteNonQuery(
                "UPDATE Invoice SET invoiceCode = @code WHERE invoiceID = @id",
                new[] {
                    new MySqlParameter("@code", "INV-" + invoiceId),
                    new MySqlParameter("@id", invoiceId)
                });
        }

        public DataTable GetInvoiceLines(long invoiceId)
        {
            string sql = @"SELECT il.deliveryNoteID AS 'Delivery Note ID', p.productCode AS 'Product',
                                  il.invoiceQuantity AS 'Qty', il.amount AS 'Amount'
                           FROM InvoiceLine il
                           INNER JOIN Product p ON il.productID = p.productID
                           WHERE il.invoiceID = @id";
            return DatabaseConnect.ExecuteQuery(sql, new[] { new MySqlParameter("@id", invoiceId) });
        }

        public int GetCount()
        {
            string sql = "SELECT COUNT(*) FROM Invoice";
            DataTable dt = DatabaseConnect.ExecuteQuery(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                return System.Convert.ToInt32(dt.Rows[0][0]);
            }
            return 0;
        }
    }
}
