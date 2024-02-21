// Commenting signalR related code but in the document it is specified to use
// use proper status codes from Core service, so now generating notifications
// from the core service (Web Api)

//using Microsoft.AspNetCore.SignalR;

//namespace ProductsMicroservice.Hubs
//{
//    public class ProductHub : Hub
//    {
//        public async Task SendMessage(string user, string message)
//        {
//            await Clients.All.SendAsync("ReceiveMessage", user, message);
//        }
//    }
//}
