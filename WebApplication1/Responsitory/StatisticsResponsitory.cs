using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.FoodDTO;
using OrderRestaurant.DTO.OrderDetailsDTO;
using OrderRestaurant.Model;
using OrderRestaurant.Service;

namespace OrderRestaurant.Responsitory
{
    public class StatisticsResponsitory : IStatistics
    {
        private readonly ApplicationDBContext _context;
        public StatisticsResponsitory(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetRevenueByDay(int date)
        {
            var total = await _context.Orders
                       .Where(o => o.Code == Constants.ORDER_PAYMENT &&
                               o.PaymentTime != null &&
                               o.PaymentTime.Value.Day == date)
                       .SumAsync(o => o.Pay);
            return (decimal)total;
        }

        public async Task<decimal> GetRevenueByMonth(int year, int month)
        {
            if (month > 12 || month < 0)
            {
                throw new ArgumentException("Số tháng không hợp lệ");
            }
            var total = await _context.Orders
                                       .Where(o => o.Code == Constants.ORDER_PAYMENT && o.PaymentTime.Value.Year == year && o.PaymentTime.Value.Month == month)
                                       .SumAsync(o => o.Pay);
            return (decimal)total;
        }

      

        // Thống kê Món ăn được yêu thích nhất
        public async Task<OrderDetailsStatistics> GetMostPopularFood()
        {
            var mostPreferredFood = await _context.OrderDetails
                                        .GroupBy(od => od.FoodId)
                                        .OrderByDescending(g => g.Count())
                                        .Select(g => new OrderDetailsStatistics
                                        {
                                            FoodId = g.Key,
                                            UsageCount = g.Count()
                                        })
                                        .FirstOrDefaultAsync();

            if (mostPreferredFood != null)
            {
                mostPreferredFood.Foods = await _context.Foods
                    .Where(f => f.FoodId == mostPreferredFood.FoodId)
                    .Select(f => new FoodsDTO
                    {
                        FoodId = f.FoodId,
                        NameFood = f.NameFood,
                        CategoryId = f.CategoryId,
                        UnitPrice = f.UnitPrice,
                        UrlImage = f.UrlImage
                    })
                    .FirstOrDefaultAsync();
            }

            return mostPreferredFood;

        }

    }
}
