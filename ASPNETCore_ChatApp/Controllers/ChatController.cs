using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASPNETCore_ChatApp.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.SignalR;

namespace ASPNETCore_ChatApp.Controllers
{
    public class ChatController : Controller
    {
        IHttpContextAccessor contextAccessor;
        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            Debug.WriteLine("Directory: "+ Directory.GetCurrentDirectory());
            return View();
        }

        public async void Logout()
        {
            await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);

            //return RedirectToAction("Login");
        }

        public IActionResult Register()
        {
            return View();
        }

        public async Task<IActionResult> Hub(IFormCollection form)
        {
            //var user = TempData["UserData"] as User;
            //Debug.WriteLine("userData: " + user.Username);



            if (ModelState.IsValid)
            {
                ChatDBContext context = HttpContext.RequestServices.GetService(typeof(ChatDBContext)) as ChatDBContext;
                User userData = new User(contextAccessor);

                var imageFile = form.Files["img-file"];

                if (form["form-type"] == "login")
                {
                    userData = context.GetUser(form["uname"].ToString(), form["pwrd"].ToString());

                    AuthenticateUser(userData.Username);

                    Debug.WriteLine("Name: "+ userData.Username + ", " + userData.Email);
                }
                else if (form["form-type"] == "register")
                {
                    User user = new User(contextAccessor)
                    {
                        Username = form["uname"].ToString(),
                        Email = form["email"].ToString(),
                        Password = form["pwrd"].ToString(),
                        Image = null
                    };
                    
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(imageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", fileName);
                        using (var fileSteam = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileSteam);
                        }
                        user.Image = fileName;
                    }

                    userData = context.InsertUser(user);
                    AuthenticateUser(userData.Username);
                }
                
                //TempData["UserData"] = user;
                //Debug.WriteLine("UserTemp: "+((User)TempData["UserData"]).Username);
                return View(userData);
            }

            return RedirectToAction("Login");
        }

        private async void AuthenticateUser(string user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user),
                new Claim(ClaimTypes.Name, user),
                new Claim(ClaimTypes.Role, "User")
            };

            var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                // Refreshing the authentication session should be allowed.

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. Required when setting the 
                // ExpireTimeSpan option of CookieAuthenticationOptions 
                // set with AddCookie. Also required when setting 
                ExpiresUtc = DateTimeOffset.Now.AddDays(1),

                IssuedUtc = DateTimeOffset.Now
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal, 
                authProperties);
        }

        protected void Session_End()
        {
            Logout();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
