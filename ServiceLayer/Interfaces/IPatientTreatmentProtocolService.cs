using DataLayer.Entities;
using ServiceLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface IPatientTreatmentProtocolService
    {
        public Task<PatientTreatmentProtocolDetailResponse> CreatePatientTreatmentProtocolAsync(CreatePatientTreatmentProtocolRequest request);
        public Task<PatientTreatmentProtocol?> GetPatientTreatmentProtocolByIdAsync(Guid patientTreatmentProtocolId);
        public Task<List<PatientTreatmentProtocol?>> GetAllPatientTreatmentProtocolsAsync();


    }
}
