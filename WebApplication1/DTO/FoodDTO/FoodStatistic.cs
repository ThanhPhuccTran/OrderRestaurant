using OrderRestaurant.Data;
using OrderRestaurant.Model;

namespace OrderRestaurant.DTO.FoodDTO
{
    public class FoodStatistic
    {
        public int FoodId { get; set; }
        public string FoodName { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? UrlImage { get; set; }
        public int? CategoryId { get; set; }
        public CategoryModel Categorys { get; set; }
        public int QuantitySold { get; set; }
    }
}
