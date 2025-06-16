using DataLayer.Entities;
using ServiceLayer.DTOs.Patient.Request;
using ServiceLayer.DTOs.Patient.Response;
using ServiceLayer.DTOs.User.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface IAppointmentService
    {
        public Task<Appointment?> GetAppointmentByIdAsync(Guid appointmentId);
        public Task<List<Appointment>>? GetAllAppointmentsAsync();
        //Task<AppointmentDetailResponse> RegisterAppointmentAsync(UserCreateAppointmentRequest request);
        //Task<AppointmentDetailResponse> StaffUpdateAppointmentAsync(StaffManageAppointmentRequest request);
        public Task<(List<Appointment?> appointments, string Message)> ViewAppointmentAsync(Guid doctorId);
        public Task<AppointmentDetailResponse> CreateAppointmentAndInitiatePaymentAsync(UserCreateAppointmentRequest request);
    }
}
