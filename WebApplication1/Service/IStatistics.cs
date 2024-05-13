using OrderRestaurant.DTO.FoodDTO;
using OrderRestaurant.DTO.OrderDetailsDTO;
using OrderRestaurant.Model;

namespace OrderRestaurant.Service
{
    public interface IStatistics
    {
        Task<decimal> GetRevenueByDay(DateTime date);
        Task<decimal> GetRevenueByMonth(int year, int month);
        Task<decimal> GetTotalRevenue();
        Task<Dictionary<string, decimal>> GetTotalRevenueByMonth(int currentYear);
        Task<List<FoodStatistic>> GetFoodStatistics();
        Task<OrderDetailsStatistics> GetMostPopularFood();
        Task<OrderDetailsStatistics> GetLeastPopularFood();
        Task<(decimal, OrderDetailsStatistics leastUsedFood, OrderDetailsStatistics popularUsedFood)> RevenueByDate(DateTime startDate, DateTime endDate);
    }
}
