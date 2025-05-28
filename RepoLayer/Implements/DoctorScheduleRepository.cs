using DataLayer.Entities;
using RepoLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Implements
{
    public class DoctorScheduleRepository : IDoctorScheduleRepository
    {
        public Task<List<DoctorSchedule>> GetAllDoctorSchedulesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<DoctorSchedule?> GetDoctorScheduleByIdAsync(Guid scheduleId)
        {
            throw new NotImplementedException();
        }
    }
}
