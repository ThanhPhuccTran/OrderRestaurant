using OrderRestaurant.Data;

namespace OrderRestaurant.DTO.Cart
{
    public class CartItemModel
    {
        public Food foods { get; set; }
        public int Quantity { get; set; }
        public string Note { get; set; }
        public decimal TotalMoney => Quantity * foods.UnitPrice;


    }
}
