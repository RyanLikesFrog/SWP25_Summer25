using DataLayer.DbContext;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using RepoLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Implements
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly SWPSU25Context _context;

        public NotificationRepository(SWPSU25Context context)
        {
            _context = context;
        }

        public async Task<Notification?> CreateNotificationAsync(Notification notification)
        {
            return (await _context.Notifications.AddAsync(notification)).Entity;
        }

        public async Task<List<Notification>> GetAllByPatientIdAsync(Guid patientId)
        {
            return await _context.Notifications
                                 .Where(n => n.PatientId == patientId)
                                 .OrderByDescending(n => n.CreatedAt)
                                 .ToListAsync();
        }

        public async Task<Notification?> GetNotificationByIdAsync(Guid id)
        {
            return await _context.Notifications.FindAsync(id);
        }

        public async Task<bool> MarkAsSeenAsync(Guid id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return false;

            notification.IsSeen = true;
            notification.SeenAt = DateTime.Now;

            return true;
        }
    }
}
