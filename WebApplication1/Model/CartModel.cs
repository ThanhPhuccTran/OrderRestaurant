using OrderRestaurant.Data;

namespace OrderRestaurant.Model
{
    public class CartModel
    {
        public int CartId { get; set; }
        public int TableId { get; set; }
        public int FoodId { get; set; }
        public int StatusId { get; set; }
        public int? EmployeeId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;
       
        public Table TableCart { get; set; }
     
        public Food FoodCart { get; set; }
       
        public Employee? EmployeeCart { get; set; }

       
        public ManageStatus ManageStatusCart { get; set; }
    }
}
