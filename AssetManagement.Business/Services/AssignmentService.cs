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

        public async Task<AssignmentDTO?> GetAssignmentByIdAsync(int id)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(id);
            return assignment != null ? MapToDTO(assignment) : null;
        }

        public async Task<AssignmentDTO> CreateAssignmentAsync(AssignmentDTO assignmentDto)
        {
            // Check if asset exists and is available
            var asset = await _assetRepository.GetByIdAsync(assignmentDto.AssetId);
            if (asset == null)
                throw new InvalidOperationException("Asset not found");

            if (asset.Status != AssetStatus.Available)
                throw new InvalidOperationException("Asset is not available for assignment");

            // Check if asset already has an active assignment
            var existingAssignment = await _assignmentRepository.GetActiveAssignmentForAssetAsync(assignmentDto.AssetId);
            if (existingAssignment != null)
                throw new InvalidOperationException("Asset is already assigned to another employee");

            // FIXED: Create assignment with proper entity structure
            var assignment = new Assignment
            {
                AssetId = assignmentDto.AssetId,
                EmployeeId = assignmentDto.EmployeeId,
                AssignmentDate = assignmentDto.AssignmentDate,
                Notes = assignmentDto.Notes ?? string.Empty,
                IsActive = true,
                IsReturned = false  // FIXED: Set IsReturned
            };

            var created = await _assignmentRepository.AddAsync(assignment);

            // FIXED: Update asset status properly by creating new entity for update
            var assetToUpdate = new Asset
            {
                AssetId = asset.AssetId,
                AssetName = asset.AssetName,
                AssetType = asset.AssetType,
                MakeModel = asset.MakeModel,
                SerialNumber = asset.SerialNumber,
                PurchaseDate = asset.PurchaseDate,
                WarrantyExpiryDate = asset.WarrantyExpiryDate,
                Condition = asset.Condition,
                Status = AssetStatus.Assigned,  // Change status to Assigned
                IsSpare = asset.IsSpare,
                Specifications = asset.Specifications
            };
            await _assetRepository.UpdateAsync(assetToUpdate);

            return MapToDTO(created);
        }

        public async Task<bool> ReturnAssetAsync(int assignmentId, DateTime returnDate, string notes)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(assignmentId);
            if (assignment == null)
                return false;

            // FIXED: Create new assignment entity for update
            var assignmentToUpdate = new Assignment
            {
                AssignmentId = assignment.AssignmentId,
                AssetId = assignment.AssetId,
                EmployeeId = assignment.EmployeeId,
                AssignmentDate = assignment.AssignmentDate,
                ReturnDate = returnDate,
                IsActive = false,
                IsReturned = true,  // FIXED: Set IsReturned to true
                Notes = !string.IsNullOrWhiteSpace(notes) ? notes : assignment.Notes
            };

            await _assignmentRepository.UpdateAsync(assignmentToUpdate);

            // FIXED: Update asset status to Available
            var asset = await _assetRepository.GetByIdAsync(assignment.AssetId);
            if (asset != null)
            {
                var assetToUpdate = new Asset
                {
                    AssetId = asset.AssetId,
                    AssetName = asset.AssetName,
                    AssetType = asset.AssetType,
                    MakeModel = asset.MakeModel,
                    SerialNumber = asset.SerialNumber,
                    PurchaseDate = asset.PurchaseDate,
                    WarrantyExpiryDate = asset.WarrantyExpiryDate,
                    Condition = asset.Condition,
                    Status = AssetStatus.Available,  // Change status back to Available
                    IsSpare = asset.IsSpare,
                    Specifications = asset.Specifications
                };
                await _assetRepository.UpdateAsync(assetToUpdate);
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
                AssignmentDate = assignment.AssignmentDate,
                ReturnDate = assignment.ReturnDate,
                Notes = assignment.Notes ?? string.Empty,
                IsActive = assignment.IsActive,
                AssetName = assignment.Asset?.AssetName ?? string.Empty,
                EmployeeName = assignment.Employee?.FullName ?? string.Empty
            };
        }
    }
}