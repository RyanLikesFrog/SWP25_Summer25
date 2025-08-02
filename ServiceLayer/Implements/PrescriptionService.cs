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
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IPrescriptionItemRepository _prescriptionItemRepository;

        public PrescriptionService(IPrescriptionRepository prescriptionRepository, IPrescriptionItemRepository prescriptionItemRepository)
        {
            _prescriptionRepository = prescriptionRepository;
            _prescriptionItemRepository = prescriptionItemRepository;
        }

        public async Task<Prescription> GetPrescriptionByIdAsync(Guid prescriptionId)
        {
            var prescription = await _prescriptionRepository.GetPrescriptionByIdAsync(prescriptionId);
            if(prescription == null)
            {
                throw new ArgumentException($"Không tìm thấy đơn thuốc với ID {prescriptionId}.");
            }

            return prescription;
        }
    }
}
