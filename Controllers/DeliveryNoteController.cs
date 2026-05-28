using MySql.Data.MySqlClient;
using Sales_user.Models;
using System.Data;

namespace Sales_user.Controllers
{
    public class DeliveryNoteController
    {
        public DataTable GetAllDeliveryNotes()
        {
            string sql = @"SELECT deliveryNoteID AS 'Delivery Note ID',
                                  deliveryNoteCode AS 'Delivery Note Code',
                                  customerID AS 'Customer ID',
                                  SalesOrderID AS 'Sales Order ID',
                                  trackingNumber AS 'Tracking Number',
                                  createDate AS 'Create Date',
                                  status AS 'Status'
                           FROM DeliveryNote
                           ORDER BY createDate DESC";
            return DatabaseConnect.ExecuteQuery(sql);
        }

        public long Insert(DeliveryNote note)
        {
            string sql = @"INSERT INTO DeliveryNote
                (deliveryNoteCode, customerID, SalesOrderID, staffID, WarehouseID,
                 shipMethod, trackingNumber, status, remark)
                VALUES (@code, @customerID, @soID, @staffID, @whID,
                        @shipMethod, @tracking, @status, @remark)";
            return DatabaseConnect.ExecuteInsertReturnId(sql, new[] {
                new MySqlParameter("@code", note.DeliveryNoteCode),
                new MySqlParameter("@customerID", note.CustomerID),
                new MySqlParameter("@soID", note.SalesOrderID),
                new MySqlParameter("@staffID", note.StaffID),
                new MySqlParameter("@whID", note.WarehouseID),
                new MySqlParameter("@shipMethod", note.ShipMethod),
                new MySqlParameter("@tracking", note.TrackingNumber),
                new MySqlParameter("@status", note.Status),
                new MySqlParameter("@remark", note.Remark ?? (object)System.DBNull.Value)
            });
        }

        public void UpdateCodeAfterInsert(long id)
        {
            DatabaseConnect.ExecuteNonQuery(
                "UPDATE DeliveryNote SET deliveryNoteCode = @code WHERE deliveryNoteID = @id",
                new[] {
                    new MySqlParameter("@code", "DN-" + id),
                    new MySqlParameter("@id", id)
                });
        }

        public DataTable GetDeliveryLines(long deliveryNoteId)
        {
            string sql = @"SELECT p.productCode AS 'Product', dpl.shipQuantity AS 'Ship Qty'
                           FROM DeliveryProductLine dpl
                           INNER JOIN Product p ON dpl.productID = p.productID
                           WHERE dpl.deliveryNoteID = @id";
            return DatabaseConnect.ExecuteQuery(sql, new[] { new MySqlParameter("@id", deliveryNoteId) });
        }
    }
}
