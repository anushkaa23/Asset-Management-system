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

        public async Task<IEnumerable<Assignment>> GetAllAsync()
        {
            return await _context.Assignments
                .Include(a => a.Asset)
                .Include(a => a.Employee)
                .OrderByDescending(a => a.AssignedDate)
                .ToListAsync();
        }

        public async Task<Assignment> GetByIdAsync(int id)
        {
            return await _context.Assignments
                .Include(a => a.Asset)
                .Include(a => a.Employee)
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
            // Detach any existing tracked entity
            var existingEntity = _context.Assignments.Local
                .FirstOrDefault(a => a.AssignmentId == assignment.AssignmentId);
            
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).State = EntityState.Detached;
            }

            _context.Assignments.Update(assignment);
            await _context.SaveChangesAsync();
            return assignment;
        }

        public async Task<IEnumerable<Assignment>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _context.Assignments
                .Include(a => a.Asset)
                .Where(a => a.EmployeeId == employeeId)
                .OrderByDescending(a => a.AssignedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Assignment>> GetByAssetIdAsync(int assetId)
        {
            return await _context.Assignments
                .Include(a => a.Employee)
                .Where(a => a.AssetId == assetId)
                .OrderByDescending(a => a.AssignedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Assignment>> GetActiveAssignmentsAsync()
        {
            return await _context.Assignments
                .Include(a => a.Asset)
                .Include(a => a.Employee)
                .Where(a => a.IsActive && a.ReturnedDate == null)
                .OrderByDescending(a => a.AssignedDate)
                .ToListAsync();
        }

        public async Task<Assignment?> GetActiveAssignmentForAssetAsync(int assetId)
        {
            return await _context.Assignments
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.AssetId == assetId &&
                                        a.IsActive && 
                                        a.ReturnedDate == null);
        }
    }
}