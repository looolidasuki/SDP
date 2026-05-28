using System;

namespace Sales_user.Models
{
    public class ProductionOrder
    {
        public long ProductionOrderID { get; set; }
        public string ProductionOrderCode { get; set; }
        public long SalesOrderID { get; set; }
        public long StaffID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime EstFinishDate { get; set; }
        public DateTime LastModifyDate { get; set; }
        public int Status { get; set; }
        public string Remark { get; set; }
    }
}
