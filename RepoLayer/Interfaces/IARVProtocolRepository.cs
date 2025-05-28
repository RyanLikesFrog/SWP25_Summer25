using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Interfaces
{
    public interface IARVProtocolRepository
    {
        public Task<ARVProtocol?> GetARVProtocolByIdAsync(Guid protocolId);
        public Task<List<ARVProtocol>> GetAllARVProtocolsAsync();

    }
}
