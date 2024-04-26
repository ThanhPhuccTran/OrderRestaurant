namespace OrderRestaurant.DTO.TableDTO
{
    public class TablesDTO
    {
        public int TableId { get; set; }
        public string TableName { get; set; }
        public int Code { get; set; }
        public string? Note { get; set; }
        public string QR_id { get; set; }
    }
}
