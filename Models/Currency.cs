namespace Sales_user.Models
{
    public class Currency
    {
        public long CurrencyID { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
        public decimal RateToBase { get; set; }
    }
}
