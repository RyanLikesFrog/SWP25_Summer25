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
    public class PatientTreatmentProtocolRepository : IPatientTreatmentProtocolRepository
    {
        private readonly SWPSU25Context _context;

        public PatientTreatmentProtocolRepository(SWPSU25Context context)
        {
            _context = context;
        }

        public async Task<PatientTreatmentProtocol?> CreatePatientTreatmentProtocol(PatientTreatmentProtocol patientTreatmentProtocol)
        {
            return (await _context.PatientTreatmentProtocols.AddAsync(patientTreatmentProtocol)).Entity;
        }

        public async Task<List<PatientTreatmentProtocol?>> GetAllPatientTreatmentProtocolsAsync()
        {
            return await _context.PatientTreatmentProtocols
                .Include(ptp => ptp.Patient)
                .Include(ptp => ptp.Doctor)
                .ToListAsync();
        }

        public async Task<PatientTreatmentProtocol?> GetPatientTreatmentProtocolByIdAsync(Guid patientTreatmentProtocolId)
        {
            return await _context.PatientTreatmentProtocols
                .Include(ptp => ptp.Patient)
                .Include(ptp => ptp.Doctor)
                .FirstOrDefaultAsync(ptp => ptp.Id == patientTreatmentProtocolId);
        }
    }
}
