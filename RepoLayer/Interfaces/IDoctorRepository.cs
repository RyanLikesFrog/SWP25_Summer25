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
        public Task AddDoctorAsync(Doctor doctor);
    }
}
