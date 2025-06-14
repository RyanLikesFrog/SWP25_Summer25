using DataLayer.Entities;
using ServiceLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface ITreatmentStageService
    {
        public Task<TreatmentStage?> GetTreatmentStagebyIdAsync(Guid treatmentStageId);
        public Task<List<TreatmentStage?>> GetAllTreatmentStagesAsync();
        public Task<TreatmentStageDetailResponse?> CreateTreatmentStageAsync(CreateTreatmentStageRequest request);
    }
}
