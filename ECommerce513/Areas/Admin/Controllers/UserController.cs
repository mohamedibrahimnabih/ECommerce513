using ECommerce513.Data;
using ECommerce513.Models;
using ECommerce513.Models.ViewModels;
using ECommerce513.Repository;
using ECommerce513.Repository.IRepository;
using ECommerce513.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Threading.Tasks;

namespace ECommerce513.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        //private readonly ApplicationDbContext _context = new();

        public UserController(IUserRepository userRepository, IRoleRepository roleRepository, UserManager<ApplicationUser> userManager)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAsync();

            return View(users.ToList());
        }

        public async Task<IActionResult> ChangeRole(string id)
        {
            var user = _userRepository.GetOne(e => e.Id == id);

            if (user is not null)
            {
                ViewBag.Roles = (await _roleRepository.GetAsync()).ToList().Select(e => new SelectListItem
                {
                    Text = e.Name,
                    Value = e.Name
                });

                return View(user);
            }

            return RedirectToAction("NotFoundPage", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(UserNameWithRoleNameVM userNameWithRoleNameVM)
        {
            if(!ModelState.IsValid)
            {
                return View(userNameWithRoleNameVM);
            }

            var applicationUser = await _userManager.FindByNameAsync(userNameWithRoleNameVM.UserName);

            if (applicationUser is not null)
            {
                var userRoles = await _userManager.GetRolesAsync(applicationUser);
                await _userManager.RemoveFromRolesAsync(applicationUser, userRoles);

                var result = await _userManager.AddToRoleAsync(applicationUser, userNameWithRoleNameVM.RoleName);

                if (result.Succeeded)
                {
                    TempData["Notification"] = "Update User Role Successfully";

                    return RedirectToAction("Index", "User", new { area = "Admin" });
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, item.Description);
                    }

                    return View(userNameWithRoleNameVM);
                }
            }

            ModelState.AddModelError("UserName", "Invalid UserName");
            return View(userNameWithRoleNameVM);
        }

        public async Task<IActionResult> LockUnLock(string id)
        {
            var user = _userRepository.GetOne(e => e.Id == id);

            if (user is not null)
            {
                if(user.LockoutEnabled)
                {
                    user.LockoutEnd = DateTime.Now.AddMonths(1);
                    TempData["Notification"] = "Lock User Successfully";
                }
                else
                {
                    user.LockoutEnd = null;
                    TempData["Notification"] = "UnLock User Successfully";
                }

                user.LockoutEnabled = !user.LockoutEnabled;
                await _userManager.UpdateAsync(user);

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("NotFoundPage", "Home");
        }
    }
}
