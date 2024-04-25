
using OrderRestaurant.Data;

namespace OrderRestaurant.Service
{
    public interface IOrder
    {
        Task<List<Order>> GetAll();
        Task UpdateAsync(Order order);

        Task<List<Order>> GetSearchType(string type ="Order");

        Task<Order> FindOrder(int orderId);
    }
}
