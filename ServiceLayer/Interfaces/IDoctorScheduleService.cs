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
        public Task<List<DoctorSchedule>>? GetAllDoctorSchedulesAsync();
    }
}
