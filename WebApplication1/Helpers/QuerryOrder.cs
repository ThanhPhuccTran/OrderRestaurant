namespace OrderRestaurant.Helpers
{
    public class QuerryOrder
    {
        public string Search { get; set; } = "";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int? Code { get; set; }
        public DateTime? fromTime { get; set; }
        public DateTime? toTime { get; set;}
    }
}
