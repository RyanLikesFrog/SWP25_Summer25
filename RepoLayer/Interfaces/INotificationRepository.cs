using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Interfaces
{
    public interface INotificationRepository
    {
        public Task<List<Notification>> GetAllByPatientIdAsync(Guid patientId);
        public Task<Notification?> GetNotificationByIdAsync(Guid id);
        public Task<Notification?> CreateNotificationAsync(Notification notification);
        public Task<bool> MarkAsSeenAsync(Guid id);

    }
}
