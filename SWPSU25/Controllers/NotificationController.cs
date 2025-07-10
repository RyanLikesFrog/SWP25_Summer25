using DataLayer.DbContext;
using DataLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.DTOs.User.Request;
using ServiceLayer.Interfaces;
using SWPSU25.SignalRHubs;

namespace SWPSU25.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IHubContext<ReminderHub> _hubContext;

        public NotificationController(INotificationService notificationService, IHubContext<ReminderHub> hubContext)
        {
            _notificationService = notificationService;
            _hubContext = hubContext;
        }

        [HttpGet("get-notification-by-patientId")]
        public async Task<IActionResult> GetAllByPatientId(Guid patientId)
        {
            var notifications = await _notificationService.GetAllByPatientIdAsync(patientId);
            return Ok(notifications);
        }
        //
        [HttpGet("get-notification-by-id")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null) return NotFound();
            return Ok(notification);
        }
        //
        [HttpPost("create-notification")]
        public async Task<IActionResult> Create([FromBody] CreateNotificationRequest request)
        {
            try
            {
                var created = await _notificationService.CreateNotificationAsync(request);

                // Gửi signalR nếu cần
                await _hubContext.Clients.User(request.PatientId.ToString()).SendAsync("ReceiveReminder", new
                {
                    notificationId = created.NotificationId,
                    message = created.Message
                });

                return Ok(created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpPost("mark-as-seen")]
        public async Task<IActionResult> MarkAsSeen([FromQuery] Guid notificationId)
        {
            var success = await _notificationService.MarkAsSeenAsync(notificationId);
            if (!success) return NotFound();

            // Lấy lại notification để truy cập PatientId
            var notification = await _notificationService.GetNotificationByIdAsync(notificationId);
            if (notification == null) return NotFound();

            await _hubContext.Clients.All.SendAsync("NotificationSeen", new
            {
                notificationId,
                patientId = notification.PatientId
            });

            return Ok(new { message = "Notification marked as seen." });
        }

    }
}
