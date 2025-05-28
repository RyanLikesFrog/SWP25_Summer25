using DataLayer.Entities;
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
        public Task<List<Appointment>>? GetAllAppointmentsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Appointment?> GetAppointmentByIdAsync(Guid appointmentId)
        {
            throw new NotImplementedException();
        }
    }
}
