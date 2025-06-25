using DataLayer.Entities;
using Firebase.Storage;
using Microsoft.Extensions.Configuration;
using RepoLayer.Implements;
using RepoLayer.Interfaces;
using ServiceLayer.DTOs;
using ServiceLayer.DTOs.LabResult.Request;
using ServiceLayer.DTOs.User.Request;
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
        private readonly ILabPictureRepository _labPictureRepository;

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
            if (request.LabResultPictures != null && request.LabResultPictures.Length > 0)
            {
                foreach (var labPic in request.LabResultPictures)
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
                PatientId = newLabResult.PatientId.Value,
                DoctorId = newLabResult.DoctorId.Value,
                TreatmentStageId = newLabResult.TreatmentStageId,
                TestType = newLabResult.TestType,
                TestDate = newLabResult.TestDate,
                ResultSummary = newLabResult.ResultSummary,
                Notes = newLabResult.Notes,
                Conclusion = newLabResult.Conclusion,
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

        public async Task<LabResultDetailResponse> UpdateLabResultAsync(UpdateLabResultRequest request)
        {
            // Step 1: Get existing LabResult
            var labResult = await _labResultRepository.GetLabResultByIdAsync(request.LabResultId);
            if (labResult == null)
                throw new ArgumentException($"❌ LabResult with ID {request.LabResultId} not found.");

            // Step 2: Validate foreign keys
            var patient = await _patientRepository.GetPatientByIdAsync(request.PatientId);
            if (patient == null)
                throw new ArgumentException($"❌ Patient with ID {request.PatientId} not found.");

            var doctor = await _doctorRepository.GetDoctorByIdAsync(request.DoctorId);
            if (doctor == null)
                throw new ArgumentException($"❌ Doctor with ID {request.DoctorId} not found.");

            if (request.TreatmentStageId.HasValue)
            {
                var stage = await _treatmentStageRepository.GetTreatmentStageByIdAsync(request.TreatmentStageId.Value);
                if (stage == null)
                    throw new ArgumentException($"❌ TreatmentStage with ID {request.TreatmentStageId.Value} not found.");
            }

            // Step 3: Update LabResult fields
            labResult.PatientId = request.PatientId;
            labResult.DoctorId = request.DoctorId;
            labResult.TreatmentStageId = request.TreatmentStageId;
            labResult.TestName = request.TestName?.Trim();
            labResult.TestType = request.TestType?.Trim();
            labResult.TestDate = request.TestDate;
            labResult.ResultSummary = request.ResultSummary?.Trim();
            labResult.Conclusion = request.Conclusion?.Trim();
            labResult.Notes = request.Notes?.Trim();

            // Step 4: Upload new lab pictures if provided
            if (request.LabResultPictures != null && request.LabResultPictures.Count > 0)
            {
                string firebaseBucket = _config["Firebase:StorageBucket"];
                var firebaseStorage = new FirebaseStorage(firebaseBucket);

                for (int i = 0; i < request.LabResultPictures.Count; i++)
                {
                    var file = request.LabResultPictures[i];
                    if (file != null && file.FileName.HasImageExtension())
                    {
                        string uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                        var fileRef = firebaseStorage.Child("LabResults").Child(uniqueFileName);

                        using (var stream = file.OpenReadStream())
                        {
                            await fileRef.PutAsync(stream);
                        }

                        string downloadUrl = await fileRef.GetDownloadUrlAsync();

                        var labPicture = new LabPicture
                        {
                            Id = Guid.NewGuid(),
                            LabResultId = labResult.Id,
                            LabPictureName = request.LabPictureNames?.ElementAtOrDefault(i) ?? file.FileName,
                            LabPictureUrl = downloadUrl,
                            isActive = request.LabPictureIsActiveFlags?.ElementAtOrDefault(i) ?? true
                        };

                        labResult.LabPictures.Add(labPicture);
                        await _labPictureRepository.AddLabPictureAsync(labPicture);
                    }
                    else
                    {
                        throw new Exception($"❌ Unsupported or missing file: {file?.FileName ?? "null"}");
                    }
                }
            }

            // Step 5: Save changes
            await _labResultRepository.UpdateLabResultAsync(labResult);
            await _repository.SaveChangesAsync();

            // Step 6: Return detailed response
            return new LabResultDetailResponse
            {
                Id = labResult.Id,
                PatientId = labResult.PatientId.Value,
                DoctorId = labResult.DoctorId.Value,
                TreatmentStageId = labResult.TreatmentStageId,
                TestName = labResult.TestName,
                TestType = labResult.TestType,
                TestDate = labResult.TestDate,
                ResultSummary = labResult.ResultSummary,
                Conclusion = labResult.Conclusion,
                Notes = labResult.Notes,
                LabPictures = labResult.LabPictures.ToList()
            };
        }


    }
}
