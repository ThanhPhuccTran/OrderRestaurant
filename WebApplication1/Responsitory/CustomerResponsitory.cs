using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.Service;

namespace OrderRestaurant.Responsitory
{
    public class CustomerResponsitory : ICustomer
    {
        private readonly ApplicationDBContext _dbContext;
        public CustomerResponsitory(ApplicationDBContext context)
        {
            _dbContext = context;
        }
        public async Task<Customer> CreateCustomer(Customer customer)
        {
            await _dbContext.Customers.AddAsync(customer);
            await _dbContext.SaveChangesAsync();
            return customer;
        }

        public async Task<List<Customer>> GetCustomers()
        {
            return await _dbContext.Customers.ToListAsync();
        }

        public Task<List<Customer?>> GetCustomersById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
