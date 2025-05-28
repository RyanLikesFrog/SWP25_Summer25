using DataLayer.Entities;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Implements
{
    public class PatientTreatmentProtocolService : IPatientTreatmentProtocolService
    {
        public Task<List<PatientTreatmentProtocol>>? GetAllPatientTreatmentProtocolsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PatientTreatmentProtocol?> GetPatientTreatmentProtocolByIdAsync(Guid patientTreatmentProtocolId)
        {
            throw new NotImplementedException();
        }
    }
}
