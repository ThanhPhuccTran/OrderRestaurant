namespace OrderRestaurant.DTO.OrderDetailsDTO
{
    public class OrderDetailUpdateModel
    {
        public int FoodId { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public string Note { get; set; }
    }
}
