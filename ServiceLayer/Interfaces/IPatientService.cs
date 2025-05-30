using DataLayer.Entities;
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
        public Task<List<Patient>>? GetAllPatientsAsync();
    }
}
