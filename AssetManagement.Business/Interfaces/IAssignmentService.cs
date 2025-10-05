using System.Collections.Generic;
using System.Threading.Tasks;
using AssetManagement.Business.DTOs;

namespace AssetManagement.Business.Interfaces
{
    public interface IAssignmentService
    {
        Task<IEnumerable<AssignmentDTO>> GetAllAssignmentsAsync();
        Task<AssignmentDTO> GetAssignmentByIdAsync(int id);
        Task<AssignmentDTO> CreateAssignmentAsync(AssignmentDTO assignmentDto);
        Task<bool> ReturnAssetAsync(int assignmentId, DateTime returnDate, string notes);
        Task<IEnumerable<AssignmentDTO>> GetAssignmentsByEmployeeAsync(int employeeId);
        Task<IEnumerable<AssignmentDTO>> GetAssignmentsByAssetAsync(int assetId);
        Task<IEnumerable<AssignmentDTO>> GetActiveAssignmentsAsync();
    }
}
