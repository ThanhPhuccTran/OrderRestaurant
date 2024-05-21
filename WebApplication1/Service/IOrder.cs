
using OrderRestaurant.Data;
using OrderRestaurant.DTO.CartDTO;
using OrderRestaurant.DTO.OrderDetailsDTO;
using OrderRestaurant.Helpers;
using OrderRestaurant.Model;

namespace OrderRestaurant.Service
{
    public interface IOrder
    {
        Task<List<OrderModel>> GetAllOrders();
        Task<List<OrderDetailModel>> GetOrderDetails(int orderId);
        Task<bool> CreateOrderAsync(CreateCartDTO cartDto);
        Task<bool> DeleteOrderAsync(int orderId);
        Task<bool> DeleteOrderDetailAsync(int orderId, int foodId);
        Task<bool> UpdateOrderDetailAsync(int orderId, int foodId, OrderDetailUpdateDto orderDetailDto);
        Task UpdateAsync(Order order);
        Task<List<Order>> FindOrdersByTable(int tableId, int code);
        Task<Order> FindOrderById(int orderid);

        Task<(int totalItems, int totalPages, List<OrderModel> items)> SearchAndPaginate(QuerryOrder querryOrder);

    }
}
