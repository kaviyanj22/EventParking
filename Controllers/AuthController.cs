using Event_parking.DTOs.Auth;
using Event_parking.Services;
using Event_parking.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Event_parking.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(
            IAuthService authService)
        {
            _authService = authService;
        }

        // POST: /api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequestDto registerDto)
        {
            ServiceResult<object> result =
                await _authService
                    .RegisterAsync(registerDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return StatusCode(
                StatusCodes.Status201Created,
                result
            );
        }

        // POST: /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequestDto loginDto)
        {
            ServiceResult<AuthResponseDto> result =
                await _authService
                    .LoginAsync(loginDto);

            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        // GET: /api/auth/verify-email?token=xxxx
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(
            [FromQuery] string token)
        {
            ServiceResult<object> result =
                await _authService
                    .VerifyEmailAsync(token);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        // POST: /api/auth/resend-verification
        [HttpPost("resend-verification")]
        public async Task<IActionResult>
            ResendVerification(
                [FromBody]
                ResendVerificationDto resendDto)
        {
            ServiceResult<object> result =
                await _authService
                    .ResendVerificationAsync(resendDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        // POST: /api/auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult>
            ForgotPassword(
                [FromBody]
                ForgotPasswordDto forgotPasswordDto)
        {
            ServiceResult<object> result =
                await _authService
                    .ForgotPasswordAsync(
                        forgotPasswordDto
                    );

            return Ok(result);
        }

        // POST: /api/auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult>
            ResetPassword(
                [FromBody]
                ResetPasswordDto resetPasswordDto)
        {
            ServiceResult<object> result =
                await _authService
                    .ResetPasswordAsync(
                        resetPasswordDto
                    );

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}