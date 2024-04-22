using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.CartDTO;
using OrderRestaurant.Model;

namespace OrderRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public CartController(ApplicationDBContext context)
        {
            _context = context;
        }

        //Nhap BE->FE
        [HttpPost]
        public async Task<IActionResult> BuyFood([FromBody] CreateCartDTO cartDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                // Kiểm tra xem bàn và món ăn có hợp lệ không
                var table = await _context.Tables.FindAsync(cartDTO.TableId);
                var food = await _context.Foods.FindAsync(cartDTO.FoodId);

                Console.WriteLine($"Thông tin của bàn: {Newtonsoft.Json.JsonConvert.SerializeObject(table)}");
                Console.WriteLine($"Thông tin của món ăn: {Newtonsoft.Json.JsonConvert.SerializeObject(food)}");
                if (table == null || food == null)
                {
                    return BadRequest("Bàn hoặc món ăn không tồn tại.");
                }
                // Kiểm tra xem bàn có trống không
                if (table.StatusId == 8) // 8 là trạng thái cho bàn trống
                {
                    // Cập nhật trạng thái của bàn thành "đã có người" (StatusId là 9)
                    table.StatusId = 7; // 7 là trạng thái cho bàn đã có người
                    _context.Tables.Update(table);
                }

                // Tạo mới một mục trong bảng Cart
                var newCart = new Cart
                {
                    TableId = cartDTO.TableId,
                    FoodId = cartDTO.FoodId,
                    StatusId = 1, // Mặc định trạng thái là "chưa làm"
                    CreateTime = DateTime.Now,
                    EmployeeId = null,
                };


                _context.CartUser.Add(newCart);

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                return Ok("Đã thêm món vào giỏ hàng.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi thêm món vào giỏ hàng: {ex.Message}");
            }
        }

        //FE->BE
        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCart([FromBody] CartList cartlist)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cart = new Cart
            {
                TableId = cartlist.TableId,
                FoodId = cartlist.FoodId,
                StatusId = 1, // Giỏ hàng mới tạo
                EmployeeId = cartlist.EmployeeId,
                CreateTime = DateTime.Now
            };
            _context.CartUser.Add(cart);
            _context.SaveChanges();
            return Ok("Thêm vào giỏ thành công");

        }




        [HttpGet]
        [Route("{cartId}")]
        public async Task<IActionResult> GetCartById(int cartId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var model = await _context.CartUser
                           .Include(c => c.TableCart)
                           .Include(c => c.ManageStatusCart)
                           .Where(s => s.CartId == cartId)
                           .Select(s => new CartModel
                           {
                               CartId = s.CartId,
                               FoodId = s.FoodId,
                               StatusId = s.StatusId,
                               TableId = s.TableId,
                               CreateTime = DateTime.Now,
                               EmployeeId = s.EmployeeId,
                               FoodCart = _context.Foods.FirstOrDefault(a => a.FoodId == s.FoodId) ?? new Food(),
                               TableCart = _context.Tables.FirstOrDefault(a => a.TableId == s.TableId) ?? new Table(),
                               ManageStatusCart = _context.Statuss.FirstOrDefault(a => a.StatusId == s.StatusId) ?? new ManageStatus(),
                               EmployeeCart = _context.Employees.FirstOrDefault(a => a.EmployeeId == s.EmployeeId) ?? new Employee(),
                           }).FirstOrDefaultAsync();

            if (model == null)
            {
                return NotFound("Không tìm thấy");
            }
            return Ok(model);
        }








    }
}
