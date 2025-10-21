using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetManagement.Business.DTOs;
using AssetManagement.Business.Interfaces;
using AssetManagement.Data.Entities;
using AssetManagement.Data.Interfaces;

namespace AssetManagement.Business.Services
{
    public class AssetService : IAssetService
    {
        private readonly IAssetRepository _assetRepository;

        public AssetService(IAssetRepository assetRepository)
        {
            _assetRepository = assetRepository;
        }

        public async Task<IEnumerable<AssetDTO>> GetAllAssetsAsync()
        {
            var assets = await _assetRepository.GetAllAsync();
            return assets.Select(MapToDTO);
        }

        public async Task<AssetDTO?> GetAssetByIdAsync(int id)
        {
            var asset = await _assetRepository.GetByIdAsync(id);
            return asset != null ? MapToDTO(asset) : null;
        }

        public async Task<AssetDTO> CreateAssetAsync(AssetDTO assetDto)
        {
            var asset = new Asset
            {
                AssetName = assetDto.AssetName,
                AssetType = assetDto.AssetType,
                MakeModel = assetDto.MakeModel,
                SerialNumber = assetDto.SerialNumber,
                PurchaseDate = assetDto.PurchaseDate,
                WarrantyExpiryDate = assetDto.WarrantyExpiryDate,
                Condition = assetDto.Condition,
                Status = assetDto.Status,
                IsSpare = assetDto.IsSpare,
                Specifications = assetDto.Specifications,
                CreatedDate = DateTime.UtcNow  // FIXED: Set CreatedDate
            };
            
            var created = await _assetRepository.AddAsync(asset);
            return MapToDTO(created);
        }

        public async Task<AssetDTO> UpdateAssetAsync(AssetDTO assetDto)
        {
            // FIXED: Create entity with AssetId for update
            var asset = new Asset
            {
                AssetId = assetDto.AssetId,  // Important: Include the ID
                AssetName = assetDto.AssetName,
                AssetType = assetDto.AssetType,
                MakeModel = assetDto.MakeModel,
                SerialNumber = assetDto.SerialNumber,
                PurchaseDate = assetDto.PurchaseDate,
                WarrantyExpiryDate = assetDto.WarrantyExpiryDate,
                Condition = assetDto.Condition,
                Status = assetDto.Status,
                IsSpare = assetDto.IsSpare,
                Specifications = assetDto.Specifications
                // Note: CreatedDate and ModifiedDate are handled by the repository
            };
            
            var updated = await _assetRepository.UpdateAsync(asset);
            return MapToDTO(updated);
        }

        public async Task<bool> DeleteAssetAsync(int id)
        {
            try
            {
                return await _assetRepository.DeleteAsync(id);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        public async Task<IEnumerable<AssetDTO>> GetAvailableAssetsAsync()
        {
            var assets = await _assetRepository.GetAvailableAssetsAsync();
            return assets.Select(MapToDTO);
        }

        public async Task<IEnumerable<AssetDTO>> GetAssetsByStatusAsync(AssetStatus status)
        {
            var assets = await _assetRepository.GetAssetsByStatusAsync(status);
            return assets.Select(MapToDTO);
        }

        private AssetDTO MapToDTO(Asset asset)
        {
            return new AssetDTO
            {
                AssetId = asset.AssetId,
                AssetName = asset.AssetName,
                AssetType = asset.AssetType,
                MakeModel = asset.MakeModel ?? string.Empty,
                SerialNumber = asset.SerialNumber ?? string.Empty,
                PurchaseDate = asset.PurchaseDate,
                WarrantyExpiryDate = asset.WarrantyExpiryDate,
                Condition = asset.Condition,
                Status = asset.Status,
                IsSpare = asset.IsSpare,
                Specifications = asset.Specifications ?? string.Empty
            };
        }
    }
}