using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderRestaurant.Data
{
    [Table("Roles")]
    public partial class Roles
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; } 
        public List<Employee> Employees { get; set; }
    }
}
