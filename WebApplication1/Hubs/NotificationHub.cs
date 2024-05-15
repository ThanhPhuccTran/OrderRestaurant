using Microsoft.AspNetCore.SignalR;
using OrderRestaurant.Data;

namespace OrderRestaurant.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendOrderNotification(Notification notification)
        {
            await Clients.All.SendAsync("ReceiveOrderNotification", notification);
        }
    }
}
