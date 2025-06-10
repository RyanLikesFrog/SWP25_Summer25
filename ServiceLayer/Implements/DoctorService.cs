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
    }
}
