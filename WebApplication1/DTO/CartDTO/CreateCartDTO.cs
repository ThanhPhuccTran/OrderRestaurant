namespace OrderRestaurant.DTO.CartDTO
{
    public class CreateCartDTO
    {
        public int TableId { get; set; }
        public List<CartList> Items { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
