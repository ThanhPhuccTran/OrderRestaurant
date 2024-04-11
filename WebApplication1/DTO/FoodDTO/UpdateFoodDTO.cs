namespace OrderRestaurant.DTO.FoodDTO
{
    public class UpdateFoodDTO
    {
        public string NameFood { get; set; }
        public double UnitPrice { get; set; }
        public int CategoryId { get; set; }
        public IFormFile Image { get; set; }
    }
}
