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

        public async Task UserLogin(string username, string password)
        {
            //await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
