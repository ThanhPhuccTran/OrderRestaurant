namespace OrderRestaurant.DTO.CartDTO
{
    public class CartList
    {
        public int TableId { get; set; }
        public int FoodId { get; set; }
        public int StatusId { get; set; }
        public int Quantity { get; set; }
        public int? EmployeeId { get; set; }
    }
}
