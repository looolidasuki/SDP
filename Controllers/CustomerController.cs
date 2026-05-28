using MySql.Data.MySqlClient;
using Sales_user.Models;
using System.Collections.Generic;
using System.Data;

namespace Sales_user.Controllers
{
    public class CustomerController
    {
        // 在 CustomerController.cs 中加入

        public DataTable GetAllCustomers()
        {
            string sql = @"SELECT customerID AS 'Customer ID',
                                  customerName AS 'Customer Name',
                                  billingAddress AS 'Billing Address',
                                  paymentTerm AS 'Payment Term',
                                  createDate AS 'Create Date',
                                  lastModifyDate AS 'Last Modify Date'
                           FROM Customer
                           ORDER BY createDate DESC";
            return DatabaseConnect.ExecuteQuery(sql);
        }

        public Customer GetById(long customerId)
        {
            string sql = @"SELECT customerID, customerName, billingAddress, paymentTerm
                           FROM Customer WHERE customerID = @id";
            DataTable dt = DatabaseConnect.ExecuteQuery(sql, new[] {
                new MySqlParameter("@id", customerId)
            });
            if (dt == null || dt.Rows.Count == 0) return null;
            var row = dt.Rows[0];
            return new Customer
            {
                CustomerID = System.Convert.ToInt64(row["customerID"]),
                CustomerName = row["customerName"].ToString(),
                BillingAddress = row["billingAddress"].ToString(),
                PaymentTerm = row["paymentTerm"].ToString()
            };
        }

        public long Insert(Customer customer)
        {
            string sql = @"INSERT INTO Customer (customerName, billingAddress, paymentTerm)
                           VALUES (@name, @address, @term)";
            return DatabaseConnect.ExecuteInsertReturnId(sql, new[] {
                new MySqlParameter("@name", customer.CustomerName ?? ""),
                new MySqlParameter("@address", customer.BillingAddress ?? (object)System.DBNull.Value),
                new MySqlParameter("@term", customer.PaymentTerm ?? (object)System.DBNull.Value)
            });
        }

        public bool Update(Customer customer)
        {
            string sql = @"UPDATE Customer SET customerName = @name, billingAddress = @address,
                           paymentTerm = @term, lastModifyDate = NOW() WHERE customerID = @id";
            return DatabaseConnect.ExecuteNonQuery(sql, new[] {
                new MySqlParameter("@name", customer.CustomerName ?? ""),
                new MySqlParameter("@address", customer.BillingAddress ?? (object)System.DBNull.Value),
                new MySqlParameter("@term", customer.PaymentTerm ?? (object)System.DBNull.Value),
                new MySqlParameter("@id", customer.CustomerID)
            }) > 0;
        }

        public DataTable GetSalesOrdersByCustomer(long customerId)
        {
            string sql = @"SELECT salesOrderCode AS 'Order Code', deliveryAddress AS 'Delivery Address',
                                  createDate AS 'Create Date', status AS 'Status'
                           FROM SalesOrder WHERE customerID = @id ORDER BY createDate DESC";
            return DatabaseConnect.ExecuteQuery(sql, new[] { new MySqlParameter("@id", customerId) });
        }

        public DataTable GetQuotationsByCustomer(long customerId)
        {
            string sql = @"SELECT quotationCode AS 'Quotation Code', createDate AS 'Create Date', status AS 'Status'
                           FROM Quotation WHERE customerID = @id ORDER BY createDate DESC";
            return DatabaseConnect.ExecuteQuery(sql, new[] { new MySqlParameter("@id", customerId) });
        }

        public DataTable Search(SearchFilterCriteria filter)
        {
            string sql = @"SELECT customerID AS 'Customer ID',
                                  customerName AS 'Customer Name',
                                  billingAddress AS 'Billing Address',
                                  paymentTerm AS 'Payment Term',
                                  createDate AS 'Create Date',
                                  lastModifyDate AS 'Last Modify Date'
                           FROM Customer WHERE 1=1";
            var conditions = new List<string>();
            var parameters = new List<MySqlParameter>();
            SearchQueryHelper.AddLike(conditions, parameters, "customerName", filter.Name ?? filter.Keyword, "@name");
            SearchQueryHelper.AddLike(conditions, parameters, "billingAddress", filter.Keyword, "@addr");
            SearchQueryHelper.AddDateFrom(conditions, parameters, "createDate", filter.FromDate);
            SearchQueryHelper.AddDateTo(conditions, parameters, "createDate", filter.ToDate);
            if (conditions.Count > 0) sql += " AND " + string.Join(" AND ", conditions);
            sql += " ORDER BY createDate DESC";
            return DatabaseConnect.ExecuteQuery(sql, parameters.ToArray());
        }

        public int GetCount()
        {
            string sql = "SELECT COUNT(*) FROM Customer";
            DataTable dt = DatabaseConnect.ExecuteQuery(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                return System.Convert.ToInt32(dt.Rows[0][0]);
            }
            return 0;
        }
    }
}
