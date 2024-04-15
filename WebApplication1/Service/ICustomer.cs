using OrderRestaurant.Data;

namespace OrderRestaurant.Service
{
    public interface ICustomer
    {
        Task<List<Customer>> GetCustomers();
        Task<List<Customer?>> GetCustomersById(int id);
        Task<Customer> CreateCustomer (Customer customer);

    }
}
