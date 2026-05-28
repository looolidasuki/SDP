namespace Sales_user.Models
{
    public class Supplier
    {
        public long SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string BillingAddress { get; set; }
        public string ContactPerson { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string PaymentTerm { get; set; }
        public string BankAccount { get; set; }
        public int Status { get; set; }
    }
}
