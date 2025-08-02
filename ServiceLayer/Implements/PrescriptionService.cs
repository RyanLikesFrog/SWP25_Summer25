using DataLayer.Entities;
using RepoLayer.Interfaces;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Implements
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IPrescriptionItemRepository _prescriptionItemRepository;
        private readonly ITreatmentStageService _treatmentStageService;
        private readonly IMedicalRecordService _medicalRecordService;

        public PrescriptionService(
            IPrescriptionRepository prescriptionRepository,
            IPrescriptionItemRepository prescriptionItemRepository,
            ITreatmentStageService treatmentStageService,
            IMedicalRecordService medicalRecordService)
        {
            _prescriptionRepository = prescriptionRepository;
            _prescriptionItemRepository = prescriptionItemRepository;
            _treatmentStageService = treatmentStageService;
            _medicalRecordService = medicalRecordService;
        }

        public async Task<Prescription> GetPrescriptionByIdAsync(Guid prescriptionId)
        {
            var prescription = await _prescriptionRepository.GetPrescriptionByIdAsync(prescriptionId);
            if(prescription == null)
            {
                throw new ArgumentException($"Không tìm thấy đơn thuốc với ID {prescriptionId}.");
            }

            return prescription;
        }

        public Task<Prescription> GetPrescriptionsByMedicalRecordIdAsync(Guid medicalRecordId)
        {
            var prescriptions = _prescriptionRepository.GetPrescriptionsByMedicalRecordIdAsync(medicalRecordId);
            if (prescriptions == null)
            {
                throw new ArgumentException($"Không tìm thấy đơn thuốc với ID {medicalRecordId}.");
            }

            return prescriptions;
        }
    }
}
