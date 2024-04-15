using OrderRestaurant.DTO.OrderDetailsDTO;

namespace OrderRestaurant.DTO.OrderDTO
{
    public class OrderUpdateModel
    {
        public int CustomerId { get; set; }
        public int TableId { get; set; }
        public int? EmployeeId { get; set; }
        public string Note { get; set; }
        public List<OrderDetailUpdateModel> OrderDetails { get; set; }
    }
}
