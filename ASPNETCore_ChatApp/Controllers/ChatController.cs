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

namespace ASPNETCore_ChatApp.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            Debug.WriteLine("Directory: "+ Directory.GetCurrentDirectory());
            return View();
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
                User userData = null;
                Cookie authCookie;

                var imageFile = form.Files["img-file"];

                if (form["form-type"] == "login")
                {
                    userData = context.GetUser(form["uname"].ToString(), form["pwrd"].ToString());

                    AuthenticateUser(form["uname"].ToString(), form["pwrd"].ToString(), out authCookie);

                    Debug.WriteLine("Cookie name: "+authCookie.Name);
                }
                else if (form["form-type"] == "register")
                {
                    User user = new User()
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
                }
                
                //TempData["UserData"] = user;
                //Debug.WriteLine("UserTemp: "+((User)TempData["UserData"]).Username);
                return View(userData);
            }

            return RedirectToAction("Login");
        }

        private static bool AuthenticateUser(string user, string password, out Cookie authCookie)
        {
            var request = WebRequest.Create("/Chat/Hub") as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.CookieContainer = new CookieContainer();

            var authCredentials = "UserName=" + user + "&Password=" + password;
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(authCredentials);
            request.ContentLength = bytes.Length;
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            using (var response = request.GetResponse() as HttpWebResponse)
            {
                authCookie = response.Cookies[FormsAuthentication.FormsCookieName];
            }

            if (authCookie != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
