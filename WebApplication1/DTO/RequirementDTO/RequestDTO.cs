using OrderRestaurant.DTO.TableDTO;

namespace OrderRestaurant.DTO.RequirementDTO
{
    public class RequestDTO
    {
        public int RequestId { get; set; }
        public int TableId { get; set; }
        public DateTime? RequestTime { get; set; }
        public string Title { get; set; }
        public string? RequestNode { get; set; }
        public int Code { get; set; }
        public TablesDTO Tables { get; set; }
    }
}
