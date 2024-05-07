using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.Service;
using System;

namespace OrderRestaurant.Responsitory
{
    public class OrderResponsitory : IOrder
    {
        private readonly ApplicationDBContext _dbContext;
        public OrderResponsitory(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
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


        public async Task<List<Order>> GetAll()
        {
            return await _dbContext.Orders.ToListAsync();
        }

        public Task<List<Order>> GetSearchType(string type = "Order")
        {
            throw new NotImplementedException();
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
    }
}
