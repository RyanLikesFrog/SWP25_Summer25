using DataLayer.Entities;
using ServiceLayer.DTOs.User.Request;
using ServiceLayer.DTOs.User.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface INotificationService
    {
        public Task<List<Notification>> GetAllByPatientIdAsync(Guid patientId);
        public Task<Notification?> GetNotificationByIdAsync(Guid id);
        public Task<NotificationDetailResponse?> CreateNotificationAsync(CreateNotificationRequest request);
        public Task<bool> MarkAsSeenAsync(Guid id);

    }
}
