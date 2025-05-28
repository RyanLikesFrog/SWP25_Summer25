using DataLayer.Entities;
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
        public Task<List<TreatmentStage>> GetAllTreatmentStagesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TreatmentStage?> GetTreatmentStageByIdAsync(Guid treatmentStageId)
        {
            throw new NotImplementedException();
        }
    }
}
