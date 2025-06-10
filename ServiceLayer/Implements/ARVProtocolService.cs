using DataLayer.Entities;
using RepoLayer.Interfaces;
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
        private readonly IARVProtocolRepository _aRVProtocolRepository;

        public async Task<List<ARVProtocol?>> GetAllARVProtocolsAsync()
        {
            return await _aRVProtocolRepository.GetAllARVProtocolsAsync();
        }

        public async Task<ARVProtocol?> GetARVProtocolByIdAsync(Guid protocolId)
        {
            return await _aRVProtocolRepository.GetARVProtocolByIdAsync(protocolId);
        }
    }
}
