using DataLayer.Entities;
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
        public Task<List<TreatmentStage>>? GetAllTreatmentStagesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TreatmentStage?> GetTreatmentStagebyIdAsync(Guid treatmentStageId)
        {
            throw new NotImplementedException();
        }
    }
}
