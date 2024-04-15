using OrderRestaurant.Data;
using OrderRestaurant.DTO.CategoryDTO;
using OrderRestaurant.DTO.CustomerDTO;

namespace OrderRestaurant.Service
{
    public interface ICustomer
    {
        Task<List<Customer>> GetCustomers();
        Task<Customer?> GetCustomersById(int id);
        Task<Customer> CreateCustomer (Customer customer);
        Task<Customer> UpdateCustomer (int id , CreateCustomerDTO updateCustomerDTO);
        Task<Customer?> DeleteCustomer(int id);
    }
}
