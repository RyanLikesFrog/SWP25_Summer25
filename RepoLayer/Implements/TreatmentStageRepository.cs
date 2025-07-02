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
    public class TreatmentStageRepository : ITreatmentStageRepository
    {
        public readonly SWPSU25Context _context;

        public TreatmentStageRepository(SWPSU25Context context)
        {
            _context = context;
        }

        public async Task<TreatmentStage?> CreateTreatmentStageAsync(TreatmentStage treatmentStage)
        {
            return (await _context.TreatmentStages.AddAsync(treatmentStage)).Entity;
        }

        public async Task<List<TreatmentStage?>> GetAllTreatmentStagesAsync()
        {
            return await _context.TreatmentStages
                .Include(u => u.LabResults)
                .ToListAsync();
        }

        public async Task<TreatmentStage?> GetTreatmentStageByIdAsync(Guid treatmentStageId)
        {
            return await _context.TreatmentStages
                                .Include(u => u.LabResults)
                                .Include(u => u.PatientTreatmentProtocol)
                                .FirstOrDefaultAsync(u => u.Id == treatmentStageId);
        }

        public async Task<TreatmentStage?> UpdateTreatmentStageAsync(TreatmentStage treatmentStage)
        {
            _context.TreatmentStages.Update(treatmentStage);
            return treatmentStage;
        }
    }
}
