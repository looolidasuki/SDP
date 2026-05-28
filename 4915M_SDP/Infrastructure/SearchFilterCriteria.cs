using System;

namespace Sales_user.Controllers
{
    public class SearchFilterCriteria
    {
        public string Keyword { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public long? CustomerID { get; set; }
        public long? SupplierID { get; set; }
        public long? StaffID { get; set; }
        public int? StatusInt { get; set; }
    }
}
