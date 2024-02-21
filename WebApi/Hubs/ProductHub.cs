using Microsoft.AspNetCore.SignalR;

namespace WebApi.Hubs
{
    public class ProductHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
