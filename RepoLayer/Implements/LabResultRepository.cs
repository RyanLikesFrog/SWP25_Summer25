using DataLayer.DbContext;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using RepoLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Implements
{
    public class LabResultRepository : ILabResultRepository
    {
        private readonly SWPSU25Context _context;

        public LabResultRepository(SWPSU25Context context)
        {
            _context = context;
        }
        public async Task<List<LabResult?>> GetAllLabResultsAsync()
        {
            return await _context.LabResults
                .Include(lr => lr.Patient)
                .ToListAsync();
        }

        public Task<LabResult?> GetLabResultByIdAsync(Guid labResultId)
        {
            return _context.LabResults
                .Include(lr => lr.Patient)
                .FirstOrDefaultAsync(lr => lr.Id == labResultId);
        }
    }
}
