using DataLayer.Entities;
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
        public Task<List<LabResult>>? GetAllLabResultsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<LabResult?> GetLabResultByIdAsync(Guid labResultId)
        {
            throw new NotImplementedException();
        }
    }
}
