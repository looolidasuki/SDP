using MySql.Data.MySqlClient;
using Sales_user.Models;
using System;
using System.Data;

namespace Sales_user.Controllers
{
    public class PaymentVoucherController
    {
        // 1. 查詢所有憑證 (對應資料庫欄位 totalAmount, paymentMethodRef)
        public DataTable GetAllPaymentVouchers()
        {
            string sql = @"SELECT paymentVoucherID AS 'ID',
                                  paymentVoucherCode AS 'Voucher Code',
                                  supplierID AS 'Supplier ID',
                                  staffID AS 'Staff ID',
                                  totalAmount AS 'Amount',
                                  paymentMethod AS 'Method',
                                  paymentMethodRef AS 'Reference',
                                  status AS 'Status',
                                  createDate AS 'Date'
                           FROM paymentvoucher
                           ORDER BY createDate DESC";
            return DatabaseConnect.ExecuteQuery(sql);
        }

        // 2. 獲取月度總計
        public DataTable GetMonthlyTotals()
        {
            string sql = @"SELECT DATE_FORMAT(createDate, '%Y-%m') AS Month,
                                  SUM(totalAmount) AS Total
                           FROM paymentvoucher
                           WHERE status != 3
                           GROUP BY DATE_FORMAT(createDate, '%Y-%m')
                           ORDER BY Month DESC
                           LIMIT 12";
            return DatabaseConnect.ExecuteQuery(sql);
        }

        // 3. 獲取付款方式佔比
        public DataTable GetMethodBreakdown()
        {
            string sql = @"SELECT paymentMethod AS Method,
                                  SUM(totalAmount) AS Total
                           FROM paymentvoucher
                           WHERE status != 3
                           GROUP BY paymentMethod";
            return DatabaseConnect.ExecuteQuery(sql);
        }

        // 4. 根據 ID 查詢 (解決第1、2、4個錯誤)
        public PaymentVoucher GetById(long id)
        {
            // ✨ 修改 SQL：移除 po.totalAmount，改用子查詢計算該採購單的總金額
            string sql = @"SELECT pv.paymentVoucherID, pv.paymentVoucherCode, pv.supplierID,
                          pv.staffID, pv.totalAmount, pv.paymentMethod, pv.paymentMethodRef, 
                          pv.remark, pv.status, pv.createDate,
                          pvpo.purchaseOrderID, pvpo.payAmount AS 'VoucherPayAmount',
                          po.purchaseOrderCode AS 'POCode',
                          (SELECT SUM(pol.price * pol.orderQuantity) 
                           FROM purchaseorderrawmaterialline pol 
                           WHERE pol.purchaseOrderID = po.purchaseOrderID) AS 'POTotalAmount'
                   FROM paymentvoucher pv
                   LEFT JOIN paymentvoucherpurchaseorder pvpo ON pv.paymentVoucherID = pvpo.paymentVoucherID
                   LEFT JOIN purchaseorder po ON pvpo.purchaseOrderID = po.purchaseOrderID
                   WHERE pv.paymentVoucherID = @id";

            var dt = DatabaseConnect.ExecuteQuery(sql, new[] { new MySqlParameter("@id", id) });
            if (dt == null || dt.Rows.Count == 0) return null;
            var row = dt.Rows[0];

            return new PaymentVoucher
            {
                PaymentVoucherID = Convert.ToInt64(row["paymentVoucherID"]),
                PaymentVoucherCode = row["paymentVoucherCode"]?.ToString(),
                SupplierID = Convert.ToInt64(row["supplierID"]),
                PurchaseOrderID = row["purchaseOrderID"] == DBNull.Value ? (long?)null : Convert.ToInt64(row["purchaseOrderID"]),
                StaffID = Convert.ToInt64(row["staffID"]),
                Amount = Convert.ToDecimal(row["totalAmount"]),
                PaymentMethod = row["paymentMethod"]?.ToString(),
                PaymentRef = row["paymentMethodRef"] == DBNull.Value ? null : row["paymentMethodRef"].ToString(),
                Remark = row["remark"] == DBNull.Value ? null : row["remark"].ToString(),
                Status = Convert.ToInt32(row["status"]),
                CreateDate = Convert.ToDateTime(row["createDate"]),

                // 這裡承接透過子查詢計算出來的 POTotalAmount
                PurchaseOrderCode = row["POCode"] == DBNull.Value ? "N/A" : row["POCode"].ToString(),
                PurchaseOrderTotalAmount = row["POTotalAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(row["POTotalAmount"]),
                VoucherPayAmount = row["VoucherPayAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(row["VoucherPayAmount"])
            };
        }

        // 5. 新增付款憑證 (解決第3個錯誤)
        public long Insert(PaymentVoucher pv)
        {
            // 使用你最原本內建的預設連接字串
            string connectionString = "Server=localhost;Port=3306;Database=furniture_erp_system;Uid=root;Pwd=;CharSet=utf8mb4;AllowPublicKeyRetrieval=True;SslMode=Disabled;";

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 步驟 A: 寫入主表
                        string sqlMain = @"INSERT INTO paymentvoucher
                            (paymentVoucherCode, supplierID, staffID, totalAmount, paymentMethod, paymentMethodRef, remark, status)
                            VALUES (@code, @supplierID, @staffID, @amount, @method, @ref, @remark, @status)";

                        long pvId = 0;
                        using (var cmd = new MySqlCommand(sqlMain, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@code", pv.PaymentVoucherCode ?? "");
                            cmd.Parameters.AddWithValue("@supplierID", pv.SupplierID);
                            cmd.Parameters.AddWithValue("@staffID", pv.StaffID);
                            cmd.Parameters.AddWithValue("@amount", pv.Amount);

                            // ✅ 修正錯誤 3：因為 pv.PaymentMethod 現在是 string，
                            // AddWithValue 會自動將其轉為 MySQL 辨識的字串，避免傳入錯誤的型態 (如 sbyte/int)
                            cmd.Parameters.AddWithValue("@method", pv.PaymentMethod ?? "");

                            cmd.Parameters.AddWithValue("@ref", pv.PaymentRef ?? "");
                            cmd.Parameters.AddWithValue("@remark", pv.Remark ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@status", pv.Status);

                            cmd.ExecuteNonQuery();
                            pvId = cmd.LastInsertedId;
                        }

                        if (pvId > 0)
                        {
                            // 步驟 B: 自動更新憑證編號
                            string sqlUpdateCode = "UPDATE paymentvoucher SET paymentVoucherCode = @code WHERE paymentVoucherID = @id";
                            using (var cmd = new MySqlCommand(sqlUpdateCode, conn, trans))
                            {
                                cmd.Parameters.AddWithValue("@code", "PV-" + pvId);
                                cmd.Parameters.AddWithValue("@id", pvId);
                                cmd.ExecuteNonQuery();
                            }

                            // 步驟 C: 寫入採購單關係表
                            if (pv.PurchaseOrderID.HasValue)
                            {
                                string sqlRelation = @"INSERT INTO paymentvoucherpurchaseorder 
                                    (paymentVoucherID, purchaseOrderID, type, payAmount) 
                                    VALUES (@pvId, @poId, @type, @payAmount)";

                                using (var cmd = new MySqlCommand(sqlRelation, conn, trans))
                                {
                                    cmd.Parameters.AddWithValue("@pvId", pvId);
                                    cmd.Parameters.AddWithValue("@poId", pv.PurchaseOrderID.Value);
                                    cmd.Parameters.AddWithValue("@type", 1);
                                    cmd.Parameters.AddWithValue("@payAmount", pv.Amount);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        trans.Commit();
                        return pvId;
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        return 0;
                    }
                }
            }
        }

        // 6. 更新資料
        public bool Update(PaymentVoucher pv)
        {
            string sql = @"UPDATE paymentvoucher
                           SET supplierID=@supplierID, staffID=@staffID,
                               totalAmount=@amount, paymentMethod=@method, paymentMethodRef=@ref,
                               remark=@remark, status=@status, lastModifyDate=NOW()
                           WHERE paymentVoucherID=@id";

            return DatabaseConnect.ExecuteNonQuery(sql, new[] {
                new MySqlParameter("@supplierID", pv.SupplierID),
                new MySqlParameter("@staffID", pv.StaffID),
                new MySqlParameter("@amount", pv.Amount),
                new MySqlParameter("@method", pv.PaymentMethod ?? ""),
                new MySqlParameter("@ref", pv.PaymentRef ?? ""),
                new MySqlParameter("@remark", pv.Remark ?? (object)DBNull.Value),
                new MySqlParameter("@status", pv.Status),
                new MySqlParameter("@id", pv.PaymentVoucherID)
            }) > 0;
        }
    }
}