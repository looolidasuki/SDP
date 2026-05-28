using MySql.Data.MySqlClient;
using Sales_user.Models;
using System.Data;

namespace Sales_user.Controllers
{
    public class ReceiptVoucherController
    {
        public DataTable GetAllReceiptVouchers()
        {
            string sql = @"SELECT receiptVoucherID AS 'ID',
                                  receiptVoucherCode AS 'Voucher Code',
                                  invoiceID AS 'Invoice ID',
                                  customerID AS 'Customer ID',
                                  staffID AS 'Staff ID',
                                  amount AS 'Amount',
                                  paymentMethod AS 'Method',
                                  paymentRef AS 'Reference',
                                  status AS 'Status',
                                  createDate AS 'Date'
                           FROM ReceiptVoucher
                           ORDER BY createDate DESC";
            return DatabaseConnect.ExecuteQuery(sql);
        }

        public DataTable GetMonthlyTotals()
        {
            string sql = @"SELECT DATE_FORMAT(createDate, '%Y-%m') AS Month,
                                  SUM(amount) AS Total
                           FROM ReceiptVoucher
                           WHERE status != 2
                           GROUP BY DATE_FORMAT(createDate, '%Y-%m')
                           ORDER BY Month DESC
                           LIMIT 12";
            return DatabaseConnect.ExecuteQuery(sql);
        }

        public DataTable GetMethodBreakdown()
        {
            string sql = @"SELECT paymentMethod AS Method,
                                  SUM(amount) AS Total
                           FROM ReceiptVoucher
                           WHERE status != 2
                           GROUP BY paymentMethod";
            return DatabaseConnect.ExecuteQuery(sql);
        }

        public ReceiptVoucher GetById(long id)
        {
            string sql = @"SELECT receiptVoucherID, receiptVoucherCode, invoiceID, customerID,
                                  staffID, amount, paymentMethod, paymentRef, remark, status, createDate
                           FROM ReceiptVoucher WHERE receiptVoucherID = @id";
            var dt = DatabaseConnect.ExecuteQuery(sql, new[] { new MySqlParameter("@id", id) });
            if (dt == null || dt.Rows.Count == 0) return null;
            var row = dt.Rows[0];
            return new ReceiptVoucher
            {
                ReceiptVoucherID = System.Convert.ToInt64(row["receiptVoucherID"]),
                ReceiptVoucherCode = row["receiptVoucherCode"]?.ToString(),
                InvoiceID = row["invoiceID"] == System.DBNull.Value ? (long?)null : System.Convert.ToInt64(row["invoiceID"]),
                CustomerID = row["customerID"] == System.DBNull.Value ? (long?)null : System.Convert.ToInt64(row["customerID"]),
                StaffID = System.Convert.ToInt64(row["staffID"]),
                Amount = System.Convert.ToDecimal(row["amount"]),
                PaymentMethod = System.Convert.ToInt32(row["paymentMethod"]),
                PaymentRef = row["paymentRef"] == System.DBNull.Value ? null : row["paymentRef"].ToString(),
                Remark = row["remark"] == System.DBNull.Value ? null : row["remark"].ToString(),
                Status = System.Convert.ToInt32(row["status"]),
                CreateDate = System.Convert.ToDateTime(row["createDate"])
            };
        }

        public long Insert(ReceiptVoucher rv)
        {
            string sql = @"INSERT INTO ReceiptVoucher
                (receiptVoucherCode, invoiceID, customerID, staffID, amount, paymentMethod, paymentRef, remark, status)
                VALUES (@code, @invoiceID, @customerID, @staffID, @amount, @method, @ref, @remark, @status)";
            long id = DatabaseConnect.ExecuteInsertReturnId(sql, new[] {
                new MySqlParameter("@code", rv.ReceiptVoucherCode),
                new MySqlParameter("@invoiceID", rv.InvoiceID ?? (object)System.DBNull.Value),
                new MySqlParameter("@customerID", rv.CustomerID ?? (object)System.DBNull.Value),
                new MySqlParameter("@staffID", rv.StaffID),
                new MySqlParameter("@amount", rv.Amount),
                new MySqlParameter("@method", rv.PaymentMethod),
                new MySqlParameter("@ref", rv.PaymentRef ?? (object)System.DBNull.Value),
                new MySqlParameter("@remark", rv.Remark ?? (object)System.DBNull.Value),
                new MySqlParameter("@status", rv.Status)
            });
            if (id > 0)
                DatabaseConnect.ExecuteNonQuery(
                    "UPDATE ReceiptVoucher SET receiptVoucherCode = @code WHERE receiptVoucherID = @id",
                    new[] { new MySqlParameter("@code", "RV-" + id), new MySqlParameter("@id", id) });
            return id;
        }

        public bool Update(ReceiptVoucher rv)
        {
            string sql = @"UPDATE ReceiptVoucher
                           SET invoiceID=@invoiceID, customerID=@customerID, staffID=@staffID,
                               amount=@amount, paymentMethod=@method, paymentRef=@ref,
                               remark=@remark, status=@status, lastModifyDate=NOW()
                           WHERE receiptVoucherID=@id";
            return DatabaseConnect.ExecuteNonQuery(sql, new[] {
                new MySqlParameter("@invoiceID", rv.InvoiceID ?? (object)System.DBNull.Value),
                new MySqlParameter("@customerID", rv.CustomerID ?? (object)System.DBNull.Value),
                new MySqlParameter("@staffID", rv.StaffID),
                new MySqlParameter("@amount", rv.Amount),
                new MySqlParameter("@method", rv.PaymentMethod),
                new MySqlParameter("@ref", rv.PaymentRef ?? (object)System.DBNull.Value),
                new MySqlParameter("@remark", rv.Remark ?? (object)System.DBNull.Value),
                new MySqlParameter("@status", rv.Status),
                new MySqlParameter("@id", rv.ReceiptVoucherID)
            }) > 0;
        }
    }
}
