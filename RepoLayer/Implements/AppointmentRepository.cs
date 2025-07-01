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

        public async Task<Appointment?> UpdateAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            return appointment;
        }
    }
}
