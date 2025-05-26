using DataLayer.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepoLayer.Interfaces;
using ServiceLayer.DTOs.Auth;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Implements
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IPatientRepository patientRepository, IDoctorRepository doctorRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetUserByUsernameAsync(request.Username);

            if (user == null)
            {
                return new LoginResponse { Success = false, Message = "Tên đăng nhập không tồn tại." };
            }

            if (request.Password != user.Password)
            {
                return new LoginResponse { Success = false, Message = "Mật khẩu không đúng." };
            }

            // Authentication successful, generate JWT token
            var token = await GenerateJwtTokenAsync(user); // Sử dụng async version

            var response = new LoginResponse
            {
                Success = true,
                Message = "Đăng nhập thành công.",
                Token = token,
                UserId = user.Id,
                Username = user.Username,
                Role = user.Role.ToString()
            };

            // Gắn các ID cụ thể dựa trên vai trò
            if (user.Role == UserRole.Patient)
            {
                var patient = await _patientRepository.GetPatientByUserIdAsync(user.Patient.Id);
                if (patient != null)
                {
                    response.PatientId = patient.Id;
                }
            }
            else if (user.Role == UserRole.Doctor)
            {
                var doctor = await _doctorRepository.GetDoctorByUserIdAsync(user.Doctor.Id);
                if (doctor != null)
                {
                    response.DoctorId = doctor.Id;
                }
            }
            // Đối với Staff, Manager, Admin, UserId là đủ nếu không có entity riêng biệt.
            // Nếu có entity riêng, bạn sẽ thêm logic tương tự ở đây.

            return response;
        }

        private async Task<string> GenerateJwtTokenAsync(DataLayer.Entities.User user) // Đổi sang async
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, user.Role.ToString()) // Thêm Role claim
            };

            // Thêm các claim cụ thể cho từng vai trò
            if (user.Role == UserRole.Patient)
            {
                var patient = await _patientRepository.GetPatientByUserIdAsync(user.Id);
                if (patient != null)
                {
                    claims.Add(new Claim("patient_id", patient.Id.ToString()));
                    claims.Add(new Claim("full_name", patient.FullName!)); // Thêm tên đầy đủ của bệnh nhân
                }
            }
            else if (user.Role == UserRole.Doctor)
            {
                var doctor = await _doctorRepository.GetDoctorByUserIdAsync(user.Id);
                if (doctor != null)
                {
                    claims.Add(new Claim("doctor_id", doctor.Id.ToString()));
                    claims.Add(new Claim("full_name", doctor.FullName!)); // Thêm tên đầy đủ của bác sĩ
                    claims.Add(new Claim("specialization", doctor.Specialization!)); // Thêm chuyên khoa
                }
            }
            // Không cần thêm claims đặc biệt cho Staff, Manager, Admin nếu bạn chỉ muốn dựa vào Role claim chung.
            // Nếu bạn có các trường thông tin đặc biệt cho các vai trò này, hãy thêm ở đây.
            // Ví dụ:
            // else if (user.Role == UserRole.Staff) { claims.Add(new Claim("staff_department", "Reception")); }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2), // Sử dụng UtcNow cho tính nhất quán
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
