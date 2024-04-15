using OrderRestaurant.Data;
using OrderRestaurant.Model;

namespace OrderRestaurant.DTO.CustomerDTO
{
    public static class CustomerMapper
    {
        public static CustomerModel ToCustomerDto (this Customer model)
        {
            return new CustomerModel
            {
                CustomerId = model.CustomerId,
                CustomerName = model.CustomerName,
                Phone = model.Phone,
            };
        }

        public static Customer ToCustomerFromCreated(this CreateCustomerDTO dto)
        {
            return new Customer
            {
                CustomerName = dto.CustomerName,
                Phone = dto.Phone,
            };
        }
    }
}
