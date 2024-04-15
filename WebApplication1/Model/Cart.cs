using OrderRestaurant.DTO.Cart;

namespace OrderRestaurant.Model
{
    public class Cart
    {
        public List<CartItemModel> Items { get; set; }

        public Cart()
        {
            Items = new List<CartItemModel>();
        }
    }
}
