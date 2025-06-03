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

        public async Task CreateDoctorScheduleAsync(DoctorSchedule doctorSchedule)
        {
            await _doctorScheduleRepository.CreateDoctorScheduleAsync(doctorSchedule);
        }

        public async Task<List<DoctorSchedule>>? GetAllDoctorSchedulesAsync()
        {
            return await _doctorScheduleRepository.GetAllDoctorSchedulesAsync();
        }

        public async Task<DoctorSchedule?> GetDoctorSchedulebyIdAsync(Guid scheduleId)
        {
            return await _doctorScheduleRepository.GetDoctorScheduleByIdAsync(scheduleId);
        }

        public async Task<(DoctorSchedule? doctorSchedule, string Message)>? GetDuplicatedDoctorScheduleByStartDateEndDateAsync(Guid? doctorId, DateTime startDate, DateTime? endDate)
        {
            var doctorSchedule = await _doctorScheduleRepository.GetDuplicatedDoctorScheduleByStartDateEndDateAsync(doctorId, startDate, endDate);
            if (doctorSchedule == null)
            {
                return (null, "Khong bi trung lich");
            }
            return (doctorSchedule, "Bi trung lich");
        }

        public async Task<(List<DoctorSchedule>? schedules, string Message)> ViewDoctorScheduleAsync(Guid doctorId)
        {
            var schedules = await _doctorScheduleRepository.GetDoctorSchedulesByDoctorIdAsync(doctorId);

            if (schedules == null || !schedules.Any())
            {
                return (null, "khong tim thay lich");
            }
            else
            {
                return (schedules, "tim lich thanh cong.");
            }
        }
    }
}
