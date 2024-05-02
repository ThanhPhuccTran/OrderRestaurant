using OrderRestaurant.DTO.TableDTO;

namespace OrderRestaurant.DTO.RequirementDTO
{
    public class CreatedRequirementDTO
    {
        public int TableId { get; set; }
        public string Title { get; set; }
        public string? RequestNote { get; set; }
       // public TablesDTO Tables { get; set; }
    }
}
