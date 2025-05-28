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
        public Task<List<DoctorSchedule>>  GetAllDoctorSchedulesAsync();

    }
}
