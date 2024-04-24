using OrderRestaurant.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderRestaurant.Model
{
    public class OrderModel
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
        public int StatusId { get; set; }

       
        public Customer? Customers { get; set; }
        
        public Table? Tables { get; set; }
        
        public Employee? Employees { get; set; }
        
        public ManageStatus? Statuss { get; set; }

    }
}
