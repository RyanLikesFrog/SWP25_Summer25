using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Interfaces
{
    public interface ITreatmentStageRepository
    {
        public Task<TreatmentStage?> GetTreatmentStageByIdAsync(Guid treatmentStageId);
        public Task<List<TreatmentStage?>> GetAllTreatmentStagesAsync();
        public Task<TreatmentStage?> CreateTreatmentStageAsync(TreatmentStage treatmentStage);
        public Task<TreatmentStage?> UpdateTreatmentStageAsync(TreatmentStage treatmentStage);
    }
}
