using HellTwitchVipApp.Data.Repositories;
using HellTwitchVipApp.Models;
using HellTwitchVipApp.Models.Dto;
using HellTwitchVipApp.Models.Response;
using HellTwitchVipApp.Models.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using FluentValidation.Results;
using HellTwitchVipApp.Models.Enum;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace HellTwitchVipApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGiverRepository _giverRepository;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IGiverRepository giverRepository, 
            SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            ILogger<HomeController> logger)
        {
            _giverRepository = giverRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Givers));
        }

        [AllowAnonymous]
        [Route("[action]")]
        public async Task<IActionResult> Givers()
        {
            var givers = _giverRepository.GetAll();
            var request = new GiversResponse(givers);

            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (await _userManager.IsInRoleAsync(user, IdentityRoles.Admin))
                    request.Admin();
            }
            return View(request);
        }

        [Authorize(Roles = IdentityRoles.Admin)]
        [HttpGet]
        public IActionResult CreateGiverModal()
        {
            var giver = new GiverDto() { Count = 1 };
            return PartialView("_PartialCreateGiverModal", giver);
        }

        [Authorize(Roles = IdentityRoles.Admin)]
        [HttpGet]
        public IActionResult UpdateGiverModal(Guid id)
        {
            var giver = _giverRepository.GetById(id);
            return PartialView("_PartialUpdateGiverModal", giver);
        }

        [Authorize(Roles = IdentityRoles.Admin)]
        [HttpPost]
        public IActionResult CreateGiver(GiverDto dto)
        {
            var validator = new GiverValidator();
            TransactionResult<GiverDto> transactionResult;

            if (validator.Validate(dto).IsValid == false)
                return RedirectToAction(nameof(Givers), new { status = ActionStatus.Error });

            var giver = _giverRepository.GetByName(dto.Name);

            if (giver is null)
            {
                transactionResult = _giverRepository.Add(dto);
            }
            else
            {
                giver.Count += dto.Count;
                transactionResult = _giverRepository.Update(giver);
            }

            return RedirectToAction(nameof(Givers), new { status = (ActionStatus)transactionResult.Status });
        }

        [Authorize(Roles = IdentityRoles.Admin)]
        [HttpPost]
        public IActionResult UpdateGiver(GiverDto dto)
        {
            var validator = new GiverValidator();

            if (validator.Validate(dto).IsValid == false)
                return RedirectToAction(nameof(Givers), new { status = ActionStatus.Error });

            var giver = _giverRepository.GetById(dto.Id);

            if (giver is null)
                return RedirectToAction(nameof(Givers), new { status = ActionStatus.Error });

            giver.IsVip = dto.IsVip;
            giver.Count = dto.Count;
            var transactionResult = _giverRepository.Update(giver);

            return RedirectToAction(nameof(Givers), new { status = (ActionStatus)transactionResult.Status });
        }

        [Authorize(Roles = IdentityRoles.Admin)]
        [HttpPost]
        public IActionResult DeleteGiver(Guid id)
        {
            var transactionResult = _giverRepository.DeleteById(id);

            return RedirectToAction(nameof(Givers), new { status = (ActionStatus)transactionResult.Status });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
