using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.CustomerDTO;
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

        public async Task<Customer?> DeleteCustomer(int id)
        {
            var model = await _dbContext.Customers.FirstOrDefaultAsync(hh=>hh.CustomerId == id);
            if(model == null)
            {
                return null;
            }
            _dbContext.Customers.Remove(model);
            await _dbContext.SaveChangesAsync();
            return model;

        }

        public async Task<List<Customer>> GetCustomers()
        {
            return await _dbContext.Customers.ToListAsync();
        }

        public async Task<Customer?> GetCustomersById(int id)
        {
            return await _dbContext.Customers.FindAsync(id);
        }

        public async Task<Customer> UpdateCustomer(int id, CreateCustomerDTO updateCustomerDTO)
        {
           var updateCustomer = await _dbContext.Customers.FirstOrDefaultAsync(hh=>hh.CustomerId==id);  
            if(updateCustomer == null)
            {
                return null;
            }    
            updateCustomer.CustomerName = updateCustomerDTO.CustomerName;
            updateCustomer.Phone = updateCustomerDTO.Phone;
            await _dbContext.SaveChangesAsync();
            return updateCustomer;
        }
    }
}
