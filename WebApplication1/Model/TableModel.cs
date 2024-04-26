namespace OrderRestaurant.Model
{
    public class TableModel
    {
        public int TableId { get; set; }
        public string TableName { get; set; }
        public int Code { get; set; }
        public string? Note { get; set; }
        public string QR_id { get; set; }
    }
}
