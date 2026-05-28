using System;

namespace Sales_user.Models
{
    public class Product
    {
        public long ProductID { get; set; }
        public string ProductCode { get; set; }
        public string Category { get; set; }
        public int? SequenceNumber { get; set; }
        public string StyleNumber { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public decimal BasePriceByCurrency { get; set; }
        public long CurrencyID { get; set; }
        public long StaffID { get; set; }
        public string Unit { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? LastModifyDate { get; set; }
        public int Status { get; set; }
        public string Remark { get; set; }
        public byte[] ProductImage { get; set; }
    }
}
