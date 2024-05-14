namespace OrderRestaurant.DTO.FoodDTO
{
    public class FoodStatisticsResult
    {
        public int TotalFood { get; set; }
        public List<FoodStatistic> Food { get; set; }
    }
}
