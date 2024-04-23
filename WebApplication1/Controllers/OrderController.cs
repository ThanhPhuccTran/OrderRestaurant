using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.Cart;
using OrderRestaurant.DTO.OrderDetailsDTO;
using OrderRestaurant.Model;
using OrderRestaurant.Service;

namespace OrderRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrder _orderRepository;
        private readonly ApplicationDBContext _context;
        public OrderController(ApplicationDBContext context, IOrder orderRepository)
        {
            _orderRepository = orderRepository;
            _context = context;
        }


        [HttpGet("get-order-all")]
        public async Task<IActionResult> GetAll()
        {
            var model = _context.Orders.Select(s => new OrderModel
            {
                OrderId = s.OrderId,
                EmployeeId = s.EmployeeId,
                TableId = s.TableId,
                CreationTime = s.CreationTime,
                ReceivingTime = s.ReceivingTime,
                PaymentTime = s.PaymentTime,
                Pay = s.Pay,
                Note = s.Note,
                StatusId = s.StatusId,
                Employees = _context.Employees.Where(a=>a.EmployeeId == s.EmployeeId).FirstOrDefault(),
                Statuss = _context.Statuss.Where(a=>a.StatusId == s.StatusId).FirstOrDefault(),
                Tables = _context.Tables.Where(a=>a.TableId == s.TableId).FirstOrDefault(),
            }).ToList();
            return Ok(model);
        }
        /*
                [HttpPost]
                public async Task<IActionResult> CreateOrder(CreateOrderDTO orderDTO)
                {
                    try
                    {
                        var order = new Order
                        {
                            TableId = orderDTO.TableId,
                            CreationTime = DateTime.Now,
                            Note = "",
                            StatusId = 100 , //Don hang moi tao
                        };
                        _context.Orders.Add(order);
                        await _context.SaveChangesAsync();

                        //Chi tiet don hang
                        foreach(var item in orderDTO.OrderDetails)
                        {
                            var orderDetail = new OrderDetails
                            {
                                OrderId = order.OrderId,
                                FoodId = item.FoodId,
                                Quantity = item.Quantity,
                                UnitPrice = item.UnitPrice,
                                Note = item.Note,
                            };
                            _context.OrderDetails.Add(orderDetail);
                        }
                        await _context.SaveChangesAsync();
                        return Ok("Tạo Order thành công");


                    }
                    catch(Exception ex)
                    {
                        return StatusCode(500, $"Bị lỗi: {ex.Message}");

                    }
                }*/
        [HttpPost("add/{tableId}")]
        public async Task<IActionResult> CreateOrderPOSt([FromRoute] int tableId, [FromForm] CartList cartModel)
        {
            try
            {
               
                if (cartModel == null || cartModel.Items == null || !cartModel.Items.Any())
                {
                    return BadRequest("Giỏ hàng trống hoặc không hợp lệ.");
                }
                var order = new Order
                {
                    TableId = tableId,
                    CreationTime = DateTime.Now,
                    StatusId = 100, // Trạng thái mới
                    OrderDetails = new List<OrderDetails>()
                };

                foreach (var item in cartModel.Items)
                {
                    var food = await _context.Foods.FindAsync(item.FoodId);

                    // Kiểm tra food null
                    if (food == null)
                    {
                        return BadRequest($"Món ăn với id {item.FoodId} không tồn tại.");
                    }

                    var TotalPrice = (decimal)(item.Quantity * food.UnitPrice);
                    var orderDetails = new OrderDetails
                    {
                        Quantity = item.Quantity,
                        UnitPrice = food.UnitPrice,
                        Note = "", 
                        TotalAmount = TotalPrice,
                        FoodId = food.FoodId,
                    };
                    order.OrderDetails.Add(orderDetails);
                }

                
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return Ok("Xác nhận thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bị lỗi: {ex.Message}");
            }
        }




        [HttpPut("ChangeStatus/{id}/{newStatus}")]
        public async Task<IActionResult> ChangeOrderStatus([FromRoute] int id, [FromRoute] int newStatus)
        {
            var order = await _orderRepository.GetAsync(id);
            if (order == null)
                return NotFound();

            // Kiểm tra xem trạng thái mới có hợp lệ không
            if (!IsValidStatus(newStatus))
                return BadRequest("Invalid new status");
            if (newStatus == Constants.ORDER_ACCEPTED)
            {
                order.ReceivingTime = DateTime.Now; // Cập nhật thời gian nhận đơn hàng thành thời gian hiện tại
            }
            // Thực hiện thay đổi trạng thái
            order.StatusId = newStatus;
            await _orderRepository.UpdateAsync(order);

            return Ok(order);
        }

        private bool IsValidStatus(int status)
        {
            // Kiểm tra xem trạng thái mới có trong danh sách hợp lệ không
            return status == Constants.ORDER_INIT || status == Constants.ORDER_ACCEPTED ||
                   status == Constants.ORDER_FINISHED || status == Constants.ORDER_CANCEL ||
                   status == Constants.ORDER_REJECTED;
        }
    }
}
