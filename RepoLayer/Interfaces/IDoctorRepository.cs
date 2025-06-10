using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Interfaces
{
    public interface IDoctorRepository
    {
        public Task<Doctor?> GetDoctorByUserIdAsync(Guid userId);
        public Task<Doctor?> GetDoctorByIdAsync(Guid doctorId);
        public Task<List<Doctor?>> GetAllDoctorsAsync();
        public Task AddDoctorAsync(Doctor doctor);
        public Task UpdateDoctorAsync(Doctor doctor);
        public Task RemoveDoctorAsync(Doctor doctor);
    }
}
