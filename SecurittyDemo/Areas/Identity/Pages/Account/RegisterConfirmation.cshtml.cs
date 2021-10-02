using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System;
using Microsoft.Extensions.DependencyInjection;
namespace SecurittyDemo.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private  UserManager<IdentityUser> _userManager;
        private  IEmailSender _sender;

        //public RegisterConfirmationModel(UserManager<IdentityUser> userManager, IEmailSender sender)
        //{
        //    _userManager = userManager;
        //    _sender = sender;
        //}
        IServiceProvider _serviceProvider;
        public RegisterConfirmationModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public string Email { get; set; }

        public bool DisplayConfirmAccountLink { get; set; }

        public string EmailConfirmationUrl { get; set; }

        public string Tenant { get; set; }

        public async Task<IActionResult> OnGetAsync(string email, string Tenant,string returnUrl = null)
        {
            _serviceProvider.GetService<TenantManager>().Tenant = Tenant;
            _userManager = _serviceProvider.GetService<UserManager<IdentityUser>>();
            _sender = _serviceProvider.GetService<IEmailSender>();
          
            if (email == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            Email = email;
            // Once you add a real email sender, you should remove this code that lets you confirm the account
            DisplayConfirmAccountLink = true;
            if (DisplayConfirmAccountLink)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                EmailConfirmationUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);
            }

            return Page();
        }
    }
}
