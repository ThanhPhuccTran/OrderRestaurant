using OrderRestaurant.Data;
using OrderRestaurant.Model;

namespace OrderRestaurant.DTO.CartDTO
{
    public class CartList
    {
       public FoodCart Foods { get; set; }
       public int Quantity { get; set; }
       public decimal TotalAmount { get; set; }
    }
}
