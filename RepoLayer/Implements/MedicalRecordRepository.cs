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
    public class MedicalRecordRepository : IMedicalRecordRepository
    {
        private readonly SWPSU25Context _context;

        public MedicalRecordRepository(SWPSU25Context context)
        {
            _context = context;
        }
        public Task<List<MedicalRecord?>> GetAllMedicalRecordsAsync()
        {
            return _context.MedicalRecords
                .Include(mr => mr.Patient)
                .Include(mr => mr.Doctor)
                .ToListAsync();
        }

        public async Task<MedicalRecord?> GetMedicalRecordByIdAsync(Guid medicalRecordId)
        {
            return await _context.MedicalRecords
                .Include(mr => mr.Patient)
                .Include(mr => mr.Doctor)
                .FirstOrDefaultAsync(mr => mr.Id == medicalRecordId);
        }
    }
}
