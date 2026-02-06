using BOB.Shared.Data;
using BOB.Shared.Entities;
using BOB.Shared.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;

namespace BOB.GUI.Controllers
{
    [Authorize]
    public class RolesController : Controller
    {
        private readonly BobDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public RolesController(BobDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var roles = await _db.Roles
                .Where(r => r.Company == user.Company)
                .ToListAsync();

            return View(roles);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Roles role)
        {
            var user = await _userManager.GetUserAsync(User);

            role.Company = user.Company;
            role.Branch = user.Branch;

            _db.Roles.Add(role);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }

}
