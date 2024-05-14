using OrderRestaurant.DTO.FoodDTO;
using OrderRestaurant.DTO.InvoiceDTO;
using OrderRestaurant.DTO.OrderDetailsDTO;
using OrderRestaurant.Model;

namespace OrderRestaurant.Service
{
    public interface IStatistics
    {
        Task<decimal> GetRevenueByDay(DateTime date);
        Task<decimal> GetRevenueByMonth(int year, int month);
        Task<decimal> GetTotalRevenue();
        Task<Dictionary<string, decimal>> GetTotalRevenueByMonthRange(DateTime startDate, DateTime endDate);
        Task<Dictionary<string, decimal>> GetTotalRevenueByDateRange(DateTime startDate, DateTime endDate);
        Task<FoodStatisticsResult> GetFoodStatistics(DateTime startDate, DateTime endDate);
        Task<OrderDetailsStatistics> GetMostPopularFood(DateTime startDate, DateTime endDate);
        Task<OrderDetailsStatistics> GetLeastPopularFood(DateTime startDate, DateTime endDate);
        Task<(decimal, OrderDetailsStatistics, OrderDetailsStatistics, Dictionary<string, decimal>, Dictionary<string, int>, FoodStatisticsResult)> RevenueByDate(DateTime startDate, DateTime endDate);



    }
}
