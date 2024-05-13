using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderRestaurant.Data
{
    [Table("Permission")]
    public class Permission
    {
        [StringLength(50)]
        public string RoleName { get; set; }
        public string Function { get; set; }
        public string FunctionTable { get; set; }
      
    }
}
