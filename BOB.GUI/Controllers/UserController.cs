using Azure;
using BOB.Server.Services;
using BOB.Shared.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using IUserService = BOB.Server.Services.IUserService;

namespace BOB.GUI.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        #region Login / Logout

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            var result = await _userService.ValidateLoginAsync(username, password);

            if (!result.Success || result.Response == null)
            {
                ModelState.AddModelError("", result.Message);
                return View();
            }

            var user = result.Response;

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim("Company", user.Company),
        new Claim("Branch", user.Branch)
    };

            var identity = new ClaimsIdentity(claims, "UserCookieAuth");
            await HttpContext.SignInAsync("UserCookieAuth", new ClaimsPrincipal(identity));

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("UserCookieAuth");
            return RedirectToAction("Login");
        }

        #endregion

        #region Register

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string username, string password, string company, string branch, string role)
        {
            var user = new Users
            {
                Username = username,
                Company = company,
                Branch = branch,
                ERPUser = false
            };

            var registerResult = await _userService.RegisterAsync(user, password);

            if (!registerResult.Success)
            {
                ModelState.Clear();

                ViewData["ClearForm"] = true;
                ModelState.AddModelError("", registerResult.Message);
                return View();


             
     
            }

            var roleDescription = role == "ADMIN"
                ? "Users have elevated privileges"
                : "Standard system users";

            var roleResult = await _userService.EnsureRoleAsync(role, roleDescription, branch, company);
            await _userService.AssignRoleAsync(user.Id, roleResult.Response, branch, company);

            return RedirectToAction("Login");
        }


        #endregion

        #region CRUD

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllAsync();
            return View(users.Response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string username, string? password, string company, string branch)
        {
            var user = new Users
            {
                Id = id,
                Username = username,
                Company = company,
                Branch = branch,
                ERPUser = false
            };

            await _userService.UpdateAsync(user, password);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        #endregion
    }
}
