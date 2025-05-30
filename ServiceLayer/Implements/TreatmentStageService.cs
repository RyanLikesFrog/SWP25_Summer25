using DataLayer.Entities;
using RepoLayer.Interfaces;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Implements
{
    public class TreatmentStageService : ITreatmentStageService
    {
        private readonly ITreatmentStageRepository _treatmentStageRepository;
        public async Task<List<TreatmentStage>>? GetAllTreatmentStagesAsync()
        {
            return await _treatmentStageRepository.GetAllTreatmentStagesAsync();
        }

        public async Task<TreatmentStage?> GetTreatmentStagebyIdAsync(Guid treatmentStageId)
        {
            return await _treatmentStageRepository.GetTreatmentStageByIdAsync(treatmentStageId);
        }
    }
}
