using DataLayer.Entities;
using RepoLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Implements
{
    public class PatientTreatmentProtocolRepository : IPatientTreatmentProtocolRepository
    {
        public Task<List<PatientTreatmentProtocol>> GetAllPatientTreatmentProtocolsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PatientTreatmentProtocol?> GetPatientTreatmentProtocolByIdAsync(Guid patientTreatmentProtocolId)
        {
            throw new NotImplementedException();
        }
    }
}
