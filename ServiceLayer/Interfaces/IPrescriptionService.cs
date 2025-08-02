using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface IPrescriptionService
    {
        public Task<Prescription> GetPrescriptionByIdAsync (Guid prescriptionId);
        public Task<Prescription> GetPrescriptionsByMedicalRecordIdAsync(Guid medicalRecordId);
    }
}
