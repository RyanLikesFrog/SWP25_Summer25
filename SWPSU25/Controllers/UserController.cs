using DataLayer.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.DTOs.User.Request;
using ServiceLayer.Interfaces;

namespace SWPSU25.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // API chung cho Admin tạo tài khoản Doctor, Staff hoặc Manager
        [HttpPost("admin/create-account")]
        [Authorize(Roles = nameof(UserRole.Admin))] // Chỉ Admin mới có thể truy cập
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountByAdminRequest request)
        {
            // Thêm validation cấp controller nếu cần thiết,
            // nhưng phần lớn logic validation đã có trong service.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _userService.CreateUserAccountByAdminAsync(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("admin/update-account")]
        [Authorize(Roles = nameof(UserRole.Admin))] // Chỉ Admin mới có thể truy cập
        public async Task<IActionResult> UpdateUserUnified([FromBody] UpdateUserRequest request)
        {
            if (!ModelState.IsValid) // Kiểm tra Data Annotations validation
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.UpdateUserInformationAsync(request.UserId, request);

            if (!result.Success)
            {
                if (result.Message.Contains("không tìm thấy"))
                {
                    return NotFound(new { Message = result.Message });
                }
                if (result.Message.Contains("đã tồn tại"))
                {
                    return Conflict(new { Message = result.Message });
                }
                if (result.Message.Contains("hạ cấp vai trò"))
                {
                    return Forbid(result.Message); // Return 403 Forbidden
                }
                return StatusCode(500, new { Message = result.Message });
            }

            return Ok(new { Message = result.Message, UserId = result.User?.Id });
        }
    }
}
