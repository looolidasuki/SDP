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
    }
}
