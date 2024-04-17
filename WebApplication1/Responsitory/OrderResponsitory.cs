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

        public async Task<List<Order>> GetAll()
        {
            return await _dbContext.Orders.ToListAsync();
        }
        public async Task<Order> GetAsync(int id)
        {
            return await _dbContext.Orders.FindAsync(id);
        }

        public async Task UpdateAsync(Order order)
        {
            _dbContext.Entry(order).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}
