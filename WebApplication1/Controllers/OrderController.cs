using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.Cart;
using OrderRestaurant.DTO.CartDTO;
using OrderRestaurant.DTO.ConfigDTO;
using OrderRestaurant.DTO.EmployeeDTO;
using OrderRestaurant.DTO.FoodDTO;
using OrderRestaurant.DTO.InvoiceDTO;
using OrderRestaurant.DTO.OrderDetailsDTO;
using OrderRestaurant.DTO.OrderDTO;
using OrderRestaurant.DTO.TableDTO;
using OrderRestaurant.Helpers;
using OrderRestaurant.Model;
using OrderRestaurant.Reponsitory;
using OrderRestaurant.Service;
using System.Globalization;
using System.Security.Claims;
using static Azure.Core.HttpHeader;

namespace OrderRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrder _orderRepository;
        private readonly ApplicationDBContext _context;
        private readonly ICommon<OrderModel> _common;
        private readonly IPermission _permissionRepository;
        private const string TYPE_Order = "Order";
        private const string TYPE_OrderDetail = "OrderDetail";
        public OrderController(ApplicationDBContext context, IOrder orderRepository, ICommon<OrderModel> common, IPermission permissionRepository)
        {
            _orderRepository = orderRepository;
            _context = context;
            _common = common;
            _permissionRepository = permissionRepository;
        }
        [HttpGet("get-search-page")]
        public async Task<IActionResult> SearchAndPaginate([FromQuery] QuerryObject parameters)
        {
            var (totalItems, totalPages, orders) = await _common.SearchAndPaginate(parameters);

            if (totalItems == 0)
            {
                return NotFound("Không tìm thấy kết quả");
            }

            var response = new
            {
                TotalItems = totalItems,
                TotalPages = totalPages,
                Orders = orders
            };

            return Ok(response);
        }

        [HttpGet("get-search")]
        public async Task<IActionResult> SearchAndPaginate([FromQuery] QuerryOrder parameters)
        {
            var (totalItems, totalPages, orders) = await _orderRepository.SearchAndPaginate(parameters);

            if (totalItems == 0)
            {
                return NotFound("Không tìm thấy kết quả");
            }

            var response = new
            {
                TotalItems = totalItems,
                TotalPages = totalPages,
                Orders = orders
            };

            return Ok(response);
        }

        [HttpGet("get-order-all")]
        public async Task<IActionResult> GetAll()
        {


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var orders = await _orderRepository.GetAllOrders();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
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
                var orderDetails = await _orderRepository.GetOrderDetails(orderId);
                return Ok(orderDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
         }


        [HttpPost("createOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateCartDTO cartDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _orderRepository.CreateOrderAsync(cartDto);
            if (result)
            {
                return Ok("THÊM VÀO GIỎ ĐƠN HÀNG THÀNH CÔNG");
            }
            else
            {
                return StatusCode(500, "Đã xảy ra lỗi khi tạo đơn hàng");
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
                var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (roleName == null)
                {
                    return NotFound("Ko co rolename");
                }

                if (!_permissionRepository.CheckPermission(roleName, Constants.Delete, TYPE_Order))
                    return Unauthorized();
                var result = await _orderRepository.DeleteOrderAsync(id);
                if (result)
                {
                    return Ok("Xóa thành công");
                }
                else
                {
                    return NotFound("Không tìm thấy mã Order hoặc đã xảy ra lỗi khi xóa");
                }
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
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
                var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (roleName == null)
                {
                    return NotFound("Ko co rolename");
                }

                if (!_permissionRepository.CheckPermission(roleName, Constants.Delete, TYPE_OrderDetail))
                    return Unauthorized();
                var result = await _orderRepository.DeleteOrderDetailAsync(orderId, foodId);
                if (result)
                {
                    return Ok("Xóa OrderDetails thành công");
                }
                else
                {
                    return NotFound("Không tìm thấy OrderDetails hoặc đã xảy ra lỗi khi xóa");
                }
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
                var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (roleName == null)
                {
                    return NotFound("Ko co rolename");
                }

                if (!_permissionRepository.CheckPermission(roleName, Constants.Put, TYPE_OrderDetail))
                    return Unauthorized();
                var result = await _orderRepository.UpdateOrderDetailAsync(orderId, foodId, orderDetailDto);
                if (result)
                {
                    return Ok("Cập nhật OrderDetail thành công");
                }
                else
                {
                    return NotFound("Không tìm thấy OrderDetail hoặc đã xảy ra lỗi khi cập nhật");
                }
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
                var model = await _orderRepository.FindOrderById(OrderId);
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

                var table = await _context.Tables.FindAsync(model.TableId);
                if (table != null)
                {

                    table.Code = Constants.TABLE_GUESTS;
                    await _context.SaveChangesAsync();
                }
                var orderDTO = new OrderDTO
                {
                    OrderId = model.OrderId,
                    EmployeeId = model.EmployeeId,
                    TableId = model.TableId,
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
        [HttpPost("PaymentOrder/{TableId}/{EmployeeId}")]
        public async Task<IActionResult> PaymentOrder(int TableId, int EmployeeId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var models = await _orderRepository.FindOrdersByTable(TableId, Constants.ORDER_APPROVE);
                if (models == null || models.Count == 0)
                {
                    return NotFound("Không tìm thấy");
                }

                foreach (var model in models)
                {
                    if (model.TableId == TableId && model.Code == Constants.ORDER_APPROVE)
                    {
                        model.Code = Constants.ORDER_PAYMENT;
                        model.EmployeeId = EmployeeId;
                        model.PaymentTime = DateTime.Now;

                        //Thanh toán xong trở về bàn trống
                        var table = await _context.Tables.FindAsync(model.TableId);
                        if (table != null)
                        {
                            table.Code = Constants.TABLE_EMPTY;
                        }
                    }
                }

                await _context.SaveChangesAsync();

                // Tạo danh sách các DTO
                var orderDTOs = new List<OrderDTO>();
                foreach (var model in models)
                {
                    var orderDTO = new OrderDTO
                    {
                        OrderId = model.OrderId,
                        EmployeeId = model.EmployeeId,
                        Code = model.Code,
                        TableId = model.TableId,
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
                    orderDTOs.Add(orderDTO);
                }

                return Ok(orderDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bị lỗi: {ex.Message}");
            }
        }
        [HttpGet("get_bill")]
        public async Task<IActionResult> GetBill(int tableId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var model = await _orderRepository.FindOrdersByTable(tableId, Constants.ORDER_APPROVE);

                if (model == null || model.Count == 0)
                {
                    return NotFound("Không tìm thấy hóa đơn cho bàn này.");
                }

                
                // Tong tien Order
                decimal ToTalOrder = (decimal) model.Sum(o => o.Pay);
                var orderDetails = model.Select(o => new GetInvoiceDTO
                {
                    OrderId = o.OrderId,
                    EmployeeId = o.EmployeeId,
                    TableId = o.TableId,
                    CreationTime = o.CreationTime,
                    ReceivingTime = o.ReceivingTime,
                    PaymentTime = o.PaymentTime,
                    Pay = o.Pay,
                    Employees = _context.Employees
                                        .Where(a => a.EmployeeId == o.EmployeeId)
                                        .Select(s => new EmployeesDTO
                                        {
                                            EmployeeId = s.EmployeeId,
                                            EmployeeName = s.EmployeeName,
                                            Email = s.Email,
                                            Password = s.Password,
                                            Phone = s.Password,
                                            Image = s.Image

                                        }).FirstOrDefault(),
                    Tables = _context.Tables.Where(a => a.TableId == o.TableId)
                                    .Select(s => new TablesDTO
                                    {
                                        TableId = s.TableId,
                                        TableName = s.TableName,
                                        Note = s.Note,
                                        QR_id = s.QR_id,
                                        Code = s.Code
                                    }).FirstOrDefault(),
                   }).ToList();
                // Lặp qua mỗi đơn hàng để lấy danh sách món ăn và tính tổng số lượng mỗi món ăn
                foreach (var order in orderDetails)
                {
                    order.Foods = _context.OrderDetails
                                        .Where(od => od.OrderId == order.OrderId)
                                        .Select(od => new FoodInvoiceDTO
                                        {
                                            FoodId = od.FoodId,
                                            NameFood = od.Food.NameFood,
                                            UrlImage = od.Food.UrlImage,
                                            UnitPrice = od.Food.UnitPrice,
                                            Quantity = od.Quantity,
                                            TotalPrice = od.Quantity * od.Food.UnitPrice
                                        })
                                        .GroupBy(food => food.FoodId)
                                        .Select(group => new FoodInvoiceDTO
                                        {
                                            FoodId = group.Key,
                                            NameFood = group.First().NameFood,
                                            UrlImage = group.First().UrlImage,
                                            UnitPrice = group.First().UnitPrice,
                                            Quantity = group.Sum(food => food.Quantity),
                                            TotalPrice = group.Sum(food => food.Quantity * food.UnitPrice)
                                        })
                                        .ToList();
                }

                // Tạo danh sách allFoods
                var allFoods = orderDetails.SelectMany(order => order.Foods).ToList();
                // Nhóm các món ăn theo foodId và tính tổng số lượng
                var groupedFoods = allFoods.GroupBy(food => food.FoodId)
                                            .Select(group => new FoodInvoiceDTO
                                            {
                                                FoodId = group.Key,
                                                NameFood = group.First().NameFood,
                                                UrlImage = group.First().UrlImage,
                                                UnitPrice = group.First().UnitPrice,
                                                Quantity = group.Sum(food => food.Quantity),
                                                TotalPrice = group.Sum(food => food.Quantity * food.UnitPrice)

                                            })
                                            .ToList();

              
                var orderData = new
                {
                    TableId = tableId,
                    TotalAmount = ToTalOrder,
                    AllFoods = groupedFoods,
                    Orders = orderDetails,
                    
                };
                return Ok(orderData);
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
                var model = await _orderRepository.FindOrderById(OrderId);
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

        
        

    }
}