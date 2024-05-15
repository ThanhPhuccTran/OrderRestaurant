using OrderRestaurant.Data;

namespace OrderRestaurant.Service
{
        public interface INotification
        {
            Task<List<Notification>> GetNotifications();
            Task<Notification> MarkNotificationAsRead(int id);
            Task MarkAllNotificationsAsRead();
            Task<int> CountNotification();
            Task<bool> NotificationExists(int id);
    }
}
