﻿using OrderRestaurant.Data;
using OrderRestaurant.DTO.ConfigDTO;

namespace OrderRestaurant.DTO.OrderDTO
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public int TableId { get; set; }
        public int Code { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? PaymentTime { get; set; }
        public DateTime? ReceivingTime { get; set; }
        public ManageStatusDTO ManageStatuss { get; set; }
    }
}
