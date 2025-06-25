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
            DateTime now = DateTime.Now; // Thời điểm hiện tại
            DateTime today = now.Date;   // Ngày hiện tại
            DateTime futureDateLimit = today.AddDays(daysInFuture);

            // Lấy tất cả các giai đoạn điều trị đang Active
            // và có ReminderTimes được định nghĩa.
            var stagesToConsider = await _context.TreatmentStages
                                                .AsNoTracking()
                                                .Where(s => s.StartDate <= futureDateLimit &&
                                                           (s.EndDate == null || s.EndDate >= today) && // Giai đoạn đang diễn ra hoặc sẽ kết thúc trong tương lai
                                                           !string.IsNullOrWhiteSpace(s.ReminderTimes) && // Phải có thời gian nhắc nhở
                                                           s.Status == PatientTreatmentStatus.Active) // Chỉ các giai đoạn đang hoạt động/chờ
                                                .ToListAsync();

            List<ReminderDetailDto> calculatedReminders = new List<ReminderDetailDto>();
            foreach (var stage in stagesToConsider)
            {
                //Khởi tạo một danh sách rỗng để lưu các giờ nhắc nhở sau khi chuyển đổi từ chuỗi.
                List<TimeSpan> dailyReminderTimes = new List<TimeSpan>();

                //Kiểm tra xem stage.ReminderTimes có giá trị không rỗng, không null, và không toàn khoảng trắng.
                if (!string.IsNullOrWhiteSpace(stage.ReminderTimes))
                {
                    //Tách chuỗi đó theo dấu phẩy , thành từng phần tử (VD: "08:00, 12:30" → "08:00" và "12:30")
                    foreach (var timeStr in stage.ReminderTimes.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (TimeSpan.TryParse(timeStr.Trim(), out TimeSpan timeSpan))
                        {
                            dailyReminderTimes.Add(timeSpan);
                        }
                    }
                }

                if (!dailyReminderTimes.Any()) continue; // Bỏ qua nếu không có giờ nhắc nhở hợp lệ

                // Ngày bắt đầu lặp: không sớm hơn StartDate và không sớm hơn hôm nay
                DateTime iterationStartDate = stage.StartDate.Date > today ? stage.StartDate.Date : today;

                // Ngày kết thúc lặp: không muộn hơn EndDate và không muộn hơn futureDateLimit
                DateTime iterationEndDate = stage.EndDate.HasValue && stage.EndDate.Value.Date < futureDateLimit ? stage.EndDate.Value.Date : futureDateLimit;

                // Đảm bảo ngày bắt đầu không vượt quá ngày kết thúc
                if (iterationStartDate > iterationEndDate) continue;

                for (DateTime date = iterationStartDate; date <= iterationEndDate; date = date.AddDays(1))
                {
                    foreach (var time in dailyReminderTimes)
                    {
                        DateTime reminderDateTime = date.Add(time);

                        if (includePastFewMinutes)
                        {
                            // Cho Background Service: lấy các nhắc nhở trong khoảng (hiện tại - 1 tieng) đến (hiện tại + 1 tieng )
                            // Khoảng thời gian này có thể điều chỉnh để đảm bảo không bỏ sót nhắc nhở
                            if (reminderDateTime > now.AddHours(-1) && reminderDateTime <= now.AddHours(1))
                            {
                                calculatedReminders.Add(new ReminderDetailDto
                                {
                                    StageId = stage.Id,
                                    StageName = stage.StageName,
                                    StageNumber = stage.StageNumber,
                                    Description = stage.Description,
                                    ReminderDateTime = reminderDateTime,
                                    PatientTreatmentProtocolId = stage.PatientTreatmentProtocolId,
                                });
                            }
                        }
                        else // Cho API (Front-end): chỉ lấy các nhắc nhở trong tương lai
                        {
                            if (reminderDateTime > now)
                            {
                                calculatedReminders.Add(new ReminderDetailDto
                                {
                                    StageId = stage.Id,
                                    StageName = stage.StageName,
                                    StageNumber = stage.StageNumber,
                                    Description = stage.Description,
                                    ReminderDateTime = reminderDateTime,
                                    PatientTreatmentProtocolId = stage.PatientTreatmentProtocolId,
                                });
                            }
                        }
                    }
                }
            }
            return calculatedReminders.OrderBy(r => r.ReminderDateTime).ToList();
        }
        /// <summary>
        /// Lấy các cuộc hẹn cần nhắc nhở (trước 1 ngày so với AppointmentStartDate).
        /// </summary>
        /// <returns>Danh sách AppointmentReminderDto.</returns>
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

            // Bạn có thể thêm logic kiểm tra xem reminder đã được gửi cho ngày này chưa
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
