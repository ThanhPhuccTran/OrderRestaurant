using OrderRestaurant.DTO.FoodDTO;
using OrderRestaurant.DTO.OrderDetailsDTO;
using OrderRestaurant.Model;

namespace OrderRestaurant.Service
{
    public interface IStatistics
    {
        Task<decimal> GetRevenueByDay(int date);
        Task<decimal> GetRevenueByMonth(int year, int month);
        Task<OrderDetailsStatistics> GetMostPopularFood();
    }
}
