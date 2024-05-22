namespace OrderRestaurant.Helpers
{
    public class QuerryFood
    {
        public string Search { get; set; } = "";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int? CategoryId { get; set; }
    }
}
