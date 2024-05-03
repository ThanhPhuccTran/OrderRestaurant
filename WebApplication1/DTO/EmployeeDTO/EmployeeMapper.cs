using OrderRestaurant.Data;
using OrderRestaurant.Model;
using System.Runtime.CompilerServices;

namespace OrderRestaurant.DTO.EmployeeDTO
{
    public static class EmployeeMapper
    {
        public static EmployeeModel ToEmployeeDto (this Employee model)
        {
            return new EmployeeModel
            {
                EmployeeId = model.EmployeeId,
                EmployeeName = model.EmployeeName,
                Image = model.Image,
                Phone= model.Phone,
                Email = model.Email,
                Password = model.Password,
                RoleName = model.RoleName,
            };
        }
    }
}
