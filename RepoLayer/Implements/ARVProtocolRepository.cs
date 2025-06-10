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
    public class ARVProtocolRepository : IARVProtocolRepository
    {
        private readonly SWPSU25Context _context;

        public ARVProtocolRepository(SWPSU25Context context)
        {
            _context = context;
        }

        public async Task<List<ARVProtocol?>> GetAllARVProtocolsAsync()
        {
            return await _context.ARVProtocols.Include(u => u.PatientTreatmentProtocols)
                                              .ToListAsync();
        }

        public async Task<ARVProtocol?> GetARVProtocolByIdAsync(Guid protocolId)
        {
            return await _context.ARVProtocols.Include(u => u.PatientTreatmentProtocols)
                                              .FirstOrDefaultAsync(u => u.ProtocolId == protocolId);
        }
    }
}
