using OrderRestaurant.Data;

namespace OrderRestaurant.Service
{
    public interface IOrderDetails
    {
        Task<OrderDetails> AddOrderDetails(OrderDetails orderDetails);

    }
}
