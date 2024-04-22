using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderRestaurant.Data
{
    [Table("Cart")]
    public partial class Cart
    {
        [Key]
        public int CartId { get; set; }
        public int TableId { get; set; }
        public int FoodId { get; set; }
        public int StatusId { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;

        [ForeignKey("TableId")]
        public Table TableCart { get; set; }
        [ForeignKey("FoodId")]
        public Food FoodCart { get; set; }
        [ForeignKey("EmployeeId")]
        public Employee? EmployeeCart { get; set; }

        [ForeignKey("StatusId")] 
        public ManageStatus ManageStatusCart { get; set; }
    }
}
