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
    public class AssignmentService : IAssignmentService
    {
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IAssetRepository _assetRepository;

        public AssignmentService(IAssignmentRepository assignmentRepository, IAssetRepository assetRepository)
        {
            _assignmentRepository = assignmentRepository;
            _assetRepository = assetRepository;
        }

        public async Task<IEnumerable<AssignmentDTO>> GetAllAssignmentsAsync()
        {
            var assignments = await _assignmentRepository.GetAllAsync();
            return assignments.Select(MapToDTO);
        }

        public async Task<AssignmentDTO> GetAssignmentByIdAsync(int id)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(id);
            return assignment != null ? MapToDTO(assignment) : null;
        }

        public async Task<AssignmentDTO> CreateAssignmentAsync(AssignmentDTO assignmentDto)
        {
            // Check if asset is available
            var asset = await _assetRepository.GetByIdAsync(assignmentDto.AssetId);
            if (asset == null)
            {
                throw new InvalidOperationException("Asset not found");
            }

            if (asset.Status != AssetStatus.Available)
            {
                throw new InvalidOperationException("Asset is not available for assignment");
            }

            // Check if asset already has an active assignment
            var existingAssignment = await _assignmentRepository.GetActiveAssignmentForAssetAsync(assignmentDto.AssetId);
            if (existingAssignment != null)
            {
                throw new InvalidOperationException("Asset is already assigned to another employee");
            }

            // Create assignment
            var assignment = new Assignment
            {
                AssetId = assignmentDto.AssetId,
                EmployeeId = assignmentDto.EmployeeId,
                AssignedDate = assignmentDto.AssignedDate,
                Notes = assignmentDto.Notes,
                IsActive = true
            };

            var created = await _assignmentRepository.AddAsync(assignment);

            // Update asset status to Assigned
            asset.Status = AssetStatus.Assigned;
            await _assetRepository.UpdateAsync(asset);

            return MapToDTO(created);
        }

        public async Task<bool> ReturnAssetAsync(int assignmentId, DateTime returnDate, string notes)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(assignmentId);
            if (assignment == null)
            {
                return false;
            }

            // Update assignment
            assignment.ReturnedDate = returnDate;
            assignment.IsActive = false;
            if (!string.IsNullOrWhiteSpace(notes))
            {
                assignment.Notes = notes;
            }

            await _assignmentRepository.UpdateAsync(assignment);

            // Update asset status to Available
            var asset = await _assetRepository.GetByIdAsync(assignment.AssetId);
            if (asset != null)
            {
                asset.Status = AssetStatus.Available;
                await _assetRepository.UpdateAsync(asset);
            }

            return true;
        }

        public async Task<IEnumerable<AssignmentDTO>> GetAssignmentsByEmployeeAsync(int employeeId)
        {
            var assignments = await _assignmentRepository.GetByEmployeeIdAsync(employeeId);
            return assignments.Select(MapToDTO);
        }

        public async Task<IEnumerable<AssignmentDTO>> GetAssignmentsByAssetAsync(int assetId)
        {
            var assignments = await _assignmentRepository.GetByAssetIdAsync(assetId);
            return assignments.Select(MapToDTO);
        }

        public async Task<IEnumerable<AssignmentDTO>> GetActiveAssignmentsAsync()
        {
            var assignments = await _assignmentRepository.GetActiveAssignmentsAsync();
            return assignments.Select(MapToDTO);
        }

        private AssignmentDTO MapToDTO(Assignment assignment)
        {
            return new AssignmentDTO
            {
                AssignmentId = assignment.AssignmentId,
                AssetId = assignment.AssetId,
                EmployeeId = assignment.EmployeeId,
                AssignedDate = assignment.AssignedDate,
                ReturnedDate = assignment.ReturnedDate,
                Notes = assignment.Notes,
                IsActive = assignment.IsActive,
                AssetName = assignment.Asset?.AssetName,
                EmployeeName = assignment.Employee?.FullName
            };
        }
    }
}
