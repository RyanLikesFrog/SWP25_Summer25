﻿using DataLayer.Entities;
using ServiceLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface IMedicalRecordService
    {
        public Task<MedicalRecord?> GetMedicalRecordByIdAsync(Guid medicalRecordId);
        public Task<List<MedicalRecord?>> GetAllMedicalRecordsAsync();
        public Task<MedicalRecordDetailResponse?> CreateMedicalRecordAsync(CreateMedicalRecordRequest request);
    }
}
