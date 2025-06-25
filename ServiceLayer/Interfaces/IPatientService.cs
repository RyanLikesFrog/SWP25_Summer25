using DataLayer.Entities;
using ServiceLayer.DTOs.User.Request;
using ServiceLayer.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface IPatientService
    {
        public Task<Patient?> GetPatientByUserIdAsync(Guid patientId);
        public Task<List<Patient?>> GetAllPatientsAsync();
        public Task<(bool Success, string Message, Patient? Patient)> UpdatePatientProfileAsync(UpdatePatientRequest request);
    }
}
