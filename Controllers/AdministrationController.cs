using EDSU_SYSTEM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EDSU_SYSTEM.Controllers
{
    [Authorize(Roles = "superAdmin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager,
                                        UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(RoleVM model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new()
                {
                    Name = model.Name
                };
                IdentityResult result = await roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("index", "administration");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
       // [Authorize(Roles = "superAdmin")]
        public IActionResult Index()
        {
            var roles = roleManager.Roles;
            roleManager.Dispose();
            return View(roles);
        }
       // [Authorize(Roles = "superAdmin")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var services = new ServiceCollection();
            var serviceProvider = services.BuildServiceProvider();
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return View("Error");
            }
            var model = new EditRoleVM
            {
                Id = role.Id,
                RoleName = role.Name
            };
            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users?.Add(user.UserName);
                }
            }

            return View(model); 
        }
        //[Authorize(Roles = "superAdmin")]
        [HttpPost]
        public async Task<IActionResult> Edit(EditRoleVM model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                return View("Error");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            return View(model); 
            }
        }
        [Authorize(Roles = "superAdmin")]
        public async Task<IActionResult> EditUserRole(string? id)
        {
            
            ViewBag.roleId = id;
            TempData["id"] = id;
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }

            var users = await userManager.Users.Where(x=>x.Type == 2).ToListAsync();

            var model = new List<UserRoleVM>();

            foreach (var user in users)
            {
                var userRoleViewModel = new UserRoleVM
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                };

                

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
               TempData["UserRoleModel"] = model; // Store the model list in TempData
                Console.Write("model ", TempData["id"]);
            }
            
            return View(model);
        }

        [Authorize(Roles = "superAdmin")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserRole(List<UserRoleVM> model, string? id)
        {
            id = (string)TempData["id"];
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }

            var storedModel = TempData["UserRoleModel"] as List<UserRoleVM>;
            Console.Write("model 3 ", storedModel);
            foreach (var userRoleViewModel in storedModel)
            {
                var user = await userManager.FindByIdAsync(userRoleViewModel.UserId);

                if (user == null)
                {
                    continue;
                }

                if (userRoleViewModel.IsSelected)
                {
                    await userManager.AddToRoleAsync(user, role.Name);
                }
                else
                {
                    await userManager.RemoveFromRoleAsync(user, role.Name);
                }
            }

            return RedirectToAction("Index", "administration"); // Replace with appropriate redirect action and controller
        }
        public IActionResult Back()
        {
            ViewBag.Users = new SelectList(userManager.Users.Where(x => x.StaffId != null), "Id", "UserName");
            ViewBag.Roles = new SelectList(roleManager.Roles, "Name", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddUserToRole(string userId, string roleName)
        {
            // Retrieve the user and role using the provided userId and roleName.
            var user = await userManager.FindByIdAsync(userId);
            var role = await roleManager.FindByNameAsync(roleName);

            if (user == null || role == null)
            {
                ModelState.AddModelError("", "User or role not found.");
                return View(); // Return to the form with an error message.
            }

            // Add the user to the role.
            var result = await userManager.AddToRoleAsync(user, role.Name);

            if (result.Succeeded)
            {
                // Redirect to a success page or display a success message.
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(); // Return to the form with an error message.
            }
        }
        public IActionResult Students()
        {
            ViewBag.Users = new SelectList(userManager.Users.Where(x => x.StudentsId != null), "Id", "UserName");
            ViewBag.Roles = new SelectList(roleManager.Roles, "Name", "Name");
            return View();
        }
        //public IActionResult UserRoles()
        //{
        //    //ViewBag.Users = new SelectList(userManager.role.Where(x => x.StaffId != null), "Id", "UserName");
        //    //var userRoles = userManager.Users.SelectMany(u => userManager.GetRolesAsync(u).Result.Select(r => new UserRolesViewModel { UserId = u.Id, RoleId = r })).ToList();
        //    //return View(userRoles);
        //}

    }
}
