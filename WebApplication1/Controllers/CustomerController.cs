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
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerDTO customerDTO)
        {
            var model = customerDTO.ToCustomerFromCreated();
            await _customer.CreateCustomer(model);
            return CreatedAtAction(nameof(GetCustomerById), new { id = model.CustomerId }, model.ToCustomerDto()); ;
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetCustomerById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var model = await _customer.GetCustomersById(id);
            if(model == null)
            {
                return BadRequest(ModelState);
            }
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerAll()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var model = await _customer.GetCustomers();
            var list = model.Select(hh => hh.ToCustomerDto());
            return Ok(list);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateCustomer([FromRoute] int id, [FromBody] CreateCustomerDTO updateCustomerDTO)
        {
            var model = await _customer.UpdateCustomer(id, updateCustomerDTO);
            if(model == null)
            {
                return BadRequest();
            }
            return Ok(model.ToCustomerDto());
        }

        [HttpDelete]
        [Route("{customerid}")]
        public async Task<IActionResult> DeleteCustomer([FromRoute]int customerid)
        {
            var model = await _customer.DeleteCustomer(customerid);
            if(model == null)
            {
                return NotFound();
            }
            return NoContent();
        }
    }

    
}
