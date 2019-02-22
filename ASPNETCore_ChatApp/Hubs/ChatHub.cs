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

        List<User> CurrentUsersList = new List<User>();
        //List<KeyValuePair<string, int>> list = dictionary.ToList();

        public ChatHub(RequestHandler requestHandler)
        {
            this.requestHandler = requestHandler;
        }

        public async Task SendMessage(string userid, string message)
        {
            
            User userObj = ((ChatDBContext)dbContext()).GetUserByUname(Context.User.Identity.Name);
            
                await Clients.Caller.SendAsync("SentMessage", userObj, message);
            //if (Context.ConnectionId == userid)
                await Clients.Others.SendAsync("ReceiveMessage", userObj, message);
        }

        public override Task OnConnectedAsync()
        {
            var name = Context.User.Identity.Name;
            System.Diagnostics.Debug.WriteLine("User isAuthenticated: " + Context.User.Identity.IsAuthenticated);
            System.Diagnostics.Debug.WriteLine("ConnectionId: " + Context.ConnectionId);
            System.Diagnostics.Debug.WriteLine("User connected: " + name);

            try
            {
                List<object> userObj = ((ChatDBContext)dbContext()).InsertOnlineUser(Context.User.Identity.Name, Context.ConnectionId);

                //CurrentUsersList = userObj;
                System.Diagnostics.Debug.WriteLine("item key count: " + CurrentUsersList.Count);
                Clients.All.SendAsync("GetConnectedUsers", userObj);
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("MySQL error: " + ex.Message);
            }
            
            
            return base.OnConnectedAsync();
        }

        /*public override Task OnReconnectedAsync(string connectionId)
        {
            System.Diagnostics.Debug.WriteLine("reconnected");

            return null;
        }*/

        public override Task OnDisconnectedAsync(Exception exception)
        {
            System.Diagnostics.Debug.WriteLine("disconnected");

            try
            {
                ((ChatDBContext)dbContext()).RemoveOnlineUser(Context.User.Identity.Name);
                //CurrentUsersList.Remove(Context.ConnectionId);
                System.Diagnostics.Debug.WriteLine("user removed");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Signalr hub error: "+ex.Message);
            }

            return base.OnDisconnectedAsync(exception); ;
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

        /*public Dictionary<string, User> GetAllActiveUsers()
        {
            System.Diagnostics.Debug.WriteLine("item key count: " + CurrentUsersList.Count); 

            foreach (var item in CurrentUsersList)
            {
                System.Diagnostics.Debug.WriteLine("item key: " + item.Key);
            }
            return CurrentUsersList.ToAsyncEnumerable();
        }*/
    }
}
