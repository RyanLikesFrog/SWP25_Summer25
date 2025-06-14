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
    public class TreatmentStageService : ITreatmentStageService
    {
        private readonly ITreatmentStageRepository _treatmentStageRepository;
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IPatientTreatmentProtocolRepository _patientTreatmentProtocolRepository;
        private readonly IRepository _repository; 

        public TreatmentStageService(ITreatmentStageRepository treatmentStageRepository, IMedicalRecordRepository medicalRecordRepository, IPatientTreatmentProtocolRepository patientTreatmentProtocolRepository, IRepository repository)
        {
            _treatmentStageRepository = treatmentStageRepository;
            _medicalRecordRepository = medicalRecordRepository;
            _patientTreatmentProtocolRepository = patientTreatmentProtocolRepository;
            _repository = repository;
        }

        public async Task<TreatmentStageDetailResponse?> CreateTreatmentStageAsync(CreateTreatmentStageRequest request)
        {
            var patientTreatmentProtocol = await _patientTreatmentProtocolRepository.GetPatientTreatmentProtocolByIdAsync(request.PatientTreatmentProtocolId);
            if (patientTreatmentProtocol == null)
            {
                throw new ArgumentException($"PatientTreatmentProtocol with ID {request.PatientTreatmentProtocolId} not found.");
            }

            if (request.StartDate == null)
            {
                throw new ArgumentException("StartDate is required.");
            }

            var newTreatmentStage = new TreatmentStage
            {
                Id = Guid.NewGuid(),
                StageName = request.StageName,
                Description = request.Description,
                PatientTreatmentProtocolId = request.PatientTreatmentProtocolId,
                StageNumber = request.StageNumber,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                ReminderTimes = request.ReminderTimes,
                Status = request.Status,
                Medicine = request.Medicine
            };

            await _treatmentStageRepository.CreateTreatmentStageAsync(newTreatmentStage);
            await _repository.SaveChangesAsync();

            if(request.PatientId != null && request.PatientId != patientTreatmentProtocol.PatientId)
            {
                throw new Exception($"Patient ID {request.PatientId} does not match the PatientTreatmentProtocol's Patient ID {patientTreatmentProtocol.PatientId}.");
            }

            if(request.DoctorId != null && request.DoctorId != patientTreatmentProtocol.DoctorId)
            {
                throw new Exception($"Doctor ID {request.DoctorId} does not match the PatientTreatmentProtocol's Doctor ID {patientTreatmentProtocol.DoctorId}.");
            }

            var newMedicalRecord = new MedicalRecord
            {
                Id = Guid.NewGuid(),
                PatientId = patientTreatmentProtocol.PatientId,
                DoctorId = patientTreatmentProtocol.DoctorId,
                ExaminationDate = request.ExaminationDate,
                Diagnosis = request.Diagnosis,
                Symptoms = request.Symptoms,
                Prescription = request.Prescription,
                Notes = request.Notes
            };

            await _medicalRecordRepository.CreateMedicalRecordAsync(newMedicalRecord);
            await _repository.SaveChangesAsync();

            return null;
        }

        public async Task<List<TreatmentStage?>> GetAllTreatmentStagesAsync()
        {
            return await _treatmentStageRepository.GetAllTreatmentStagesAsync();
        }

        public async Task<TreatmentStage?> GetTreatmentStagebyIdAsync(Guid treatmentStageId)
        {
            return await _treatmentStageRepository.GetTreatmentStageByIdAsync(treatmentStageId);
        }
    }
}
