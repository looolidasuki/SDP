using MySql.Data.MySqlClient;
using Sales_user.Models;
using System.Data;

namespace Sales_user.Controllers
{
    public class PurchaseOrderController
    {
        public DataTable GetAllPurchaseOrders()
        {
            string sql = @"SELECT po.purchaseOrderID AS 'Purchase Order ID',
                                  po.purchaseOrderCode AS 'Purchase Order Code',
                                  s.supplierName AS 'Supplier',
                                  po.createDate AS 'Create Date',
                                  po.requestDeliveryDate AS 'Request Delivery Date',
                                  po.status AS 'Status'
                           FROM PurchaseOrder po
                           LEFT JOIN Supplier s ON po.supplierID = s.supplierID
                           ORDER BY po.createDate DESC";
            return DatabaseConnect.ExecuteQuery(sql);
        }

        public DataTable GetAllPurchaseOrderLines()
        {
            string sql = @"SELECT po.purchaseOrderCode AS 'Purchase Order',
                                  rm.rawMaterialCode AS 'Raw Material',
                                  pol.price AS 'Price',
                                  pol.orderQuantity AS 'Order Qty',
                                  pol.receivedQuantity AS 'Received Qty'
                           FROM PurchaseOrderRawMaterialLine pol
                           INNER JOIN PurchaseOrder po ON pol.purchaseOrderID = po.purchaseOrderID
                           INNER JOIN RawMaterial rm ON pol.rawMaterialID = rm.rawMaterialID
                           ORDER BY po.createDate DESC";
            return DatabaseConnect.ExecuteQuery(sql);
        }

        public long Insert(PurchaseOrder order)
        {
            string sql = @"INSERT INTO PurchaseOrder
                (purchaseOrderCode, supplierID, staffID, relatedShortageReport,
                 requestDeliveryDate, status, remark)
                VALUES (@code, @supplierID, @staffID, @shortageReport,
                        @requestDeliveryDate, @status, @remark)";
            return DatabaseConnect.ExecuteInsertReturnId(sql, new[] {
                new MySqlParameter("@code", order.PurchaseOrderCode),
                new MySqlParameter("@supplierID", order.SupplierID),
                new MySqlParameter("@staffID", order.StaffID),
                new MySqlParameter("@shortageReport", order.RelatedShortageReport ?? (object)System.DBNull.Value),
                new MySqlParameter("@requestDeliveryDate", order.RequestDeliveryDate),
                new MySqlParameter("@status", order.Status),
                new MySqlParameter("@remark", order.Remark ?? (object)System.DBNull.Value)
            });
        }

        public void UpdateCodeAfterInsert(long id)
        {
            DatabaseConnect.ExecuteNonQuery(
                "UPDATE PurchaseOrder SET purchaseOrderCode = @code WHERE purchaseOrderID = @id",
                new[] {
                    new MySqlParameter("@code", "PO-" + id),
                    new MySqlParameter("@id", id)
                });
        }

        public bool InsertLine(long purchaseOrderId, long rawMaterialId, decimal price, decimal orderQty)
        {
            string sql = @"INSERT INTO PurchaseOrderRawMaterialLine
                (purchaseOrderID, rawMaterialID, price, orderQuantity)
                VALUES (@poID, @rmID, @price, @qty)";
            return DatabaseConnect.ExecuteNonQuery(sql, new[] {
                new MySqlParameter("@poID", purchaseOrderId),
                new MySqlParameter("@rmID", rawMaterialId),
                new MySqlParameter("@price", price),
                new MySqlParameter("@qty", orderQty)
            }) > 0;
        }
    }
}
