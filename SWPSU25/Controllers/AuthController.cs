using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.DTOs.Auth;
using ServiceLayer.DTOs.Patient.Request;
using ServiceLayer.Interfaces;

namespace SWPSU25.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.LoginAsync(request);

            if (!response.Success)
            {
                return Unauthorized(response);
            }

            return Ok(response);
        }
        [HttpPost("register-patient")] // <-- Thêm endpoint này
        public async Task<IActionResult> RegisterPatient([FromBody] PatientRegisterRequest request)
        {
            // Kiểm tra các ràng buộc từ model (Data Annotations)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.RegisterPatientAsync(request);
            if (response.Success)
            {
                return Ok(response); // Đăng ký thành công
            }
            // Nếu không thành công (ví dụ: username/email đã tồn tại)
            return BadRequest(response);
        }
    }
}
