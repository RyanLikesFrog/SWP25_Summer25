using DataLayer.Entities;
using RepoLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Implements
{
    public class LabResultRepository : ILabResultRepository
    {
        public Task<List<LabResult>> GetAllLabResultsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<LabResult?> GetLabResultByIdAsync(Guid labResultId)
        {
            throw new NotImplementedException();
        }
    }
}
