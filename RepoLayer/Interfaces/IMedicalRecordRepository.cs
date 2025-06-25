using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Interfaces
{
    public interface IMedicalRecordRepository
    {
        public Task<MedicalRecord?> GetMedicalRecordByIdAsync(Guid medicalRecordId);
        public Task<List<MedicalRecord?>> GetAllMedicalRecordsAsync();
        public Task<MedicalRecord?> CreateMedicalRecordAsync(MedicalRecord medicalRecord);
        public Task<MedicalRecord?> UpdateMedicalRecordAsync(MedicalRecord medicalRecord);
    }
}
