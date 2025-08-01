using DataLayer.DbContext;
using DataLayer.Entities;
using RepoLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Implements
{
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly SWPSU25Context _context;

        public PrescriptionRepository(SWPSU25Context context)
        {
            _context = context;
        }

        public async Task CreatePrescriptionAsync(Prescription prescription)
        {
            await _context.Prescriptions.AddAsync(prescription);
        }
    }
}
