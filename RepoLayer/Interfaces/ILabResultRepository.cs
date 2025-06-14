using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Interfaces
{
    public interface ILabResultRepository
    {

        public Task<LabResult?> GetLabResultByIdAsync(Guid labResultId);
        public Task<List<LabResult?>> GetAllLabResultsAsync();
        public Task<LabResult?> CreateLabResultAsync(LabResult labResult);
    }
}
