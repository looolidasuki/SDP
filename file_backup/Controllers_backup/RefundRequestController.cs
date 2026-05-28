using MySql.Data.MySqlClient;
using Sales_user.Models;
using System.Data;

namespace Sales_user.Controllers
{
    public class RefundRequestController
    {
        public DataTable GetAllRefundRequests()
        {
            string sql = @"SELECT refundRequestCode AS 'Request Code',
                                  ReceiptVoucherID AS 'Receipt Voucher ID',
                                  InvoiceID AS 'Invoice ID',
                                  createDate AS 'Request Date',
                                  refundAmount AS 'Amount',
                                  refundMethod AS 'Refund Method',
                                  refundReason AS 'Reason',
                                  status AS 'Status'
                           FROM RefundRequest
                           ORDER BY createDate DESC";
            return DatabaseConnect.ExecuteQuery(sql);
        }

        public bool CreateRefundRequest(RefundRequest refund)
        {
            string sql = @"INSERT INTO RefundRequest
                (refundRequestCode, staffID, ReceiptVoucherID, InvoiceID,
                 refundAmount, refundMethod, refundRef, refundReason, status, remark)
                VALUES (@code, @staffID, @receiptID, @invoiceID,
                        @amount, @method, @ref, @reason, @status, @remark)";

            long id = DatabaseConnect.ExecuteInsertReturnId(sql, new[] {
                new MySqlParameter("@code", refund.RefundRequestCode),
                new MySqlParameter("@staffID", refund.StaffID),
                new MySqlParameter("@receiptID", refund.ReceiptVoucherID ?? (object)System.DBNull.Value),
                new MySqlParameter("@invoiceID", refund.InvoiceID ?? (object)System.DBNull.Value),
                new MySqlParameter("@amount", refund.RefundAmount),
                new MySqlParameter("@method", refund.RefundMethod),
                new MySqlParameter("@ref", refund.RefundRef ?? (object)System.DBNull.Value),
                new MySqlParameter("@reason", refund.RefundReason),
                new MySqlParameter("@status", refund.Status),
                new MySqlParameter("@remark", refund.Remark ?? (object)System.DBNull.Value)
            });
            if (id > 0)
            {
                DatabaseConnect.ExecuteNonQuery(
                    "UPDATE RefundRequest SET refundRequestCode = @code WHERE refundRequestID = @id",
                    new[] {
                        new MySqlParameter("@code", "RF-" + id),
                        new MySqlParameter("@id", id)
                    });
            }
            return id > 0;
        }

        public bool UpdateStatus(string requestCode, int newStatus, long staffID)
        {
            string sql = @"UPDATE RefundRequest
                           SET status = @status, staffID = @staffID, lastModifyDate = NOW()
                           WHERE refundRequestCode = @code";

            MySqlParameter[] parameters = {
                new MySqlParameter("@status", newStatus),
                new MySqlParameter("@staffID", staffID),
                new MySqlParameter("@code", requestCode)
            };

            return DatabaseConnect.ExecuteNonQuery(sql, parameters) > 0;
        }

        public RefundRequest GetByCode(string requestCode)
        {
            string sql = @"SELECT refundRequestID, refundRequestCode, staffID, receiptVoucherID, invoiceID,
                                  refundAmount, refundMethod, refundRef, refundReason, status, remark
                           FROM RefundRequest
                           WHERE refundRequestCode = @code";
            DataTable dt = DatabaseConnect.ExecuteQuery(sql, new[] { new MySqlParameter("@code", requestCode) });
            if (dt == null || dt.Rows.Count == 0) return null;
            var row = dt.Rows[0];
            return new RefundRequest
            {
                RefundRequestID = System.Convert.ToInt64(row["refundRequestID"]),
                RefundRequestCode = row["refundRequestCode"]?.ToString(),
                StaffID = System.Convert.ToInt64(row["staffID"]),
                ReceiptVoucherID = row["receiptVoucherID"] == System.DBNull.Value ? (long?)null : System.Convert.ToInt64(row["receiptVoucherID"]),
                InvoiceID = row["invoiceID"] == System.DBNull.Value ? (long?)null : System.Convert.ToInt64(row["invoiceID"]),
                RefundAmount = System.Convert.ToDecimal(row["refundAmount"]),
                RefundMethod = System.Convert.ToInt32(row["refundMethod"]),
                RefundRef = row["refundRef"] == System.DBNull.Value ? null : row["refundRef"].ToString(),
                RefundReason = row["refundReason"]?.ToString(),
                Status = System.Convert.ToInt32(row["status"]),
                Remark = row["remark"] == System.DBNull.Value ? null : row["remark"].ToString()
            };
        }

        public bool Update(RefundRequest refund)
        {
            string sql = @"UPDATE RefundRequest
                           SET staffID=@staffID, receiptVoucherID=@receiptID, invoiceID=@invoiceID,
                               refundAmount=@amount, refundMethod=@method, refundRef=@ref, refundReason=@reason,
                               status=@status, remark=@remark, lastModifyDate=NOW()
                           WHERE refundRequestID=@id";
            return DatabaseConnect.ExecuteNonQuery(sql, new[] {
                new MySqlParameter("@staffID", refund.StaffID),
                new MySqlParameter("@receiptID", refund.ReceiptVoucherID ?? (object)System.DBNull.Value),
                new MySqlParameter("@invoiceID", refund.InvoiceID ?? (object)System.DBNull.Value),
                new MySqlParameter("@amount", refund.RefundAmount),
                new MySqlParameter("@method", refund.RefundMethod),
                new MySqlParameter("@ref", refund.RefundRef ?? (object)System.DBNull.Value),
                new MySqlParameter("@reason", refund.RefundReason ?? ""),
                new MySqlParameter("@status", refund.Status),
                new MySqlParameter("@remark", refund.Remark ?? (object)System.DBNull.Value),
                new MySqlParameter("@id", refund.RefundRequestID)
            }) > 0;
        }
    }
}
