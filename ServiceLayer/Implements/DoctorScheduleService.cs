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
    public class DoctorScheduleService : IDoctorScheduleService
    {
        private readonly IDoctorScheduleRepository _doctorScheduleRepository;
        public async Task<List<DoctorSchedule>>? GetAllDoctorSchedulesAsync()
        {
            return await _doctorScheduleRepository.GetAllDoctorSchedulesAsync();
        }

        public async Task<DoctorSchedule?> GetDoctorSchedulebyIdAsync(Guid scheduleId)
        {
            return await _doctorScheduleRepository.GetDoctorScheduleByIdAsync(scheduleId);
        }
    }
}
