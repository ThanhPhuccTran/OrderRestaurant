using OrderRestaurant.Data;

namespace OrderRestaurant.DTO.Cart
{
    public class CartItemModel
    {
        public int FoodId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        //  public string Note { get; set; }
        public decimal? TotalMoney => Quantity * UnitPrice;


    }
}
