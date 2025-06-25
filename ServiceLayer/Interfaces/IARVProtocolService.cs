// File: ServiceLayer/Interfaces/IARVProtocolService.cs
using DataLayer.Entities; // Required for ARVProtocol entity and ARVProtocol?
using ServiceLayer.DTOs.User.Request; // Required for CreateARVProtocolRequest
using ServiceLayer.DTOs.User.Response; // Required for ARVProtocolDetailResponse

namespace ServiceLayer.Interfaces
{
    public interface IARVProtocolService
    {
        public Task<ARVProtocolDetailResponse?> CreateARVProtocolAsync(CreateARVProtocolRequest request);
        public Task<ARVProtocol?> GetARVProtocolByIdAsync(Guid protocolId);
        public Task<List<ARVProtocol?>> GetAllARVProtocolsAsync();
        public Task<List<ARVProtocol?>> GetDefaultARVProtocolsAsync();
    }
}