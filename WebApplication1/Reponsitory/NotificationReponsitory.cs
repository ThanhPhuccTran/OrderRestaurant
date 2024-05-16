using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.Service;

namespace OrderRestaurant.Reponsitory
{
    public class NotificationReponsitory : INotification
    {
        private readonly ApplicationDBContext _context;
        public NotificationReponsitory(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<int> CountNotification()
        {
            return await _context.Notifications.CountAsync(n => !n.IsCheck);
        }

        public async Task<List<Notification>> GetNotifications()
        {
            return await _context.Notifications.OrderByDescending(c=>c.CreatedAt).ToListAsync();
        }
        public async Task<List<Notification>> Get10Notifications()
        {
            return await _context.Notifications.Take(10).OrderByDescending(c => c.CreatedAt).ToListAsync();
        }

        public async Task MarkAllNotificationsAsRead()
        {
            var notifications = await _context.Notifications.ToListAsync();
            foreach (var notification in notifications)
            {
                notification.IsCheck = true;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<Notification> MarkNotificationAsRead(int id)
        {
            var notify = await _context.Notifications.FindAsync(id);
            if(notify != null)
            {
                notify.IsCheck = true;
                await _context.SaveChangesAsync();
            }
            return notify;
        }

        public async Task<bool> NotificationExists(int id)
        {
            return await _context.Notifications.AnyAsync(n => n.NotificationId == id);
        }
    }
}
