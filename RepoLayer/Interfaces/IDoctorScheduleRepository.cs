using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Interfaces
{
    public interface IDoctorScheduleRepository
    {
        public Task<DoctorSchedule?> GetDoctorScheduleByIdAsync(Guid scheduleId);
        public Task<List<DoctorSchedule?>>  GetAllDoctorSchedulesAsync();
        public Task<DoctorSchedule?> GetDuplicatedDoctorScheduleByStartDateEndDateAsync(Guid? doctorId, DateTime startDate, DateTime? endDate);
        public Task<List<DoctorSchedule?>> GetDoctorSchedulesByDoctorIdAsync(Guid doctorId);
        public Task<DoctorSchedule?> CreateDoctorScheduleAsync(DoctorSchedule doctorSchedule);
        public Task<DoctorSchedule?> GetDoctorScheduleByAppointmentIdAsync(Guid? appointmentId);
        public Task<List<DoctorSchedule?>> GetTodayDoctorSchedulesByDoctorIdAsync(Guid doctorId);
        public Task<DoctorSchedule?> UpdateDoctorScheduleAsync(DoctorSchedule doctorSchedule);

    }
}
