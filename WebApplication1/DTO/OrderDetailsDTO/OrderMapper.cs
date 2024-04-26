using OrderRestaurant.Data;
using OrderRestaurant.Model;

namespace OrderRestaurant.DTO.OrderDetailsDTO
{
    public static class OrderMapper
    {
        public static OrderModel ToModelDto(this Order model)
        {
            return new OrderModel
            {
                OrderId = model.OrderId,
                CustormerId = model.CustomerId,
                TableId = model.TableId,
                EmployeeId = model.EmployeeId.Value,
                CreationTime = model.CreationTime.Value,
                ReceivingTime = model.ReceivingTime.Value,
                PaymentTime = model.PaymentTime.Value,
                Pay = model.Pay.Value,
                Note = model.Note,
                Code = model.Code,
            };
        }
    }
}
