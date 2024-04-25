using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.Cart;
using OrderRestaurant.DTO.CartDTO;
using OrderRestaurant.DTO.ConfigDTO;
using OrderRestaurant.DTO.EmployeeDTO;
using OrderRestaurant.DTO.OrderDetailsDTO;
using OrderRestaurant.DTO.OrderDTO;
using OrderRestaurant.DTO.TableDTO;
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
               /* Employees = _context.Employees
                            .Where(a => a.EmployeeId == s.EmployeeId)
                            .Select(o => new EmployeesDTO
                            {
                               EmployeeId = o.EmployeeId,
                               EmployeeName = o.EmployeeName,
                               Email = o.Email,
                               Password = o.Password,
                               Phone = o.Password,
                               Image = o.Image
                               
                            })
                            .FirstOrDefault(),*/
                Statuss = _context.Statuss
                            .Where(a => a.StatusId == s.StatusId)
                            .Select(o => new ManageStatusDTO
                            {
                                StatusId = o.StatusId,
                                Code = o.Code,
                                Type = o.Type,
                                Value = o.Value,
                                Description = o.Description,
                            })
                            .FirstOrDefault(), 
                /*Tables = _context.Tables.Where(a => a.TableId == s.TableId)
                            .Select(o => new TablesDTO
                            {
                                TableId = o.TableId,
                                TableName = o.TableName,
                                Note = o.Note,
                                QR_id = o.QR_id,
                                StatusId = o.StatusId
                            })
                            .FirstOrDefault(),*/
            }).ToList();

            return Ok(model);
        }


        [HttpGet("get-order-details/{orderId}")]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var model = _context.OrderDetails
                    .Where(s=>s.OrderId == orderId)
                    .Select(s => new OrderDetailModel
                {
                    OrderId = s.OrderId,
                    FoodId = s.FoodId,
                    Quantity = s.Quantity,
                    UnitPrice = s.UnitPrice,
                    Note = s.Note,
                    TotalAmount = s.TotalAmount,
                    Foods = _context.Foods.Where(a => a.FoodId == s.FoodId).FirstOrDefault() ?? new Food(),
                    Orders = _context.Orders.Where(a=>a.OrderId == s.OrderId).FirstOrDefault(),
                }).ToList();
                return Ok(model);
            }catch(Exception ex)
            {
                return StatusCode(500, $"Bị lỗi: {ex.Message}");
            }
        }
       
        [HttpPost("checkout")]
        public async Task<IActionResult> CheckOut([FromBody] CreateCartDTO cartDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                //check bàn có người hay chưa
                var table = await _context.Tables.FindAsync(cartDto.TableId);
                if(table == null)
                {
                    return NotFound("Không tìm thấy bàn");
                }
                if (table.StatusId == Constants.TABLE_GUESTS)
                {
                    return BadRequest("Bàn đã có khách ngồi, không thể đặt bàn .");
                }
                var order = new Order
                {
                    TableId = cartDto.TableId,
                    CreationTime = DateTime.Now,
                    StatusId = Constants.ORDER_INIT,
                    Pay = cartDto.TotalAmount,
                    Note ="",
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                //check bàn chuyển qua trạng thái có người 
                table.StatusId = Constants.TABLE_GUESTS;
                await _context.SaveChangesAsync();
               


                foreach (var item in cartDto.Items)
                {
                    var food = await _context.Foods.FindAsync(item.Foods.FoodId);
                    Console.WriteLine(food);
                    if (food == null)
                    {
                        return NotFound("Không tìm thấy ");
                    }
                    var orderDetails = new OrderDetails
                    {
                        OrderId = order.OrderId,
                        Quantity = item.Quantity,
                        UnitPrice = food.UnitPrice,
                        Note = "",
                        TotalAmount = item.Quantity * food.UnitPrice,
                        FoodId = item.Foods.FoodId
                    };
                    _context.OrderDetails.Add(orderDetails);
                }
                await _context.SaveChangesAsync();

                return Ok("THÊM VÀO GIỎ ĐƠN HÀNG THÀNH CÔNG");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bị lỗi: {ex.Message}");
            }
        }
        // GET: api/orders/search?type=Order
        [HttpGet("search")]
        public async Task<ActionResult<List<Order>>> GetOrdersByType(string type = "Order")
        {
            var orders = await _orderRepository.GetSearchType(type);
            return Ok(orders);
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteOrder (int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var model = await _context.Orders.Include(o=>o.OrderDetails).FirstOrDefaultAsync(i => i.OrderId == id);
                if(model == null)
                {
                    return NotFound("Không tìm thấy mã Order");
                }
                _context.OrderDetails.RemoveRange(model.OrderDetails); //Để xóa list liên quan OrderId
                _context.Orders.Remove(model); // xóa một cái OrderId
                await _context.SaveChangesAsync();
                return Ok("Xóa thành công");
            }catch(Exception ex)
            {
                return StatusCode(500, $"Bị lỗi: {ex.Message}");
            }
        }
        [HttpDelete("DeleteOrderDetail/{orderId}/{foodId}")]
        public async Task<IActionResult> DeleteOrderDetail(int orderId, int foodId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                // Tìm kiếm OrderDetails cần xóa
                var orderDetailsToDelete = _context.OrderDetails
                    .Where(od => od.OrderId == orderId && od.FoodId == foodId)
                    .ToList();

                // Kiểm tra nếu không tìm thấy OrderDetails
                if (orderDetailsToDelete.Count == 0)
                {
                    return NotFound("Không tìm thấy OrderDetails");
                }

                // Xóa OrderDetails
                _context.OrderDetails.RemoveRange(orderDetailsToDelete);
                // Tính tổng TotalAmount của các OrderDetails còn lại
                decimal? totalAmount = _context.OrderDetails
                    .Where(od => od.OrderId == orderId)
                    .Sum(od => od.TotalAmount);

                // Cập nhật lại trường Pay của Order
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
                if (order != null)
                {
                    order.Pay = totalAmount;
                }
                await _context.SaveChangesAsync();
                return Ok("Xóa OrderDetails thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bị lỗi: {ex.Message}");
            }
        }

        [HttpPost("ApproveOrder/{OrderId}/{EmployeeId}")]
        public async Task<IActionResult> ApproveOrder(int OrderId, int EmployeeId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var model = await _orderRepository.FindOrder(OrderId);
                if (model == null)
                {
                    return NotFound("Không tìm thấy");
                }

                if(model.StatusId != Constants.ORDER_INIT)
                {
                    return BadRequest("Trạng thái không phải là đơn mới");
                }
                model.StatusId = Constants.ORDER_APPROVE;
                model.EmployeeId = EmployeeId;
                model.ReceivingTime = DateTime.Now;


                var orderDTO = new OrderDTO
                {
                    OrderId = model.OrderId,
                    EmployeeId = model.EmployeeId,
                    StatusId = model.StatusId,
                    ReceivingTime = model.ReceivingTime,
                    ManageStatuss = _context.Statuss
                        .Where(a => a.StatusId == model.StatusId)
                        .Select(s => new ManageStatusDTO
                        {
                            StatusId = s.StatusId,
                            Code = s.Code,
                            Type = s.Type,
                            Value = s.Value,
                            Description = s.Description
                        })
                        .FirstOrDefault() ?? new ManageStatusDTO(),
                };

                await _orderRepository.UpdateAsync(model);
                return Ok(orderDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bị lỗi: {ex.Message}");
            }
        }
        [HttpPost("PaymentOrder/{OrderId}/{EmployeeId}")]
        public async Task<IActionResult> PaymentOrder(int OrderId, int EmployeeId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var model = await _orderRepository.FindOrder(OrderId);
                if (model == null)
                {
                    return NotFound("Không tìm thấy");
                }

                if (model.StatusId != Constants.ORDER_APPROVE)
                {
                    return BadRequest("Trạng thái không phải là đơn đã duyệt");
                }
                model.StatusId = Constants.ORDER_PAYMENT;
                model.EmployeeId = EmployeeId;
                model.PaymentTime = DateTime.Now;

                //Thanh toán xong trở về bàn trống
                var table = await _context.Tables.FindAsync(model.TableId);
                if (table != null)
                {
                    
                    table.StatusId = Constants.TABLE_EMPTY;
                    await _context.SaveChangesAsync();
                }

                var orderDTO = new OrderDTO
                {
                    OrderId = model.OrderId,
                    EmployeeId = model.EmployeeId,
                    StatusId = model.StatusId,
                    ReceivingTime = model.ReceivingTime,
                    PaymentTime = model.PaymentTime,
                    ManageStatuss = _context.Statuss
                        .Where(a => a.StatusId == model.StatusId)
                        .Select(s => new ManageStatusDTO
                        {
                            StatusId = s.StatusId,
                            Code = s.Code,
                            Type = s.Type,
                            Value = s.Value,
                            Description = s.Description
                        })
                        .FirstOrDefault() ?? new ManageStatusDTO(),
                };

                await _orderRepository.UpdateAsync(model);
                return Ok(orderDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bị lỗi: {ex.Message}");
            }
        }
        [HttpPost("RefuseOrder/{OrderId}/{EmployeeId}")]
        public async Task<IActionResult> RefuseOrder(int OrderId, int EmployeeId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var model = await _orderRepository.FindOrder(OrderId);
                if (model == null)
                {
                    return NotFound("Không tìm thấy");
                }

                if (model.StatusId != Constants.ORDER_INIT)
                {
                    return BadRequest("Trạng thái không phải là đơn đã duyệt");
                }
                model.StatusId = Constants.ORDER_REFUSE;
                model.EmployeeId = EmployeeId;


                var orderDTO = new OrderDTO
                {
                    OrderId = model.OrderId,
                    EmployeeId = model.EmployeeId,
                    StatusId = model.StatusId,
                    ReceivingTime = model.ReceivingTime,
                    PaymentTime = model.PaymentTime,
                    ManageStatuss = _context.Statuss
                        .Where(a => a.StatusId == model.StatusId)
                        .Select(s => new ManageStatusDTO
                        {
                            StatusId = s.StatusId,
                            Code = s.Code,
                            Type = s.Type,
                            Value = s.Value,
                            Description = s.Description
                        })
                        .FirstOrDefault() ?? new ManageStatusDTO(),
                };

                await _orderRepository.UpdateAsync(model);
                return Ok(orderDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bị lỗi: {ex.Message}");
            }
        }

    }
}
