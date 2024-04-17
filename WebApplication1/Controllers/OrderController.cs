using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderRestaurant.Model;
using OrderRestaurant.Service;

namespace OrderRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrder _orderRepository;
        public OrderController(IOrder orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var order = await _orderRepository.GetAll();
            return Ok(order);
        }

        [HttpPut("ChangeStatus/{id}/{newStatus}")]
        public async Task<IActionResult> ChangeOrderStatus([FromRoute] int id, [FromRoute] int newStatus)
        {
            var order = await _orderRepository.GetAsync(id);
            if (order == null)
                return NotFound();

            // Kiểm tra xem trạng thái mới có hợp lệ không
            if (!IsValidStatus(newStatus))
                return BadRequest("Invalid new status");
            if (newStatus == Constants.ORDER_ACCEPTED)
            {
                order.ReceivingTime = DateTime.Now; // Cập nhật thời gian nhận đơn hàng thành thời gian hiện tại
            }
            // Thực hiện thay đổi trạng thái
            order.StatusId = newStatus;
            await _orderRepository.UpdateAsync(order);

            return Ok(order);
        }

        private bool IsValidStatus(int status)
        {
            // Kiểm tra xem trạng thái mới có trong danh sách hợp lệ không
            return status == Constants.ORDER_INIT || status == Constants.ORDER_ACCEPTED ||
                   status == Constants.ORDER_FINISHED || status == Constants.ORDER_CANCEL ||
                   status == Constants.ORDER_REJECTED;
        }
    }
}
