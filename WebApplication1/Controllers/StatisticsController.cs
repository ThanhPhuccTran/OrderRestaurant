using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.Model;
using OrderRestaurant.Service;

namespace OrderRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        
        private readonly ApplicationDBContext _context;
        private readonly IStatistics _statistics;
       
        public StatisticsController(ApplicationDBContext context, IStatistics statistics)
        {
            _statistics = statistics;
            _context = context;
           
        }
        //Thống kê doanh số theo ngày,tháng năm 
        [HttpGet("revenue-by-day/{date}")]
        public async Task<IActionResult> GetRevenueByDay(DateTime date)
        {
            try
            {
                var dailyRevenue = await _statistics.GetRevenueByDay(date);
                return Ok(dailyRevenue);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        // theo tháng năm
        [HttpGet("revenue-by-month/{year}/{month}")]
        public async Task<IActionResult> GetRevenueByMonth(int year, int month)
        {
            try
            {
                var monthlyRevenue = await _statistics.GetRevenueByMonth(year, month);
                return Ok(monthlyRevenue);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        //Thống kê doanh thu từ trước tới giờ 
        [HttpGet("Total-Revenue")]
        public async Task<IActionResult> GetTotalRevenue()
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var totalRevenue = await _statistics.GetTotalRevenue();
                if(totalRevenue == null)
                {
                    return BadRequest("Doanh thu hiện không có");
                }
                return Ok(totalRevenue);
            }catch(Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }
        // Thống kê doanh thu từ trước tới giờ theo tháng nhập theo năm
        [HttpGet("Total-Revenue-Month")]
        public async Task<IActionResult> GetTotalRevenueByMonth(DateTime startDate, DateTime endDate)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var totalRevenue = await _statistics.GetTotalRevenueByMonthRange(startDate, endDate);
                if (totalRevenue == null)
                {
                    return BadRequest("Doanh thu hiện không có");
                }
                return Ok(totalRevenue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpGet("Total-Revenue-Date")]
        public async Task<IActionResult> GetTotalRevenueByDate(DateTime startDate, DateTime endDate)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var totalRevenue = await _statistics.GetTotalRevenueByDateRange(startDate, endDate);
                if (totalRevenue == null)
                {
                    return BadRequest("Doanh thu hiện không có");
                }
                return Ok(totalRevenue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }
        // Thống kê tổng hóa đơn đã thanh toán

        [HttpGet("Total-Order")]
        public async Task<IActionResult> GetTotalOrder()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var totalOrders = await _context.Orders
                                            .Where(o => o.Code == Constants.ORDER_PAYMENT)
                                            .CountAsync();
                return Ok(totalOrders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        //Thống kê món ăn , sắp xếp theo số lượng mua 
        [HttpGet("Get-Food-Statistics")]
        public async Task<IActionResult> GetFoodStatistics([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try 
            {
                var model = await _statistics.GetFoodStatistics(startDate, endDate);
                if(model == null)
                {
                    return BadRequest("không thống kê được");
                }
                return Ok(model);

            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }
        //món ăn sử dụng nhiều nhất
        [HttpGet("favourite-food")]
        public async Task<IActionResult> GetFavouriteFood([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
               var model = await _statistics.GetMostPopularFood(startDate, endDate);
                if(model == null)
                {
                    return BadRequest("Không có món ăn nào ");
                }
                return Ok(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        //món ăn ít sử dụng  nhất
        [HttpGet("leastpopular-food")]
        public async Task<IActionResult> GetLeastPopularFood([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var model = await _statistics.GetLeastPopularFood(startDate, endDate);
                if (model == null)
                {
                    return BadRequest("Không có món ăn nào ");
                }
                return Ok(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenueByDate([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest("Ngày bắt đầu không thể sau ngày kết thúc");
                }

                var (totalRevenue, totalOrder,leastUsedFood, popularUsedFood, doanhso,donhang,dayFood,food) = await _statistics.RevenueByDate(startDate, endDate);
                /*var totalRevenue = result.Item1;
                var leastUsedFood = result.Item2;
                var popularUsedFood = result.Item3;
                var totalRevenueByDate = result.Item4;*/
              //  var listRevenueByDate = result.Item4;

                return Ok(new
                {
                    TotalRevenue = totalRevenue,
                    LeastUsedFood = leastUsedFood,
                    PopularUsedFood = popularUsedFood,
                    DoanhSo = doanhso,
                    DonHang = donhang,
                    TotalOrder = totalOrder,
                    DayFood = dayFood,
                    Food = food,
                    
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi không xác định. Vui lòng thử lại sau.");
            }
        }

     


    }
}
