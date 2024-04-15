using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.CustomerDTO;
using OrderRestaurant.Service;

namespace OrderRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly ICustomer _customer;
        public CustomerController(ApplicationDBContext context , ICustomer customer)
        {
            _context = context;
            _customer = customer;
        }

    }

   /* [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerDTO customerDTO)
    {
        var model = customerDTO.ToCustomerFromCreated();
        await _
    }*/
}
