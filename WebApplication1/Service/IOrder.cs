
using OrderRestaurant.Data;

namespace OrderRestaurant.Service
{
    public interface IOrder
    {
        Task<List<Order>> GetAll();
        Task UpdateAsync(Order order);
        Task<Order> GetAsync(int id);
    }
}
