using Microsoft.AspNetCore.SignalR;

namespace FPTJob_1670.Settings
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string UserId, string message)
        {
            await Clients.User(UserId).SendAsync("ReceiveNotification", message);
        }

    }
}
