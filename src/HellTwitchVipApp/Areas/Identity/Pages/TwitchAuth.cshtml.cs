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
    public class TwitchAuthModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ITwitchService _twitchService;
        private readonly ILogger<TwitchAuthModel> _logger;

        public TwitchAuthModel(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager, 
            ITwitchService twitchService,
            ILogger<TwitchAuthModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _twitchService = twitchService;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public List<string> Admins = new List<string>() { "deimozowen", "hellyeahplay" };

        public class InputModel
        {
            public string DisplayName { get; set; }
            public string Login { get; set; }
            public string Email { get; set; } 
            public string ImageUrl { get; set; }
        }

        public async Task OnGet(string code, string state)
        {
            var codeResponse = await _twitchService.GetAccessTokenFromCodeAsync(code);
            var validateAccessToken = await _twitchService.ValidateAccessTokenAsync(codeResponse.AccessToken);
            var userInfo = await _twitchService.GetUserInfo(codeResponse.AccessToken, validateAccessToken.Login);

            Input = new InputModel
            {
                Login = userInfo.Login,
                DisplayName = userInfo.DisplayName,
                Email = userInfo.Email,
                ImageUrl = userInfo.ProfileImageUrl
            };

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user is null)
            {
                user = new IdentityUser {
                    UserName = Input.Login,
                    Email = Input.Email,
                    EmailConfirmed = true
                };
                var createdUser = await _userManager.CreateAsync(user, $"{Input.Login}{Input.Email}");

                if (Admins.Contains(Input.Login))
                    await _userManager.AddToRoleAsync(user, IdentityRoles.Admin);
                else
                    await _userManager.AddToRoleAsync(user, IdentityRoles.Guest);

                await _userManager.AddClaimsAsync(user, new List<System.Security.Claims.Claim>() {
                    new System.Security.Claims.Claim(IdentityClaims.ImageUrl, Input.ImageUrl),
                    new System.Security.Claims.Claim(IdentityClaims.DisplayName, Input.DisplayName)
                });
            }
            await _signInManager.PasswordSignInAsync(Input.Login, $"{Input.Login}{Input.Email}", true, lockoutOnFailure: false);
        }
    }
}
