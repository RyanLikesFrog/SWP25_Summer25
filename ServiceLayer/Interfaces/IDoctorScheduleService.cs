using DataLayer.Entities;
using ServiceLayer.DTOs;
using ServiceLayer.DTOs.User.Request;
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
        public Task<(DoctorSchedule? doctorSchedule, string Message)>? GetDuplicatedDoctorScheduleByStartDateEndDateAsync(Guid? doctorId, DateTime startDate, DateTime? endDate);
        public Task<(List<DoctorSchedule?> schedules, string Message)> ViewDoctorScheduleAsync(Guid doctorId);
        public Task<DoctorScheduleDetailResponse> CreateDoctorScheduleAsync(CreateDoctorScheduleRequest request);
        public Task<List<DoctorSchedule?>> GetTodayDoctorScheduleByDoctorIdAsync(Guid doctorId);
        public Task<DoctorSchedule?> UpdateDoctorScheduleAsync(UpdateDoctorScheduleRequest request);


    }
}
