using DataLayer.Entities;
using DataLayer.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Interfaces
{
    public interface IAppointmentRepository
    {
        public Task<Appointment?> GetAppointmentByIdAsync(Guid appointmentId);
        public Task<List<Appointment?>> GetAllAppointmentsAsync();
        public Task CreateAppointmentAsync(Appointment appointment);
        public Task<Appointment?> UpdateAppointmentAsync(Appointment appointment);
        public Task<List<Appointment?>> GetAppointmentsByDoctorIdAsync(Guid doctorId);
        public Task<Appointment?> ReArrangeDateAppointmentAsync (Appointment appointment);
        public Task<bool> IsTimeSlotAvailableAsync(Guid? doctorId, DateTime startDate, DateTime? endDate, Guid excludeAppointmentId);
        public Task UpdateStatusAsync(Appointment appointment, AppointmentStatus newStatus, string? note);

    }
}   
    