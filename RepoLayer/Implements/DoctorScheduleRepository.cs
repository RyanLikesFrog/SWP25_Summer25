using DataLayer.DbContext;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using RepoLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Implements
{
    public class DoctorScheduleRepository : IDoctorScheduleRepository
    {
        private readonly SWPSU25Context _context;

        public DoctorScheduleRepository (SWPSU25Context context)
        {
            _context = context;
        }

        public async Task<List<DoctorSchedule?>> GetAllDoctorSchedulesAsync()
        {
            return await _context.DoctorSchedules
                .ToListAsync();
        }

        public async Task<DoctorSchedule?> GetDoctorScheduleByIdAsync(Guid scheduleId)
        {
            return await _context.DoctorSchedules
                .FirstOrDefaultAsync(ds => ds.Id == scheduleId);    
        }

        public Task<DoctorSchedule?> GetDuplicatedDoctorScheduleByStartDateEndDateAsync(Guid? doctorId, DateTime startDate, DateTime? endDate)
        {
            if (endDate != null)
            {
                return _context.DoctorSchedules.Where(x => x.DoctorId == doctorId && (x.StartTime >= startDate && x.StartTime <= endDate)).FirstOrDefaultAsync();
            }
            else
            {
                return _context.DoctorSchedules.Where(x => x.DoctorId == doctorId && x.StartTime >= startDate && x.StartTime <= startDate.AddHours(1)).FirstOrDefaultAsync();
            }
        }
        public async Task<DoctorSchedule?> CreateDoctorScheduleAsync(DoctorSchedule doctorSchedule)
        {
            return (await _context.DoctorSchedules.AddAsync(doctorSchedule)).Entity;
        }

        public async Task<List<DoctorSchedule?>> GetDoctorSchedulesByDoctorIdAsync(Guid doctorId)
        {
            return await _context.DoctorSchedules
                                 .AsNoTracking()
                                 .Where(s => s.DoctorId == doctorId) 
                                 .ToListAsync(); 
        }

        public async Task<DoctorSchedule?> GetDoctorScheduleByAppointmentIdAsync(Guid? appointmentId)
        {
            return await _context.DoctorSchedules
                .FirstOrDefaultAsync(ds => ds.AppointmentId == appointmentId);
        }
        public async Task<List<DoctorSchedule?>> GetTodayDoctorSchedulesByDoctorIdAsync(Guid doctorId)
        {
            var today = DateTime.Today;
            return await _context.DoctorSchedules
                .Where(ds => ds.DoctorId == doctorId && (ds.StartTime >= today && ds.StartTime <= today.AddHours(24)))
                .ToListAsync();
        }
        public Task UpdateDoctorSchedule(DoctorSchedule doctorSchedule)
        {
            _context.Update(doctorSchedule);
            return Task.CompletedTask;
        }
    }
}
