using ServiceLayer.DTOs.Auth;
using ServiceLayer.DTOs.Patient.Request;
using ServiceLayer.DTOs.Patient.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface IAuthService
    {
        public Task<LoginResponse> LoginAsync(LoginRequest request);
        public Task<RegisterResponse> RegisterPatientAsync(PatientRegisterRequest request);
    }
}
