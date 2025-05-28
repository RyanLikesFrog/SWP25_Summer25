using DataLayer.Entities;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Implements
{
    public class ARVProtocolService : IARVProtocolService
    {
        public Task<List<Appointment>>? GetAllARVProtocolsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ARVProtocol?> GetARVProtocolByIdAsync(Guid protocolId)
        {
            throw new NotImplementedException();
        }
    }
}
