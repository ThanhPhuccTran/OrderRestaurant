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

              
                if (table == null || food == null)
                {
                    return BadRequest("Bàn hoặc món ăn không tồn tại.");
                }
                // Kiểm tra xem bàn có trống không
                if (table.StatusId == 8) // 8 là trạng thái cho bàn trống
                {
                    // Cập nhật trạng thái của bàn thành "đã có người" 
                    table.StatusId = 7; // 7 là trạng thái cho bàn đã có người
                    _context.Tables.Update(table);
                }
                var check = await _context.CartUser.FirstOrDefaultAsync(c=>c.FoodId == cartDTO.FoodId && c.TableId == cartDTO.TableId);
                if (check != null)
                {
                    check.Quantity++;
                    _context.CartUser.Update(check);
                }
                else
                {
                    // Tạo mới một mục trong bảng Cart
                    var newCart = new Cart
                    {
                        TableId = cartDTO.TableId,
                        FoodId = cartDTO.FoodId,
                        StatusId = 1, // Mặc định trạng thái là "chưa làm"
                        CreateTime = DateTime.Now,
                        Quantity = 1,
                        EmployeeId = null, // nhân viên chưa xác nhận 
                    };


                    _context.CartUser.Add(newCart);
                }
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
            var check = await _context.CartUser.FirstOrDefaultAsync(c => c.FoodId == cartlist.FoodId && c.TableId == cartlist.TableId);
            if (check != null)
            {
                check.Quantity++;
                _context.CartUser.Update(check);
            }
            else
            {
                var cart = new Cart
                {
                    TableId = cartlist.TableId,
                    FoodId = cartlist.FoodId,
                    StatusId = 1, // Giỏ hàng mới tạo
                    Quantity = 1,
                    EmployeeId = null,
                    CreateTime = DateTime.Now
                };
                _context.CartUser.Add(cart);
            }
            // Kiểm tra xem bàn có trống không
            var table = await _context.Tables.FindAsync(cartlist.TableId);
            if (table != null && table.StatusId == 8) // 8 là trạng thái cho bàn trống
            {
                
                table.StatusId = 7; // 7 là trạng thái cho bàn đã có người
                _context.Tables.Update(table);
            }
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
                               Quantity = s.Quantity,
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

        [HttpGet("get-cart-by-table/{tableId}")]
        public async Task<IActionResult> GetCartByTable(int tableId)
        {
            var model = await _context.CartUser.Where(s => s.TableId == tableId)
                                               
                                               .Select(s => new  
                                               {
                                                FoodId = s.FoodId,
                                                Quantity = s.Quantity,
                                                FoodCart = _context.Foods.FirstOrDefault(a => a.FoodId == s.FoodId) ?? new Food(),

                                               }).ToListAsync();
            if(model == null)
            {
                return NotFound("Không tìm thấy");
            }
            return Ok(model);
        }


        //Khi Khách hàng chấp nhận giỏ hàng
        [HttpPost("select-cart")]
        public async Task<IActionResult> SelectCart([FromBody] SelectCartDTO selectDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var cartItem = await _context.CartUser.Where(c => c.TableId == selectDTO.TableId).ToListAsync();
                if(cartItem == null || !cartItem.Any())
                {
                    return BadRequest("Không có mặt hàng trong giỏ");
                }
                foreach(var item in cartItem)
                {
                    item.StatusId = 2; // Chờ xác nhận đơn hàng
                    _context.CartUser.Update(item);
                }
                await _context.SaveChangesAsync();
                return Ok("Order món ăn thành công , Vui lòng chờ xử lý");
            }
            catch(Exception ex) 
            {
                return StatusCode(500, $"Lỗi khi xử lý giỏ hàng: {ex.Message}");
            };
        }
        //Nhân viên xử lý Cart
        [HttpPost("processing-cart")]
        public async Task<IActionResult> ProcessingCart([FromBody] ProcessingCartDTO processing)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                
                var cartItems = await _context.CartUser.Where(c => c.TableId == processing.TableId).ToListAsync();
                if (cartItems == null || !cartItems.Any())
                {
                    return BadRequest("Không tìm thấy giỏ hàng.");
                }

                // Cập nhật trạng thái của từng giỏ hàng
                foreach (var cartItem in cartItems)
                {
                    cartItem.StatusId = 3; // 3 là trạng thái "đang xử lý"
                    cartItem.EmployeeId = processing.EmployeeId;
                    _context.CartUser.Update(cartItem);
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                return Ok("Đã xử lý đơn hàng");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi xử lý giỏ hàng: {ex.Message}");
            }
        }


        [HttpPost("complete-cart")]
        public async Task<IActionResult> CompleteCart([FromBody] ProcessingCartDTO processing)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var cartItem = await _context.CartUser.Where(c => c.TableId == processing.TableId).ToListAsync();
                if (cartItem == null)
                {
                    return BadRequest("Không tìm thấy giỏ hàng.");
                }
                // Cập nhật trạng thái của từng giỏ hàng
                foreach (var item in cartItem)
                {
                    item.StatusId = 4; // 4 là trạng thái "làm xong giỏ hàng"
                    item.EmployeeId = processing.EmployeeId;
                    _context.CartUser.Update(item);
                }
                await _context.SaveChangesAsync();
                return Ok("Đã xử lý đơn hàng");

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi xử lý giỏ hàng: {ex.Message}");

            }
        }

    }
}
