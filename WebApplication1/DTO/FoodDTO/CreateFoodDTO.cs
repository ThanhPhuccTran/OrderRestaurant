namespace OrderRestaurant.DTO.FoodDTO
{
    public class CreateFoodDTO
    {
        public string NameFood { get; set; }
        public decimal UnitPrice { get; set; }

        public string? ImageFile { get; set; }
        public int CategoryId { get; set; }
    }
}
