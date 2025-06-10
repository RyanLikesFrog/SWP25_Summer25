using Azure.Core;
using DataLayer.Entities;
using RepoLayer.Interfaces;
using ServiceLayer.DTOs.User.Request;
using ServiceLayer.DTOs.User.Response;
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
        private readonly IRepository _repository;

        public ARVProtocolService(
            IARVProtocolRepository aRVProtocolRepository,
            IRepository repository)
        {
            _aRVProtocolRepository = aRVProtocolRepository;
            _repository = repository;
        }

        public async Task<ARVProtocolDetailResponse?> CreateARVProtocolAsync(CreateARVProtocolRequest request)
        {
            var newProtocol = new ARVProtocol
            {
                ProtocolId = Guid.NewGuid(),
                ProtocolName = request.ProtocolName,
                Description = request.Description,
                Indications = request.Indications,
                Dosage = request.Dosage,
                SideEffects = request.SideEffects,
                IsDefault = request.IsDefault,
                ProtocolType = request.ProtocolType,
            };

            await _aRVProtocolRepository.CreateARVProtocolAsync(newProtocol);
            await _repository.SaveChangesAsync();

            return new ARVProtocolDetailResponse
            {
                ProtocolId = newProtocol.ProtocolId,
                ProtocolName = newProtocol.ProtocolName,
                Description = newProtocol.Description,
                Indications = newProtocol.Indications,
                Dosage = newProtocol.Dosage,
                SideEffects = newProtocol.SideEffects,
                IsDefault = newProtocol.IsDefault,
                ProtocolType = newProtocol.ProtocolType,
            };
        }

        public async Task<List<ARVProtocol?>> GetAllARVProtocolsAsync()
        {
            return await _aRVProtocolRepository.GetAllARVProtocolsAsync();
        }

        public async Task<ARVProtocol?> GetARVProtocolByIdAsync(Guid protocolId)
        {
            return await _aRVProtocolRepository.GetARVProtocolByIdAsync(protocolId);
        }

        public async Task<List<ARVProtocol?>> GetDefaultARVProtocolsAsync()
        {
            return await _aRVProtocolRepository.GetDefaultProtocolAsync();
        }


    }
}
