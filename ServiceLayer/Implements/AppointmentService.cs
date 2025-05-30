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
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public async Task<List<Appointment>>? GetAllAppointmentsAsync()
        {
            return await _appointmentRepository.GetAllAppointmentsAsync();
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(Guid appointmentId)
        {
            return await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
        }
    }
}
