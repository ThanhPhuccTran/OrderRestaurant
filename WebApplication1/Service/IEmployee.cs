using OrderRestaurant.Data;
using OrderRestaurant.DTO.EmployeeDTO;

namespace OrderRestaurant.Service
{
    public interface IEmployee
    {
        Task<List<Employee>> GetEmployees();
        Task<Employee?> GetEmployeeById(int id);
        Task<Employee?> CreateEmployee(Employee employee);
        Task<Employee> UpdateEmployee(int id, CreateEmployeeDTO updateEmployeeDTO);
        Task<Employee?> DeleteEmployee(int id);
        Task<bool> EmployeeExits(int id);
    }
}
