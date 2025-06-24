using Microsoft.AspNetCore.Mvc;
using ServiceLayer.DTOs.Patient.Response;
using ServiceLayer.Implements.Reminder;

namespace SWPSU25.Controllers
{
    [Route("api/reminder")]
    [ApiController]
    public class RemindersController : ControllerBase
    {
        private readonly ReminderService _reminderService;

        public RemindersController(ReminderService reminderService)
        {
            _reminderService = reminderService;
        }

        /// <summary>
        /// </summary>
        /// <param name="futureDays">Số ngày trong tương lai muốn lấy reminder (mặc định 7 ngày).</param>
        /// <returns>Danh sách các ReminderDetailDto đã sắp xếp theo thời gian.</returns>
        [HttpGet("upcomingReminderForDrinkMedicine")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ReminderDetailDto>>> GetUpcomingTreatmentStageMedicineReminders([FromQuery] int futureDays = 7)
        {
            if (futureDays <= 0 || futureDays > 365)
            {
                return BadRequest("`futureDays` must be a positive number and not exceed 365.");
            }

            var reminders = await _reminderService.GetCalculatedRemindersAsync(futureDays, false);
            return Ok(reminders);
        }
        /// <summary>
        /// </summary>
        /// <param name="futureDays">Số ngày trong tương lai muốn lấy reminder (mặc định 7 ngày).</param>
        /// <returns>Danh sách các ReminderDetailDto đã sắp xếp theo thời gian.</returns>
        [HttpGet("upcomingAppointment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<AppointmentReminderDto>>> GetUpcomingAppointmentReminders()
        {
            var reminders = await _reminderService.GetDueAppointmentRemindersAsync();
            return Ok(reminders);
        }

    }
}
