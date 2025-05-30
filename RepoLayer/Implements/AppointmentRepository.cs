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

        public async Task<List<Appointment>> GetAllAppointmentsAsync()
        {
            return await _context.Appointments.Include(u => u.Patient)
                                              .ToListAsync();
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(Guid appointmentId)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);
        }
    }
}
