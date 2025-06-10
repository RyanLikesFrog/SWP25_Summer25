using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Interfaces
{
    public interface IPatientTreatmentProtocolRepository
    {
        public Task<PatientTreatmentProtocol?> GetPatientTreatmentProtocolByIdAsync(Guid patientTreatmentProtocolId);
        public Task<List<PatientTreatmentProtocol?>> GetAllPatientTreatmentProtocolsAsync();
    }
}
