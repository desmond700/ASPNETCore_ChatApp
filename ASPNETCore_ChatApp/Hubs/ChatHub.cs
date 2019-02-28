using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ASPNETCore_ChatApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using ASPNETCore_ChatApp.Helper;
using MySql.Data.MySqlClient;

namespace ASPNETCore_ChatApp.Hubs
{
    [Authorize]
    public class ChatHub: Hub
    {
        private readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();
        private RequestHandler requestHandler;
        private string CallerConnectionId { get { return Context.ConnectionId; } }

        public ChatHub(RequestHandler requestHandler)
        {
            this.requestHandler = requestHandler;
        }

        public async Task SendMessage(int from, int to, string message, string userConnectionId)
        {
            
            Message messageObj = ((ChatDBContext)dbContext()).InsertMessage(from, to, message);
            
            await Clients.Caller.SendAsync("SentMessage", messageObj);
            
            await Clients.Client(userConnectionId).SendAsync("ReceiveMessage", messageObj);
        }

        public override Task OnConnectedAsync()
        {
            var name = Context.User.Identity.Name;
            System.Diagnostics.Debug.WriteLine("User isAuthenticated: " + Context.User.Identity.IsAuthenticated);
            System.Diagnostics.Debug.WriteLine("ConnectionId: " + Context.ConnectionId);
            System.Diagnostics.Debug.WriteLine("User connected: " + name);
            
            try
            {
                List<object> userObj = ((ChatDBContext)dbContext()).SetOnlineUser(Context.User.Identity.Name, Context.ConnectionId);

                Clients.All.SendAsync("GetConnectedUsers", userObj);
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("MySQL error: " + ex.Message);
            }
            
            
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            System.Diagnostics.Debug.WriteLine("disconnected");

            try
            {
                ((ChatDBContext)dbContext()).UpdateUserStatus(Context.User.Identity.Name, "Offline");
                ((ChatDBContext)dbContext()).RemoveOnlineUser(Context.User.Identity.Name);
                List<object> userObj = ((ChatDBContext)dbContext()).GetOnlineUsers();
                Clients.All.SendAsync("GetConnectedUsers", userObj);
                System.Diagnostics.Debug.WriteLine("user removed");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Signalr hub error: "+ex.Message);
            }

            return base.OnDisconnectedAsync(exception); ;
        }

        public List<Message> GetMessages(int sender, int receiver)
        {
            return ((ChatDBContext)dbContext()).GetMessages(sender, receiver);
        }

        public string GetConnectionId()
        {
            
            return Context.ConnectionId;
        }

        public dynamic dbContext()
        {
            ChatDBContext context = this.requestHandler.ChatDBRequest();
            return context ?? null;
        }
    }
}
