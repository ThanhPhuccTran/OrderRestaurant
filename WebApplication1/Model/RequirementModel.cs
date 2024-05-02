using OrderRestaurant.Data;
using OrderRestaurant.DTO.ConfigDTO;
using OrderRestaurant.DTO.TableDTO;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderRestaurant.Model
{
    public class RequirementModel
    {
        public int RequestId { get; set; }
        public int TableId { get; set; }
        public DateTime? RequestTime { get; set; }
        public string Title { get; set; }
        public string? RequestNote { get; set; }
        public int Code { get; set; }
        public ManageStatusDTO? Statuss { get; set; }
        public TablesDTO Tables { get; set; }
    }
}
