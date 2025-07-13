using DataLayer.DbContext;
using DataLayer.Entities;
using DataLayer.Enum;
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
                .Include(ptp => ptp.TreatmentStages)
                .ToListAsync();
        }

        public async Task<PatientTreatmentProtocol?> GetPatientTreatmentProtocolByIdAsync(Guid patientTreatmentProtocolId)
        {
            return await _context.PatientTreatmentProtocols
                .Include(ptp => ptp.TreatmentStages)
                .FirstOrDefaultAsync(ptp => ptp.Id == patientTreatmentProtocolId);
        }

        public async Task<bool> UpdatePatientTreatmentProtocolStatusAsync(Guid protocolId, PatientTreatmentStatus newStatus)
        {
            var affectedRows = await _context.PatientTreatmentProtocols
                .Where(p => p.Id == protocolId)
                .ExecuteUpdateAsync(setter => setter
                    .SetProperty(p => p.Status, newStatus));

            return affectedRows > 0;
        }
    }
}
