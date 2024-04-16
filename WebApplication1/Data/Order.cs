using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderRestaurant.Data
{
    [Table("Order")]
    public partial class Order
    {
        [Key]
        public int OrderId { get; set; }
        [ForeignKey("CustomerId ")]
        public int? CustomerId { get; set; }
        [ForeignKey("TableId")]
        public int TableId { get; set; }
        [ForeignKey("EmployeeId")]
        public int? EmployeeId { get; set; }
        public DateTime? CreationTime { get; set; }
        public DateTime? ReceivingTime { get; set; }
        public DateTime? PaymentTime { get; set; }
        public double? Pay {  get; set; }
        public string? Note { get; set; }
        public int Status { get; set; }
       
        public Customer? Customers { get; set; }
        public Table? Tables { get; set; }
        public Employee? Employees { get; set; }
        public List<OrderDetails> OrderDetails { get; set; } 
    }
}
