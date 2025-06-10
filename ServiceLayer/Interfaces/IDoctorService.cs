using DataLayer.Entities;
using ServiceLayer.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface IDoctorService
    {
        public Task<Doctor?> GetDoctorByIdAsync(Guid doctorId);
        public Task<List<Doctor>>? GetAllDoctorsAsync();

        public Task<(List<DoctorSchedule?> schedules, string Message)> ViewDoctorScheduleAsync(Guid doctorId);
        public Task<(List<Appointment?> appointments, string Message)> ViewAppointmentAsync(Guid doctorId);
    }
}
