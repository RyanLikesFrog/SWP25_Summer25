using DataLayer.Entities;
using DataLayer.Enum;
using Firebase.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RepoLayer.Implements;
using RepoLayer.Interfaces;
using ServiceLayer.DTOs.User.Request;
using ServiceLayer.Interfaces;
using ServiceLayer.Validator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Implements
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;
        private readonly IRepository _repository;

        public PatientService(IPatientRepository patientRepository, IUserRepository userRepository, IConfiguration configuration, IRepository repository)
        {
            _patientRepository = patientRepository;
            _userRepository = userRepository;
            _config = configuration;
            _repository = repository;
        }
        public async Task<List<Patient?>> GetAllPatientsAsync()
        {
            return await _patientRepository.GetAllPatientsAsync();
        }


        public async Task<Patient?> GetPatientByUserIdAsync(Guid patientId)
        {
            return await _patientRepository.GetPatientByUserIdAsync(patientId);
        }

        public async Task<(bool Success, string Message, Patient? Patient)> UpdatePatientProfileAsync(UpdatePatientRequest request)
        {
            var patient = await _patientRepository.GetPatientByIdAsync(request.PatientId);
            if (patient == null)
            {
                return (false, $"Người dùng với ID {patient.UserId} không tìm thấy.", null);
            }

            // --- Cập nhật thông tin User ---
         
            if (!string.IsNullOrEmpty(request.Email))
            {
                var existingUserWithEmail = await _userRepository.GetUserByEmailAsync(request.Email);
                if (existingUserWithEmail != null && existingUserWithEmail.Id != patient.UserId)
                {
                    return (false, "Email đã tồn tại.", null);
                }
                patient.User.Email = request.Email;
            }

            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                patient.User.PhoneNumber = request.PhoneNumber;
            }

            // Update avatar moi 
            if (request.AvatarPicture != null && request.AvatarPicture.Length > 0)
            {
                if (request.AvatarPicture.FileName.HasImageExtension())
                {
                    string firebaseBucket = _config["Firebase:StorageBucket"];

                    // Initialize FirebaseStorage instance
                    var firebaseStorage = new FirebaseStorage(firebaseBucket);

                    // Generate a unique file name
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + request.AvatarPicture.FileName;

                    // Get reference to the file in Firebase Storage
                    var fileReference = firebaseStorage.Child("StaffImages").Child(uniqueFileName);

                    // Upload the file to Firebase Storage
                    using (var stream = request.AvatarPicture.OpenReadStream())
                    {
                        await fileReference.PutAsync(stream);
                    }

                    // Get the download URL of the uploaded file
                    string downloadUrl = await fileReference.GetDownloadUrlAsync();
                    patient.User.ProfilePictureURL = downloadUrl; // Lưu URL vào biến
                }
                else
                {
                    throw new Exception("Not support file type" + nameof(request.AvatarPicture.FileName).ToString());
                }
            }
            patient.FullName = request.FullName;

            
            
            // Không làm gì nếu không phải Doctor và không chuyển sang Doctor

            patient.User.UpdatedAt = DateTime.UtcNow;

            // --- Lưu tất cả các thay đổi ---
            try
            {
                await _patientRepository.UpdatePatientAsync(patient);

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

            return (true, $"Cập nhật thông tin người dùng {patient.User.Role} thành công.", patient);
        }
    }
}
