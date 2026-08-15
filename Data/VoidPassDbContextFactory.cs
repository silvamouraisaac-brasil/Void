using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace VoidPass.Data;

public class VoidPassDbContextFactory : IDesignTimeDbContextFactory<VoidPassDbContext>
{
    public VoidPassDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var connectionString =
            configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<VoidPassDbContext>();

        optionsBuilder.UseNpgsql(connectionString);

        return new VoidPassDbContext(optionsBuilder.Options);
    }
}