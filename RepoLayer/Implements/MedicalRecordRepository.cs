using DataLayer.Entities;
using RepoLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Implements
{
    public class MedicalRecordRepository : IMedicalRecordRepository
    {
        public Task<List<MedicalRecord>> GetAllMedicalRecordsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<MedicalRecord?> GetMedicalRecordByIdAsync(Guid medicalRecordId)
        {
            throw new NotImplementedException();
        }
    }
}
