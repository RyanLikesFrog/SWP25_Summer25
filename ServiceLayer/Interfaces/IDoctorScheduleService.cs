using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface IDoctorScheduleService
    {
        public Task<DoctorSchedule?> GetDoctorSchedulebyIdAsync(Guid scheduleId);
        public Task<List<DoctorSchedule?>> GetAllDoctorSchedulesAsync();
        public Task CreateDoctorScheduleAsync(DoctorSchedule doctorSchedule);
        public Task<(DoctorSchedule? doctorSchedule, string Message)>? GetDuplicatedDoctorScheduleByStartDateEndDateAsync(Guid? doctorId, DateTime startDate, DateTime? endDate);
        public Task<(List<DoctorSchedule?> schedules, string Message)> ViewDoctorScheduleAsync(Guid doctorId);

    }
}
