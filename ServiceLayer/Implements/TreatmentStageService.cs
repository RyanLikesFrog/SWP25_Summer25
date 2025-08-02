using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using RepoLayer.Interfaces;
using ServiceLayer.DTOs;
using ServiceLayer.DTOs.User.Request;
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
        private readonly IPrescriptionItemRepository _prescriptionItemRepository;
        private readonly IPrescriptionRepository _prescriptionRepository;

        public TreatmentStageService(ITreatmentStageRepository treatmentStageRepository, IMedicalRecordRepository medicalRecordRepository, IPatientTreatmentProtocolRepository patientTreatmentProtocolRepository, IRepository repository, IPrescriptionItemRepository prescriptionItemRepository, IPrescriptionRepository prescriptionRepository)
        {
            _treatmentStageRepository = treatmentStageRepository;
            _medicalRecordRepository = medicalRecordRepository;
            _patientTreatmentProtocolRepository = patientTreatmentProtocolRepository;
            _repository = repository;
            _prescriptionItemRepository = prescriptionItemRepository;
            _prescriptionRepository = prescriptionRepository;
        }

        public async Task<TreatmentStageDetailResponse?> CreateTreatmentStageAsync(CreateTreatmentStageRequest request)
        {
            // Bước 1: Kiểm tra sự tồn tại của PatientTreatmentProtocolId
            var patientTreatmentProtocol = await _patientTreatmentProtocolRepository.GetPatientTreatmentProtocolByIdAsync(request.PatientTreatmentProtocolId);
            if (patientTreatmentProtocol == null)
            {
                throw new ArgumentException($"Không tìm thấy phác đồ điều trị với ID {request.PatientTreatmentProtocolId}.");
            }

            // Bước 2: Tạo TreatmentStage và thêm vào DbContext
            var newTreatmentStage = new TreatmentStage
            {
                Id = Guid.NewGuid(),
                StageName = request.StageName,
                StageNumber = request.StageNumber,
                Description = request.Description,
                PatientTreatmentProtocolId = request.PatientTreatmentProtocolId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                ReminderTimes = request.ReminderTimes,
                Status = request.Status,
            };
            await _treatmentStageRepository.CreateTreatmentStageAsync(newTreatmentStage);

            // Bước 3: Tạo MedicalRecord và thêm vào DbContext
            var newMedicalRecord = new MedicalRecord
            {
                Id = Guid.NewGuid(),
                PatientId = patientTreatmentProtocol.PatientId,
                DoctorId = patientTreatmentProtocol.DoctorId,
                TreatmentStageId = newTreatmentStage.Id,
                ExaminationDate = request.ExaminationDate,
                Diagnosis = request.Diagnosis,
                Symptoms = request.Symptoms,
                Notes = request.Notes,
            };
            await _medicalRecordRepository.CreateMedicalRecordAsync(newMedicalRecord);

            // Bước 4: Tạo Prescription (nếu có) và thêm vào DbContext
            if (request.Prescription != null)
            {
                var newPrescription = new Prescription
                {
                    Id = Guid.NewGuid(),
                    MedicalRecordId = newMedicalRecord.Id,
                    CreatedAt = DateTime.UtcNow,
                    Note = request.Prescription.PrescriptionNote,
                    Items = new List<PrescriptionItem>()
                };

                if (request.Prescription.PrescriptionItems != null && request.Prescription.PrescriptionItems.Any())
                {
                    foreach (var item in request.Prescription.PrescriptionItems)
                    {
                        var prescriptionItem = new PrescriptionItem
                        {
                            Id = Guid.NewGuid(),
                            PrescriptionId = newPrescription.Id,
                            DrugName = item.DrugName,
                            Dosage = item.Dosage,
                            Frequency = item.Frequency
                        };
                        newPrescription.Items.Add(prescriptionItem);
                    }
                }
                await _prescriptionRepository.CreatePrescriptionAsync(newPrescription);
            }

            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                Console.WriteLine("Lỗi khi lưu dữ liệu vào cơ sở dữ liệu.");
                Console.WriteLine($"Message: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw; 
            }

            var repsonse = new TreatmentStageDetailResponse()
            {
                Id = newTreatmentStage.Id,
                StageName = newTreatmentStage.StageName,
            };
            return repsonse;
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
