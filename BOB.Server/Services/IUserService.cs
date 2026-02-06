using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BOB.Shared.Entities;

namespace BOB.Server.Services
{
    public interface IUserService
    {
        Task<ServiceResponse<Users>> ValidateLoginAsync(string username, string password);
        Task<ServiceResponse<bool>> RegisterAsync(Users user, string password);

        Task<ServiceResponse<IEnumerable<Users>>> GetAllAsync();
        Task<ServiceResponse<bool>> UpdateAsync(Users user, string? newPassword = null);
        Task<ServiceResponse<bool>> DeleteAsync(int id);

        Task<ServiceResponse<int>> EnsureRoleAsync(string roleName, string roleDescription, string branch, string company);
        Task<ServiceResponse<bool>> AssignRoleAsync(int userId, int roleId, string branch, string company);
    }

}
