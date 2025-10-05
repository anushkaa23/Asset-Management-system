using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace AssetManagement.Data.Repositories
{
    public class DapperRepository
    {
        private readonly string _connectionString;

        public DapperRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // Dashboard statistics
        public async Task<Dictionary<string, int>> GetDashboardStatsAsync()
        {
            using var connection = CreateConnection();
            
            var sql = @"
                SELECT 
                    (SELECT COUNT(*) FROM Assets) as TotalAssets,
                    (SELECT COUNT(*) FROM Assets WHERE Status = 1) as AssignedAssets,
                    (SELECT COUNT(*) FROM Assets WHERE Status = 0) as AvailableAssets,
                    (SELECT COUNT(*) FROM Assets WHERE Status = 2) as UnderRepairAssets,
                    (SELECT COUNT(*) FROM Assets WHERE Status = 3) as RetiredAssets,
                    (SELECT COUNT(*) FROM Assets WHERE IsSpare = 1) as SpareAssets,
                    (SELECT COUNT(*) FROM Employees WHERE IsActive = 1) as ActiveEmployees";

            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(sql);
            
            return new Dictionary<string, int>
            {
                { "TotalAssets", result?.TotalAssets ?? 0 },
                { "AssignedAssets", result?.AssignedAssets ?? 0 },
                { "AvailableAssets", result?.AvailableAssets ?? 0 },
                { "UnderRepairAssets", result?.UnderRepairAssets ?? 0 },
                { "RetiredAssets", result?.RetiredAssets ?? 0 },
                { "SpareAssets", result?.SpareAssets ?? 0 },
                { "ActiveEmployees", result?.ActiveEmployees ?? 0 }
            };
        }

        // Assets by type
        public async Task<Dictionary<string, int>> GetAssetsByTypeAsync()
        {
            using var connection = CreateConnection();
            
            var sql = "SELECT AssetType, COUNT(*) as Count FROM Assets GROUP BY AssetType";
            var results = await connection.QueryAsync<(string AssetType, int Count)>(sql);
            
            return results.ToDictionary(r => r.AssetType, r => r.Count);
        }

        // Assets nearing warranty expiry (within 30 days)
        public async Task<IEnumerable<dynamic>> GetAssetsNearingWarrantyExpiryAsync(int days = 30)
        {
            using var connection = CreateConnection();
            
            var sql = @"
                SELECT 
                    AssetId,
                    AssetName,
                    AssetType,
                    SerialNumber,
                    WarrantyExpiryDate,
                    DATEDIFF(DAY, GETDATE(), WarrantyExpiryDate) as DaysRemaining
                FROM Assets
                WHERE WarrantyExpiryDate IS NOT NULL
                    AND WarrantyExpiryDate >= GETDATE()
                    AND DATEDIFF(DAY, GETDATE(), WarrantyExpiryDate) <= @Days
                ORDER BY WarrantyExpiryDate";

            return await connection.QueryAsync(sql, new { Days = days });
        }

        // Assignment history report
        public async Task<IEnumerable<dynamic>> GetAssignmentHistoryReportAsync()
        {
            using var connection = CreateConnection();
            
            var sql = @"
                SELECT 
                    a.AssignmentId,
                    ast.AssetName,
                    ast.AssetType,
                    ast.SerialNumber,
                    e.FullName as EmployeeName,
                    e.Department,
                    a.AssignedDate,
                    a.ReturnedDate,
                    a.Notes,
                    a.IsActive
                FROM Assignments a
                INNER JOIN Assets ast ON a.AssetId = ast.AssetId
                INNER JOIN Employees e ON a.EmployeeId = e.EmployeeId
                ORDER BY a.AssignedDate DESC";

            return await connection.QueryAsync(sql);
        }
    }
}