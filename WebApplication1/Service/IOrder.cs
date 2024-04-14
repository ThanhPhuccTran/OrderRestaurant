namespace OrderRestaurant.Service
{
    public interface IOrder
    {
        Task AddFoodToOrder( int foodId, int quantity);
    }
}
