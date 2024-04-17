using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.EmployeeDTO;
using OrderRestaurant.Service;

namespace OrderRestaurant.Responsitory
{
    public class EmployeeResponsitory : IEmployee
    {
        private readonly ApplicationDBContext _dbContext;
        public EmployeeResponsitory(ApplicationDBContext context)
        {
            _dbContext = context;
        }
        public Task<Employee?> CreateEmployee(Employee employee)
        {
            throw new NotImplementedException();
        }

        public async Task<Employee?> DeleteEmployee(int id)
        {
            var model = await _dbContext.Employees.FirstOrDefaultAsync(i => i.EmployeeId == id);
            if (model == null)
            {
                return null;
            }
            _dbContext.Employees.Remove(model);
            await _dbContext.SaveChangesAsync();
            return model;
        }

        public Task<bool> EmployeeExits(int id)
        {
            return _dbContext.Employees.AnyAsync(s => s.EmployeeId == id);
        }

        public async Task<Employee?> GetEmployeeById(int id)
        {
            return await _dbContext.Employees.FindAsync(id);
        }

        public async Task<List<Employee>> GetEmployees()
        {
            return await _dbContext.Employees.ToListAsync();
        }

        public async Task<Employee> UpdateEmployee(int id, CreateEmployeeDTO updateEmployeeDTO)
        {
            var updateEmployee = await _dbContext.Employees.FirstOrDefaultAsync(hh => hh.EmployeeId == id);
            if(updateEmployee == null)
            {
                return null;
            }
            updateEmployee.EmployeeName = updateEmployeeDTO.EmployeeName;
            updateEmployee.Phone = updateEmployeeDTO.Phone;
            updateEmployee.Email = updateEmployeeDTO.Email;
            updateEmployee.Password = updateEmployeeDTO.Password;
            if (updateEmployeeDTO.Image.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    updateEmployeeDTO.Image.CopyTo(ms);
                    var imageBytes = ms.ToArray();
                    var base64String = Convert.ToBase64String(imageBytes);
                    updateEmployee.Image = base64String;
                }
            }
            else
            {
                updateEmployee.Image = "";
            }
            await _dbContext.SaveChangesAsync();
            return updateEmployee;
        }

       
    }
}
