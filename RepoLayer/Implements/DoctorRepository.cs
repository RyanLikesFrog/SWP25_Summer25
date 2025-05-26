using DataLayer.DbContext;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using RepoLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Implements
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly SWPSU25Context _context;
        public DoctorRepository(SWPSU25Context context)
        {
            _context = context;
        }

        public async Task AddDoctorAsync(Doctor doctor)
        {
            await _context.Doctors.AddAsync(doctor);
        }

        public async Task<Doctor?> GetDoctorByUserIdAsync(Guid userId)
        {
            return await _context.Doctors.FirstOrDefaultAsync(x => x.UserId == userId);
        }
    }
}
