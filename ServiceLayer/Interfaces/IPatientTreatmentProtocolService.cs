using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface IPatientTreatmentProtocolService
    {
        public Task<PatientTreatmentProtocol?> GetPatientTreatmentProtocolByIdAsync(Guid patientTreatmentProtocolId);
        public Task<List<PatientTreatmentProtocol>>? GetAllPatientTreatmentProtocolsAsync();
    }
}
