using BOB.Shared.Identity;
using BOB.Shared.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace BOB.GUI.Extensions
{
    public static class UserExtensions
    {
        public static async Task<string> GetCompany(
            this ClaimsPrincipal user,
            UserManager<ApplicationUser> userManager)
        {
            return (await userManager.GetUserAsync(user)).Company;
        }
    }

}
