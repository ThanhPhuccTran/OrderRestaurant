using OrderRestaurant.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OrderRestaurant.Model
{
    public class OrdertailsModel
    {
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public string Note { get; set; }
        public int FoodId { get; set; }
        public Food Food { get; set; }
        
    }
}
