using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAppMVC.Models;

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
            var users = _userManager.Users.ToList();
            var model = new List<UserWithRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user); // Lấy role của user
                model.Add(new UserWithRolesViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    Roles = roles.ToList()
                });
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
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
        public async Task<IActionResult> Edit(EditRoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null) return NotFound();

            var oldRoles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, oldRoles);

            var addRoles = model.Roles.Where(x => x.IsSelected).Select(x => x.RoleName);

            await _userManager.AddToRolesAsync(user, addRoles);

            return RedirectToAction("Index");
        }
    }
}
