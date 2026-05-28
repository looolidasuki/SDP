using System;

namespace Sales_user.Models
{
    public class GoodsReceivedNote
    {
        public long GoodsReceivedNoteID { get; set; }
        public string GoodsReceivedNoteCode { get; set; }
        public long SupplierID { get; set; }
        public long PurchaseOrderID { get; set; }
        public long StaffID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastModifyDate { get; set; }
        public int Status { get; set; }
        public string Remark { get; set; }
    }
}
