using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ASPNETCore_ChatApp.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Panel");
        }

        public IActionResult Login()
        {
            return View();
        }
        [Route("Admin/ListUsers")]
        public IActionResult ListUsers()
        {
            return View();
        }

        [Route("Admin/ListUsers/{username:string}")]
        public IActionResult ListUsers(string username)
        {
            return View();
        }
    }
}