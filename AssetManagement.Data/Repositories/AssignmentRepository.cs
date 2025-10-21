using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data.Context;
using AssetManagement.Data.Entities;
using AssetManagement.Data.Interfaces;

namespace AssetManagement.Data.Repositories
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly AssetDbContext _context;

        public AssignmentRepository(AssetDbContext context)
        {
            _context = context;
        }

        // Interface method
        public async Task<IEnumerable<Assignment>> GetAllAsync()
        {
            return await _context.Assignments
                .AsNoTracking()
                .Include(a => a.Employee)
                .Include(a => a.Asset)
                .OrderByDescending(a => a.AssignmentDate)
                .ToListAsync();
        }

        // Internal pagination version (optional, preserved from your original)
        public async Task<IEnumerable<Assignment>> GetAllAsync(int page, int pageSize)
        {
            return await _context.Assignments
                .AsNoTracking()
                .Include(a => a.Employee)
                .Include(a => a.Asset)
                .OrderByDescending(a => a.AssignmentDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Assignment> GetByIdAsync(int id)
        {
            return await _context.Assignments
                .AsNoTracking()
                .Include(a => a.Employee)
                .Include(a => a.Asset)
                .FirstOrDefaultAsync(a => a.AssignmentId == id);
        }

        public async Task<Assignment> AddAsync(Assignment assignment)
        {
            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();
            return assignment;
        }

        public async Task<Assignment> UpdateAsync(Assignment assignment)
        {
            // FIXED: Fetch and update existing entity to avoid tracking conflicts
            var existingAssignment = await _context.Assignments
                .FirstOrDefaultAsync(a => a.AssignmentId == assignment.AssignmentId);

            if (existingAssignment == null)
                throw new InvalidOperationException("Assignment not found");

            existingAssignment.EmployeeId = assignment.EmployeeId;
            existingAssignment.AssetId = assignment.AssetId;
            existingAssignment.AssignmentDate = assignment.AssignmentDate;
            existingAssignment.ReturnDate = assignment.ReturnDate;
            existingAssignment.IsReturned = assignment.IsReturned;
            existingAssignment.Notes = assignment.Notes;
            existingAssignment.IsActive = assignment.IsActive;
            existingAssignment.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingAssignment;
        }

        // Preserved from your original (not in interface but might be used)
        public async Task<bool> DeleteAsync(int id)
        {
            var assignment = await _context.Assignments
                .FirstOrDefaultAsync(a => a.AssignmentId == id);

            if (assignment == null) return false;

            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();
            return true;
        }

        // Interface method
        public async Task<IEnumerable<Assignment>> GetActiveAssignmentsAsync()
        {
            return await _context.Assignments
                .AsNoTracking()
                .Include(a => a.Employee)
                .Include(a => a.Asset)
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.AssignmentDate)
                .ToListAsync();
        }

        // Interface method
        public async Task<IEnumerable<Assignment>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _context.Assignments
                .AsNoTracking()
                .Include(a => a.Asset)
                .Where(a => a.EmployeeId == employeeId)
                .OrderByDescending(a => a.AssignmentDate)
                .ToListAsync();
        }

        // Optional internal method for pagination (preserved from your original)
        public async Task<IEnumerable<Assignment>> GetAssignmentsByEmployeeAsync(int employeeId, int page, int pageSize)
        {
            return await _context.Assignments
                .AsNoTracking()
                .Include(a => a.Asset)
                .Where(a => a.EmployeeId == employeeId)
                .OrderByDescending(a => a.AssignmentDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Interface method
        public async Task<IEnumerable<Assignment>> GetByAssetIdAsync(int assetId)
        {
            return await _context.Assignments
                .AsNoTracking()
                .Include(a => a.Employee)
                .Where(a => a.AssetId == assetId)
                .OrderByDescending(a => a.AssignmentDate)
                .ToListAsync();
        }

        // Optional internal method for pagination (preserved from your original)
        public async Task<IEnumerable<Assignment>> GetAssignmentsByAssetAsync(int assetId, int page, int pageSize)
        {
            return await _context.Assignments
                .AsNoTracking()
                .Include(a => a.Employee)
                .Where(a => a.AssetId == assetId)
                .OrderByDescending(a => a.AssignmentDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Preserved from your original (not in interface but might be used)
        public async Task<Assignment> GetActiveAssignmentByAssetAsync(int assetId)
        {
            return await _context.Assignments
                .AsNoTracking()
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.AssetId == assetId && a.IsActive);
        }

        // Interface method
        public async Task<Assignment> GetActiveAssignmentForAssetAsync(int assetId)
        {
            return await GetActiveAssignmentByAssetAsync(assetId);
        }

        // Preserved from your original (not in interface but might be used)
        public async Task<bool> ReturnAssetAsync(int assignmentId, DateTime returnDate)
        {
            var assignment = await _context.Assignments
                .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);

            if (assignment == null) return false;

            assignment.ReturnDate = returnDate;
            assignment.IsActive = false;
            assignment.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}