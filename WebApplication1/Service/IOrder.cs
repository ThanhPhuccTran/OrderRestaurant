
using OrderRestaurant.Data;

namespace OrderRestaurant.Service
{
    public interface IOrder
    {
        Task<List<Order>> GetAll();
        Task UpdateAsync(Order order);

        Task<List<Order>> GetSearchType(string type ="Order");

        Task<List<Order>> FindOrdersByTable(int tableId, int code);
        Task<Order> FindOrderById(int orderid);
    }
}
