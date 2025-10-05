using System.Collections.Generic;
using System.Threading.Tasks;
using AssetManagement.Business.DTOs;

namespace AssetManagement.Business.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDTO> GetDashboardDataAsync();
        Task<IEnumerable<dynamic>> GetWarrantyExpiryReportAsync(int days = 30);
        Task<IEnumerable<dynamic>> GetAssignmentHistoryReportAsync();
    }
}
