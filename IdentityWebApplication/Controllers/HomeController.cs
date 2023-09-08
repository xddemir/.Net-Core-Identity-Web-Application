using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using IdentityWebApplication.Extensions;
using Microsoft.AspNetCore.Mvc;
using IdentityWebApplication.Models;
using IdentityWebApplication.Services;
using IdentityWebApplication.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Elfie.Serialization;

namespace IdentityWebApplication.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IEmailService _emailService;

    public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager, IEmailService emailService)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpViewModel request)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        var identityResult = await _userManager.CreateAsync(new()
        {
            UserName = request.UserName,
            PhoneNumber = request.Phone,
            Email = request.EmailAddress,
            City = "Istanbul"
        }, request.Password);

        if (!identityResult.Succeeded)
        {
            ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());
            return View();
        }

        var exchangeExpireClaim = new Claim("ExchangeExpireDate", DateTime.Now.AddDays(10).ToString());
        var user = await _userManager.FindByNameAsync(request.UserName);

        var claimsResult = await _userManager.AddClaimAsync(user!, exchangeExpireClaim);

        if (!claimsResult.Succeeded)
        {
            return View();
        }

        TempData["SuccessMessage"] = "User created!";

        return RedirectToAction(nameof(HomeController.SignUp));
    }

    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignIn(SignInViewModel request, string? returnUrl = null)
    {
        returnUrl = returnUrl ?? Url.Action("Index", "Member");

        var isUser = await _userManager.FindByEmailAsync(request.Email);

        if (isUser == null)
        {
            ModelState.AddModelError(string.Empty, "Email or Password is incorrect!");
        }

        var signInResult = await _signInManager.PasswordSignInAsync(isUser,
            request.Password,
            request.RememberMe,
            true);

        if (signInResult.IsLockedOut)
        {
            ModelState.AddModelErrorList(new List<string>() { "Your account has been blocked for 3 minutes" });
            return View();
        }

        if (!signInResult.Succeeded)
        {
            var failCount = await _userManager.GetAccessFailedCountAsync(isUser);
            ModelState.AddModelErrorList(new List<string>()
                { $"Password or Email is incorrect! ({failCount} attempts)" });
            return View();
        }

        if (isUser.BirthDate.HasValue)
        {
            await _signInManager.SignInWithClaimsAsync(isUser, request.RememberMe, new[]
            {
                new Claim("Birthdate", isUser.BirthDate.Value.ToString())
            });
        }

        return Redirect(returnUrl);
    }

    public IActionResult SignIn()
    {
        return View();
    }

    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
    {
        var hasUser = await _userManager.FindByEmailAsync(forgotPasswordViewModel.Email);

        if (hasUser == null)
        {
            ModelState.AddModelError(string.Empty, "Not user found by this email!");
            return View();
        }

        string resetToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser);

        // https://localhost:7059?userId=123&token=qwelqw
        var resetLink = Url.Action("ResetPassword",
            "Home",
            new { userId = hasUser.Id, Token = resetToken },
            protocol: HttpContext.Request.Scheme);

        await _emailService.SendResetPasswordEmailLink(resetLink, hasUser.Email);

        TempData["Success"] = "Reset Password link has been sent to your email address";
        return RedirectToAction(nameof(HomeController.ForgotPassword));
    }

    public IActionResult ResetPassword(string userId, string token)
    {
        TempData["userId"] = userId;
        TempData["Token"] = token;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel request)
    {
        var userId = TempData["userId"];
        var token = TempData["Token"];

        if (userId == null || token == null)
        {
            throw new Exception("Error occurred!");
        }

        var hasUser = await _userManager.FindByIdAsync(userId.ToString()!);
        if (hasUser == null)
        {
            ModelState.AddModelError(string.Empty, "User not found");
            return View();
        }

        var result = await _userManager.ResetPasswordAsync(hasUser, token.ToString()!, request.Password);

        if (result.Succeeded)
        {
            TempData["Success"] = "Password successfully changed";
        }
        else
        {
            ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());
        }

        return View();
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}