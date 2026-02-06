using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace BOB.Shared.Data
{
    public class BobDbContextFactory : IDesignTimeDbContextFactory<BobDbContext>
    {
        public BobDbContext CreateDbContext(string[] args)
        {
            // Read the connection string from appsettings.json in the startup project
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // EF runs from startup project
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<BobDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseSqlServer(connectionString);

            return new BobDbContext(builder.Options);
        }
    }
}
