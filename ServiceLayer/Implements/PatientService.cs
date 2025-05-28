using DataLayer.Entities;
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
        public Task<List<Patient>>? GetAllPatientsAsync()
        {
            throw new NotImplementedException();
        }


        public Task<Patient?> GetPatientByIdAsync(Guid patientId)
        {
            throw new NotImplementedException();
        }
    }
}
