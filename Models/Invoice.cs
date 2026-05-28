using System;

namespace Sales_user.Models
{
    public class Invoice
    {
        public long InvoiceID { get; set; }
        public string InvoiceCode { get; set; }
        public long CustomerID { get; set; }
        public long SalesOrderID { get; set; }
        public long StaffID { get; set; }
        public int InvoiceType { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? LastModifyDate { get; set; }
        public string Remark { get; set; }
        public int Status { get; set; }
    }
}
