using Microsoft.AspNetCore.SignalR;

namespace TMP_mvc.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task JoinCityGroup(string city)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, city);
        }
    }
}
