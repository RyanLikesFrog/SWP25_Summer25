using DataLayer.Entities;
using Firebase.Storage;
using Microsoft.Extensions.Configuration;
using RepoLayer.Implements;
using RepoLayer.Interfaces;
using ServiceLayer.DTOs;
using ServiceLayer.DTOs.User.Request;
using ServiceLayer.Interfaces;
using ServiceLayer.Validator;
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
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly ITreatmentStageRepository _treatmentStageRepository;
        private readonly IConfiguration _config;
        private readonly IRepository _repository;

        public MedicalRecordService(
            IMedicalRecordRepository medicalRecordRepository,
            IRepository repository,
            IConfiguration configuration,
            IPatientRepository patientRepository,
            IDoctorRepository doctorRepository,
            ITreatmentStageRepository treatmentStageRepository)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _repository = repository;
            _config = configuration;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _treatmentStageRepository = treatmentStageRepository;
        }

        public async Task<MedicalRecordDetailResponse?> CreateMedicalRecordAsync(CreateMedicalRecordRequest request)
        {
            var patient = await _patientRepository.GetPatientByIdAsync(request.PatientId);
            if (patient == null)
                throw new ArgumentException($"Patient with ID {request.PatientId} not found.");

            var doctor = await _doctorRepository.GetDoctorByIdAsync(request.DoctorId);
            if (doctor == null)
                throw new ArgumentException($"Doctor with ID {request.DoctorId} not found.");

            var treatmentStage = await _treatmentStageRepository.GetTreatmentStageByIdAsync(request.TreatmentStageId);
            if (treatmentStage == null)
                throw new ArgumentException($"TreatmentStage with ID {request.TreatmentStageId} not found.");

            var medicalRecord = new MedicalRecord
            {
                Id = Guid.NewGuid(),
                PatientId = request.PatientId,
                DoctorId = request.DoctorId,
                TreatmentStageId = request.TreatmentStageId,
                ExaminationDate = request.ExaminationDate,
                Diagnosis = request.Diagnosis,
                Symptoms = request.Symptoms,
                Prescription = request.Prescription,
                Notes = request.Notes
            };

            await _medicalRecordRepository.CreateMedicalRecordAsync(medicalRecord);
            await _repository.SaveChangesAsync();

            return new MedicalRecordDetailResponse
            {
                Id = medicalRecord.Id,
                PatientId = medicalRecord.PatientId,
                DoctorId = medicalRecord.DoctorId,
                TreatmentStageId = medicalRecord.TreatmentStageId,
                ExaminationDate = medicalRecord.ExaminationDate,
                Diagnosis = medicalRecord.Diagnosis,
                Symptoms = medicalRecord.Symptoms,
                Prescription = medicalRecord.Prescription,
                Notes = medicalRecord.Notes
            };
        }
        public async Task<List<MedicalRecord?>> GetAllMedicalRecordsAsync()
        {
            return await _medicalRecordRepository.GetAllMedicalRecordsAsync();
        }

        public async Task<MedicalRecord?> GetMedicalRecordByIdAsync(Guid medicalRecordId)
        {
            return await _medicalRecordRepository.GetMedicalRecordByIdAsync(medicalRecordId);
        }

        public async Task<MedicalRecord?> UpdateMedicalRecordAsync(UpdateMedicalRecord request)
        {
            var existingRecord = await _medicalRecordRepository.GetMedicalRecordByIdAsync(request.MedicalRecordId);
            if (existingRecord == null)
                throw new ArgumentException($"Medical record with ID {request.MedicalRecordId} not found.");

            var patient = await _patientRepository.GetPatientByIdAsync(request.PatientId);
            if (patient == null)
                throw new ArgumentException($"Patient with ID {request.PatientId} not found.");

            var doctor = await _doctorRepository.GetDoctorByIdAsync(request.DoctorId);
            if (doctor == null)
                throw new ArgumentException($"Doctor with ID {request.DoctorId} not found.");

            if (request.TreatmentStageId.HasValue)
            {
                var treatmentStage = await _treatmentStageRepository.GetTreatmentStageByIdAsync(request.TreatmentStageId.Value);
                if (treatmentStage == null)
                    throw new ArgumentException($"TreatmentStage with ID {request.TreatmentStageId.Value} not found.");
            }

            existingRecord.PatientId = request.PatientId;
            existingRecord.DoctorId = request.DoctorId;
            existingRecord.TreatmentStageId = request.TreatmentStageId;
            existingRecord.ExaminationDate = request.ExaminationDate;
            existingRecord.Diagnosis = request.Diagnosis;
            existingRecord.Symptoms = request.Symptoms;
            existingRecord.Prescription = request.Prescription;
            existingRecord.Notes = request.Notes;

            await _medicalRecordRepository.UpdateMedicalRecordAsync(existingRecord);
            await _repository.SaveChangesAsync();

            return existingRecord;
        }
    }
}
