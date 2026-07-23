using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Data;

public sealed class SSDbContextFactory : IDesignTimeDbContextFactory<SS_DbContext>
{
    public SS_DbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SS_DbContext>();
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\MSSQLLocalDB;Database=SkiiShopDesignTime;Trusted_Connection=True;TrustServerCertificate=True;");

        return new SS_DbContext(optionsBuilder.Options);
    }
}