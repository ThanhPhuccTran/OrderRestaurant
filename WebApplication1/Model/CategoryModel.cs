using System.ComponentModel.DataAnnotations;

namespace OrderRestaurant.Model
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; }

       // public List<FoodModel> FoodM { get; set; } = new List<FoodModel>();
    }
}
