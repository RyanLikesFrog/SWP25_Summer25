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
        private readonly SWPSU25Context _Context;

        public AppointmentRepository(SWPSU25Context context)
        {
            _Context = context;
        }

        public async Task CreateAppointmentAsync(Appointment appointment)
        {
            await _Context.Appointments.AddAsync(appointment);
        }

        public async Task<List<Appointment>> GetAllAppointmentsAsync()
        {
            return await _Context.Appointments.Include(u => u.Patient)
                                              .ToListAsync();
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(Guid appointmentId)
        {
            return await _Context.Appointments.Include(u => u.Patient)
                                              .FirstOrDefaultAsync(u => u.Id == appointmentId);
        }

        public Task UpdateAppointmentAsync(Appointment appointment)
        {
            _Context.Appointments.Update(appointment);
            return Task.CompletedTask;
        }
    }
}
