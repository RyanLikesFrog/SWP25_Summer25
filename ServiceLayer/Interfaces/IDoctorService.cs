using DataLayer.Entities;
using ServiceLayer.DTOs.User.Request;
using ServiceLayer.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface IDoctorService
    {
        public Task<Doctor?> GetDoctorByIdAsync(Guid doctorId);
        public Task<List<Doctor>>? GetAllDoctorsAsync();
        public Task<Doctor?> UpdateDoctorAsync(UpdateDoctorRequest request);

    }
}
