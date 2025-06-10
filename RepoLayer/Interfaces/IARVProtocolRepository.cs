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
        public Task <ARVProtocol?> CreateARVProtocolAsync(ARVProtocol arvProto);
        public Task<ARVProtocol?> GetARVProtocolByIdAsync(Guid protocolId);
        public Task<List<ARVProtocol?>> GetAllARVProtocolsAsync();
        public Task<List<ARVProtocol?>> GetDefaultProtocolAsync();

    }
}
