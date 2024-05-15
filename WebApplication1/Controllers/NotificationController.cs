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

        [HttpGet("all-time")]
        public async Task<ActionResult<List<object>>> GetAllTimeNotifications()
        {
            var notifications = await _notification.GetNotifications();
            var notificationObjects = new List<object>();

            foreach (var notification in notifications)
            {
                TimeSpan timeSinceNotification = (TimeSpan)(DateTime.Now - notification.CreatedAt);
                string timeSinceNotificationString = "";

                if (timeSinceNotification.TotalDays >= 1)
                {
                    timeSinceNotificationString = $"{(int)timeSinceNotification.TotalDays} ngày trước";
                }
                else if (timeSinceNotification.TotalHours >= 1)
                {
                    timeSinceNotificationString = $"{(int)timeSinceNotification.TotalHours} giờ trước";
                }
                else if (timeSinceNotification.TotalMinutes >= 1)
                {
                    timeSinceNotificationString = $"{(int)timeSinceNotification.TotalMinutes} phút trước";
                }
                else if(timeSinceNotification.TotalSeconds >= 1)
                {
                    timeSinceNotificationString = $"{(int)timeSinceNotification.TotalSeconds} giây trước";
                }
                else
                {
                    timeSinceNotificationString = "Ngay bây giờ";
                }

                var notificationObject = new
                {
                    NotificationId = notification.NotificationId,
                    Title = notification.Title,
                    Content = notification.Content,
                    Type = notification.Type,
                    IsCheck = notification.IsCheck,
                    TimeSinceCreation = timeSinceNotificationString
                };

                notificationObjects.Add(notificationObject);
            }

            return Ok(notificationObjects);
        }
    }
}
