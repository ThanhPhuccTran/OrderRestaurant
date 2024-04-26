using OrderRestaurant.Data;
using OrderRestaurant.DTO.ConfigDTO;
using OrderRestaurant.DTO.EmployeeDTO;
using OrderRestaurant.DTO.TableDTO;

namespace OrderRestaurant.DTO.OrderDTO
{
    public class Order_DetailsDTO
    {
        public int OrderId { get; set; }

        public int? CustormerId { get; set; }

        public int TableId { get; set; }

        public int? EmployeeId { get; set; }
        public DateTime? CreationTime { get; set; }
        public DateTime? ReceivingTime { get; set; }
        public DateTime? PaymentTime { get; set; }
        public decimal? Pay { get; set; }
        public string? Note { get; set; }
        public int Code { get; set; }
        public Customer? Customers { get; set; }

        public TablesDTO? Tables { get; set; }

        public EmployeesDTO? Employees { get; set; }

        public ManageStatusDTO? Statuss { get; set; }
    }
}
