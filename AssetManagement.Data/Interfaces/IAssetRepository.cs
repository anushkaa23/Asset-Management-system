using System.Collections.Generic;
using System.Threading.Tasks;
using AssetManagement.Data.Entities;

namespace AssetManagement.Data.Interfaces
{
    public interface IAssetRepository
    {
        Task<IEnumerable<Asset>> GetAllAsync();
        Task<Asset> GetByIdAsync(int id);
        Task<Asset> AddAsync(Asset asset);
        Task<Asset> UpdateAsync(Asset asset);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Asset>> GetAvailableAssetsAsync();
        Task<IEnumerable<Asset>> GetAssetsByTypeAsync(string type);
        Task<IEnumerable<Asset>> GetAssetsByStatusAsync(AssetStatus status);
    }
}
