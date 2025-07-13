using DataLayer.Entities;
using RepoLayer.Interfaces;
using ServiceLayer.DTOs;
using ServiceLayer.DTOs.User.Request;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Implements
{
    public class PatientTreatmentProtocolService : IPatientTreatmentProtocolService
    {
        private readonly IPatientTreatmentProtocolRepository _patientTreatmentProtocolRepository;
        private readonly IRepository _repository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IARVProtocolRepository _aRVProtocolRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDoctorScheduleRepository _doctorScheduleRepository;

        public PatientTreatmentProtocolService(
            IPatientTreatmentProtocolRepository patientTreatmentProtocolRepository,
            IRepository repository,
            IPatientRepository patientRepository,
            IDoctorRepository doctorRepository,
            IARVProtocolRepository aRVProtocolRepository,
            IAppointmentRepository appointmentRepository,
            IDoctorScheduleRepository doctorScheduleRepository)
        {
            _patientTreatmentProtocolRepository = patientTreatmentProtocolRepository;
            _repository = repository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _aRVProtocolRepository = aRVProtocolRepository;
            _appointmentRepository = appointmentRepository;
            _doctorScheduleRepository = doctorScheduleRepository;
        }

        public async Task<PatientTreatmentProtocolDetailResponse> CreatePatientTreatmentProtocolAsync(CreatePatientTreatmentProtocolRequest request)
        {
            var patient = await _patientRepository.GetPatientByIdAsync(request.PatientId);
            if (patient == null)
            {
                throw new ArgumentException("Patient not found with the provided Patient ID.");
            }

            if (request.DoctorId != Guid.Empty) // Fix: Check if DoctorId is not an empty Guid instead of using HasValue
            {
                var doctor = await _doctorRepository.GetDoctorByIdAsync(request.DoctorId);
                if (doctor == null)
                {
                    throw new ArgumentException("Doctor not found with the provided Doctor ID");
                }
            }

            if (request.ARVProtocolId.HasValue)
            {
                var protocol = await _aRVProtocolRepository.GetARVProtocolByIdAsync(request.ARVProtocolId.Value);
                if (protocol == null)
                {
                    throw new ArgumentException("ARVPRotocol not found with the provided Protocol ID");
                }
            }

            if (request.AppointmentId.HasValue)
            {
                var appointment = await _appointmentRepository.GetAppointmentByIdAsync(request.AppointmentId.Value);
                if (appointment == null)
                {
                    throw new ArgumentException("Appointment not found with the provided Appointment ID");
                }
            }

            var newPatientTreatmentProtocol = new PatientTreatmentProtocol
            {
                Id = Guid.NewGuid(),
                PatientId = request.PatientId,
                DoctorId = request.DoctorId,
                ARVProtocolId = request.ARVProtocolId,
                AppointmentId = request.AppointmentId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = request.Status
            };
            
            var doctorSchedule = await _doctorScheduleRepository.GetDoctorScheduleByAppointmentIdAsync(request.AppointmentId);
            if(doctorSchedule == null)
            {
                throw new ArgumentException("Doctor schedule not found for the provided Appointment ID");
            }
            doctorSchedule.IsAvailable = false;
            await _doctorScheduleRepository.UpdateDoctorScheduleAsync(doctorSchedule);

            await _patientTreatmentProtocolRepository.CreatePatientTreatmentProtocol(newPatientTreatmentProtocol);
            await _repository.SaveChangesAsync();

            return new PatientTreatmentProtocolDetailResponse
            {
                Id = newPatientTreatmentProtocol.Id,
                PatientId = newPatientTreatmentProtocol.PatientId,
                DoctorId = newPatientTreatmentProtocol.DoctorId,
                ProtocolId = newPatientTreatmentProtocol.ARVProtocolId,
                AppointmentId = newPatientTreatmentProtocol.AppointmentId,
                StartDate = newPatientTreatmentProtocol.StartDate,
                EndDate = newPatientTreatmentProtocol.EndDate,
                Status = newPatientTreatmentProtocol.Status,

                TreatmentStages = new List<TreatmentStageResponse>()
            };
        }

        public async Task<List<PatientTreatmentProtocol?>> GetAllPatientTreatmentProtocolsAsync()
        {
            return await _patientTreatmentProtocolRepository.GetAllPatientTreatmentProtocolsAsync();
        }

        public async Task<PatientTreatmentProtocol?> GetPatientTreatmentProtocolByIdAsync(Guid patientTreatmentProtocolId)
        {
            return await _patientTreatmentProtocolRepository.GetPatientTreatmentProtocolByIdAsync(patientTreatmentProtocolId);
        }

        public async Task<bool> UpdatePatientTreatmentProtocolStatusAsync(UpdatePatientTreatmentProtocolStatusRequest request)
        {
            if (request == null)
            {
                return false;
            }

            var treatmentProtocol = await _patientTreatmentProtocolRepository.GetPatientTreatmentProtocolByIdAsync(request.ProtocolId);

            if (treatmentProtocol == null)
            {
                throw new ArgumentException("Treatment Protocol not found with the provided Protocol ID");
            }

            var success = await _patientTreatmentProtocolRepository.UpdatePatientTreatmentProtocolStatusAsync(request.ProtocolId, request.Status);

            return success;
        }
    }
}