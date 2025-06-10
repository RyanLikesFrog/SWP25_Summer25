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
        public readonly SWPSU25Context _Context;

        public TreatmentStageRepository(SWPSU25Context context)
        {
            _Context = context;
        }


        public async Task<List<TreatmentStage?>> GetAllTreatmentStagesAsync()
        {
            return await _Context.TreatmentStages.Include(u => u.PatientTreatmentProtocol)
                                                  .ToListAsync();
        }

        public async Task<TreatmentStage?> GetTreatmentStageByIdAsync(Guid treatmentStageId)
        {
            return await _Context.TreatmentStages.Include(u => u.PatientTreatmentProtocol)
                                                  .FirstOrDefaultAsync(u => u.Id == treatmentStageId);
        }
    }
}
