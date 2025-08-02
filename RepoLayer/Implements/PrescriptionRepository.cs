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
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly SWPSU25Context _context;

        public PrescriptionRepository(SWPSU25Context context)
        {
            _context = context;
        }

        public async Task CreatePrescriptionAsync(Prescription prescription)
        {
            await _context.Prescriptions.AddAsync(prescription);
        }

        public async Task<Prescription?> GetPrescriptionByIdAsync(Guid prescrptionId)
        {
            return await _context.Prescriptions.Include(p => p.Items)   
                                                .FirstOrDefaultAsync(p => p.Id == prescrptionId) 
                                                ?? throw new KeyNotFoundException("Prescription not found.");
        }

        public async Task<Prescription?> GetPrescriptionsByMedicalRecordIdAsync(Guid medicalRecordId)
        {
            return await _context.Prescriptions.Include(p => p.Items)
                                                .FirstOrDefaultAsync(p => p.MedicalRecordId == medicalRecordId) 
                                                ?? throw new KeyNotFoundException("Prescription not found for the given medical record ID.");
        }
    }
}
