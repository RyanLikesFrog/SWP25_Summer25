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
    public class PatientTreatmentProtocolService : IPatientTreatmentProtocolService
    {
        private readonly IPatientTreatmentProtocolRepository _patientTreatmentProtocolRepository;
        public async Task<List<PatientTreatmentProtocol?>> GetAllPatientTreatmentProtocolsAsync()
        {
            return await _patientTreatmentProtocolRepository.GetAllPatientTreatmentProtocolsAsync();
        }

        public async Task<PatientTreatmentProtocol?> GetPatientTreatmentProtocolByIdAsync(Guid patientTreatmentProtocolId)
        {
            return await _patientTreatmentProtocolRepository.GetPatientTreatmentProtocolByIdAsync(patientTreatmentProtocolId);
        }
    }
}
