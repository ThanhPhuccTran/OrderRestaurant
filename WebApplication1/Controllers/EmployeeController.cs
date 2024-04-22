﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.EmployeeDTO;
using OrderRestaurant.Helpers;
using OrderRestaurant.Service;

namespace OrderRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IEmployee _employee;
        public EmployeeController(ApplicationDBContext context, IEmployee employee)
        {
            _context = context;
            _employee = employee;

        }
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] QuerryObject querry , string search = "")
        {
            if(querry.PageNumber <=0 || querry.PageSize <= 0)
            {
                return BadRequest("Không hợp lệ");
            }
            var (totalItems, totalPages, employee) = await _employee.GetSearchEmployee(querry,search);
            if (totalItems == 0)
            {
                return NotFound("Không tìm thấy kết quả");
            }
            var response = new
            {
                TotalItems = totalItems,
                TotalPages = totalPages,
                Employees = employee
            };
            return Ok(response);
        }
        [HttpPost("postEmployee")]
        public async Task<IActionResult> CreateEmployeeImage([FromBody] CreateEmployeeDTO p)
        {
            var employee = new Employee { EmployeeName = p.EmployeeName, Phone = p.Phone, Email = p.Email, Password = p.Password , Image = p.Image };

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
            var model = await _employee.GetEmployees();
            var list = model.Select(hh => hh.ToEmployeeDto());
            return Ok(list );
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

