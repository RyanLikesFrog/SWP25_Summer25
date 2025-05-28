using DataLayer.Entities;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Implements
{
    public class DoctorService : IDoctorService
    {
        public Task<List<Doctor>>? GetAllDoctorsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Doctor?> GetDoctorByIdAsync(Guid doctorId)
        {
            throw new NotImplementedException();
        }
    }
}
