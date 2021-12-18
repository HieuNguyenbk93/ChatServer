using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Hubs
{
    //public class ChatHub : Hub
    //{
    //    public override Task OnConnectedAsync()
    //    {
    //        Clients.Client(Context.ConnectionId).SendAsync("ReceiveConnID", Context.ConnectionId);
    //        return base.OnConnectedAsync();
    //    }
    //    public async Task SendMessage(string username, string message)
    //    {
    //        await Clients.All.SendAsync("ReceiveMessage", username, "join room !!!");
    //    }
    //}
    //https://stackoverflow.com/questions/13514259/get-number-of-listeners-clients-connected-to-signalr-hub
    public class ChatHub : Hub
    {
        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();
        public override Task OnConnectedAsync()
        {
            //Console.WriteLine("--> Connection Opened: " + Context.ConnectionId);
            string name = Context.User.Identity.Name;
            _connections.Add(name, Context.ConnectionId);
            //Clients.Client(Context.ConnectionId).SendAsync("ReceiveConnID", Context.ConnectionId);
            Clients.Client(Context.ConnectionId).SendAsync("connection", _connections);
            return base.OnConnectedAsync();
        }


        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine("--> Connection Closed: " + Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }


        public async Task SendMessageAsync(string message)
        {
            var routeOb = JsonConvert.DeserializeObject<dynamic>(message);
            //Console.WriteLine("To: " + routeOb.To.ToString());
            //Console.WriteLine("Message Recieved on: " + Context.ConnectionId);
            if (routeOb.To.ToString() == string.Empty)
            {
                Console.WriteLine("Broadcast");
                await Clients.All.SendAsync("ReceiveMessage", message);
            }
            else
            {
                string toClient = routeOb.To;
                Console.WriteLine("Targeted on: " + toClient);

                await Clients.Client(toClient).SendAsync("ReceiveMessage", message);

            }
        }
    }
}
