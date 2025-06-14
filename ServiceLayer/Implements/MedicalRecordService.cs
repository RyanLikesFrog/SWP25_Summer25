using DataLayer.Entities;
using RepoLayer.Interfaces;
using ServiceLayer.DTOs;
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
        private readonly IMedicalRecordRepository _medicalRecordRepository;

        public Task<MedicalRecordDetailResponse?> CreateMedicalRecordAsync(CreateMedicalRecordRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<List<MedicalRecord?>> GetAllMedicalRecordsAsync()
        {
            return await _medicalRecordRepository.GetAllMedicalRecordsAsync();
        }

        public async Task<MedicalRecord?> GetMedicalRecordByIdAsync(Guid medicalRecordId)
        {
            return await _medicalRecordRepository.GetMedicalRecordByIdAsync(medicalRecordId);
        }
    }
}
