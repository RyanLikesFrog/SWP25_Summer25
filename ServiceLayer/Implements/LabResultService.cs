using DataLayer.Entities;
using Firebase.Storage;
using Microsoft.Extensions.Configuration;
using RepoLayer.Implements;
using RepoLayer.Interfaces;
using ServiceLayer.DTOs;
using ServiceLayer.Interfaces;
using ServiceLayer.Validator;
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
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IRepository _repository;
        private readonly ITreatmentStageRepository _treatmentStageRepository;
        private readonly IConfiguration _config;

        public LabResultService
            (
            ILabResultRepository labResultRepository, 
            IPatientRepository patientRepository,
            IDoctorRepository doctorRepository,
            IRepository repository,
            ITreatmentStageRepository treatmentStageRepository,
            IConfiguration config)
        {
            _labResultRepository = labResultRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _repository = repository;
            _treatmentStageRepository = treatmentStageRepository;
            _config = config;
        }

        public async Task<LabResultDetailResponse> CreateLabResultAsync(CreateLabResultRequest request)
        {
            if (request.TreatmentStageId == null)
            {
                throw new ArgumentException("TreatmentStageId is required.");
            }

            var treatmentStage = await _treatmentStageRepository.GetTreatmentStageByIdAsync(request.TreatmentStageId.Value);
            if (treatmentStage == null)
            {
                throw new ArgumentException($"TreatmentStage with ID {request.TreatmentStageId} not found.");
            }

            if (treatmentStage?.PatientTreatmentProtocol == null)
            {
                throw new InvalidOperationException("PatientTreatmentProtocol is missing from the treatment stage.");
            }

            if (request.PatientId != treatmentStage.PatientTreatmentProtocol.PatientId)
            {
                throw new ArgumentException($"Patient ID {request.PatientId} does not match the TreatmentStage's Patient ID {treatmentStage.PatientTreatmentProtocol.PatientId}.");
            }

            if (request.DoctorId != treatmentStage.PatientTreatmentProtocol.DoctorId)
            {
                throw new ArgumentException($"Doctor ID {request.DoctorId} does not match the TreatmentStage's Doctor ID {treatmentStage.PatientTreatmentProtocol.DoctorId}.");
            }

            var newLabResult = new LabResult
            {
                Id = Guid.NewGuid(),
                PatientId = request.PatientId,
                TreatmentStageId = request.TreatmentStageId,
                TestType = request.TestType,
                TestDate = request.TestDate,
                ResultSummary = request.ResultSummary,
                Notes = request.Notes,
                Conclusion = request.Conclusion,
                DoctorId = request.DoctorId,
                TestName = request.TestName,
            };

            var listpic = new List<LabPicture>();
            if (request.LabPictures != null && request.LabPictures.Count > 0)
            {
                foreach (var labPic in request.LabPictures)
                {
                    if (labPic.FileName.HasImageExtension())
                    {
                        string firebaseBucket = _config["Firebase:StorageBucket"];

                        // Initialize FirebaseStorage instance
                        var firebaseStorage = new FirebaseStorage(firebaseBucket);

                        // Generate a unique file name
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + labPic.FileName;

                        // Get reference to the file in Firebase Storage
                        var fileReference = firebaseStorage.Child("LabResults").Child(request.TestName).Child(uniqueFileName);

                        // Upload the file to Firebase Storage
                        using (var stream = labPic.OpenReadStream())
                        {
                            await fileReference.PutAsync(stream);
                        }

                        // Get the download URL of the uploaded file
                        string downloadUrl = await fileReference.GetDownloadUrlAsync();
                        listpic.Add(new LabPicture
                        {
                            Id = Guid.NewGuid(),
                            LabResultId = newLabResult.Id,
                            LabPictureUrl = downloadUrl,
                            LabPictureName = uniqueFileName
                        });
                    }
                    else
                    {
                        throw new Exception("Not support file type" + nameof(labPic.FileName).ToString());
                    }

                }
                newLabResult.LabPictures = listpic;
            }


            await _labResultRepository.CreateLabResultAsync(newLabResult);
            await _repository.SaveChangesAsync();

            return new LabResultDetailResponse
            {
                Id = newLabResult.Id,
                PatientId = newLabResult.PatientId,
                TreatmentStageId = newLabResult.TreatmentStageId,
                TestType = newLabResult.TestType,
                TestDate = newLabResult.TestDate,
                ResultSummary = newLabResult.ResultSummary,
                Notes = newLabResult.Notes,
                Conclusion = newLabResult.Conclusion,
                DoctorId = newLabResult.DoctorId,
                TestName = newLabResult.TestName
            };
        }

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
