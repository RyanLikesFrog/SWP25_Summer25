// File: ServiceLayer/Interfaces/IARVProtocolService.cs
using DataLayer.Entities; // Required for ARVProtocol entity and ARVProtocol?
using ServiceLayer.DTOs.User.Request; // Required for CreateARVProtocolRequest
using ServiceLayer.DTOs.User.Response; // Required for ARVProtocolDetailResponse
using ServiceLayer.Requests;
using System; // Required for Guid
using System.Collections.Generic; // Required for List<T>
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface IARVProtocolService
    {
        public Task<ARVProtocolDetailResponse?> CreateARVProtocolAsync(CreateARVProtocolRequest request);
        public Task<ARVProtocol?> UpdateARVProtocolAsync(UpdateARVProtocolRequest request);
        public Task<ARVProtocol?> GetARVProtocolByIdAsync(Guid protocolId);
        public Task<List<ARVProtocol?>> GetAllARVProtocolsAsync();
        public Task<List<ARVProtocol?>> GetDefaultARVProtocolsAsync();
    }
}