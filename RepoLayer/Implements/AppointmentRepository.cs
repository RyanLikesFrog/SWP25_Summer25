using DataLayer.Entities;
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
        public Task<List<Appointment>> GetAllAppointmentsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Appointment?> GetAppointmentByIdAsync(Guid appointmentId)
        {
            throw new NotImplementedException();
        }
    }
}
