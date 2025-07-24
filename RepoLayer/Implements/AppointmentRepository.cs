using DataLayer.DbContext;
using DataLayer.Entities;
using DataLayer.Enum;
using Microsoft.EntityFrameworkCore;
using RepoLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Implements
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly SWPSU25Context _context;

        public AppointmentRepository(SWPSU25Context context)
        {
            _context = context;
        }

        public async Task CreateAppointmentAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
        }

        public async Task<List<Appointment?>> GetAllAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(u => u.PaymentTransaction)
                .ToListAsync();
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(Guid appointmentId)
        {
            return await _context.Appointments
                .Include(a => a.PaymentTransaction)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);
        }

        public async Task<List<Appointment?>> GetAppointmentsByDoctorIdAsync(Guid doctorId)
        {
            return await _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .Include(a => a.PaymentTransaction)
                .ToListAsync(); 
        }

        public async Task<Appointment?> ReArrangeDateAppointmentAsync(Appointment appointment)
        {
            try
            {
                await _context.SaveChangesAsync();
                return appointment;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Đã xảy ra lỗi khi cập nhật lịch hẹn với ID {appointment.Id}.", ex);
            }
        }
        public async Task<bool> IsTimeSlotAvailableAsync(Guid? doctorId, DateTime startDate, DateTime? endDate, Guid excludeAppointmentId)
        {
            var conflicts = await _context.Appointments
                .Where(a => a.Id != excludeAppointmentId &&
                            a.DoctorId == doctorId &&
                            a.Status != DataLayer.Enum.AppointmentStatus.Cancelled &&
                            (
                                (a.AppointmentStartDate >= startDate && a.AppointmentStartDate < endDate) ||
                                (a.AppointmentEndDate > startDate && a.AppointmentEndDate <= endDate) ||
                                (startDate >= a.AppointmentStartDate && startDate < a.AppointmentEndDate)
                            )
                )
                .AnyAsync();

            return !conflicts;
        }

        public async Task<Appointment?> UpdateAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            return appointment;
        }

        public async Task UpdateStatusAsync(Appointment appointment, AppointmentStatus newStatus, string? note)
        {
            appointment.Status = newStatus;
            if (!string.IsNullOrWhiteSpace(note))
            {
                appointment.Notes = note;
            }

            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }
    }
}
