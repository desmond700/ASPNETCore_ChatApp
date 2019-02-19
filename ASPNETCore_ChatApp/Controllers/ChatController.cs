using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASPNETCore_ChatApp.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

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

        public IActionResult Hub(IFormCollection form)
        {
            //var user = TempData["UserData"] as User;
            //Debug.WriteLine("userData: " + user.Username);

            if (ModelState.IsValid)
            {
                ChatDBContext context = HttpContext.RequestServices.GetService(typeof(ChatDBContext)) as ChatDBContext;
                User userData = null;
                if (form["form-type"] == "login")
                {
                    userData = context.GetUser(form["uname"].ToString(), form["pwrd"].ToString());
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

                    if (form.Files["img-file"] != null && form.Files["img-file"].Length > 0)
                    {
                        var fileName = Path.GetFileName(form.Files["img-file"].FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", fileName);
                        using (var fileSteam = new FileStream(filePath, FileMode.Create))
                        {
                            var task = Task.Run(async () =>
                            {
                                await form.Files["img-file"].CopyToAsync(fileSteam);
                            });
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
