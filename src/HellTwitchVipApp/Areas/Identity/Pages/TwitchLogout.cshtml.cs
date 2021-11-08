using HellTwitchVipApp.Models;
using HellTwitchVipApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HellTwitchVipApp.Areas.Identity.Pages
{
    public class TwitchLogoutModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<TwitchAuthModel> _logger;

        public TwitchLogoutModel(SignInManager<IdentityUser> signInManager, ILogger<TwitchAuthModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/");
        }
    }
}
