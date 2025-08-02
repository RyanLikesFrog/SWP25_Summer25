using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Interfaces
{
    public interface IPrescriptionRepository
    {
        public Task CreatePrescriptionAsync(Prescription prescription);
        public Task<Prescription?> GetPrescriptionByIdAsync(Guid prescrptionId);
        public Task<Prescription?> GetPrescriptionsByMedicalRecordIdAsync(Guid medicalRecordId);
    }
}
