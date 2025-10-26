using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace AssetManagement.Data.Repositories
{
    public class DapperRepository
    {
        private readonly IConfiguration _configuration;

        public DapperRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private IDbConnection CreateConnection()
        {
            var databaseUrl = System.Environment.GetEnvironmentVariable("DATABASE_URL");
            
            if (!string.IsNullOrEmpty(databaseUrl))
            {
                // Production: Parse DATABASE_URL for PostgreSQL
                var uriString = databaseUrl.Replace("postgres://", "postgresql://");
                var uri = new System.Uri(uriString);
                
                var username = uri.UserInfo.Split(':')[0];
                var password = uri.UserInfo.Split(':')[1];
                var host = uri.Host;
                var port = uri.Port > 0 ? uri.Port : 5432;
                var database = uri.AbsolutePath.TrimStart('/');
                
                var connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
                
                return new NpgsqlConnection(connectionString);
            }
            else
            {
                // Local: Use local PostgreSQL
                var connectionString = "Host=localhost;Port=5432;Database=AssetManagementDB;Username=postgres;Password=postgres";
                return new NpgsqlConnection(connectionString);
            }
        }

        // Dashboard statistics
        public async Task<Dictionary<string, int>> GetDashboardStatsAsync()
        {
            using var connection = CreateConnection();
            
            var sql = @"
                SELECT 
                    (SELECT COUNT(*) FROM ""Assets"") as TotalAssets,
                    (SELECT COUNT(*) FROM ""Assets"" WHERE ""Status"" = 1) as AssignedAssets,
                    (SELECT COUNT(*) FROM ""Assets"" WHERE ""Status"" = 0) as AvailableAssets,
                    (SELECT COUNT(*) FROM ""Assets"" WHERE ""Status"" = 2) as UnderRepairAssets,
                    (SELECT COUNT(*) FROM ""Assets"" WHERE ""Status"" = 3) as RetiredAssets,
                    (SELECT COUNT(*) FROM ""Assets"" WHERE ""IsSpare"" = true) as SpareAssets,
                    (SELECT COUNT(*) FROM ""Employees"" WHERE ""IsActive"" = true) as ActiveEmployees";

            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(sql);
            
            return new Dictionary<string, int>
            {
                { "TotalAssets", result?.totalassets ?? 0 },
                { "AssignedAssets", result?.assignedassets ?? 0 },
                { "AvailableAssets", result?.availableassets ?? 0 },
                { "UnderRepairAssets", result?.underrepairassets ?? 0 },
                { "RetiredAssets", result?.retiredassets ?? 0 },
                { "SpareAssets", result?.spareassets ?? 0 },
                { "ActiveEmployees", result?.activeemployees ?? 0 }
            };
        }

        // Assets by type
        public async Task<Dictionary<string, int>> GetAssetsByTypeAsync()
        {
            using var connection = CreateConnection();
            
            var sql = @"SELECT ""AssetType"", COUNT(*) as Count FROM ""Assets"" GROUP BY ""AssetType""";
            var results = await connection.QueryAsync<(string AssetType, int Count)>(sql);
            
            return results.ToDictionary(r => r.AssetType, r => r.Count);
        }

        // Assets nearing warranty expiry (within 30 days)
        public async Task<IEnumerable<dynamic>> GetAssetsNearingWarrantyExpiryAsync(int days = 30)
        {
            using var connection = CreateConnection();
            
            var sql = @"
                SELECT 
                    ""AssetId"",
                    ""AssetName"",
                    ""AssetType"",
                    ""SerialNumber"",
                    ""WarrantyExpiryDate"",
                    EXTRACT(DAY FROM (""WarrantyExpiryDate"" - CURRENT_DATE)) as DaysRemaining
                FROM ""Assets""
                WHERE ""WarrantyExpiryDate"" IS NOT NULL
                    AND ""WarrantyExpiryDate"" >= CURRENT_DATE
                    AND EXTRACT(DAY FROM (""WarrantyExpiryDate"" - CURRENT_DATE)) <= @Days
                ORDER BY ""WarrantyExpiryDate""";

            return await connection.QueryAsync(sql, new { Days = days });
        }

        // Assignment history report
        public async Task<IEnumerable<dynamic>> GetAssignmentHistoryReportAsync()
        {
            using var connection = CreateConnection();
            
            var sql = @"
                SELECT 
                    a.""AssignmentId"",
                    ast.""AssetName"",
                    ast.""AssetType"",
                    ast.""SerialNumber"",
                    e.""FullName"" as EmployeeName,
                    e.""Department"",
                    a.""AssignedDate"",
                    a.""ReturnedDate"",
                    a.""Notes"",
                    a.""IsActive""
                FROM ""Assignments"" a
                INNER JOIN ""Assets"" ast ON a.""AssetId"" = ast.""AssetId""
                INNER JOIN ""Employees"" e ON a.""EmployeeId"" = e.""EmployeeId""
                ORDER BY a.""AssignedDate"" DESC";

            return await connection.QueryAsync(sql);
        }
    }
}