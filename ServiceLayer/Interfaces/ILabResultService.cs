﻿using DataLayer.Entities;
using ServiceLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface ILabResultService
    {
        public Task<LabResult?> GetLabResultByIdAsync(Guid labResultId);
        public Task<List<LabResult?>> GetAllLabResultsAsync();
        public Task<LabResultDetailResponse> CreateLabResultAsync(CreateLabResultRequest request);
    }
}
