using DataLayer.Entities;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Implements
{
    public class DoctorScheduleService : IDoctorScheduleService
    {
        public Task<List<DoctorSchedule>>? GetAllDoctorSchedulesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<DoctorSchedule?> GetDoctorSchedulebyIdAsync(Guid scheduleId)
        {
            throw new NotImplementedException();
        }
    }
}
