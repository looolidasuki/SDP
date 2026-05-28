using System;

namespace Sales_user.Models
{
    public class RefundRequest
    {
        public long RefundRequestID { get; set; }
        public string RefundRequestCode { get; set; }
        public long StaffID { get; set; }
        public DateTime CreateDate { get; set; }
        public long? ReceiptVoucherID { get; set; }
        public long? InvoiceID { get; set; }
        public decimal RefundAmount { get; set; }
        public int RefundMethod { get; set; }
        public string RefundRef { get; set; }
        public string RefundReason { get; set; }
        public int Status { get; set; }
        public DateTime LastModifyDate { get; set; }
        public string Remark { get; set; }
    }
}
