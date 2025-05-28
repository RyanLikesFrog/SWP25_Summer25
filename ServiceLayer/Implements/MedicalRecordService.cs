using DataLayer.Entities;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Implements
{
    public class MedicalRecordService : IMedicalRecordService
    {
        public Task<List<MedicalRecord>>? GetAllMedicalRecordsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<MedicalRecord?> GetMedicalRecordByIdAsync(Guid medicalRecordId)
        {
            throw new NotImplementedException();
        }
    }
}
