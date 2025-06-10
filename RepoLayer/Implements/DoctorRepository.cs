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
            return await _context.Doctors
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<List<Doctor?>> GetAllDoctorsAsync()
        {
            return await _context.Doctors.ToListAsync();
        }

        public Task RemoveDoctorAsync(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
            return Task.CompletedTask;
        }

        public Task UpdateDoctorAsync(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
            return Task.CompletedTask;
        }

        public async Task<List<DoctorSchedule?>> ViewDoctorScheduleAsync(Guid doctorId)
        {
            return await _context.DoctorSchedules
                                .Where(ds => ds.Id == doctorId)
                                .ToListAsync();
        }

        public async Task<Doctor?> GetDoctorByIdAsync(Guid doctorId)
        {
            return await _context.Doctors
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(p => p.Id == doctorId);
        }
    }
}
