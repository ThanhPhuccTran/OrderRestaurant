using OrderRestaurant.DTO.FoodDTO;

namespace OrderRestaurant.DTO.OrderDetailsDTO
{
    public class OrderDetailsStatistics
    {
        public int FoodId { get; set; }
        public FoodsDTO Foods { get; set; }
        public int UsageCount { get; set; }
    }
}
