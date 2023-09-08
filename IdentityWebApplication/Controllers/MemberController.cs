using System.Security.Claims;
using IdentityWebApplication.Extensions;
using IdentityWebApplication.Models;
using IdentityWebApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.Extensions.FileProviders;

namespace IdentityWebApplication.Controllers;

[Authorize]
public class MemberController : Controller
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IFileProvider _fileProvider;

    public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager,
        IFileProvider fileProvider)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _fileProvider = fileProvider;
    }

    public async Task<IActionResult> Index()
    {
        var currUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

        var userViewModel = new UserViewModel()
        {
            Email = currUser.Email!,
            Phone = currUser.PhoneNumber!,
            UserName = currUser.UserName!
        };

        return View(userViewModel);
    }

    // GET
    public async Task Logout()
    {
        await _signInManager.SignOutAsync();
    }

    public IActionResult PasswordChange()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> PasswordChange(PasswordChangeViewModel request)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        var currUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

        var checkOldPassword = await _userManager.CheckPasswordAsync(currUser, request.PasswordOld);

        if (!checkOldPassword)
        {
            ModelState.AddModelError(string.Empty, "Old password is incorrect");
            return View();
        }

        var result = await _userManager.ChangePasswordAsync(currUser,
            request.PasswordOld,
            request.PasswordNew);

        if (!result.Succeeded)
        {
            ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());
            return View();
        }

        await _userManager.UpdateSecurityStampAsync(currUser);
        await _signInManager.SignOutAsync();
        await _signInManager.PasswordSignInAsync(currUser, request.PasswordNew, true, false);

        TempData["Success"] = "Password successfully changed";

        return View();
    }

    public async Task<IActionResult> UserEdit()
    {
        ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));
        var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

        var userEditModel = new UserEditViewModel()
        {
            UserName = currentUser!.UserName!,
            BirthDate = currentUser.BirthDate,
            City = currentUser.City,
            EmailAddress = currentUser.Email!,
            Gender = currentUser.Gender,
            Phone = currentUser.PhoneNumber!,
        };

        return View(userEditModel);
    }

    [HttpPost]
    public async Task<IActionResult> UserEdit(UserEditViewModel request)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

        currentUser.UserName = request.UserName;
        currentUser.Email = request.EmailAddress;
        currentUser.BirthDate = request.BirthDate;
        currentUser.City = request.City;
        currentUser.Gender = request.Gender;
        currentUser.PhoneNumber = request.Phone;

        if (request.Picture != null && request.Picture.Length > 0)
        {
            var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");
            var randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(request.Picture.FileName)}";

            var newPicturePath = Path.Combine(wwwrootFolder.First(x => x.Name == "userpictures").PhysicalPath!);

            using var stream = new FileStream(newPicturePath, FileMode.Create);

            await request.Picture.CopyToAsync(stream);

            currentUser.Picture = randomFileName;
        }

        var result = await _userManager.UpdateAsync(currentUser);

        if (!result.Succeeded)
        {
            ModelState.AddModelErrorList(result.Errors);
            return View();
        }

        await _userManager.UpdateSecurityStampAsync(currentUser);
        await _signInManager.SignOutAsync();

        if (request.BirthDate.HasValue)
        {
            await _signInManager.SignInWithClaimsAsync(currentUser, true, new[]
            {
                new Claim("birthDate", currentUser!.BirthDate!.Value.ToString())
            });
        }
        else
        {
            await _signInManager.SignInAsync(currentUser, true);
        }

        TempData["Success"] = "User information successfully changed";

        return View(new UserEditViewModel()
        {
            BirthDate = currentUser.BirthDate,
            City = currentUser.City,
            EmailAddress = currentUser.Email,
            Gender = currentUser.Gender,
            Phone = currentUser.PhoneNumber,
            UserName = currentUser.UserName
        });
    }

    [HttpGet]
    public IActionResult Claims()
    {
        var userClaim = User.Claims.Select(x => new ClaimViewModel()
        {
            Issuer = x.Issuer,
            Type = x.Type,
            Value = x.Value
        }).ToList();

        return View(userClaim);
    }

    [HttpGet]
    [Authorize(policy: "AnkaraPolicy")]
    public IActionResult AnkaraPage()
    {
        return View();
    }
    
    [HttpGet]
    [Authorize(policy: "ViolencePolicy")]
    public IActionResult ViolencePage()
    {
        return View();
    }

    [HttpGet]
    [Authorize(policy: "ExchangeExpireDate")]
    public IActionResult ExchangeExpireDate()
    {
        return View();
    }
}