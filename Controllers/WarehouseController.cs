using System;
using MySql.Data.MySqlClient;
using Sales_user.Models;
using System.Data;

namespace Sales_user.Controllers
{
    public class WarehouseController
    {
        public DataTable GetAllWarehouses()
        {
            string sql = @"SELECT warehouseID AS 'Warehouse ID',
                                  warehouseName AS 'Warehouse Name',
                                  warehouseAddress AS 'Address'
                           FROM Warehouse
                           ORDER BY warehouseName";
            return DatabaseConnect.ExecuteQuery(sql);
        }

        public long Insert(Warehouse warehouse)
        {
            string sql = @"INSERT INTO Warehouse (warehouseName, warehouseAddress)
                           VALUES (@name, @address)";
            return DatabaseConnect.ExecuteInsertReturnId(sql, new[] {
                new MySqlParameter("@name", warehouse.WarehouseName),
                new MySqlParameter("@address", warehouse.WarehouseAddress)
            });
        }

        public Warehouse GetById(long id)
        {
            var dt = DatabaseConnect.ExecuteQuery(
                "SELECT warehouseID, warehouseName, warehouseAddress FROM Warehouse WHERE warehouseID = @id",
                new[] { new MySqlParameter("@id", id) });
            if (dt == null || dt.Rows.Count == 0) return null;
            var row = dt.Rows[0];
            return new Warehouse
            {
                WarehouseID = Convert.ToInt64(row["warehouseID"]),
                WarehouseName = row["warehouseName"]?.ToString(),
                WarehouseAddress = row["warehouseAddress"]?.ToString()
            };
        }

        public void Update(Warehouse warehouse)
        {
            DatabaseConnect.ExecuteNonQuery(
                "UPDATE Warehouse SET warehouseName = @name, warehouseAddress = @address WHERE warehouseID = @id",
                new[] {
                    new MySqlParameter("@name", warehouse.WarehouseName),
                    new MySqlParameter("@address", warehouse.WarehouseAddress ?? (object)System.DBNull.Value),
                    new MySqlParameter("@id", warehouse.WarehouseID)
                });
        }

        public DataTable GetWarehouseProducts(long warehouseId)
        {
            string sql = @"SELECT p.productCode AS 'Product Code', wp.physicalQuantity AS 'Physical Qty',
                                  wp.reservedQuantity AS 'Reserved', wp.purchasedQuantity AS 'Purchased'
                           FROM WarehouseProduct wp
                           INNER JOIN Product p ON wp.productID = p.productID
                           WHERE wp.warehouseID = @id";
            return DatabaseConnect.ExecuteQuery(sql, new[] { new MySqlParameter("@id", warehouseId) });
        }
    }
}
