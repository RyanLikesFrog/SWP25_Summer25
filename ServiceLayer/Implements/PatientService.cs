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
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        public async Task<List<Patient>>? GetAllPatientsAsync()
        {
            return await _patientRepository.GetAllPatientsAsync();
        }


        public async Task<Patient?> GetPatientByUserIdAsync(Guid patientId)
        {
            return await _patientRepository.GetPatientByUserIdAsync(patientId);
        }
    }
}
