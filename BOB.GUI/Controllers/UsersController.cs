using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BOB.Shared.Data;
using BOB.Shared.Entities;
using Microsoft.AspNetCore.Identity;
using BOB.Shared.Identity;

[Authorize]
public class UsersController : BaseController
{
    private readonly BobDbContext _db;

    public UsersController(
        BobDbContext db,
        UserManager<ApplicationUser> userManager)
        : base(userManager)
    {
        _db = db;
    }

    // Assign role to user
    [HttpPost]
    public async Task<IActionResult> AssignRole(int userId, int roleId)
    {
        var company = await CurrentCompany();
        var branch = await CurrentBranch();

        // Prevent cross-company role leaks
        bool roleValid = await _db.Roles
            .AnyAsync(r => r.Id == roleId && r.Company == company);

        if (!roleValid)
            return Unauthorized();

        // Prevent duplicate role assignment
        bool exists = await _db.UserRoles.AnyAsync(ur =>
            ur.UserId == userId &&
            ur.RoleId == roleId &&
            ur.Company == company);

        if (!exists)
        {
            _db.UserRoles.Add(new UserRoles
            {
                UserId = userId,
                RoleId = roleId,
                Company = company,
                Branch = branch
            });

            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Index()
    {
        var company = await CurrentCompany();

        ViewBag.Roles = await _db.Roles
            .Where(r => r.Company == company)
            .ToListAsync();

        var users = await _db.Users
            .Where(u => u.Company == company)
            .ToListAsync();

        return View(users);
    }

}

