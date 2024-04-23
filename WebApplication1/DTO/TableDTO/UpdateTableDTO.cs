namespace OrderRestaurant.DTO.TableDTO
{
    public class UpdateTableDTO
    {
        public string? TableName { get; set; }
        public int StatusId { get; set; } 
        public string? Note { get; set; }
        public string? QR_id { get; set; }
    }
}
