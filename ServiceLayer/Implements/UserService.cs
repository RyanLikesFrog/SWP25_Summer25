using DataLayer.DbContext;
using DataLayer.Entities;
using DataLayer.Enum;
using Microsoft.EntityFrameworkCore;
using RepoLayer.Interfaces;
using ServiceLayer.DTOs.User.Request;
using ServiceLayer.DTOs.User.Response;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Implements
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IDoctorRepository _doctorRepository; // Cần inject DoctorRepository
        private readonly IRepository _repository; // Inject DbContext trực tiếp để quản lý transaction

        public UserService(
            IUserRepository userRepository,
            IDoctorRepository doctorRepository,
            IRepository repository)
        {
            _userRepository = userRepository;
            _doctorRepository = doctorRepository;
            _repository = repository;
        }

        public async Task<CreateUserResponse> CreateUserAccountByAdminAsync(CreateAccountByAdminRequest request)
        {
            // 1. Kiểm tra username hoặc email đã tồn tại chưa
            var existingUserByUsername = await _userRepository.GetUserByUsernameAsync(request.Username);
            if (existingUserByUsername != null)
            {
                return new CreateUserResponse { Success = false, Message = "Username already exists." };
            }

            var existingUserByEmail = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUserByEmail != null)
            {
                return new CreateUserResponse { Success = false, Message = "Email already registered." };
            }

            // Đảm bảo chỉ các vai trò Admin, Doctor, Staff, Manager được tạo qua endpoint này
            if (request.Role == UserRole.Patient) // Hoặc bất kỳ vai trò nào không được phép tạo bởi Admin
            {
                return new CreateUserResponse { Success = false, Message = "Cannot create Patient accounts via this endpoint. Please use the patient registration endpoint." };
            }
            // 3. Tạo đối tượng User
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Password = request.Password,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Role = request.Role,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // 4. Lưu User vào database (chỉ đánh dấu là thêm)
            await _userRepository.AddUserAsync(newUser);

            Guid? associatedEntityId = null;

            // 5. Nếu vai trò là Doctor, tạo thêm đối tượng Doctor
            if (request.Role == UserRole.Doctor)
            {
                var newDoctor = new Doctor
                {
                    Id = Guid.NewGuid(),
                    UserId = newUser.Id, // Liên kết Doctor với User vừa tạo
                    FullName = request.FullName,
                    Specialization = request.Specialization,
                    Qualifications = request.Qualifications,
                    Experience = request.Experience,
                    Bio = request.Bio,
                    ProfilePictureURL = request.ProfilePictureURL
                };
                await _doctorRepository.AddDoctorAsync(newDoctor);
                associatedEntityId = newDoctor.Id;
            }
            // Có thể thêm các 'else if' cho các vai trò khác nếu cần tạo các entity liên quan

            await _repository.SaveChangesAsync(); // Commit transaction cho cả User và Doctor (nếu có)
            return new CreateUserResponse
            {
                Success = true,
                Message = $"{request.Role} account created successfully.",
                UserId = newUser.Id,
                Username = newUser.Username,
                Email = newUser.Email,
                Role = newUser.Role,
                AssociatedEntityId = associatedEntityId
            };
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetUserByUsernameAsync(username);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }
    }
}

