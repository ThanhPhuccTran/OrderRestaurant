namespace OrderRestaurant.DTO.FoodDTO
{
    public class FoodsDTO
    {
        public int FoodId { get; set; }
        public string? NameFood { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? UrlImage { get; set; }
        public int? CategoryId { get; set; }
    }
}
