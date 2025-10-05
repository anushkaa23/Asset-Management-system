using System.Collections.Generic;
using System.Threading.Tasks;
using AssetManagement.Business.DTOs;
using AssetManagement.Data.Entities;

namespace AssetManagement.Business.Interfaces
{
    public interface IAssetService
    {
        Task<IEnumerable<AssetDTO>> GetAllAssetsAsync();
        Task<AssetDTO> GetAssetByIdAsync(int id);
        Task<AssetDTO> CreateAssetAsync(AssetDTO assetDto);
        Task<AssetDTO> UpdateAssetAsync(AssetDTO assetDto);
        Task<bool> DeleteAssetAsync(int id);
        Task<IEnumerable<AssetDTO>> GetAvailableAssetsAsync();
        Task<IEnumerable<AssetDTO>> GetAssetsByStatusAsync(AssetStatus status);
    }
}
