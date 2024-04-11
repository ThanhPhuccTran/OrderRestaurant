namespace OrderRestaurant.DTO.FoodDTO
{
    public class CreateFoodDTO
    {
        public string NameFood { get; set; }
        public double UnitPrice { get; set; }

        public IFormFile ImageFile { get; set; }
        public int CategoryId { get; set; }
    }
}
