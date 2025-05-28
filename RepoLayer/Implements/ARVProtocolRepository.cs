using DataLayer.Entities;
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
        public Task<List<ARVProtocol>> GetAllARVProtocolsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ARVProtocol?> GetARVProtocolByIdAsync(Guid protocolId)
        {
            throw new NotImplementedException();
        }
    }
}
