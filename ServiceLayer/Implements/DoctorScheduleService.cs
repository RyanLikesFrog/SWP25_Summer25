using DataLayer.Entities;
using RepoLayer.Interfaces;
using ServiceLayer.DTOs;
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
        private readonly IDoctorRepository _doctorRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IRepository _repository;

        public DoctorScheduleService(IDoctorScheduleRepository doctorScheduleRepository, IDoctorRepository doctorRepository, IAppointmentRepository appointmentRepository, IRepository repository)
        {
            _doctorScheduleRepository = doctorScheduleRepository;
            _doctorRepository = doctorRepository;
            _appointmentRepository = appointmentRepository;
            _repository = repository;
        }

        public async Task<DoctorScheduleDetailResponse> CreateDoctorScheduleAsync(CreateDoctorScheduleRequest request)
        {

            if (request.DoctorId != Guid.Empty) 
            {
                var doctor = await _doctorRepository.GetDoctorByIdAsync(request.DoctorId);
                if (doctor == null)
                {
                    throw new ArgumentException("Doctor not found with the provided Doctor ID");
                }
            }

            var newDoctorSchedule = new DoctorSchedule
            {
                Id = Guid.NewGuid(),
                DoctorId = request.DoctorId,
                AppointmentId = request.AppointmentId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Notes = request.Notes,
                IsAvailable = request.IsAvailable
            };

            await _doctorScheduleRepository.CreateDoctorScheduleAsync(newDoctorSchedule);
            await _repository.SaveChangesAsync();

            return new DoctorScheduleDetailResponse
            {
                Id = newDoctorSchedule.Id,
                DoctorId = newDoctorSchedule.DoctorId,
                AppointmentId = newDoctorSchedule.AppointmentId,
                StartTime = newDoctorSchedule.StartTime,
                EndTime = newDoctorSchedule.EndTime,
                Notes = newDoctorSchedule.Notes,
                IsAvailable = newDoctorSchedule.IsAvailable
            };
        }

        public async Task<List<DoctorSchedule?>> GetAllDoctorSchedulesAsync()
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

        public async Task<(List<DoctorSchedule?> schedules, string Message)> ViewDoctorScheduleAsync(Guid doctorId)
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
