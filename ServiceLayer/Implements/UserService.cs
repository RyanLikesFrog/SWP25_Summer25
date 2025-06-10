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
        private readonly IPatientRepository _patientRepository; // Cần inject PatientRepository nếu cần

        public UserService(
            IUserRepository userRepository,
            IDoctorRepository doctorRepository,
            IRepository repository,
            IPatientRepository patientRepository)
        {
            _userRepository = userRepository;
            _doctorRepository = doctorRepository;
            _repository = repository;
            _patientRepository = patientRepository;
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

        public async Task<(bool Success, string Message, User? User)> UpdateUserInformationAsync(Guid userId, UpdateUserRequest request)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return (false, $"Người dùng với ID {userId} không tìm thấy.", null);
            }

            // --- Cập nhật thông tin User ---
            if (!string.IsNullOrEmpty(request.Username) && user.Username != request.Username)
            {
                var existingUserWithUsername = await _userRepository.GetUserByUsernameAsync(request.Username);
                if (existingUserWithUsername != null && existingUserWithUsername.Id != userId)
                {
                    return (false, "Tên người dùng đã tồn tại.", null);
                }
                user.Username = request.Username;
            }
            
            if (!string.IsNullOrEmpty(request.Email) && user.Email != request.Email)
            {
                var existingUserWithEmail = await _userRepository.GetUserByEmailAsync(request.Email);
                if (existingUserWithEmail != null && existingUserWithEmail.Id != userId)
                {
                    return (false, "Email đã tồn tại.", null);
                }
                user.Email = request.Email;
            }

            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                user.PhoneNumber = request.PhoneNumber;
            }

            // --- Xử lý thay đổi Role và thông tin Doctor ---
            bool roleChanged = false;
            UserRole? oldRole = user.Role;

            if (request.Role.HasValue && user.Role != request.Role.Value)
            {
                // Logic bảo vệ để ngăn chặn việc hạ cấp Admin khác
                if (user.Role == UserRole.Admin && request.Role != UserRole.Admin)
                {
                    return (false, "Không thể hạ cấp vai trò của một quản trị viên khác.", null);
                }
                user.Role = request.Role.Value;
                roleChanged = true;
            }

            Doctor? doctor = null;
            if (user.Role == UserRole.Doctor)
            {
                doctor = await _doctorRepository.GetDoctorByUserIdAsync(userId);

                if (doctor == null && (oldRole != UserRole.Doctor || roleChanged))
                {
                    // Nếu user được chuyển sang vai trò Doctor và chưa có hồ sơ Doctor
                    doctor = new Doctor
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        FullName = request.FullName ?? user.Username, // Dùng FullName từ request hoặc Username
                        Specialization = request.Specialization,
                        Qualifications = request.Qualifications,
                        Experience = request.Experience,
                        Bio = request.Bio,
                        ProfilePictureURL = request.ProfilePictureURL
                    };
                    await _doctorRepository.AddDoctorAsync(doctor);
                }
                else if (doctor != null)
                {
                    // Cập nhật thông tin Doctor hiện có
                    if (!string.IsNullOrEmpty(request.FullName)) doctor.FullName = request.FullName;
                    if (!string.IsNullOrEmpty(request.Specialization)) doctor.Specialization = request.Specialization;
                    if (!string.IsNullOrEmpty(request.Qualifications)) doctor.Qualifications = request.Qualifications;
                    if (!string.IsNullOrEmpty(request.Experience)) doctor.Experience = request.Experience;
                    if (!string.IsNullOrEmpty(request.Bio)) doctor.Bio = request.Bio;
                    if (!string.IsNullOrEmpty(request.ProfilePictureURL)) doctor.ProfilePictureURL = request.ProfilePictureURL;

                    await _doctorRepository.UpdateDoctorAsync(doctor);
                }
                // Nếu doctor vẫn là null ở đây, có thể do lỗi logic hoặc dữ liệu không nhất quán
                // Có thể thêm log hoặc throw exception
            }
            else if (oldRole == UserRole.Doctor && user.Role != UserRole.Doctor)
            {
                // Chuyển từ Doctor sang vai trò khác: Xóa thông tin Doctor
                var doctorToDelete = await _doctorRepository.GetDoctorByUserIdAsync(userId);
                if (doctorToDelete != null)
                {
                    doctorToDelete.isActive = false; // Đánh dấu là không hoạt động thay vì xóa
                }
            }
            // Không làm gì nếu không phải Doctor và không chuyển sang Doctor

            user.UpdatedAt = DateTime.UtcNow;

            // --- Lưu tất cả các thay đổi ---
            try
            {
                await _userRepository.UpdateUserAsync(user);

                // Lưu thay đổi của Doctor nếu có (Add, Update, Remove đều được theo dõi bởi DbContext)
                await _repository.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // _logger.LogError(ex, $"Lỗi cập nhật cơ sở dữ liệu cho người dùng {userId}.");
                return (false, "Lỗi cơ sở dữ liệu khi cập nhật thông tin.", null);
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, $"Lỗi không mong muốn khi cập nhật người dùng {userId}.");
                return (false, "Đã xảy ra lỗi không mong muốn.", null);
            }

            return (true, $"Cập nhật thông tin người dùng {user.Role} thành công.", user);
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _userRepository.GetUserByIdAsync(userId);
        }

        public async Task<List<User?>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<(bool Success, string Message, User? User)> InActiveUserAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return (false, $"Người dùng với ID {userId} không tìm thấy.", null);
            }
            try
            {
                user.isActive = false;  // Đánh dấu người dùng là không hoạt động
                await _userRepository.UpdateUserAsync(user);

                if (user.Role == UserRole.Doctor)
                {
                    var doctor = await _doctorRepository.GetDoctorByUserIdAsync(userId);
                    if (doctor == null)
                    {
                        return (false, "Không tìm thấy thông tin Doctor liên kết với người dùng này.", null);
                    }
                    doctor.isActive = false; // Đánh dấu Doctor là không hoạt động
                }
                else if (user.Role == UserRole.Patient)
                {
                    var patient = await _patientRepository.GetPatientByUserIdAsync(userId);
                    if (patient == null)
                    {
                        return (false, "Không tìm thấy thông tin Patient liên kết với người dùng này.", null);
                    }
                    patient.IsActive = false; // Đánh dấu Patient là không hoạt động
                }
                else if (user.Role == UserRole.Admin)
                {
                    return (false, "Không thể đánh dấu quản trị viên là không hoạt động.", null);
                }

                await _repository.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu

                return (true, "Người dùng đã được đánh dấu là không hoạt động.", user);
            }
            catch (DbUpdateException ex)
            {
                // Log lỗi nếu cần thiết
                return (false, "Lỗi cơ sở dữ liệu khi đánh dấu người dùng là không hoạt động.", null);
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần thiết
                return (false, "Đã xảy ra lỗi không mong muốn khi đánh dấu người dùng là không hoạt động.", null);
            }
        }
    }
}

