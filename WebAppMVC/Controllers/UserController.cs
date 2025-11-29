using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebAppMVC.Models;
using WebAppMVC.Models.ViewModels;

namespace WebAppMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUserRoles(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();
            var model = new EditRoleViewModel
            {
                UserId = user.Id,
                Email = user.Email ?? "",
                Roles = allRoles.Select(role => new RoleItem
                {
                    RoleName = role.Name ?? "",
                    IsSelected = userRoles.Contains(role.Name ?? "")
                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserRoles(EditRoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();
            var oldRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, oldRoles);
            var addRoles = model.Roles.Where(x => x.IsSelected).Select(x => x.RoleName);
            await _userManager.AddToRolesAsync(user, addRoles);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditUserClaims(String id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var existingUserClaims = await _userManager.GetClaimsAsync(user);
            var model = new UserClaimsViewModel
            {
                UserId = user.Id,
                UserEmail = user.Email ?? "",
                Claims = ClaimsStore.GetAllClaims().Select(c => new UserClaim
                {
                    ClaimType = c.Type,
                    ClaimValue = c.Value,
                    IsSelected = existingUserClaims.Any(ec => ec.Type == c.Type)
                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditUserClaims(UserClaimsViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();
            var existingUserClaims = await _userManager.GetClaimsAsync(user);
            foreach (var claim in model.Claims)
            {
                if (claim.IsSelected)
                {
                    if (!existingUserClaims.Any(c => c.Type == claim.ClaimType))
                    {
                        await _userManager.AddClaimAsync(user, new Claim(claim.ClaimType, claim.ClaimValue));
                    }
                }
                else
                {
                    var userClaim = existingUserClaims.FirstOrDefault(c => c.Type == claim.ClaimType);
                    if (userClaim != null)
                    {
                        await _userManager.RemoveClaimAsync(user, userClaim);
                    }
                }
            }
            return RedirectToAction("Index");
        }
    }
}