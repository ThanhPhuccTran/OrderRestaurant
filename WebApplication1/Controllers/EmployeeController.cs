using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.EmployeeDTO;
using OrderRestaurant.Helpers;
using OrderRestaurant.Model;
using OrderRestaurant.Service;

namespace OrderRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IEmployee _employee;
        private readonly ICommon<EmployeeModel> _common;
        public EmployeeController(ApplicationDBContext context, IEmployee employee, ICommon<EmployeeModel> common)
        {
            _context = context;
            _employee = employee;
            _common = common;
        }
        [HttpGet("get-search-page")]
        public async Task<IActionResult> SearchAndPaginate([FromQuery] QuerryObject parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {


                var (totalItems, totalPages, employee) = await _common.SearchAndPaginate(parameters);

                if (totalItems == 0)
                {
                    return NotFound("Không tìm thấy kết quả");
                }

                var response = new
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    Employee = employee
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpPost("postEmployee")]
        public async Task<IActionResult> CreateEmployeeImage([FromBody] CreateEmployeeDTO p)
        {
            var employee = new Employee { EmployeeName = p.EmployeeName, Phone = p.Phone, Email = p.Email, Password = p.Password , Image = p.Image, RoleName = Constants.ROLE_EMPLOYEE  };

           /* if (p.Image.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    p.Image.CopyTo(ms);
                    var imageBytes = ms.ToArray();
                    var base64String = Convert.ToBase64String(imageBytes);
                    employee.Image = base64String;
                }
            }
            else
            {
                employee.Image = "";
            }*/
            _context.Employees.Add(employee);
            _context.SaveChanges();

            return Ok(new { message = "Thành công",employee });
        }
        [HttpGet]
        public async Task<IActionResult> GetEmployeeAll()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }
            var model = _context.Employees.Select(s => new EmployeeModel
            {
                EmployeeId = s.EmployeeId,
                EmployeeName = s.EmployeeName,
                Email = s.Email,
                Image = s.Image,
                Password = s.Password,
                Phone  = s.Phone,
                RoleName = s.RoleName,
            }).ToList();
            return Ok(model);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _employee.GetEmployeeById(id);
            if(employee == null)
            {
                return NotFound();
            }
            return employee;
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateEmployee([FromRoute] int id, [FromBody] CreateEmployeeDTO employeeDTO)
        {
            var model = await _employee.UpdateEmployee(id, employeeDTO);
            if(model == null)
            {
                return BadRequest("Sửa thất bại");    
            }
            return Ok(model.ToEmployeeDto());
        }

     
        [HttpPut("UpdateEmployeeAdmin")]
        
        public async Task<IActionResult> UpdateEmployeeAdmin(int employeeId,string rolename = Constants.ROLE_ADMIN)
        {
            var model = await _employee.UpdateAdmin(employeeId, rolename);
            if (model == null)
            {
                return BadRequest("Cập nhật thất bại");
            }
            return Ok("cập nhật thành công");
        }

        [HttpDelete]
        [Route("{employeeId}")]
        public async Task<IActionResult> DeleteEmployee([FromRoute] int employeeId)
        {
            var model = await _employee.DeleteEmployee(employeeId);
            if(model == null)
            {
                return NotFound("Không tìm thấy employeeID");
            }
            return NoContent();
        }

    }
}

