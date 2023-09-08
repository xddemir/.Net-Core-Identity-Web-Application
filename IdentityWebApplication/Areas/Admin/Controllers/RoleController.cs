using IdentityWebApplication.Areas.Admin.Models;
using IdentityWebApplication.Extensions;
using IdentityWebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityWebApplication.Controllers;

[Authorize("Admin")]
[Area("Admin")]
public class RoleController : Controller
{
    private readonly UserManager<AppUser> _appUser;
    private readonly RoleManager<AppRole> _roleManager;

    public RoleController(UserManager<AppUser> appUser, RoleManager<AppRole> roleManager)
    {
        _appUser = appUser;
        _roleManager = roleManager;
    }

    [Authorize(Roles= "Admin,role-action")]
    public async Task<IActionResult> Index()
    {
        var roles = await _roleManager.Roles.Select(x => new RoleViewModel()
        {
            Id = x.Id,
            Name = x.Name!
        }).ToListAsync();

        return View(roles);
    }
    
    [Authorize(Roles= "role-action")]
    public IActionResult RoleCreate()
    {
        return View();
    }
    
    [HttpPost]
    [Authorize(Roles= "role-action")]
    public async Task<IActionResult> RoleCreate(RoleCreateViewModel request)
    {
        var result = await _roleManager.CreateAsync(new AppRole()
        {
            Name = request.Name

        });

        if (!result.Succeeded) {
            ModelState.AddModelErrorList(result.Errors);
            return View();
        }
        
        TempData["Success"] = "Role Created";
        ViewData["Success"] = "Role successfully created.";

        return RedirectToAction(nameof(RoleController.Index));
    }
    
    [Authorize(Roles= "role-action")]
    public async Task<IActionResult> RoleUpdate(string id)
    {
        var roleToUpdate = await _roleManager.FindByIdAsync(id);

        if (roleToUpdate == null) {
            throw new Exception("Could not find the particular role");
        }
        
        return View(new RoleUpdateViewModel()
        {
            Id = id,
            Name = roleToUpdate.Name!
        });
    }
    
    [HttpPost]
    [Authorize(Roles= "role-action")]
    public async Task<IActionResult> RoleUpdate(RoleUpdateViewModel request)
    {
        var roleToUpdate = await _roleManager.FindByIdAsync(request.Id);
        
        if (roleToUpdate == null) {
            throw new Exception("Could not find the particular role");
        }

        roleToUpdate.Name = request.Name;

        await _roleManager.UpdateAsync(roleToUpdate);

        ViewData["Success"] = "Role successfully updated.";
        
        return View();
    }
    
    [Authorize(Roles= "role-action")]
    public async Task<IActionResult> RoleDelete(string id)
    {
        var roleToDelete = await _roleManager.FindByIdAsync(id);

        var result = await _roleManager.DeleteAsync(roleToDelete);
        
        ViewData["Success"] = "Role successfully updated.";

        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.Select(x => x.Description).First());
        }

        TempData["Success"] = "Role Deleted";

        return RedirectToAction(nameof(RoleController.Index));
    }

    public async Task<IActionResult> RoleAssign(string id)
    {
        var currentUser = await _appUser.FindByIdAsync(id);

        ViewBag.userId = id;

        var roles = await _roleManager.Roles.ToListAsync();

        var roleViewModels = new List<RoleAssignViewModel>();

        var userRoles = await _appUser.GetRolesAsync(currentUser!);

        foreach (var role in roles)
        {
            var roleModel = new RoleAssignViewModel()
            {
                Id = role.Id,
                Name = role.Name!,
            };
            
            if (userRoles.Contains(role.Name!)) {
                roleModel.Exist = true;
            }

            roleViewModels.Add(roleModel);
        }
        
        return View(roleViewModels);
    }

    [HttpPost]
    public async Task<IActionResult> RoleAssign(string userId, List<RoleAssignViewModel> requestList)
    {
        var user = await _appUser.FindByIdAsync(userId);

        foreach (var roles in requestList)
        {
            if (roles.Exist) {
                await _appUser.AddToRoleAsync(user, roles.Name);
            }
            else {
                await _appUser.RemoveFromRoleAsync(user, roles.Name);
            }
        }


        return RedirectToAction(nameof(Areas.Admin.Controllers.HomeController.UserList), "Home");
    }
}