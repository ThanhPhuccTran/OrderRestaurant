namespace OrderRestaurant.DTO.InvoiceDTO
{
    public class FoodInvoiceDTO
    {
        public int FoodId { get; set; }
        public string? NameFood { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? UrlImage { get; set; }
        public int Quantity { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}
