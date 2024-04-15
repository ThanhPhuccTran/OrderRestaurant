﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderRestaurant.Data
{
    [Table("Order")]
    public partial class Order
    {
        [Key]
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int TableId { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? CreationTime { get; set; }
        public DateTime? ReceivingTime { get; set; }
        public DateTime? PaymentTime { get; set; }
        public double? Pay {  get; set; }
        public string? Note { get; set; }
        public int Status { get; set; }
        [ForeignKey("CustomerId ")]
        public Customer? Customers { get; set; }
        [ForeignKey("EmployeeId")]
        public Employee? Employees { get; set; }
        public List<OrderDetails> OrderDetails { get; set; } 
    }
}
