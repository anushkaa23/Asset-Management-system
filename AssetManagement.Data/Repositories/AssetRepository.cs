using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data.Context;
using AssetManagement.Data.Entities;
using AssetManagement.Data.Interfaces;

namespace AssetManagement.Data.Repositories
{
    public class AssetRepository : IAssetRepository
    {
        private readonly AssetDbContext _context;

        public AssetRepository(AssetDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Asset>> GetAllAsync()
        {
            return await _context.Assets
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
        }

        public async Task<Asset> GetByIdAsync(int id)
        {
            return await _context.Assets
                .Include(a => a.Assignments)
                .FirstOrDefaultAsync(a => a.AssetId == id);
        }

        public async Task<Asset> AddAsync(Asset asset)
        {
            try
            {
                _context.Assets.Add(asset);
                await _context.SaveChangesAsync();
                return asset;
            }
            catch (DbUpdateException dbEx)
            {
                // throw a new exception with inner details so Blazor can show it,
                // or log it with your logging framework (preferred).
                var inner = dbEx.InnerException?.Message ?? dbEx.Message;
                // for development you can throw:
                throw new Exception("DB save failed: " + inner, dbEx);
                // in production, log detailed message and throw a safe message.
            }
        }

        public async Task<Asset> UpdateAsync(Asset asset)
        {
            // Detach any existing tracked entity
            var existingEntity = _context.Assets.Local
                .FirstOrDefault(a => a.AssetId == asset.AssetId);
            
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).State = EntityState.Detached;
            }

            asset.ModifiedDate = DateTime.UtcNow;
            _context.Assets.Update(asset);
            await _context.SaveChangesAsync();
            return asset;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset == null) return false;

            // Check if asset has any assignments
            var hasAssignments = await _context.Assignments
                .AnyAsync(a => a.AssetId == id);

            if (hasAssignments)
            {
                throw new InvalidOperationException("Cannot delete asset with assignment history");
            }

            _context.Assets.Remove(asset);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Asset>> GetAvailableAssetsAsync()
        {
            return await _context.Assets
                .Where(a => a.Status == AssetStatus.Available)
                .OrderBy(a => a.AssetName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Asset>> GetAssetsByTypeAsync(string type)
        {
            return await _context.Assets
                .Where(a => a.AssetType == type)
                .ToListAsync();
        }

        public async Task<IEnumerable<Asset>> GetAssetsByStatusAsync(AssetStatus status)
        {
            return await _context.Assets
                .Where(a => a.Status == status)
                .ToListAsync();
        }
    }
}
