using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ASPNETCore_ChatApp.Helper
{
    public class IdentityUserHandler
    {
        public IdentityUserHandler(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public UserManager<IdentityUser> UserManager { get; }

        public SignInManager<IdentityUser> SignInManager { get; }

        public async Task<bool> CreateUser(string name, string email, string password)
        {
            var user = new IdentityUser
            {
                UserName = name,
                Email = email
            };
            var result = await UserManager.CreateAsync(user, password);
            Debug.WriteLine("result.Succeeded: " + result.Succeeded);
            if (result.Succeeded)
            {
                Debug.WriteLine("User created a new account with password.");

                /*var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { userId = user.Id, code = code },
                    protocol: Request.Scheme);*/

                /*await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");*/

                await SignInManager.SignInAsync(user, isPersistent: false);
                return true;// LocalRedirect(returnUrl);
            }
            foreach (var error in result.Errors)
            {
                Debug.WriteLine("Create Error: "+ error.Description);
            }

            return false;
        }
    }
}
