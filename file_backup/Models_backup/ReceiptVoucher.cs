using System;

namespace Sales_user.Models
{
    public class ReceiptVoucher
    {
        public long ReceiptVoucherID { get; set; }
        public string ReceiptVoucherCode { get; set; }
        public long? InvoiceID { get; set; }
        public long? CustomerID { get; set; }
        public long StaffID { get; set; }
        public decimal Amount { get; set; }
        public int PaymentMethod { get; set; }  // 0=Cash, 1=Bank Transfer, 2=Credit Card, 3=Cheque
        public string PaymentRef { get; set; }
        public string Remark { get; set; }
        public int Status { get; set; }         // 0=Draft, 1=Confirmed, 2=Cancelled
        public DateTime CreateDate { get; set; }
        public DateTime? LastModifyDate { get; set; }
    }
}
