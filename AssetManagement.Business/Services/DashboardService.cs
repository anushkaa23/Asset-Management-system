using System.Collections.Generic;
using System.Threading.Tasks;
using AssetManagement.Business.DTOs;
using AssetManagement.Business.Interfaces;
using AssetManagement.Data.Repositories;

namespace AssetManagement.Business.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly DapperRepository _dapperRepository;

        public DashboardService(DapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        public async Task<DashboardDTO> GetDashboardDataAsync()
        {
            var stats = await _dapperRepository.GetDashboardStatsAsync();
            var assetsByType = await _dapperRepository.GetAssetsByTypeAsync();

            return new DashboardDTO
            {
                TotalAssets = stats.GetValueOrDefault("TotalAssets", 0),
                AssignedAssets = stats.GetValueOrDefault("AssignedAssets", 0),
                AvailableAssets = stats.GetValueOrDefault("AvailableAssets", 0),
                UnderRepairAssets = stats.GetValueOrDefault("UnderRepairAssets", 0),
                RetiredAssets = stats.GetValueOrDefault("RetiredAssets", 0),
                SpareAssets = stats.GetValueOrDefault("SpareAssets", 0),
                ActiveEmployees = stats.GetValueOrDefault("ActiveEmployees", 0),
                AssetsByType = assetsByType
            };
        }

        public async Task<IEnumerable<dynamic>> GetWarrantyExpiryReportAsync(int days = 30)
        {
            return await _dapperRepository.GetAssetsNearingWarrantyExpiryAsync(days);
        }

        public async Task<IEnumerable<dynamic>> GetAssignmentHistoryReportAsync()
        {
            return await _dapperRepository.GetAssignmentHistoryReportAsync();
        }
    }
}