using OrderRestaurant.Data;

namespace OrderRestaurant.DTO.OrderDTO
{
    public class CreateOrderDTO
    {
        public int TableId { get; set; } // ID của bàn mà đơn hàng được đặt
        public int StatusId { get; set; } 
        public List<OrderDetails> OrderDetails { get; set; } // Danh sách chi tiết đơn hàng
    }
}
