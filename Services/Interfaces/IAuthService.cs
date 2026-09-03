using Event_parking.DTOs.Auth;

namespace Event_parking.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult<object>> RegisterAsync(
            RegisterRequestDto registerDto
        );

        Task<ServiceResult<AuthResponseDto>> LoginAsync(
            LoginRequestDto loginDto
        );

        Task<ServiceResult<object>> VerifyEmailAsync(
            string token
        );

        Task<ServiceResult<object>>
            ResendVerificationAsync(
                ResendVerificationDto resendDto
            );

        Task<ServiceResult<object>>
            ForgotPasswordAsync(
                ForgotPasswordDto forgotPasswordDto
            );

        Task<ServiceResult<object>>
            ResetPasswordAsync(
                ResetPasswordDto resetPasswordDto
            );
    }
}