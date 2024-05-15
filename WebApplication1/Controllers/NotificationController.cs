using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderRestaurant.Data;
using OrderRestaurant.Reponsitory;
using OrderRestaurant.Service;

namespace OrderRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotification _notification;
        public NotificationController(INotification notification)
        {
            _notification = notification;
        }


        [HttpGet("get-notif")]
        public async Task<IActionResult> GetNotifications()
        {
            var notifications = await _notification.GetNotifications();
            return Ok(notifications);
        }

        [HttpPut("{id}/mark-read")]
        public async Task<IActionResult> MarkNotificationAsRead(int id)
        {
            var notification = await _notification.MarkNotificationAsRead(id);
            if (notification == null)
            {
                return NotFound();
            }
            return Ok("Đã đọc thông báo");
        }

        [HttpPut("mark-all-read")]
        public async Task<IActionResult> MarkAllNotificationsAsRead()
        {
            await _notification.MarkAllNotificationsAsRead();
            return Ok("Đã đọc tất cả thông báo");
        }

        [HttpGet("count-notification")]
        public async Task<IActionResult> CountNotification()
        {
            var count = await _notification.CountNotification();
            return Ok(count);
        }
    }
}
