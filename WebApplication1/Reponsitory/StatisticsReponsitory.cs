using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.FoodDTO;
using OrderRestaurant.DTO.OrderDetailsDTO;
using OrderRestaurant.Model;
using OrderRestaurant.Service;
using System.Collections.Generic;

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
        public async Task<Dictionary<string, decimal>> GetTotalRevenueByMonthRange(DateTime startDate, DateTime endDate)
        {
            var totalRevenueByDate = await _context.Orders
                .Where(o => o.Code == Constants.ORDER_PAYMENT &&
                            o.PaymentTime != null &&
                            o.PaymentTime >= startDate &&
                            o.PaymentTime <= endDate)
                .GroupBy(o => new { Year = o.PaymentTime.Value.Year, Month = o.PaymentTime.Value.Month })
                .Select(g => new
                {
                    Month = $"Tháng {g.Key.Month} - {g.Key.Year}",
                    TotalRevenue = g.Sum(o => o.Pay),

                })
                .ToDictionaryAsync(x => x.Month, x => (decimal)x.TotalRevenue);

            return totalRevenueByDate;
        }
        public async Task<Dictionary<string, decimal>> GetTotalRevenueByDateRange(DateTime startDate, DateTime endDate)
        {
            var totalRevenueByDate = await _context.Orders
                .Where(o => o.Code == Constants.ORDER_PAYMENT &&
                            o.PaymentTime != null &&
                            o.PaymentTime.Value.Date >= startDate.Date &&
                            o.PaymentTime.Value.Date <= endDate.Date)
                .GroupBy(o => o.PaymentTime.Value.Date)
                .Select(g => new
                {
                    Date = $"Ngày {g.Key.Day}/{g.Key.Month}/{g.Key.Year}",
                    TotalRevenue = g.Sum(o => o.Pay)
                })
                .ToDictionaryAsync(x => x.Date, x => (decimal)x.TotalRevenue);

            /* // Đảm bảo cả ngày bắt đầu và kết thúc của khoảng thời gian đều có kết quả
             for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
             {
                 var dateKey = $"Ngày {date.Day}/{date.Month}/{date.Year}";
                 if (!totalRevenueByDate.ContainsKey(dateKey))
                 {
                     totalRevenueByDate[dateKey] = 0m; // Gán giá trị mặc định là 0 nếu không có kết quả
                 }
             }*/

            return totalRevenueByDate;
        }



        // Thống kê Món ăn được yêu thích nhất
        public async Task<OrderDetailsStatistics> GetMostPopularFood(DateTime startDate, DateTime endDate)
        {
            var popularUsedFood = await _context.OrderDetails
                                       .Where(a => a.Order.Code == Constants.ORDER_PAYMENT &&
                                                   a.Order.PaymentTime != null &&
                                                   a.Order.PaymentTime.Value.Date >= startDate.Date &&
                                                   a.Order.PaymentTime.Value.Date <= endDate.Date)
                                       .GroupBy(od => od.FoodId)
                                       .Select(g => new OrderDetailsStatistics
                                       {
                                           FoodId = g.Key,
                                           UsageCount = g.Sum(od => od.Quantity)
                                       })
                                       .OrderByDescending(g => g.UsageCount)
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

            return popularUsedFood;

        }
        // Thống kê Món ăn ít được yêu thích nhất
        public async Task<OrderDetailsStatistics> GetLeastPopularFood(DateTime startDate, DateTime endDate)
        {
            var leastUsedFood = await _context.OrderDetails
                                   .Where(a => a.Order.Code == Constants.ORDER_PAYMENT &&
                                               a.Order.PaymentTime != null &&
                                               a.Order.PaymentTime.Value.Date >= startDate.Date &&
                                               a.Order.PaymentTime.Value.Date <= endDate.Date)
                                   .GroupBy(od => od.FoodId)
                                   .Select(g => new OrderDetailsStatistics
                                   {
                                       FoodId = g.Key,
                                       UsageCount = g.Sum(od => od.Quantity)
                                   })
                                   .OrderBy(g => g.UsageCount)
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

            return leastUsedFood;
        }
        //Thống kê số lượng món ăn , sắp xếp theo danh sách bán 
        public async Task<FoodStatisticsResult> GetFoodStatistics(DateTime startDate, DateTime endDate)
        {
            var foodStatistics = await _context.OrderDetails
                .Where(od => od.Order.Code == Constants.ORDER_PAYMENT &&
                                            od.Order.PaymentTime != null &&
                                            od.Order.PaymentTime.Value.Date >= startDate.Date &&
                                            od.Order.PaymentTime.Value.Date <= endDate.Date)
                .GroupBy(oi => oi.FoodId)
                .Select(g => new FoodStatistic
                {
                    FoodId = g.Key,
                    FoodName = g.First().Food.NameFood,
                    UnitPrice = g.First().Food.UnitPrice,
                    UrlImage = g.First().Food.UrlImage,
                    CategoryId = g.First().Food.CategoryId,
                    Categorys = _context.Categoies.Where(a => a.CategoryId == g.First().Food.CategoryId)
                                                    .Select(o => new CategoryModel
                                                    {
                                                        CategoryId = o.CategoryId,
                                                        CategoryName = o.CategoryName,
                                                        Description = o.Description,
                                                    }).FirstOrDefault() ?? new CategoryModel(),
                    QuantitySold = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(fs => fs.QuantitySold)
                .ToListAsync();

            var totalFood = foodStatistics.Sum(fs => fs.QuantitySold);

            var result = new FoodStatisticsResult
            {
                TotalFood = totalFood,
                Food = foodStatistics
            };

            return result;
        }


        public async Task<(decimal, int, OrderDetailsStatistics, OrderDetailsStatistics, Dictionary<string, decimal>, Dictionary<string, int>, Dictionary<string, int>, FoodStatisticsResult)> RevenueByDate(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException("Ngày bắt đầu không thể sau ngày kết thúc");
            }

            Dictionary<string, decimal> doanhso;
            Dictionary<string, int> ordersSummary = new Dictionary<string, int>();
            Dictionary<string, int> foodSummary = new Dictionary<string, int>();
            if (startDate.Month != endDate.Month)
            {
                doanhso = await GetTotalRevenueByMonthRange(startDate, endDate);
                var ordersByMonth = await _context.Orders
                    .Where(o => o.Code == Constants.ORDER_PAYMENT &&
                            o.PaymentTime != null &&
                            o.PaymentTime.Value.Date >= startDate.Date &&
                            o.PaymentTime.Value.Date <= endDate.Date)
                    .GroupBy(o => o.PaymentTime.Value.Month)
                    .Select(g => new
                    {
                        Month = g.Key,
                        OrderCount = g.Count()
                    })
                    .ToDictionaryAsync(g => $"Tháng {g.Month}", g => g.OrderCount);

                foreach (var item in ordersByMonth)
                {
                    ordersSummary.Add(item.Key, item.Value);
                }
                var foodsByMonth = await _context.OrderDetails
                                                .Where(od => od.Order.Code == Constants.ORDER_PAYMENT
                                                        && od.Order.PaymentTime != null
                                                        && od.Order.PaymentTime.Value.Date >= startDate.Date
                                                        && od.Order.PaymentTime.Value.Date <= endDate.Date)
                                                .GroupBy(o => o.Order.PaymentTime.Value.Month)
                                                .Select(g => new
                                                {
                                                    Month = g.Key,
                                                    FoodCount = g.Sum(oi => oi.Quantity)
                                                }).ToDictionaryAsync(g => $"Tháng {g.Month}", g => g.FoodCount);
                foreach (var item in foodsByMonth)
                {
                    foodSummary.Add(item.Key, item.Value);
                }
            }
            else
            {
                doanhso = await GetTotalRevenueByDateRange(startDate, endDate);
                var ordersByDate = await _context.Orders
                    .Where(o => o.Code == Constants.ORDER_PAYMENT &&
                            o.PaymentTime != null &&
                            o.PaymentTime.Value.Date >= startDate.Date &&
                            o.PaymentTime.Value.Date <= endDate.Date)
                    .GroupBy(o => o.PaymentTime.Value.Date)
                    .Select(g => new
                    {
                        Date = $"Ngày {g.Key.Day}/{g.Key.Month}/{g.Key.Year}",
                        OrderCount = g.Count()
                    })
                    .ToDictionaryAsync(g => g.Date, g => g.OrderCount);

                foreach (var item in ordersByDate)
                {
                    ordersSummary.Add(item.Key, item.Value);
                }

                var foodsByMonth = await _context.OrderDetails
                                                .Where(od => od.Order.Code == Constants.ORDER_PAYMENT
                                                        && od.Order.PaymentTime != null
                                                        && od.Order.PaymentTime.Value.Date >= startDate.Date
                                                        && od.Order.PaymentTime.Value.Date <= endDate.Date)
                                                .GroupBy(o => o.Order.PaymentTime.Value.Date)
                                                .Select(g => new
                                                {
                                                    Date = $"Ngày {g.Key.Day}/{g.Key.Month}/{g.Key.Year}",
                                                    FoodCount = g.Sum(oi => oi.Quantity)
                                                }).ToDictionaryAsync(g => g.Date, g => g.FoodCount);
                foreach (var item in foodsByMonth)
                {
                    foodSummary.Add(item.Key, item.Value);
                }
            }

            var total = await _context.Orders
                .Where(o => o.Code == Constants.ORDER_PAYMENT &&
                        o.PaymentTime != null &&
                        o.PaymentTime.Value.Date >= startDate.Date &&
                        o.PaymentTime.Value.Date <= endDate.Date)
                .SumAsync(o => o.Pay);
            var totalOrder = await _context.Orders
                                         .Where(o => o.Code == Constants.ORDER_PAYMENT &&
                                                     o.PaymentTime != null &&
                                                     o.PaymentTime.Value.Date >= startDate.Date &&
                                                     o.PaymentTime.Value.Date <= endDate.Date)
                                         .CountAsync();

            OrderDetailsStatistics leastUsedFoodTask = await GetLeastPopularFood(startDate, endDate);
            OrderDetailsStatistics popularUsedFoodTask = await GetMostPopularFood(startDate, endDate);
            FoodStatisticsResult foodStatistics = await GetFoodStatistics(startDate, endDate);
            return ((decimal)total, (int)totalOrder, leastUsedFoodTask, popularUsedFoodTask, doanhso, ordersSummary, foodSummary, foodStatistics);
        }




    }
}
