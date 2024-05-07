using OrderRestaurant.Data;
using OrderRestaurant.DTO.ConfigDTO;
using OrderRestaurant.DTO.EmployeeDTO;
using OrderRestaurant.DTO.FoodDTO;
using OrderRestaurant.DTO.TableDTO;

namespace OrderRestaurant.DTO.InvoiceDTO
{
    public class GetInvoiceDTO
    {
        public int OrderId { get; set; }
        public int? TableId { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? CreationTime { get; set; }
        public DateTime? ReceivingTime { get; set; }
        public DateTime? PaymentTime { get; set; }
        public decimal? Pay { get; set; }
        public TablesDTO? Tables { get; set; }
        public EmployeesDTO? Employees { get; set; }
        public List<FoodInvoiceDTO> Foods { get; set; }
        
    }
}
