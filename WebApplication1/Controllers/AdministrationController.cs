using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Security.Claims;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    [Authorize(Roles ="Admin rady")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<AdministrationController> logger;

        public AdministrationController(RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManager,
            ILogger<AdministrationController> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = userManager.Users;
            return View(users);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if(ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole()
                {
                    Name = model.RoleName
                };
                var IdentityResult = await roleManager.CreateAsync(role);

                if(IdentityResult.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administration");
                }
                foreach(var error in IdentityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if(role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id={id} cannot be found";
                return View("NotFound");
            }
            var model = new EditRoleViewModel()
            {
                RoleName = role.Name,
                Id = role.Id,
            };
            foreach(var user in await userManager.Users.ToListAsync())
            {

                if(await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);

                }
            }
            return View(model);

        }
        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id={model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);
                if(result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            
            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with id {role.Id} couldn't be found";
                return View("NotFound");
            }

            var model = new List<UserRoleViewModel>();
            foreach(var user in await userManager.Users.ToListAsync())
            {
                var userRoleViewModel = new UserRoleViewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                };
                if(await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"role with id = {roleId} was not found";
                return View("NotFound");
            }
            foreach (var modelet in model)
            {
                var user = await userManager.FindByIdAsync(modelet.UserId);
                if (modelet.IsSelected && !(await userManager.IsInRoleAsync(user, role.Name))) {
                    var result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!modelet.IsSelected && (await userManager.IsInRoleAsync(user, role.Name)))
                {
                    var result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
            }

            return RedirectToAction("EditRole", new { id = roleId });
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with id {id} couldn't be found";
                return View("NotFound");
            }

            var userClaims = await userManager.GetClaimsAsync(user);
            var userRoles = await userManager.GetRolesAsync(user);

            var model = new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                City = user.city,
                Claims = userClaims.Select(c => c.Type + " : " + c.Value).ToList(),
                Roles = userRoles
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id {model.Id} couldn't be found";
                return View("NotFound");
            }

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.city = model.City;

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("ListUsers", "Administration");
            }

            foreach(var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }



            return View(model);
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if( user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = await userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("ListUsers");
            }
        }

        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                try
                {
                    var result = await roleManager.DeleteAsync(role);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View("ListRoles");
                }
                catch(DbUpdateException ex) 
                {
                    logger.LogError($"Error deleting role {ex}");
                    ViewBag.ErrorTitle = $"Role {role.Name} is in use";
                    ViewBag.ErrorMsg = $"{role.Name} cannot be deleted because there are users in this role" +
                        $"if you want to delete this role,please remove all users from the role and try again";
                    return View("Error");
                }

            }
        }

        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            ViewBag.Id = userId;

            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with id: {userId} could't be found";
                return View("NotFound");
            }

            var roles = await roleManager.Roles.ToListAsync();
            var model = new List<ManageUserViewModel>();
            foreach (var role in roles)
            {
                var usermodel = new ManageUserViewModel()
                {
                    roleName = role.Name,
                    roleId = role.Id,
                };

                if(await userManager.IsInRoleAsync(user, role.Name))
                {
                    usermodel.isSelected = true;
                }
                else
                {
                    usermodel.isSelected = false;
                }

                model.Add(usermodel);   
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserRoles(List<ManageUserViewModel> model,string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id: {userId} could't be found";
                return View("NotFound");
            }

            var roles = await userManager.GetRolesAsync(user);

            var result = await userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Couldn't remove user from roles");
                return View(model);
            }
            var addedResult = await userManager.AddToRolesAsync(user, model.Where(x => x.isSelected).Select(y=>y.roleName));

            if (!addedResult.Succeeded){
                ModelState.AddModelError("", "Couldn't added user to roles");
                return View(model);
            }

            return RedirectToAction("EditUser", new {id = userId });
        }

        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with id: {userId} could not be found";
                return View("NotFound");
            }

            var existingClaims = await userManager.GetClaimsAsync(user);
            var model = new UserClaimsViewModel()
            {
                UserId = userId
            };

            foreach (var claim in ClaimsStore.AllClaims)
            {
                var userClaim = new UserClaim()
                {
                    claimType = claim.Type
                };
                System.Diagnostics.Debug.WriteLine(claim.Type + " : " +claim.Value);

                if (existingClaims.Any(c=>c.Type == claim.Type && c.Value=="true"))
                {
                    userClaim.isSelected = true;
                }

                model.Claims.Add(userClaim);

            }

            return View(model);

        }
        [HttpPost]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)

        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id: {model.UserId} could not be found, this is my fault";
                return View("NotFound");
            }

            var claims = await userManager.GetClaimsAsync(user);
            var result = await userManager.RemoveClaimsAsync(user, claims);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Couldn't remove user Claims");
                return View(model);
            }

            result = await userManager.AddClaimsAsync(user, model.Claims.Select(c=> new Claim(c.claimType,c.isSelected ? "true":"false")));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Couldn't Add selected claims to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = model.UserId });


        }
    }
}
