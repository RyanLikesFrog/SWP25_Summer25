using DataLayer.DbContext;
using DataLayer.Enum;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.DTOs.Patient.Response;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Implements.Reminder
{
    public class ReminderService
    {
        private readonly SWPSU25Context _context;

        public ReminderService(SWPSU25Context context)
        {
            _context = context;
        }

        /// <summary>
        /// Tính toán và lấy danh sách các thời điểm nhắc nhở cho các giai đoạn điều trị đang hoạt động.
        /// </summary>
        /// <param name="daysInFuture">Số ngày trong tương lai muốn tính toán nhắc nhở.</param>
        /// <param name="includePastFewMinutes">Nếu true, sẽ bao gồm các nhắc nhở đã trôi qua trong vài phút gần đây (cho Background Service).</param>
        /// <returns>Danh sách các ReminderDetailDto đã sắp xếp theo thời gian.</returns>
        public async Task<List<ReminderDetailDto>> GetCalculatedRemindersAsync(int daysInFuture = 7, bool includePastFewMinutes = false)
        {
            DateTime now = DateTime.Now;
            DateTime today = now.Date;
            DateTime futureDateLimit = today.AddDays(daysInFuture);

            var stagesToConsider = await _context.TreatmentStages
                .AsNoTracking()
                .Include(ts => ts.MedicalRecords) // Include Prescription
                    .ThenInclude(p => p.Prescription) // Include PrescriptionItems
                .Where(s => s.StartDate <= futureDateLimit &&
                            (s.EndDate == null || s.EndDate >= today) &&
                            !string.IsNullOrWhiteSpace(s.ReminderTimes) &&
                            s.Status == PatientTreatmentStatus.Active)
                .ToListAsync();

            List<ReminderDetailDto> calculatedReminders = new List<ReminderDetailDto>();

            foreach (var stage in stagesToConsider)
            {
                // Parse ReminderTimes to TimeSpans
                List<TimeSpan> dailyReminderTimes = stage.ReminderTimes?
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => TimeSpan.TryParse(t.Trim(), out var ts) ? ts : (TimeSpan?)null)
                    .Where(ts => ts.HasValue)
                    .Select(ts => ts.Value)
                    .ToList() ?? new();

                if (!dailyReminderTimes.Any()) continue;

                DateTime iterationStartDate = stage.StartDate.Date > today ? stage.StartDate.Date : today;
                DateTime iterationEndDate = stage.EndDate.HasValue && stage.EndDate.Value.Date < futureDateLimit
                                            ? stage.EndDate.Value.Date : futureDateLimit;

                if (iterationStartDate > iterationEndDate) continue;

                var prescriptionItems = stage.MedicalRecords
                    .Where(mr => mr.Prescription != null)
                    .SelectMany(mr => mr.Prescription.Items)
                    .ToList();
                if (prescriptionItems == null || !prescriptionItems.Any()) continue;

                for (DateTime date = iterationStartDate; date <= iterationEndDate; date = date.AddDays(1))
                {
                    foreach (var time in dailyReminderTimes)
                    {
                        DateTime reminderDateTime = date.Add(time);

                        bool isValidTime = includePastFewMinutes
                            ? (reminderDateTime > now.AddHours(-1) && reminderDateTime <= now.AddHours(1))
                            : (reminderDateTime > now);

                        if (!isValidTime) continue;

                        foreach (var item in prescriptionItems)
                        {
                            calculatedReminders.Add(new ReminderDetailDto
                            {
                                StageId = stage.Id,
                                StageName = stage.StageName,
                                StageNumber = stage.StageNumber,
                                Description = stage.Description,
                                ReminderDateTime = reminderDateTime,
                                PatientTreatmentProtocolId = stage.PatientTreatmentProtocolId,
                                PrescriptionItemId = item.Id,
                                DrugName = item.DrugName,
                                Dosage = item.Dosage,
                                Frequency = item.Frequency,
                            });
                        }
                    }
                }
            }

            return calculatedReminders.OrderBy(r => r.ReminderDateTime).ToList();
        }

        public async Task<List<AppointmentReminderDto>> GetDueAppointmentRemindersAsync()
        {
            DateTime now = DateTime.Now;
            DateTime tomorrow = now.Date.AddDays(1); // Ngày mai, đầu ngày

            // Lấy các cuộc hẹn có AppointmentStartDate là ngày mai
            // và chưa được thanh toán hoặc thanh toán đang chờ xử lý
            var appointments = await _context.Appointments
                .AsNoTracking()
                .Where(a => a.AppointmentStartDate.Date == tomorrow &&
                            (a.Status == AppointmentStatus.Confirmed || a.PaymentStatus == PaymentStatus.Paid)) // Chỉ nhắc nhở các cuộc hẹn đã lên lịch
                .Select(a => new AppointmentReminderDto
                {
                    AppointmentId = a.Id,
                    AppointmentTitle = a.AppointmentTitle,
                    AppointmentStartDate = a.AppointmentStartDate,
                    PatientName = a.Patient != null ? a.Patient.FullName : "Anonymous Patient", // Lấy tên bệnh nhân nếu có
                    DoctorName = a.Doctor != null ? a.Doctor.FullName : "N/A", // Lấy tên bác sĩ nếu có
                    OnlineLink = a.OnlineLink,
                    ReminderSentTime = DateTime.MinValue, // Sẽ được cập nhật khi gửi
                    ReminderMessage = $"Reminder: Your appointment '{a.AppointmentTitle}' is scheduled for tomorrow at {a.AppointmentStartDate.ToShortTimeString()}." // Tin nhắn nhắc nhở
                })
                .ToListAsync();

            // Có thể thêm logic kiểm tra xem reminder đã được gửi cho ngày này chưa
            // Hiện tại, tôi sẽ giả định rằng chúng ta gửi một lần vào ngày trước đó.
            // Để tránh gửi trùng, bạn cần thêm một trường LastAppointmentReminderSentDate vào Appointment entity
            // và kiểm tra nó ở đây.

            // Để đơn giản, giả sử nhắc nhở sẽ gửi vào một giờ cố định trong ngày (ví dụ: 9 AM ngày hôm trước)
            // Hoặc bạn có thể thiết lập nhiều giờ nhắc nhở cho cuộc hẹn nếu cần.
            // Tuy nhiên, yêu cầu là "trước 1 ngày", nên việc kiểm tra `AppointmentStartDate.Date == tomorrow`
            // là đủ cho logic lấy cuộc hẹn.
            // Việc kiểm tra "đã gửi chưa" sẽ được xử lý trong BackgroundService.

            return appointments;
        }
    }
}
