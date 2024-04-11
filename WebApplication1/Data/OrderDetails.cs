using OrderRestaurant.Migrations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderRestaurant.Data
{
    [Table("OrderDetails")]
    public partial class OrderDetails
    {

        public int OrderId { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public double UnitPrice {get;set;}
        public string Note { get; set; }
        public int FoodId { get; set; }

        [ForeignKey("FoodId")]
        public Food Food { get; set; }
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }
    }
}
