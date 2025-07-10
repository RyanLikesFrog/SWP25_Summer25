using DataLayer.Entities;
using Microsoft.Extensions.Logging;
using RepoLayer.Interfaces;
using ServiceLayer.DTOs.User.Request;
using ServiceLayer.DTOs.User.Response;
using ServiceLayer.Interfaces;
using ServiceLayer.PaymentGateways;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Implements
{
    public class NotificationService : INotificationService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IRepository _repository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ITreatmentStageRepository _treatmentStageRepository;

        public NotificationService(
            IPatientRepository patientRepository, 
            INotificationRepository notificationRepository, 
            IAppointmentRepository appointmentRepository,
            ITreatmentStageRepository treatmentStageRepository,
            IRepository repository)
        {
            _patientRepository = patientRepository;
            _notificationRepository = notificationRepository;
            _appointmentRepository = appointmentRepository;
            _treatmentStageRepository = treatmentStageRepository;
            _repository = repository;
        }

        public async Task<NotificationDetailResponse?> CreateNotificationAsync(CreateNotificationRequest request)
        {
            var patient = await _patientRepository.GetPatientByIdAsync(request.PatientId);
            if (patient == null)
                throw new ArgumentException($"Patient ID {request.PatientId} không tồn tại.");

            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(request.AppointmentId.Value);
            if (appointment == null)
                throw new ArgumentException($"Patient ID {request.AppointmentId} không tồn tại.");

            var treatmentStage = await _treatmentStageRepository.GetTreatmentStageByIdAsync(request.TreatmentStageId.Value);
            if (treatmentStage == null)
                throw new ArgumentException($"Patient ID {request.TreatmentStageId} không tồn tại.");


            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                PatientId = request.PatientId,
                AppointmentId = request.AppointmentId,
                TreatmentStageId = request.TreatmentStageId,
                Message = request.Message,
                CreatedAt = request.CreatedAt,
                IsSeen = false
            };

            var created = await _notificationRepository.CreateNotificationAsync(notification);
            await _repository.SaveChangesAsync();

            return new NotificationDetailResponse
            {
                NotificationId = created.NotificationId,
                PatientId = created.PatientId,
                AppointmentId = created.AppointmentId,
                TreatmentStageId = created.TreatmentStageId,
                Message = created.Message,
                CreatedAt = created.CreatedAt,
                IsSeen = created.IsSeen,
                SeenAt = created.SeenAt
            };
        }

        public async Task<List<Notification>> GetAllByPatientIdAsync(Guid patientId)
        {
            return await _notificationRepository.GetAllByPatientIdAsync(patientId);
        }

        public async Task<Notification?> GetNotificationByIdAsync(Guid id)
        {
            return await _notificationRepository.GetNotificationByIdAsync(id);
        }

        public async Task<bool> MarkAsSeenAsync(Guid id)
        {
            var updated = await _notificationRepository.MarkAsSeenAsync(id);
            if (updated)
                await _repository.SaveChangesAsync();
            return updated;
        }
    }
}
