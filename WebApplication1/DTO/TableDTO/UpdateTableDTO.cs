namespace OrderRestaurant.DTO.TableDTO
{
    public class UpdateTableDTO
    {
        public string TableName { get; set; }
        public int Status { get; set; } 
        public string Note { get; set; }
        public IFormFile QR_id { get; set; }
    }
}
