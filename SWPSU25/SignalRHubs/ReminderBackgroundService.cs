using DataLayer.DbContext;
using DataLayer.Entities;
using DataLayer.Enum;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.Implements.Reminder;

namespace SWPSU25.SignalRHubs
{
    public class ReminderBackgroundService : BackgroundService
    {
        private readonly ILogger<ReminderBackgroundService> _logger;
        private readonly IServiceScopeFactory _scopeFactory; // Để tạo scope cho các services có vòng đời scoped (như DbContext)
        private readonly IHubContext<ReminderHub> _hubContext; // Để gửi tin nhắn SignalR

        public ReminderBackgroundService(
            ILogger<ReminderBackgroundService> logger,
            IServiceScopeFactory scopeFactory,
            IHubContext<ReminderHub> hubContext)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Reminder Background Service is starting.");

            // Chờ 5 giây trước khi bắt đầu vòng lặp kiểm tra đầu tiên
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope()) // Tạo scope mới cho mỗi lần chạy
                    {
                        var reminderService = scope.ServiceProvider.GetRequiredService<ReminderService>();

                        var dbContext = scope.ServiceProvider.GetRequiredService<SWPSU25Context>();

                        // Lấy các reminders trong khoảng thời gian rất gần (ví dụ: trong 1 ngày tới, bao gồm cả những cái vừa trôi qua)
                        var potentialReminders = await reminderService.GetCalculatedRemindersAsync(1, true);


                        foreach (var reminder in potentialReminders)
                        {
                            // Lấy TreatmentStage từ DB để kiểm tra và cập nhật `LastDailyReminderSentDate`
                            // Sử dụng FindAsync để lấy theo khóa chính một cách hiệu quả
                            var stage = await dbContext.TreatmentStages
                                                                        .Include(s => s.PatientTreatmentProtocol)
                                                                        .ThenInclude(ptp => ptp.Patient) 
                                                                        .Include(s => s.PrescriptionItems) 
                                                                        .FirstOrDefaultAsync(s => s.Id == reminder.StageId);
                            var patientId = stage.PatientTreatmentProtocol?.PatientId;
                            // Điều kiện để gửi reminder:
                            // 1. Giai đoạn phải tồn tại và đang ở trạng thái Active
                            // 2. Thời điểm nhắc nhở nằm trong "cửa sổ gửi" (ví dụ: từ 30 giây trước đến 30 giây sau thời điểm hiện tại)
                            // 3. CHƯA được gửi cho ngày hiện tại (kiểm tra `LastDailyReminderSentDate`)
                            if (stage != null &&
                                stage.Status == PatientTreatmentStatus.Active && // Chỉ gửi nhắc nhở cho giai đoạn đang hoạt động
                                reminder.ReminderDateTime > DateTime.Now.AddSeconds(-30) && // Đã đến hạn hoặc vừa trôi qua trong 30s
                                reminder.ReminderDateTime <= DateTime.Now.AddSeconds(30) &&
                                patientId != null) // Đến hạn trong 30s tới
                            {
                                _logger.LogInformation($"--- Sending reminder for Stage: '{reminder.StageName}' (ID: {reminder.StageId}) at {reminder.ReminderDateTime} ---");

                                // Gửi thông báo qua SignalR đến TẤT CẢ các client đang kết nối
                                // "ReceiveReminder" là tên phương thức/event mà client sẽ lắng nghe
                                //

                                // 1. Tạo Notification trước khi gửi
                                var notification = new Notification
                                {
                                    NotificationId = Guid.NewGuid(),
                                    PatientId = patientId.Value,
                                    TreatmentStageId = stage.Id,
                                    CreatedAt = DateTime.Now,
                                    IsSeen = false,
                                    Message = "Nhắc nhở uống thuốc: " + string.Join(", ",stage.PrescriptionItems.Select(p => $"{p.DrugName} {p.Dosage}"))
                                };

                                dbContext.Notifications.Add(notification);
                                await dbContext.SaveChangesAsync(stoppingToken);

                                // 2. Gửi SignalR riêng cho bệnh nhân đó
                                await _hubContext.Clients
                                    .User(patientId.ToString()) // Bệnh nhân nhận riêng
                                    .SendAsync("ReceiveReminder", new
                                    {
                                        notificationId = notification.NotificationId,
                                        message = notification.Message,
                                        reminder // thông tin bổ sung từ ReminderDetailDto
                                    }, stoppingToken);

                                _logger.LogInformation($"Reminder sent to patient {patientId}, notificationId = {notification.NotificationId}");

                                //

                                await dbContext.SaveChangesAsync(stoppingToken); // Lưu thay đổi vào DB
                                _logger.LogInformation($"Reminder for Stage '{reminder.StageName}' marked as sent for {reminder.ReminderDateTime.Date.ToShortDateString()}.");
                            }
                        }
                        var dueAppointments = await reminderService.GetDueAppointmentRemindersAsync();

                        foreach (var appointmentReminder in dueAppointments)
                        {
                            // Lấy lại entity Appointment để cập nhật trạng thái
                            var appointment = await dbContext.Appointments.FindAsync(appointmentReminder.AppointmentId);

                            // Kiểm tra:
                            // 1. Cuộc hẹn tồn tại
                            // 2. Trạng thái là Scheduled hoặc Pending
                            // 3. Thanh toán Pending hoặc AwaitingPayment
                            // 4. CHƯA được gửi reminder cho ngày hôm nay (tránh gửi trùng mỗi 30s)
                            if (appointment != null &&
                                appointment.Status == AppointmentStatus.Confirmed &&
                                appointment.PaymentStatus == PaymentStatus.Paid)
                            {
                                _logger.LogInformation($"--- Sending Appointment Reminder for: '{appointment.AppointmentTitle}' (ID: {appointment.Id}) for {appointment.AppointmentStartDate.ToShortDateString()} ---");

                                appointmentReminder.ReminderSentTime = DateTime.Now; // Cập nhật thời gian gửi thực tế
                                await _hubContext.Clients.User(appointment.PatientId.ToString()) // hoặc DoctorId nếu bác sĩ cần nhận
                                                         .SendAsync("ReceiveAppointmentReminder", appointmentReminder, stoppingToken);


                                await dbContext.SaveChangesAsync(stoppingToken);
                                _logger.LogInformation($"Appointment Reminder for '{appointment.AppointmentTitle}' marked as sent for {DateTime.Now.Date.ToShortDateString()}.");
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in Reminder Background Service.");
                }

                // Chờ 30 giây trước khi kiểm tra lại
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }

            _logger.LogInformation("Reminder Background Service is stopping.");
        }
    }
}
