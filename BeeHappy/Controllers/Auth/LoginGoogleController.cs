using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Security.Claims;
using BusinessObjects;
using BusinessObjects.NestedObjects;
using CommonObjects.AppConstants;
using Services.Implementations;

namespace BeeHappy.Controllers.Auth;

public class LoginGoogleController : Controller
{
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;

    public LoginGoogleController(IUserService userService, IJwtService jwtService, IConfiguration configuration)
    {
        _userService = userService;
        _jwtService = jwtService;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Login(string? source = "/")
    {
        var redirectUrl = Url.Action("GoogleResponse", "LoginGoogle", new { from = source });
        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUrl
        };

        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet]
    public async Task<IActionResult> GoogleResponse(string from = "/")
    {
        var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

        if (!result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }

        var googleId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
        var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(googleId) || string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Index", "Home");
        }

        // check user tồn tại
        var user = (await _userService.GetUsersAsync(u => u.Email.Equals(email))).FirstOrDefault();

        if (user == null)
        {
            user = new User
            {
                Email = email,
                Username = name ?? email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Roles = new List<string> { RoleConstants.User },
                Badges = new(),
                Paints = new(),
                IsPremium = false,
                Profile = new(),
                Editors = new(),
                GoogleId = googleId,
            };

            await _userService.InsertUserAsync(user);
            user = (await _userService.GetUsersAsync(u => u.Email.Equals(email))).FirstOrDefault();
        }

        if (from == "extension")
        {
            return HandleLoginFromExtension(user);
        }

        // Claims để tạo cookie đăng nhập
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(UserConstants.USER_AVATAR,
                string.IsNullOrEmpty(user.Profile?.AvatarUrl)
                    ? UserConstants.DEFAULT_USER_AVATAR_LINK
                    : user.Profile.AvatarUrl)
        };

        if (user.Roles != null && user.Roles.Any())
        {
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }
        else
        {
            claims.Add(new Claim(ClaimTypes.Role, RoleConstants.User));
        }

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(claimsIdentity);

        // Tạo cookie auth
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        HttpContext.Session.SetString("Username", user.Username);
        HttpContext.Session.SetString("Roles", string.Join(",", user.Roles ?? [RoleConstants.User]));

        return RedirectToAction("Index", "Home");
    }

    private IActionResult HandleLoginFromExtension(User user)
    {
        var jwt = _jwtService.GenerateToken(user.Id.ToString(), user.Email, user.Roles.FirstOrDefault() ?? RoleConstants.User);
        // Have to build full URL, because if the token is lowercase, it will not work
        var url = Url.Action("AuthComplete", "Extension", null, Request.Scheme);
        return Redirect($"{url}?token={jwt}");
    }
}