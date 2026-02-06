using BOB.Shared.Data;
using BOB.Shared.Entities;
using BOB.Shared.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BOB.GUI.Controllers
{
    public class ClientsController : Controller
    {
        private readonly BobDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClientsController(BobDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(await _context.Users.Where(c => c.Company == user.Company).ToListAsync());
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Users client)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                client.Company = user.Company;
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client = await _context.Users.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);
            if (client == null || client.Company != user.Company) return Unauthorized();
            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Users client)
        {
            if (id != client.Id) return NotFound();
            var user = await _userManager.GetUserAsync(User);
            if (client.Company != user.Company) return Unauthorized();

            _context.Update(client);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = await _context.Users.FindAsync(id);
            return client == null ? NotFound() : View(client);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Users.FindAsync(id);
            _context.Users.Remove(client);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
