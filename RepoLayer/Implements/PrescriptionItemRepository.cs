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
    public class PrescriptionItemRepository : IPrescriptionItemRepository
    {
        private readonly SWPSU25Context _context;

        public PrescriptionItemRepository(SWPSU25Context context)
        {
            _context = context;
        }

        public async Task CreatePrescriptionItemAsync(PrescriptionItem prescriptionItem)
        {
            await _context.PrescriptionItems.AddAsync(prescriptionItem);
        }
    }
}
