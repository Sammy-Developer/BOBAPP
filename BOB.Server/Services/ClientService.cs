using BOB.Shared.Data;
using BOB.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace BOB.Server.Services
{
    public class ClientService
    {
        private readonly BobDbContext _context;
        public ClientService(BobDbContext context) => _context = context;

        public async Task<List<Users>> GetClientsByCompanyAsync(string company)
        {
            return await _context.Users.Where(u => u.Company == company).ToListAsync();
        }
    }
}
