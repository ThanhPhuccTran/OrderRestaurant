using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.Cart;
using OrderRestaurant.DTO.CartDTO;
using OrderRestaurant.DTO.ConfigDTO;
using OrderRestaurant.DTO.EmployeeDTO;
using OrderRestaurant.DTO.FoodDTO;
using OrderRestaurant.DTO.OrderDetailsDTO;
using OrderRestaurant.DTO.OrderDTO;
using OrderRestaurant.DTO.TableDTO;
using OrderRestaurant.Model;
using OrderRestaurant.Service;
using System.Globalization;

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
        [HttpGet("get-search-all")]
        public async Task<IActionResult> Search(int page = 1, int pageSize = 10)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var model = _context.Orders
                    .OrderByDescending(s => s.CreationTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(s => new OrderModel
                    {

                        OrderId = s.OrderId,
                        EmployeeId = s.EmployeeId,
                        TableId = s.TableId,
                        CreationTime = s.CreationTime,
                        ReceivingTime = s.ReceivingTime,
                        PaymentTime = s.PaymentTime,
                        Pay = s.Pay,
                        Note = s.Note,
                        Code = s.Code,
                        Employees = _context.Employees
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
                                .FirstOrDefault(),
                        Statuss = _context.Statuss
                                .Where(a => a.Code == s.Code && a.Type == "Order")
                                .Select(o => new ManageStatusDTO
                                {
                                    StatusId = o.StatusId,
                                    Code = o.Code,
                                    Type = o.Type,
                                    Value = o.Value,
                                    Description = o.Description,
                                })
                                .FirstOrDefault(),
                        Tables = _context.Tables.Where(a => a.TableId == s.TableId)
                                .Select(o => new TablesDTO
                                {
                                    TableId = o.TableId,
                                    TableName = o.TableName,
                                    Note = o.Note,
                                    QR_id = o.QR_id,
                                    Code = o.Code
                                })
                                .FirstOrDefault(),
                    }).ToList();


                return Ok(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bị lỗi: {ex.Message}");
            }
        }

        [HttpGet("get-order-all")]
        public async Task<IActionResult> GetAll()
        {


            var model = _context.Orders
                .OrderByDescending(s => s.CreationTime)
                .Select(s => new OrderModel
                {

                    OrderId = s.OrderId,
                    EmployeeId = s.EmployeeId,
                    TableId = s.TableId,
                    CreationTime = s.CreationTime,
                    ReceivingTime = s.ReceivingTime,
                    PaymentTime = s.PaymentTime,
                    Pay = s.Pay,
                    Note = s.Note,
                    Code = s.Code,
                    Employees = _context.Employees
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
                            .FirstOrDefault(),
                    Statuss = _context.Statuss
                            .Where(a => a.Code == s.Code && a.Type == "Order")
                            .Select(o => new ManageStatusDTO
                            {
                                StatusId = o.StatusId,
                                Code = o.Code,
                                Type = o.Type,
                                Value = o.Value,
                                Description = o.Description,
                            })
                            .FirstOrDefault(),
                    Tables = _context.Tables.Where(a => a.TableId == s.TableId)
                            .Select(o => new TablesDTO
                            {
                                TableId = o.TableId,
                                TableName = o.TableName,
                                Note = o.Note,
                                QR_id = o.QR_id,
                                Code = o.Code
                            })
                            .FirstOrDefault(),
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
                    .Where(s => s.OrderId == orderId)
                    .Include(s => s.Food)
                    .Include(s => s.Order.Employees)
                    .Include(s => s.Order.Tables)
                    .Select(s => new OrderDetailModel
                    {
                        OrderId = s.OrderId,
                        FoodId = s.FoodId,
                        Quantity = s.Quantity,
                        UnitPrice = s.UnitPrice,
                        Note = s.Note,
                        TotalAmount = s.TotalAmount,

                        Foods = _context.Foods.Where(a => a.FoodId == s.FoodId)
                                           .Select(h => new FoodsDTO
                                           {
                                               FoodId = h.FoodId,
                                               NameFood = h.NameFood,
                                               UnitPrice = h.UnitPrice,
                                               UrlImage = h.UrlImage,
                                               CategoryId = h.CategoryId
                                           }).FirstOrDefault() ?? new FoodsDTO(),
                        Orders = new Order_DetailsDTO
                        {
                            OrderId = s.Order.OrderId,
                            EmployeeId = s.Order.Employees != null ? s.Order.Employees.EmployeeId : null,
                            TableId = s.Order.TableId,
                            Code = s.Order.Code,
                            Pay = _context.OrderDetails
                                          .Where(od => od.OrderId == s.Order.OrderId)
                                          .Sum(od => od.TotalAmount),
                            CreationTime = s.Order.CreationTime,
                            PaymentTime = s.Order.PaymentTime??null,
                            ReceivingTime = s.Order.ReceivingTime ?? null,
                            Note = s.Note,
                            CustormerId = s.Order.CustomerId??null,

                            Employees = s.Order.Employees != null ? new EmployeesDTO
                                {
                                    EmployeeId = s.Order.Employees.EmployeeId,
                                    EmployeeName = s.Order.Employees.EmployeeName,
                                    Image = s.Order.Employees.Image,
                                    Phone = s.Order.Employees.Phone,
                                    Email = s.Order.Employees.Email,
                                    Password = s.Order.Employees.Password,
                                } : null,
                            Tables = new TablesDTO
                                {
                                    TableId = s.Order.Tables.TableId,
                                    TableName = s.Order.Tables.TableName,
                                    Code = s.Order.Tables.Code,
                                    Note = s.Order.Tables.Note,
                                    QR_id = s.Order.Tables.QR_id,
                                },
                            Statuss = _context.Statuss
                                            .Where(a => a.Code == s.Order.Code && a.Type == "Order")
                                            .Select(o => new ManageStatusDTO
                                            {
                                                StatusId = o.StatusId,
                                                Code = o.Code,
                                                Type = o.Type,
                                                Value = o.Value,
                                                Description = o.Description,
                                            })
                                            .FirstOrDefault(),

                        }
                    }).ToList();
                return Ok(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bị lỗi: {ex.Message}");
            }
        }


        [HttpPost("createOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateCartDTO cartDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Kiểm tra xem có Order chưa thanh toán nào cho bàn đó không
                var existingOrder = await _context.Orders.FirstOrDefaultAsync(o => o.TableId == cartDto.TableId && o.Code == Constants.ORDER_INIT);

                if (existingOrder != null)
                {
                    // Nếu đã có Order chưa thanh toán, thêm các món mới vào Order đó
                    foreach (var item in cartDto.Items)
                    {
                        var existingOrderDetail = await _context.OrderDetails.FirstOrDefaultAsync(od => od.OrderId == existingOrder.OrderId && od.FoodId == item.Foods.FoodId);
                        var food = await _context.Foods.FindAsync(item.Foods.FoodId);

                        if (existingOrderDetail != null)
                        {
                            // Nếu món đã tồn tại trong Order, cập nhật số lượng
                            existingOrderDetail.Quantity += item.Quantity;
                            existingOrderDetail.TotalAmount += item.Quantity * food.UnitPrice;
                        }
                        else
                        {
                            // Nếu món chưa tồn tại trong Order, tạo một bản ghi mới
                            var orderDetails = new OrderDetails
                            {
                                OrderId = existingOrder.OrderId,
                                Quantity = item.Quantity,
                                UnitPrice = food.UnitPrice,
                                Note = "",
                                TotalAmount = item.Quantity * food.UnitPrice,
                                FoodId = item.Foods.FoodId
                            };
                            _context.OrderDetails.Add(orderDetails);
                        }
                    }
                    // Cập nhật tổng số tiền của Order
                    existingOrder.Pay += cartDto.TotalAmount;
                    await _context.SaveChangesAsync();

                    return Ok("Thêm vào giỏ hàng thành công");
                }
                else
                {
                    // Nếu chưa có Order chưa thanh toán, tạo một Order mới
                    var order = new Order
                    {
                        TableId = cartDto.TableId,
                        CreationTime = DateTime.Now,
                        Code = Constants.ORDER_INIT,
                        Pay = cartDto.TotalAmount,
                        Note = "",
                    };
                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    // Thêm các món vào Order mới
                    foreach (var item in cartDto.Items)
                    {
                        var food = await _context.Foods.FindAsync(item.Foods.FoodId);
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

                    return Ok("Tạo đơn hàng mới thành công");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bị lỗi: {ex.Message}");
            }
        }


        /* // GET: api/orders/search?type=Order
         [HttpGet("search")]
         public async Task<ActionResult<List<Order>>> GetOrdersByType(string type = "Order")
         {
             var orders = await _orderRepository.GetSearchType(type);
             return Ok(orders);
         }*/
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var model = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(i => i.OrderId == id);
                if (model == null)
                {
                    return NotFound("Không tìm thấy mã Order");
                }
                _context.OrderDetails.RemoveRange(model.OrderDetails); //Để xóa list liên quan OrderId
                _context.Orders.Remove(model); // xóa một cái OrderId
                await _context.SaveChangesAsync();
                return Ok("Xóa thành công");
            }
            catch (Exception ex)
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
                // Tính tổng TotalAmount của các OrderDetails cần xóa
                decimal? deletedAmount = orderDetailsToDelete.Sum(od => od.TotalAmount);

                // Xóa OrderDetails
                _context.OrderDetails.RemoveRange(orderDetailsToDelete);

                // Cập nhật lại trường Pay của Order
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
                if (order != null)
                {
                    order.Pay = order.Pay - deletedAmount;
                }
                await _context.SaveChangesAsync();
                return Ok("Xóa OrderDetails thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bị lỗi: {ex.Message}");
            }
        }
        [HttpPut("UpdateOrderDetail/{orderId}/{foodId}")]
        public async Task<IActionResult> UpdateOrderDetail(int orderId, int foodId, [FromBody] OrderDetailUpdateDto orderDetailDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {

                var orderDetailToUpdate = await _context.OrderDetails.FirstOrDefaultAsync(od => od.OrderId == orderId && od.FoodId == foodId);
                if (orderDetailToUpdate == null)
                {
                    return NotFound("Không tìm thấy");
                }
                //Lấy giá tiền của FoodId
                var foodPrice = await _context.Foods.Where(f => f.FoodId == foodId).Select(f => f.UnitPrice).FirstOrDefaultAsync();
                // Cập nhật thông tin của OrderDetail
                orderDetailToUpdate.Quantity = orderDetailDto.Quantity;

                orderDetailToUpdate.Note = orderDetailDto.Note;

                // tính tổng tiền
                orderDetailToUpdate.TotalAmount = orderDetailDto.Quantity * foodPrice;

                // Lấy tổng số tiền của các OrderDetail còn lại của Order
                decimal? totalAmount = _context.OrderDetails
                    .Where(od => od.OrderId == orderId && od.FoodId != foodId)
                    .Sum(od => od.TotalAmount);
                // Cập nhật lại trường Pay của Order
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
                if (order != null)
                {
                    order.Pay = totalAmount + orderDetailToUpdate.TotalAmount;
                }

                await _context.SaveChangesAsync();
                return Ok("Cập nhật OrderDetail thành công");
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

                if (model.Code != Constants.ORDER_INIT)
                {
                    return BadRequest("Trạng thái không phải là đơn mới");
                }
                model.Code = Constants.ORDER_APPROVE;
                model.EmployeeId = EmployeeId;
                model.ReceivingTime = DateTime.Now;


                var orderDTO = new OrderDTO
                {
                    OrderId = model.OrderId,
                    EmployeeId = model.EmployeeId,
                    Code = model.Code,
                    ReceivingTime = model.ReceivingTime,
                    ManageStatuss = _context.Statuss
                        .Where(a => a.Code == model.Code && a.Type == "Order")
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

                if (model.Code != Constants.ORDER_APPROVE)
                {
                    return BadRequest("Trạng thái không phải là đơn đã duyệt");
                }
                model.Code = Constants.ORDER_PAYMENT;
                model.EmployeeId = EmployeeId;
                model.PaymentTime = DateTime.Now;

                //Thanh toán xong trở về bàn trống
                var table = await _context.Tables.FindAsync(model.TableId);
                if (table != null)
                {

                    table.Code = Constants.TABLE_EMPTY;
                    await _context.SaveChangesAsync();
                }

                var orderDTO = new OrderDTO
                {
                    OrderId = model.OrderId,
                    EmployeeId = model.EmployeeId,
                    Code = model.Code,
                    ReceivingTime = model.ReceivingTime,
                    PaymentTime = model.PaymentTime,
                    ManageStatuss = _context.Statuss
                        .Where(a => a.Code == model.Code)
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

                if (model.Code != Constants.ORDER_INIT)
                {
                    return BadRequest("Trạng thái không phải là đơn đã duyệt");
                }
                model.Code = Constants.ORDER_REFUSE;
                model.EmployeeId = EmployeeId;
                var table = await _context.Tables.FindAsync(model.TableId);
                if (table != null)
                {

                    table.Code = Constants.TABLE_EMPTY;
                    await _context.SaveChangesAsync();
                }

                var orderDTO = new OrderDTO
                {
                    OrderId = model.OrderId,
                    EmployeeId = model.EmployeeId,
                    Code = model.Code,
                    ReceivingTime = model.ReceivingTime,
                    PaymentTime = model.PaymentTime,
                    ManageStatuss = _context.Statuss
                        .Where(a => a.Code == model.Code && a.Type == "Order")
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

        /* //Hóa đơn
         [HttpGet(" ")]

         public async Task<IActionResult> GetInvoice(int orderId)
         {
             if (!ModelState.IsValid)
             {
                 return BadRequest(ModelState);
             }
             try
             {
                 var invoice = await _context.OrderDetails.Where(s => s.OrderId == orderId)
                                                 .Include(s => s.FoodId)
                                                 .ToListAsync();
                 if(invoice == null )
                 {

                 }
                 return Ok();
             }
             catch (Exception ex)
             {
                 return StatusCode(500, $"Bị lỗi: {ex.Message}");
             }
         }*/
        //Tổng tiền trong ngày

        [HttpGet("revenue-by-day/{date}")]
        public IActionResult GetRevenueByDay(int date)
        {
            try
            {
                var dailyRevenue = _context.Orders
                    .Where(o => o.Code == Constants.ORDER_PAYMENT &&
                           o.PaymentTime != null &&
                           o.PaymentTime.Value.Day == date)
                    .Sum(o => o.Pay);

                return Ok(dailyRevenue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        //Tính tổng tiền theo tháng năm
        [HttpGet("revenue-by-month/{year}/{month}")]
        public IActionResult GetRevenueByMonth(int year, int month)
        {
            try
            {
                if (month > 12 || month < 0)
                {
                    return BadRequest("Số tháng không hợp lệ");
                }
                var monthlyRevenue = _context.Orders
                    .Where(o => o.Code == Constants.ORDER_PAYMENT && o.PaymentTime.Value.Year == year && o.PaymentTime.Value.Month == month)
                    .Sum(o => o.Pay);

                return Ok(monthlyRevenue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

    }
}