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
    public class PatientRepository : IPatientRepository
    {
        private readonly SWPSU25Context _context;
        public PatientRepository(SWPSU25Context context)
        {
            _context = context;
        }

        public async Task AddPatientAsync(Patient patient)
        {
            await _context.Patients.AddAsync(patient);
        }

        public async Task<List<Patient?>> GetAllPatientsAsync() 
        {
            return await _context.Patients
                .Include(x => x.User)
                .Include(x => x.PatientTreatmentProtocols)
                .Include(x => x.LabResults)
                .Include(x => x.MedicalRecords)
                .ToListAsync();
        }

        public async Task<Patient?> GetPatientByIdAsync(Guid patientId)
        {
            return await _context.Patients.Include(p => p.User)
                .Include(x => x.User)
                .Include(x => x.PatientTreatmentProtocols)
                .Include(x => x.LabResults)
                .Include(x => x.MedicalRecords)
                .FirstOrDefaultAsync(p => p.Id == patientId);
        }

        public async Task<Patient?> GetPatientByUserIdAsync(Guid userId)
        {
            return await _context.Patients
                .Include(x => x.User)
                .Include(x => x.PatientTreatmentProtocols)
                .Include(x => x.LabResults)
                .Include(x => x.MedicalRecords)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task UpdatePatientAsync(Patient patient)
        {
            _context.Patients.Update(patient);
        }
    }
}
