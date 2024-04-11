using System.ComponentModel.DataAnnotations;

namespace OrderRestaurant.Model
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }
        public string TenLoai { get; set; } = string.Empty;
    }
}
