namespace OrderRestaurant.DTO.TableDTO
{
    public class CreateTableDTO
    {
        public string TableName { get; set; }
      /*  public int Status { get; set; } // Trạng thái bàn trống*/
        public string Note { get; set; }
        public IFormFile? QR_id { get; set; }

    }
}
