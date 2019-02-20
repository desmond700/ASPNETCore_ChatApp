using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ASP.NetCore_ChatApp.Hubs
{
    public class ChatHub: Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public override Task OnConnectedAsync()
        {
            var name = Context.User.Identity.Name;
            System.Diagnostics.Debug.WriteLine("User isAuthenticated: " + Context.User.Identity.IsAuthenticated);
            System.Diagnostics.Debug.WriteLine("ConnectionId: " + Context.ConnectionId);
            System.Diagnostics.Debug.WriteLine("User connected: " + name);
            return base.OnConnectedAsync();
        }
    }
}
