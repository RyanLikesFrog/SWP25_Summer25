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

        public async Task<MedicalRecord?> CreateMedicalRecordAsync(MedicalRecord medicalRecord)
        {
            return (await _context.MedicalRecords.AddAsync(medicalRecord)).Entity;
        }

        public Task<List<MedicalRecord?>> GetAllMedicalRecordsAsync()
        {
            return _context.MedicalRecords
                .ToListAsync();
        }

        public async Task<MedicalRecord?> GetMedicalRecordByIdAsync(Guid medicalRecordId)
        {
            return await _context.MedicalRecords
                .FirstOrDefaultAsync(mr => mr.Id == medicalRecordId);
        }
        public async Task<MedicalRecord?> UpdateMedicalRecordAsync(MedicalRecord medicalRecord)
        {
            _context.MedicalRecords.Update(medicalRecord);
            return await Task.FromResult(medicalRecord);
        }
    }
}
