using OrderRestaurant.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using OrderRestaurant.DTO.FoodDTO;
using OrderRestaurant.DTO.OrderDTO;

namespace OrderRestaurant.Model
{
    public class OrderDetailModel
    {
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal? UnitPrice { get; set; }

        public string? Note { get; set; }
        public decimal? TotalAmount { get; set; }
        public int FoodId { get; set; }
        public FoodsDTO Foods { get; set; }
        public Order_DetailsDTO? Orders { get; set; }
    }
}
