using OrderRestaurant.Data;

namespace OrderRestaurant.Model
{
    public class CartItemModel
    {
        public Food foods { get; set; }
        public int Quantity { get; set; }
        public string Note { get; set; }
        public double TotalMoney => Quantity * foods.UnitPrice;

    }
}
