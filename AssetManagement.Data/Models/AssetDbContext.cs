using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AssetManagement.Data.Context
{
    public class AssetDbContextFactory : IDesignTimeDbContextFactory<AssetDbContext>
    {
        public AssetDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AssetDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=AssetManagementDB;Trusted_Connection=True;");

            return new AssetDbContext(optionsBuilder.Options);
        }
    }
}
