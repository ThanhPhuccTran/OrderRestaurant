using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.Model;
using OrderRestaurant.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace OrderRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCart : ControllerBase
    {
        private readonly IFood _foodRepository;
        private readonly ApplicationDBContext _context;

        public ShoppingCart(IFood foodRepository, ApplicationDBContext context)
        {
            _foodRepository = foodRepository;
            _context = context;
        }

        [HttpPost]
        [Route("add-food")]
        public async Task<IActionResult> AddFoodToOrder(int foodId)
        {
            try
            {
                // Lấy thông tin món đồ ăn từ cơ sở dữ liệu
                var foodToAdd = await _context.Foods.FindAsync(foodId);
                if (foodToAdd == null)
                {
                    return NotFound("Món đồ ăn không tồn tại");
                }

                // Kiểm tra xem giỏ hàng có tồn tại trong session không
                var cartItemsJson = HttpContext.Session.GetString("CartItems");
                List<CartItemModel> cartItems;
                if (cartItemsJson != null)
                {
                    cartItems = JsonSerializer.Deserialize<List<CartItemModel>>(cartItemsJson);
                }
                else
                {
                    cartItems = new List<CartItemModel>();
                }

                // Kiểm tra xem món đồ ăn đã tồn tại trong giỏ hàng chưa
                var existingCartItem = cartItems.FirstOrDefault(item => item.foods.FoodId == foodId);
                if (existingCartItem != null)
                {
                    // Nếu đã tồn tại, tăng số lượng lên 1
                    existingCartItem.Quantity++;
                }
                else
                {
                    // Nếu chưa tồn tại, thêm món đồ ăn vào giỏ hàng với số lượng là 1
                    cartItems.Add(new CartItemModel
                    {
                        foods = foodToAdd,
                        Quantity = 1
                    });
                }

                // Lưu thông tin giỏ hàng vào session
                HttpContext.Session.SetString("CartItems", JsonSerializer.Serialize(cartItems));

                return Ok("Món đã được thêm vào giỏ hàng");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("get-cart")]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                // Kiểm tra giỏ hàng có tồn tại trong session không
                var cartItemsJson = HttpContext.Session.GetString("CartItems");
                if (cartItemsJson == null)
                {
                    // Nếu giỏ hàng không tồn tại, trả về giỏ hàng trống
                    return Ok(new List<CartItemModel>());
                }
                else
                {
                    // Nếu giỏ hàng tồn tại, trả về thông tin của giỏ hàng
                    var cartItems = JsonSerializer.Deserialize<List<CartItemModel>>(cartItemsJson);
                    return Ok(cartItems);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }
        [HttpPost]
        [Route("checkout")]
        public async Task<IActionResult> Checkout(int customerId, int tableId, int? employeeId, string note)
        {
            try
            {
                // Kiểm tra tính hợp lệ của dữ liệu đầu vào
                if (customerId <= 0)
                    return BadRequest("Id khách hàng không hợp lệ");

                if (tableId <= 0)
                    return BadRequest("Id bàn không hợp lệ");

                // Lấy thông tin giỏ hàng từ session
                var cartItemsJson = HttpContext.Session.GetString("CartItems");
                if (cartItemsJson == null)
                {
                    return BadRequest("Giỏ hàng trống");
                }

                // Chuyển đổi chuỗi JSON thành danh sách các mục trong giỏ hàng
                var cartItems = JsonSerializer.Deserialize<List<CartItemModel>>(cartItemsJson);

                // Tạo đối tượng Order
                var order = new Order
                {
                    CustormerId = customerId,
                    TableId = tableId,
                    EmployeeId = employeeId,
                    CreationTime = DateTime.Now,
                    Status = 1, // 0 là trạng thái chưa thanh toán
                    Note = note,
                    OrderDetails = new List<OrderDetails>()
                };

                // Tạo đối tượng OrderDetails cho mỗi món trong giỏ hàng và cập nhật tổng tiền cho đơn hàng
                double totalAmount = 0;
                foreach (var item in cartItems)
                {
                    var food = await _context.Foods.FindAsync(item.foods.FoodId);
                    if (food == null)
                        return BadRequest($"Món ăn với id {item.foods.FoodId} không tồn tại");

                    var orderDetail = new OrderDetails
                    {
                        Quantity = item.Quantity,
                        UnitPrice = food.UnitPrice,
                        Note = note,
                        FoodId = food.FoodId
                    };
                    order.OrderDetails.Add(orderDetail);

                    totalAmount += item.Quantity * food.UnitPrice;
                }

                // Lưu đơn hàng và chi tiết đơn hàng vào cơ sở dữ liệu
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Xóa giỏ hàng sau khi đã checkout thành công
                HttpContext.Session.Remove("CartItems");

                return Ok("Checkout thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("update-order/{orderId}")]
        public IActionResult UpdateOrder(int orderId, [FromBody] OrderUpdateModel orderUpdateModel)
        {
            try
            {
                // Kiểm tra xem orderId có tồn tại không
                var order = _context.Orders.Include(o => o.OrderDetails).FirstOrDefault(o => o.OrderId == orderId);
                if (order == null)
                {
                    return NotFound("Không tìm thấy đơn hàng");
                }

                // Cập nhật thông tin đơn hàng
                order.CustormerId = orderUpdateModel.CustomerId;
                order.TableId = orderUpdateModel.TableId;
                order.EmployeeId = orderUpdateModel.EmployeeId;
                order.Note = orderUpdateModel.Note;

                // Cập nhật thông tin chi tiết đơn hàng
                foreach (var orderDetailUpdateModel in orderUpdateModel.OrderDetails)
                {
                    var orderDetail = order.OrderDetails.FirstOrDefault(od => od.OrderId == orderId && od.FoodId == orderDetailUpdateModel.FoodId);
                    if (orderDetail != null)
                    {
                        orderDetail.Quantity = orderDetailUpdateModel.Quantity;
                        orderDetail.UnitPrice = orderDetailUpdateModel.UnitPrice;
                        orderDetail.Note = orderDetailUpdateModel.Note;
                    }
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                _context.SaveChanges();

                return Ok("Đã cập nhật đơn hàng thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("update-cart")]
        public async Task<IActionResult> UpdateCart([FromBody] List<CartItemUpdateModel> updatedCartItems)
        {
            try
            {
                // Kiểm tra xem giỏ hàng có tồn tại trong session không
                var cartItemsJson = HttpContext.Session.GetString("CartItems");
                if (cartItemsJson == null)
                {
                    return BadRequest("Giỏ hàng trống");
                }

                // Chuyển đổi chuỗi JSON thành danh sách các mục trong giỏ hàng
                var cartItems = JsonSerializer.Deserialize<List<CartItemModel>>(cartItemsJson);

                // Cập nhật thông tin giỏ hàng
                foreach (var updatedCartItem in updatedCartItems)
                {
                    var existingCartItem = cartItems.FirstOrDefault(item => item.foods.FoodId == updatedCartItem.FoodId);
                    if (existingCartItem != null)
                    {
                        existingCartItem.Quantity = updatedCartItem.Quantity;
                    }
                }

                // Lưu lại thông tin giỏ hàng đã được cập nhật vào session
                HttpContext.Session.SetString("CartItems", JsonSerializer.Serialize(cartItems));

                return Ok("Cập nhật giỏ hàng thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("remove-item/{foodId}")]
        public IActionResult RemoveItem(int foodId)
        {
            try
            {
                // Kiểm tra giỏ hàng có tồn tại trong session không
                var cartItemsJson = HttpContext.Session.GetString("CartItems");
                if (cartItemsJson == null)
                {
                    return BadRequest("Giỏ hàng trống");
                }

                // Chuyển đổi chuỗi JSON thành danh sách các mục trong giỏ hàng
                var cartItems = JsonSerializer.Deserialize<List<CartItemModel>>(cartItemsJson);

                // Tìm kiếm và xóa mục có FoodId tương ứng khỏi giỏ hàng
                var itemToRemove = cartItems.FirstOrDefault(item => item.foods.FoodId == foodId);
                if (itemToRemove != null)
                {
                    cartItems.Remove(itemToRemove);

                    // Lưu lại thông tin giỏ hàng đã được cập nhật vào session
                    HttpContext.Session.SetString("CartItems", JsonSerializer.Serialize(cartItems));

                    return Ok("Đã xóa món khỏi giỏ hàng");
                }
                else
                {
                    return NotFound("Món không tồn tại trong giỏ hàng");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }


        [HttpDelete]
        [Route("clear-cart")]
        public IActionResult ClearCart()
        {
            try 
            {
                HttpContext.Session.Remove("CartItem");
                return Ok("Đã xóa tất cả các món khỏi giỏ hàng");
            }
            catch(Exception ex) 
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

    }
}
