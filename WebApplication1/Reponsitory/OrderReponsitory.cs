using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.CartDTO;
using OrderRestaurant.DTO.ConfigDTO;
using OrderRestaurant.DTO.EmployeeDTO;
using OrderRestaurant.DTO.FoodDTO;
using OrderRestaurant.DTO.OrderDetailsDTO;
using OrderRestaurant.DTO.OrderDTO;
using OrderRestaurant.DTO.TableDTO;
using OrderRestaurant.Helpers;
using OrderRestaurant.Hubs;
using OrderRestaurant.Model;
using OrderRestaurant.Service;
using System;

namespace OrderRestaurant.Responsitory
{
    public class OrderReponsitory : IOrder, ICommon<OrderModel>
    {
        private readonly ApplicationDBContext _dbContext;
      /*  private readonly IHubContext<NotificationHub> _hubContext;*/
        public OrderReponsitory(ApplicationDBContext dbContext/*, IHubContext<NotificationHub> hubContext*/)
        {
            _dbContext = dbContext;
           /* _hubContext = hubContext;*/
        }

        public async Task<bool> CreateOrderAsync(CreateCartDTO cartDto)
        {
            try
            {
                var order = new Order
                {
                    TableId = cartDto.TableId,
                    CreationTime = DateTime.Now,
                    Code = Constants.ORDER_INIT,
                    Pay = cartDto.TotalAmount,
                    Note = ""
                };
                _dbContext.Orders.Add(order);
                await _dbContext.SaveChangesAsync();

                foreach (var item in cartDto.Items)
                {
                    var food = await _dbContext.Foods.FindAsync(item.Foods.FoodId);
                    if (food == null)
                    {
                        return false; // Không tìm thấy món ăn
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
                    _dbContext.OrderDetails.Add(orderDetails);
                }
                var table = await _dbContext.Tables.FindAsync(cartDto.TableId);
                if (table == null)
                {
                    return false;
                }    

                //Notifi
                var notifi = new Notification
                {
                    Title = "Có đơn mới",
                    Content = $"{table.TableName}",
                    Type = "Order",
                    IsCheck = false,
                    CreatedAt = DateTime.Now,
                };
                _dbContext.Notifications.Add(notifi);
               // await _hubContext.Clients.All.SendAsync("ReceiveOrderNotification", notifi);
                await _dbContext.SaveChangesAsync();
                return true; // Thành công
            }
            catch
            {
                return false; // Gặp lỗi
            }
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            try
            {
                var order = await _dbContext.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(i => i.OrderId == orderId);
                if (order == null)
                {
                    return false; // Không tìm thấy mã Order
                }

                _dbContext.OrderDetails.RemoveRange(order.OrderDetails); // Xóa các OrderDetails liên quan đến OrderId
                _dbContext.Orders.Remove(order); // Xóa một đơn hàng
                await _dbContext.SaveChangesAsync();
                return true; // Xóa thành công
            }
            catch
            {
                return false; // Gặp lỗi
            }

        }

        public async Task<bool> DeleteOrderDetailAsync(int orderId, int foodId)
        {
            try
            {
                // Tìm kiếm OrderDetails cần xóa
                var orderDetailsToDelete = _dbContext.OrderDetails
                    .Where(od => od.OrderId == orderId && od.FoodId == foodId)
                    .ToList();

                // Kiểm tra nếu không tìm thấy OrderDetails
                if (orderDetailsToDelete.Count == 0)
                {
                    return false; // Không tìm thấy OrderDetails
                }

                // Tính tổng TotalAmount của các OrderDetails cần xóa
                decimal? deletedAmount = orderDetailsToDelete.Sum(od => od.TotalAmount);

                // Xóa OrderDetails 
                _dbContext.OrderDetails.RemoveRange(orderDetailsToDelete);

                // Cập nhật lại trường Pay của Order
                var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
                if (order != null)
                {
                    order.Pay = order.Pay - deletedAmount;
                }
                await _dbContext.SaveChangesAsync();
                return true; // Xóa OrderDetails thành công
            }
            catch
            {
                return false; // Gặp lỗi
            }
        }

        public async Task<Order> FindOrderById(int orderid)
        {
            return await _dbContext.Orders.FindAsync(orderid);
        }

        public async Task<List<Order>> FindOrdersByTable(int tableId, int code)
        {
           /* DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);*/
            return await _dbContext.Orders
                .Where(o => o.TableId == tableId && o.Code == code /*&& o.CreationTime >= today && o.CreationTime < tomorrow*/)
                .ToListAsync();
        }

     
        public async Task<List<OrderModel>> GetAllOrders()
        {
            var orders = await _dbContext.Orders
                               .OrderByDescending(s => s.CreationTime)
                               .ToListAsync();

            var orderModels = orders.Select(s => new OrderModel
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
                Employees = _dbContext.Employees
                                    .Where(a => a.EmployeeId == s.EmployeeId)
                                    .Select(o => new EmployeesDTO
                                    {
                                        EmployeeId = o.EmployeeId,
                                        EmployeeName = o.EmployeeName,
                                        Email = o.Email,
                                        Password = o.Password,
                                        Phone = o.Phone,
                                        Image = o.Image
                                    })
                                    .FirstOrDefault(),
                Statuss = _dbContext.Statuss
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
                Tables = _dbContext.Tables.Where(a => a.TableId == s.TableId)
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

            return orderModels;
        }

        public async Task<List<OrderDetailModel>> GetOrderDetails(int orderId)
        {
            try
            {
                var orderDetails = await _dbContext.OrderDetails
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

                        Foods = _dbContext.Foods.Where(a => a.FoodId == s.FoodId)
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
                            Pay = _dbContext.OrderDetails
                                .Where(od => od.OrderId == s.Order.OrderId)
                                .Sum(od => od.TotalAmount),
                            CreationTime = s.Order.CreationTime,
                            PaymentTime = s.Order.PaymentTime ?? null,
                            ReceivingTime = s.Order.ReceivingTime ?? null,
                            Note = s.Note,
                            CustormerId = s.Order.CustomerId ?? null,

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

                            Statuss = _dbContext.Statuss
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
                    }).ToListAsync();

                return orderDetails;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi: {ex.Message}");
            }
        }

        public async Task<(int totalItems, int totalPages, List<OrderModel> items)> SearchAndPaginate(QuerryObject querryObject)
        {
            var query = _dbContext.Orders
                                  .OrderByDescending(s => s.CreationTime)
                                  .Include(o => o.Customers)
                                  .Include(o => o.Tables)
                                  .Include(o => o.Employees)
                                  .AsQueryable();

            // Áp dụng tìm kiếm nếu có
            if (!string.IsNullOrWhiteSpace(querryObject.Search))
            {
                query = query.Where(f => EF.Functions.Like(f.Tables.TableName, $"%{querryObject.Search}%"));
            }

            // Tính toán số trang và lấy dữ liệu phân trang
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / querryObject.PageSize);
            var skipNumber = (querryObject.PageNumber - 1) * querryObject.PageSize;
            var items = await query.Skip(skipNumber).Take(querryObject.PageSize).ToListAsync();
            var orderModels = items.Select(s => new OrderModel
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
                Employees = _dbContext.Employees
                                   .Where(a => a.EmployeeId == s.EmployeeId)
                                   .Select(o => new EmployeesDTO
                                   {
                                       EmployeeId = o.EmployeeId,
                                       EmployeeName = o.EmployeeName,
                                       Email = o.Email,
                                       Password = o.Password,
                                       Phone = o.Phone,
                                       Image = o.Image
                                   })
                                   .FirstOrDefault(),
                Statuss = _dbContext.Statuss
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
                Tables = _dbContext.Tables.Where(a => a.TableId == s.TableId)
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
            return (totalItems, totalPages, orderModels);
        }


        /* public async Task<List<Order>> GetSearchType(string type = "Order")
         {
             var query = _dbContext.Orders.AsQueryable();
             if (!string.IsNullOrWhiteSpace(type))
             {
                 query = query.Where(s => s.Statuss.Type == type);
             }

             var orders = await query
                 .Select(f => new Order
                 {
                     OrderId = f.OrderId,
                     CreationTime = f.CreationTime,
                     StatusId = f.StatusId,
                     Pay = f.Pay,
                     Note = f.Note,
                     TableId = f.TableId,
                     EmployeeId = f.EmployeeId,
                     CustomerId = f.CustomerId,
                     PaymentTime = f.PaymentTime,
                     ReceivingTime = f.ReceivingTime,
                 })
                 .ToListAsync();

             return orders;
         }*/


        public async Task UpdateAsync(Order order)
        {
            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateOrderDetailAsync(int orderId, int foodId, OrderDetailUpdateDto orderDetailDto)
        {
            try
            {
                var orderDetailToUpdate = await _dbContext.OrderDetails.FirstOrDefaultAsync(od => od.OrderId == orderId && od.FoodId == foodId);
                if (orderDetailToUpdate == null)
                {
                    return false; // Không tìm thấy OrderDetail
                }

                // Lấy giá tiền của FoodId
                var foodPrice = await _dbContext.Foods.Where(f => f.FoodId == foodId).Select(f => f.UnitPrice).FirstOrDefaultAsync();

                // Cập nhật thông tin của OrderDetail
                orderDetailToUpdate.Quantity = orderDetailDto.Quantity;
                orderDetailToUpdate.Note = orderDetailDto.Note;

                // tính tổng tiền
                orderDetailToUpdate.TotalAmount = orderDetailDto.Quantity * foodPrice;

                // Lấy tổng số tiền của các OrderDetail còn lại của Order
                decimal? totalAmount = _dbContext.OrderDetails
                    .Where(od => od.OrderId == orderId && od.FoodId != foodId)
                    .Sum(od => od.TotalAmount);

                // Cập nhật lại trường Pay của Order
                var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
                if (order != null)
                {
                    order.Pay = totalAmount + orderDetailToUpdate.TotalAmount;
                }

                await _dbContext.SaveChangesAsync();
                return true; // Cập nhật OrderDetail thành công
            }
            catch
            {
                return false; // Gặp lỗi
            }
        }
    }
}
