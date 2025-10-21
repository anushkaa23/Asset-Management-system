using System;
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

        // Interface method - no parameters
        public async Task<IEnumerable<Asset>> GetAllAsync()
        {
            return await _context.Assets
                .AsNoTracking()
                .OrderBy(a => a.AssetName)
                .ToListAsync();
        }

        // Optional overload for pagination (preserved from your original)
        public async Task<IEnumerable<Asset>> GetAllAsync(int page, int pageSize)
        {
            return await _context.Assets
                .AsNoTracking()
                .OrderBy(a => a.AssetName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Asset> GetByIdAsync(int id)
        {
            return await _context.Assets
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AssetId == id);
        }

        public async Task<Asset> AddAsync(Asset asset)
        {
            _context.Assets.Add(asset);
            await _context.SaveChangesAsync();
            return asset;
        }

        public async Task<Asset> UpdateAsync(Asset asset)
        {
            try
            {
                // FIXED: Fetch and update existing entity to avoid tracking conflicts
                var existingAsset = await _context.Assets
                    .FirstOrDefaultAsync(a => a.AssetId == asset.AssetId);

                if (existingAsset == null)
                    throw new InvalidOperationException("Asset not found");

                existingAsset.AssetName = asset.AssetName;
                existingAsset.AssetType = asset.AssetType;
                existingAsset.MakeModel = asset.MakeModel;
                existingAsset.SerialNumber = asset.SerialNumber;
                existingAsset.PurchaseDate = asset.PurchaseDate;
                existingAsset.WarrantyExpiryDate = asset.WarrantyExpiryDate;
                existingAsset.Condition = asset.Condition;
                existingAsset.Status = asset.Status;
                existingAsset.IsSpare = asset.IsSpare;
                existingAsset.Specifications = asset.Specifications;
                existingAsset.ModifiedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return existingAsset;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Database error: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var asset = await _context.Assets
                    .FirstOrDefaultAsync(a => a.AssetId == id);

                if (asset == null) return false;

                // FIXED: Check if asset status is "Retired" using enum
                if (asset.Status != AssetStatus.Retired)
                    throw new InvalidOperationException("Only retired assets can be deleted");

                // Check if asset has any active assignments
                var hasActiveAssignments = await _context.Assignments
                    .AnyAsync(a => a.AssetId == id && a.IsActive);

                if (hasActiveAssignments)
                    throw new InvalidOperationException("Cannot delete asset with active assignments");

                // Check if asset has ANY assignments (even inactive ones)
                var hasAnyAssignments = await _context.Assignments
                    .AnyAsync(a => a.AssetId == id);

                if (hasAnyAssignments)
                    throw new InvalidOperationException("Cannot delete asset with assignment history. Please retire the asset instead.");

                _context.Assets.Remove(asset);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Database error while deleting: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Asset>> GetAvailableAssetsAsync()
        {
            return await _context.Assets
                .AsNoTracking()
                .Where(a => a.Status == AssetStatus.Available)
                .OrderBy(a => a.AssetName)
                .ToListAsync();
        }

        // Preserved from your original (not in interface but might be used)
        public async Task<IEnumerable<Asset>> GetAssetsByCategoryAsync(string category)
        {
            return await _context.Assets
                .AsNoTracking()
                .Where(a => a.AssetType == category)
                .OrderBy(a => a.AssetName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Asset>> GetAssetsByTypeAsync(string type)
        {
            return await _context.Assets
                .AsNoTracking()
                .Where(a => a.AssetType == type)
                .OrderBy(a => a.AssetName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Asset>> GetAssetsByStatusAsync(AssetStatus status)
        {
            return await _context.Assets
                .AsNoTracking()
                .Where(a => a.Status == status)
                .OrderBy(a => a.AssetName)
                .ToListAsync();
        }

        // Preserved from your original (not in interface but might be used)
        public async Task<bool> AssetTagExistsAsync(string assetTag, int? excludeId = null)
        {
            if (excludeId.HasValue)
                return await _context.Assets.AnyAsync(a => a.AssetType == assetTag && a.AssetId != excludeId.Value);

            return await _context.Assets.AnyAsync(a => a.AssetType == assetTag);
        }

        // Preserved from your original (not in interface but might be used)
        public async Task<bool> SerialNumberExistsAsync(string serialNumber, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(serialNumber)) return false;

            if (excludeId.HasValue)
                return await _context.Assets.AnyAsync(a => a.SerialNumber == serialNumber && a.AssetId != excludeId.Value);

            return await _context.Assets.AnyAsync(a => a.SerialNumber == serialNumber);
        }
    }
}