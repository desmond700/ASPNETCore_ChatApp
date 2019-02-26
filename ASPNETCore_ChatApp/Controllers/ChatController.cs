using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASPNETCore_ChatApp.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Identity;
using ASPNETCore_ChatApp.Helper;
using ASPNETCore_ChatApp.ViewModels;

namespace ASPNETCore_ChatApp.Controllers
{
    public class ChatController : Controller
    {
        public IdentityUserHandler UserHandler;
        public ChatController(UserManager<IdentityUser> userManager, 
            SignInManager<IdentityUser> signInManager)
        {
            UserHandler = new IdentityUserHandler(userManager, signInManager);
        }

        public ChatDBContext MySqlDBContext
        {
            get
            {
                return HttpContext.RequestServices.GetService(typeof(ChatDBContext)) as ChatDBContext;
            }
        }

        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                var user = await UserHandler.UserManager.FindByNameAsync(login.UserName);

                if (user == null)
                {
                    Debug.WriteLine("User is null: ");
                    ModelState.AddModelError(string.Empty, "Invalid login");
                    return View();
                }
                var passwordSignInResult = await UserHandler.SignInManager.PasswordSignInAsync(user, login.Password, isPersistent: login.RememberMe, lockoutOnFailure: false);
                if (!passwordSignInResult.Succeeded)
                {
                    Debug.WriteLine("Invalid login password");
                    ModelState.AddModelError(string.Empty, "Invalid login password");
                    return View();
                }
                try
                {
                    User userData = MySqlDBContext.GetUser(login.UserName, login.Password);
                    MySqlDBContext.UpdateUserStatus(user.UserName, "Online");
                    return RedirectToAction("Hub", new { user = userData.Username });
                }
                catch (MySqlException ex)
                {
                    Debug.WriteLine("Mysql Error: " + ex.Message);
                }
            }
            else
            {
                Debug.WriteLine("ModelState Error: " + string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)));
            }      

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            var currentUser = Request.HttpContext.User.Identity.Name;

            await UserHandler.SignInManager.SignOutAsync();
            MySqlDBContext.UpdateUserStatus(currentUser, "Offline");
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel register)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    User user = new User()
                    {
                        Username = register.UserName,
                        Email = register.Email,
                        Password = register.Password,
                        Image = null
                    };

                    if (register.Image != null && register.Image.Length > 0)
                    {
                        var fileName = Path.GetFileName(register.Image.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", fileName);
                        using (var fileSteam = new FileStream(filePath, FileMode.Create))
                        {
                            await register.Image.CopyToAsync(fileSteam);
                        }
                        user.Image = fileName;
                    }
                    MySqlDBContext.InsertUser(user);
                    await UserHandler.CreateUser(user.Username, user.Email, user.Password);
                    return RedirectToAction("Login");
                }
                catch (MySqlException ex)
                {
                    Debug.WriteLine("Mysql Error: " + ex.Message);
                }
            }
            else
            {
                Debug.WriteLine("ModelState Error: " + string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)));
            }
            
            return View();
        }

        public IActionResult Hub(string user)
        {
            var currentUser = Request.HttpContext.User.Identity.Name;

            Debug.WriteLine("loggedInUser: " + currentUser);
            if (!string.IsNullOrEmpty(user) && currentUser == user)
            {
                try
                {
                    User userData = MySqlDBContext.GetUserByUname(user);
                    return View(userData);
                }
                catch (NullReferenceException ex)
                {
                    Debug.WriteLine("Error: " + ex.Message);
                    return RedirectToAction("Login");
                } 
            }
            return RedirectToAction("Login");
        }

        [HttpPost]
        public bool IsUserAvailable(string username)
        {
            var IsUserExist = MySqlDBContext.IsUsernameExist(username);
            Debug.WriteLine("IsUsernameExist: " + IsUserExist);
            return IsUserExist;
        }

        [HttpPost]
        public bool IsEmailAvailable(string email)
        {
            var IsUserExist = MySqlDBContext.IsEmailExist(email);
            Debug.WriteLine("IsEmailAvailable: " + IsUserExist);
            return IsUserExist;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
