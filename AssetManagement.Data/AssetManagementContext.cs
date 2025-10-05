using Microsoft.EntityFrameworkCore;
using AssetManagement.Data.Models;

namespace AssetManagement.Data
{
    public class AssetManagementContext : DbContext
    {
        public AssetManagementContext(DbContextOptions<AssetManagementContext> options)
            : base(options)
        {
        }

        public DbSet<Asset> Assets { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
