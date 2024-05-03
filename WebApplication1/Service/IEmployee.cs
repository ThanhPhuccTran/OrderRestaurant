using OrderRestaurant.Data;
using OrderRestaurant.DTO.EmployeeDTO;
using OrderRestaurant.Helpers;

namespace OrderRestaurant.Service
{
    public interface IEmployee
    {
        Task<List<Employee>> GetEmployees();
        Task<Employee?> GetEmployeeById(int id);
        Task<Employee?> CreateEmployee(Employee employee);
        Task<Employee> UpdateEmployee(int id, CreateEmployeeDTO updateEmployeeDTO);
        Task<Employee>UpdateAdmin(int id , string rolename);
        Task<Employee?> DeleteEmployee(int id);
        Task<bool> EmployeeExits(int id);
        Task<(int totalItems, int totalPages, List<Employee> lstemployee)> GetSearchEmployee(QuerryObject querry, string search = "");
    }
}
