using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Interfaces
{
    public interface IPatientRepository
    {
        public Task<Patient?> GetPatientByUserIdAsync(Guid userId);
        public Task<Patient?> GetPatientByIdAsync(Guid patientId);
        public Task<List<Patient?>> GetAllPatientsAsync();
        public Task AddPatientAsync(Patient patient);
        public Task UpdatePatientAsync(Patient patient);
    }
}
