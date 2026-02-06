using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BOB.Shared.Identity;

public abstract class BaseController : Controller
{
    protected readonly UserManager<ApplicationUser> UserManager;

    protected BaseController(UserManager<ApplicationUser> userManager)
    {
        UserManager = userManager;
    }

    protected async Task<ApplicationUser> CurrentUser()
    {
        return await UserManager.GetUserAsync(User);
    }

    protected async Task<string> CurrentCompany()
    {
        return (await CurrentUser()).Company;
    }

    protected async Task<string> CurrentBranch()
    {
        return (await CurrentUser()).Branch;
    }
}
