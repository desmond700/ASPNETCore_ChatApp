using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNETCore_ChatApp.Models
{
    public class User
    {
        public static IServiceProvider ServiceProvider;
        private static IHttpContextAccessor _contextAccessor;

        public User(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
            
        }

        public static ChatDBContext context
        {
            get
            {
                return _contextAccessor.HttpContext.RequestServices.GetService(typeof(ChatDBContext)) as ChatDBContext;
            }
        }
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Image { get; set; }
    }
}
