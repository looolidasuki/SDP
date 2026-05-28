namespace Sales_user.Models
{
    public class RawMaterial
    {
        public long RawMaterialID { get; set; }
        public string RawMaterialCode { get; set; }
        public string Category { get; set; }
        public int? SequenceNumber { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public decimal MinimumStockLevel { get; set; }
        public int Status { get; set; }
    }
}
