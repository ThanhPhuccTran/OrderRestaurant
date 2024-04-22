using OrderRestaurant.DTO.Cart;

namespace OrderRestaurant.Model
{
    public class CartDATA
    {
        public List<CartItemModel> Items { get; set; }

        public CartDATA()
        {
            Items = new List<CartItemModel>();
        }
    }
}
