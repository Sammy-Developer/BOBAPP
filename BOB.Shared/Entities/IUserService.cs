using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOB.Shared.Entities
{
    public interface IUserService
    {
        Task<Users?> GetByIdAsync(int id);
        Task<Users?> GetByUsernameAsync(string username);
        Task<IEnumerable<Users>> GetAllAsync();
        Task<bool> RegisterAsync(Users user, string password);
        Task<Users?> ValidateLoginAsync(string username, string password);
        Task<bool> UpdateAsync(Users user, string? newPassword = null);
        Task<bool> DeleteAsync(int id);
    }
}
