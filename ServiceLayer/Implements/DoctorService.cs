using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using RepoLayer.Interfaces;
using ServiceLayer.DTOs.User.Request;
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
            return await _doctorRepository.GetDoctorByIdAsync(doctorId);
        }

        public async Task<Doctor?> UpdateDoctorAsync(UpdateDoctorRequest request)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(request.DoctorId);
            if (doctor == null)
            {
                throw new ArgumentException("Doctor not found.");
            }

            // Update only if the values are provided
            if (!string.IsNullOrWhiteSpace(request.FullName))
                doctor.FullName = request.FullName;

            if (!string.IsNullOrWhiteSpace(request.Specialization))
                doctor.Specialization = request.Specialization;

            if (!string.IsNullOrWhiteSpace(request.Qualifications))
                doctor.Qualifications = request.Qualifications;

            if (!string.IsNullOrWhiteSpace(request.Experience))
                doctor.Experience = request.Experience;

            if (!string.IsNullOrWhiteSpace(request.Bio))
                doctor.Bio = request.Bio;

            if (request.IsActive.HasValue)
                doctor.isActive = request.IsActive.Value;

            await _doctorRepository.UpdateDoctorAsync(doctor);
            await _repository.SaveChangesAsync();

            return doctor;
        }
    }
}
