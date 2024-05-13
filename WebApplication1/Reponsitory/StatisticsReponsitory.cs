using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.FoodDTO;
using OrderRestaurant.DTO.OrderDetailsDTO;
using OrderRestaurant.Model;
using OrderRestaurant.Service;

namespace OrderRestaurant.Responsitory
{
    public class StatisticsReponsitory : IStatistics
    {
        private readonly ApplicationDBContext _context;
        public StatisticsReponsitory(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetRevenueByDay(DateTime date)
        {
            var startDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            var endDate = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);

            var total = await _context.Orders
                        .Where(o => o.Code == Constants.ORDER_PAYMENT &&
                                o.PaymentTime != null &&
                                o.PaymentTime >= startDate &&
                                o.PaymentTime <= endDate)
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

        //Tổng doanh thu
        public async Task<decimal> GetTotalRevenue()
        {
            var totalRevenue = await _context.Orders
                                    .Where(o => o.Code == Constants.ORDER_PAYMENT && o.PaymentTime != null)
                                    .SumAsync(o => o.Pay);

            return (decimal)totalRevenue;
        }
        public async Task<Dictionary<string, decimal>> GetTotalRevenueByMonth(int currentYear)
        {
            var totalRevenueByMonth = await _context.Orders
                .Where(o => o.Code == Constants.ORDER_PAYMENT && o.PaymentTime != null &&o.PaymentTime.Value.Year == currentYear)
                .GroupBy(o => o.PaymentTime.Value.Month)
                .Select(g => new
                {
                    Month = $"Tháng {g.Key}",
                    TotalRevenue = g.Sum(o => o.Pay)
                })
                .ToDictionaryAsync(x => x.Month, x => (decimal)x.TotalRevenue);

            return totalRevenueByMonth;
        }

        // Thống kê Món ăn được yêu thích nhất
        public async Task<OrderDetailsStatistics> GetMostPopularFood()
        {
            var mostPreferredFood = await _context.OrderDetails
                                        .Where(a => a.Order.Code == Constants.ORDER_PAYMENT)
                                        .GroupBy(od => od.FoodId)
                                        .Select(g => new OrderDetailsStatistics
                                        {
                                            FoodId = g.Key,
                                            UsageCount = g.Sum(od => od.Quantity)
                                        })
                                        .OrderByDescending(g => g.UsageCount)
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
                    .FirstOrDefaultAsync() ?? new FoodsDTO();
            }

            return mostPreferredFood;

        }
        // Thống kê Món ăn ít được yêu thích nhất
        public async Task<OrderDetailsStatistics> GetLeastPopularFood()
        {
            var leastPreferredFood = await _context.OrderDetails
                                        .Where(a => a.Order.Code == Constants.ORDER_PAYMENT)
                                        .GroupBy(od => od.FoodId)
                                        
                                        .Select(g => new OrderDetailsStatistics
                                        {
                                            FoodId = g.Key,
                                            UsageCount = g.Sum(od => od.Quantity)
                                        })
                                        .OrderBy(g => g.UsageCount)
                                        .FirstOrDefaultAsync();

            if (leastPreferredFood != null)
            {
                leastPreferredFood.Foods = await _context.Foods
                    .Where(f => f.FoodId == leastPreferredFood.FoodId)
                    .Select(f => new FoodsDTO
                    {
                        FoodId = f.FoodId,
                        NameFood = f.NameFood,
                        CategoryId = f.CategoryId,
                        UnitPrice = f.UnitPrice,
                        UrlImage = f.UrlImage
                    })
                    .FirstOrDefaultAsync() ?? new FoodsDTO();
            }

            return leastPreferredFood;
        }
        //Thống kê số lượng món ăn , sắp xếp theo danh sách bán 
        public async Task<List<FoodStatistic>> GetFoodStatistics()
        {
            var foodStatistics = await _context.OrderDetails
                .Where(od => od.Order.Code == Constants.ORDER_PAYMENT)
                .GroupBy(oi => oi.FoodId)
                .Select(g => new FoodStatistic
                {
                    FoodId = g.Key,
                    FoodName = g.First().Food.NameFood,
                    UnitPrice=g.First().Food.UnitPrice,
                    UrlImage = g.First().Food.UrlImage,
                    CategoryId=g.First().Food.CategoryId,
                    Categorys = _context.Categoies.Where(a=>a.CategoryId ==  g.First().Food.CategoryId)
                                                    .Select(o=> new CategoryModel 
                                                    {
                                                        CategoryId = o.CategoryId,
                                                        CategoryName = o.CategoryName,
                                                        Description = o.Description,
                                                    }).FirstOrDefault()?? new CategoryModel(),
                    QuantitySold = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(fs => fs.QuantitySold)
                .ToListAsync();

            return foodStatistics;
        }

        public async Task<(decimal, OrderDetailsStatistics leastUsedFood , OrderDetailsStatistics popularUsedFood) > RevenueByDate(DateTime startDate , DateTime endDate)
        {
            if(startDate > endDate)
            {
                throw new ArgumentException("Ngày bắt đầu không thể sau ngày kết thúc");
            }
            var total = await _context.Orders
                               .Where(o => o.Code == Constants.ORDER_PAYMENT &&
                                       o.PaymentTime != null &&
                                       o.PaymentTime.Value.Date >= startDate.Date &&
                                       o.PaymentTime.Value.Date <= endDate.Date)
                               .SumAsync(o => o.Pay);

            var leastUsedFood = await _context.OrderDetails
                                      .Where(od => od.Order.PaymentTime.Value.Date >= startDate.Date &&
                                                   od.Order.PaymentTime.Value.Date <= endDate.Date)
                                      .GroupBy(od => od.FoodId)
                                      .OrderBy(g => g.Count())
                                      .Select(g => new OrderDetailsStatistics
                                      {
                                          FoodId = g.Key,
                                          UsageCount = g.Count()
                                      })
                                      .FirstOrDefaultAsync();

            if (leastUsedFood != null)
            {
                leastUsedFood.Foods = await _context.Foods
                    .Where(f => f.FoodId == leastUsedFood.FoodId)
                    .Select(f => new FoodsDTO
                    {
                        FoodId = f.FoodId,
                        NameFood = f.NameFood,
                        CategoryId = f.CategoryId,
                        UnitPrice = f.UnitPrice,
                        UrlImage = f.UrlImage
                    })
                    .FirstOrDefaultAsync() ?? new FoodsDTO();
            }


            var popularUsedFood = await _context.OrderDetails
                                        .Where(od => od.Order.PaymentTime.Value.Date >= startDate.Date &&
                                                   od.Order.PaymentTime.Value.Date <= endDate.Date)
                                        .GroupBy(od => od.FoodId)
                                        .OrderByDescending(g => g.Count())
                                        .Select(g => new OrderDetailsStatistics
                                        {
                                            FoodId = g.Key,
                                            UsageCount = g.Count()
                                        })
                                        .FirstOrDefaultAsync();

            if (popularUsedFood != null)
            {
                popularUsedFood.Foods = await _context.Foods
                    .Where(f => f.FoodId == popularUsedFood.FoodId)
                    .Select(f => new FoodsDTO
                    {
                        FoodId = f.FoodId,
                        NameFood = f.NameFood,
                        CategoryId = f.CategoryId,
                        UnitPrice = f.UnitPrice,
                        UrlImage = f.UrlImage
                    })
                    .FirstOrDefaultAsync() ?? new FoodsDTO();
            }

            return ((decimal)total, leastUsedFood,popularUsedFood);
        }

    }
}
