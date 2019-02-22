using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASPNETCore_ChatApp.Models;

namespace ASPNETCore_ChatApp.Helper
{
    public class RequestHandler
    {
        IHttpContextAccessor _httpContextAccessor;
        public RequestHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ChatDBContext ChatDBRequest()
        {
            return _httpContextAccessor.HttpContext.RequestServices.GetService(typeof(ChatDBContext)) as ChatDBContext;
        }

        internal void HandleAboutRequest()
        {
            // handle the request  
            var message = "The HttpContextAccessor seems to be working!!";
            _httpContextAccessor.HttpContext.Session.SetString("message", message);
        }
    }
}
