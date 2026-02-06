using BOB.Shared.Data;
using BOB.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace BOB.Server.Services
{
    public class UserService : IUserService
    {
        private readonly BobDbContext _context;

        public UserService(BobDbContext context)
        {
            _context = context;
        }

        // ---------------- LOGIN ----------------

        public async Task<ServiceResponse<Users>> ValidateLoginAsync(string username, string password)
        {
            var response = new ServiceResponse<Users>();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null || !VerifyPassword(password, user.PassHash, user.PassKey))
            {
                response.Success = false;
                response.Message = "Invalid username or password";
                return response;
            }

            response.Response = user;
            return response;
        }

        // ---------------- REGISTER ----------------

        public async Task<ServiceResponse<bool>> RegisterAsync(Users user, string password)
        {
            var response = new ServiceResponse<bool>();

            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            {
                response.Success = false;
                response.Message = "Username already exists";
                response.Response = false;
                return response;
            }

            var (hash, key) = HashPassword(password);
            user.PassHash = hash;
            user.PassKey = key;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            response.Response = true;
            return response;
        }

        // ---------------- CRUD ----------------

        public async Task<ServiceResponse<IEnumerable<Users>>> GetAllAsync()
        {
            return new ServiceResponse<IEnumerable<Users>>
            {
                Response = await _context.Users.ToListAsync()
            };
        }

        public async Task<ServiceResponse<bool>> UpdateAsync(Users user, string? newPassword = null)
        {
            var response = new ServiceResponse<bool>();

            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            existingUser.Username = user.Username;
            existingUser.Company = user.Company;
            existingUser.Branch = user.Branch;
            existingUser.ERPUser = user.ERPUser;

            if (!string.IsNullOrEmpty(newPassword))
            {
                var (hash, key) = HashPassword(newPassword);
                existingUser.PassHash = hash;
                existingUser.PassKey = key;
            }

            await _context.SaveChangesAsync();
            response.Response = true;
            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(int id)
        {
            var response = new ServiceResponse<bool>();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            response.Response = true;
            return response;
        }

        // ---------------- ROLES ----------------

        public async Task<ServiceResponse<int>> EnsureRoleAsync(string roleName, string roleDescription, string branch, string company)
        {
            var response = new ServiceResponse<int>();

            var role = await _context.Roles.FirstOrDefaultAsync(r =>
                r.RoleName == roleName &&
                r.Branch == branch &&
                r.Company == company);

            if (role == null)
            {
                role = new Roles
                {
                    RoleName = roleName,
                    RoleDescription = roleDescription,
                    Branch = branch,
                    Company = company
                };

                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
            }

            response.Response = role.Id;
            return response;
        }

        public async Task<ServiceResponse<bool>> AssignRoleAsync(int userId, int roleId, string branch, string company)
        {
            var response = new ServiceResponse<bool>();

            var exists = await _context.UserRoles.AnyAsync(ur =>
                ur.UserId == userId &&
                ur.RoleId == roleId &&
                ur.Branch == branch &&
                ur.Company == company);

            if (!exists)
            {
                _context.UserRoles.Add(new UserRoles
                {
                    UserId = userId,
                    RoleId = roleId,
                    Branch = branch,
                    Company = company
                });

                await _context.SaveChangesAsync();
            }

            response.Response = true;
            return response;
        }

        // ---------------- PASSWORD ----------------

        private static (string Hash, string Key) HashPassword(string password)
        {
            using var hmac = new HMACSHA256();
            return (
                Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password))),
                Convert.ToBase64String(hmac.Key)
            );
        }

        private static bool VerifyPassword(string password, string storedHash, string storedKey)
        {
            using var hmac = new HMACSHA256(Convert.FromBase64String(storedKey));
            var computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
            return computedHash == storedHash;
        }
    }
}
