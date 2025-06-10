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
    public class LabResultService : ILabResultService
    {
        private readonly ILabResultRepository _labResultRepository;
        public async Task<List<LabResult?>> GetAllLabResultsAsync()
        {
            return await _labResultRepository.GetAllLabResultsAsync();
        }

        public async Task<LabResult?> GetLabResultByIdAsync(Guid labResultId)
        {
            return await _labResultRepository.GetLabResultByIdAsync(labResultId);
        }
    }
}
