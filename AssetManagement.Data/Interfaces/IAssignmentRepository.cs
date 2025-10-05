using System.Collections.Generic;
using System.Threading.Tasks;
using AssetManagement.Data.Entities;

namespace AssetManagement.Data.Interfaces
{
    public interface IAssignmentRepository
    {
        Task<IEnumerable<Assignment>> GetAllAsync();
        Task<Assignment> GetByIdAsync(int id);
        Task<Assignment> AddAsync(Assignment assignment);
        Task<Assignment> UpdateAsync(Assignment assignment);
        Task<IEnumerable<Assignment>> GetByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<Assignment>> GetByAssetIdAsync(int assetId);
        Task<IEnumerable<Assignment>> GetActiveAssignmentsAsync();
        Task<Assignment> GetActiveAssignmentForAssetAsync(int assetId);
    }
}