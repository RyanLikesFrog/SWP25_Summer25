using DataLayer.Entities;
using DataLayer.Enum;
using RepoLayer.Interfaces;
using ServiceLayer.DTOs.Patient.Request;
using ServiceLayer.DTOs.Patient.Response;
using ServiceLayer.DTOs.User.Request;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Implements
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDoctorScheduleRepository _doctorScheduleRepository;
        private readonly IRepository _repository;

        public AppointmentService(
            IPatientRepository patientRepository,
            IAppointmentRepository appointmentRepository,
            IRepository repository,
            IDoctorScheduleRepository doctorScheduleRepository)
        {
            _patientRepository = patientRepository;
            _appointmentRepository = appointmentRepository;
            _repository = repository;
            _doctorScheduleRepository = doctorScheduleRepository;
        }
        public async Task<List<Appointment>>? GetAllAppointmentsAsync()
        {
            return await _appointmentRepository.GetAllAppointmentsAsync();
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(Guid appointmentId)
        {
            return await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
        }

        public async Task<AppointmentDetailResponse> RegisterAppointmentAsync(UserCreateAppointmentRequest request)
        {
            // Sử dụng các Repository để lấy dữ liệu
            var patient = await _patientRepository.GetPatientByUserIdAsync(request.PatientId);
            if (patient == null)
            {
                throw new ArgumentException("Patient not found.");
            }

            // Tạo Entity
            var newAppointment = new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = request.PatientId,
                DoctorId = request.DoctorId, // Có thể null nếu không có bác sĩ
                AppointmentTitle = request.ApointmentTitle,
                AppointmentStartDate = request.AppointmentStartDate,
                AppointmentType = request.AppointmentType,
                Status = AppointmentStatus.Pending,
                Notes = request.Notes,
                IsAnonymousAppointment = request.IsAnonymousAppointment,
            };
            await _appointmentRepository.CreateAppointmentAsync(newAppointment);
            await _repository.SaveChangesAsync();
            // Map Entity sang DTO
            return new AppointmentDetailResponse
            {
                Id = newAppointment.Id,
                PatientId = newAppointment.PatientId.Value,
                PatientFullName = patient.FullName,
                AppointmentStartDate = newAppointment.AppointmentStartDate,
                AppointmentType = newAppointment.AppointmentType,
                Status = newAppointment.Status,
                Notes = newAppointment.Notes,
                IsAnonymousAppointment = newAppointment.IsAnonymousAppointment,
                ApointmentTitle = newAppointment.AppointmentTitle
            };
        }

        public async Task<AppointmentDetailResponse> StaffUpdateAppointmentAsync(StaffManageAppointmentRequest request)
        {
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(request.AppointmentId);
            if (appointment == null)
            {
                throw new ArgumentException("Appointment not found.");
            }
            // Cập nhật thông tin cuộc hẹn
            appointment.Status = request.NewStatus;
            appointment.AppointmentType = request.AppointmentType;
            appointment.DoctorId = request.DoctorId;
            appointment.AppointmentStartDate = request.AppointmentStartDate;
            appointment.AppointmentEndDate = request.AppointmentEndDate;
            appointment.AppointmentTitle = request.AppointmentTitle;
            appointment.Notes = request.Notes;
            appointment.OnlineLink = request.OnlineLink;
            // Lưu thay đổi
            await _appointmentRepository.UpdateAppointmentAsync(appointment);

            // Nếu có thay đổi về bác sĩ, cần cập nhật schedule cho bác sĩ
            var dupDoctorSchedule = await _doctorScheduleRepository.GetDuplicatedDoctorScheduleByStartDateEndDateAsync(appointment.DoctorId, appointment.AppointmentStartDate, appointment.AppointmentEndDate);
            if (dupDoctorSchedule != null)
            {
                throw new ArgumentException("Duplicated schedule");
            }
            else
            {
                var doctorSchedule = new DoctorSchedule
                {
                    Id = Guid.NewGuid(),
                    AppointmentId = appointment.Id,
                    DoctorId = appointment.DoctorId.Value,
                    StartTime = request.AppointmentStartDate,
                    EndTime = request.AppointmentEndDate.Value,
                    IsAvailable = true,
                };
                await _doctorScheduleRepository.CreateDoctorScheduleAsync(doctorSchedule);
            }

            await _repository.SaveChangesAsync();
            return new AppointmentDetailResponse
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId.Value,
                DoctorId = appointment.DoctorId,
                PatientFullName = appointment.Patient?.FullName, // Lấy tên bệnh nhân nếu có
                //DoctorFullName = appointment.Doctor?.FullName, // Lấy tên bác sĩ nếu có
                AppointmentStartDate = appointment.AppointmentStartDate,
                AppointmentEndDate = appointment.AppointmentEndDate,
                AppointmentType = appointment.AppointmentType,
                Status = appointment.Status,
                Notes = appointment.Notes,
                IsAnonymousAppointment = appointment.IsAnonymousAppointment,
                OnlineLink = appointment.OnlineLink,
                ApointmentTitle = appointment.AppointmentTitle
            };
        }

        public async Task<(List<Appointment>? appointments, string Message)> ViewAppointmentAsync(Guid doctorId)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByDoctorIdAsync(doctorId);

            if (appointments == null || !appointments.Any())
            {
                return (null, "No appointments found for this doctor.");
            }

            return (appointments, "Doctor appointments retrieved successfully.");
        }
    }
}
