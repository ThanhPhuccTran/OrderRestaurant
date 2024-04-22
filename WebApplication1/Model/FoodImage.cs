using OrderRestaurant.Data;

namespace OrderRestaurant.Model
{
    public class FoodImage
    {
        public string NameFood { get; set; }
        public decimal UnitPrice { get; set; }
        public int CategoryId { get; set; }
        public string Image {  get; set; }
    }
}
