using BOB.Shared.Identity;
using BOB.Shared.Data;
using BOB.Shared.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;

namespace BOB.GUI.Controllers
{
    public class TasksController : Controller
    {
        private readonly BobDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TasksController(BobDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var tasks = await _context.Tasks
                .Where(t => t.Company == user.Company)
                .ToListAsync();
            return View(tasks);
        }

        // CRUD methods go here...
    }
}
