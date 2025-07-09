using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SWPSU25.SignalRHubs;
using DataLayer.DbContext;

namespace SWPSU25.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly SWPSU25Context _context;
        private readonly IHubContext<ReminderHub> _hubContext;

        public NotificationController(SWPSU25Context context, IHubContext<ReminderHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost("mark-as-seen")]
        public async Task<IActionResult> MarkAsSeen(Guid notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null) return NotFound();

            notification.IsSeen = true;
            notification.SeenAt = DateTime.Now;
            await _context.SaveChangesAsync();

            // Gửi thông báo cho Staff biết rằng bệnh nhân đã đọc
            await _hubContext.Clients.All.SendAsync("NotificationSeen", new
            {
                notificationId,
                patientId = notification.PatientId
            });

            return Ok(new { message = "Notification marked as seen." });
        }
    }
}
