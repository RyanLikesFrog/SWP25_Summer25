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
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IDoctorScheduleRepository _doctorScheduleRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IRepository _repository;

        public DoctorService(
            IRepository repository,
            IAppointmentRepository appointmentRepository,
            IDoctorScheduleRepository doctorScheduleRepository,
            IDoctorRepository doctorRepository)

        {
            _doctorRepository = doctorRepository;
            _doctorScheduleRepository = doctorScheduleRepository;
            _appointmentRepository = appointmentRepository;
            _repository = repository;
        }

        public async Task<List<Doctor>>? GetAllDoctorsAsync()
        {
            return await _doctorRepository.GetAllDoctorsAsync();
        }

        public async Task<Doctor?> GetDoctorByIdAsync(Guid doctorId)
        {
            return await _doctorRepository.GetDoctorByUserIdAsync(doctorId);
        }

        public async Task<(List<DoctorSchedule>? schedules, string Message)> ViewDoctorScheduleAsync(Guid doctorId)
        {
            var schedules = await _doctorScheduleRepository.GetDoctorSchedulesByDoctorIdAsync(doctorId);

            if (schedules == null || !schedules.Any())
            {
                return (null, "khong tim thay lich.");
            }
            else
            {
                return (schedules, "tim lich thanh cong.");
            }
        }

        public async Task<(List<Appointment>? appointments, string Message)> ViewAppointmentAsync(Guid doctorId)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByDoctorIdAsync(doctorId);

            if (appointments == null || !appointments.Any())
            {
                return (null, "khong tim thay cuoc hen.");
            }
            else
            {
                return (appointments, "tim cuoc hen thanh cong.");
            }
        }
    }
}
