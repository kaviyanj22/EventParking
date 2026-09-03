using System.Net;
using Event_parking.Configurations;
using Event_parking.DTOs.Auth;
using Event_parking.Helpers;
using Event_parking.Models;
using Event_parking.Repositories.Interfaces;
using Event_parking.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Event_parking.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly ICustomerRepository
            _customerRepository;

        private readonly IEmailService
            _emailService;

        private readonly PasswordHelper
            _passwordHelper;

        private readonly JwtHelper
            _jwtHelper;

        private readonly EmailSettings
            _emailSettings;

        public AuthService(
            ICustomerRepository customerRepository,
            IEmailService emailService,
            PasswordHelper passwordHelper,
            JwtHelper jwtHelper,
            IOptions<EmailSettings> emailOptions)
        {
            _customerRepository =
                customerRepository;

            _emailService =
                emailService;

            _passwordHelper =
                passwordHelper;

            _jwtHelper =
                jwtHelper;

            _emailSettings =
                emailOptions.Value;
        }

        // ======================================
        // REGISTER
        // ======================================

        public async Task<ServiceResult<object>>
            RegisterAsync(
                RegisterRequestDto registerDto)
        {
            string normalizedEmail =
                registerDto.Email
                    .Trim()
                    .ToLower();

            Customer? existingCustomer =
                await _customerRepository
                    .GetByEmailAsync(normalizedEmail);

            if (existingCustomer != null)
            {
                return ServiceResult<object>.Fail(
                    "This email address is already registered."
                );
            }

            string verificationToken =
                TokenHelper.CreateToken();

            string verificationTokenHash =
                TokenHelper.HashToken(
                    verificationToken
                );

            var customer = new Customer
            {
                FullName =
                    registerDto.FullName.Trim(),

                Email = normalizedEmail,

                Phone =
                    registerDto.Phone.Trim(),

                Role = "Customer",

                Status = "Active",

                EmailVerified = false,

                EmailVerificationTokenHash =
                    verificationTokenHash,

                EmailVerificationTokenExpiresAt =
                    DateTime.UtcNow.AddHours(24),

                CreatedAt = DateTime.UtcNow
            };

            customer.PasswordHash =
                _passwordHelper.HashPassword(
                    customer,
                    registerDto.Password
                );

            await _customerRepository
                .AddAsync(customer);

            await _customerRepository
                .SaveChangesAsync();

            string encodedToken =
                WebUtility.UrlEncode(
                    verificationToken
                );

            string verificationLink =
                $"{_emailSettings.FrontendBaseUrl
                    .TrimEnd('/')}" +
                $"/verify-email.html?token={encodedToken}";

            string emailBody = $"""
                <h2>Event Parking Account Verification</h2>

                <p>
                    Hello
                    {WebUtility.HtmlEncode(customer.FullName)},
                </p>

                <p>
                    Your Event Parking account was
                    registered successfully.
                </p>

                <p>
                    Click the link below to verify
                    your email address:
                </p>

                <p>
                    <a href="{verificationLink}">
                        Verify Email Address
                    </a>
                </p>

                <p>
                    This verification link expires
                    in 24 hours.
                </p>
                """;

            await _emailService.SendEmailAsync(
                customer.Email,
                "Verify your Event Parking account",
                emailBody
            );

            return ServiceResult<object>.Ok(
                null,
                "Registration successful. " +
                "Check your email to verify the account."
            );
        }

        // ======================================
        // LOGIN
        // ======================================

        public async Task<
            ServiceResult<AuthResponseDto>>
            LoginAsync(
                LoginRequestDto loginDto)
        {
            string normalizedEmail =
                loginDto.Email
                    .Trim()
                    .ToLower();

            Customer? customer =
                await _customerRepository
                    .GetByEmailAsync(normalizedEmail);

            if (customer == null)
            {
                return ServiceResult<AuthResponseDto>
                    .Fail(
                        "Invalid email or password."
                    );
            }

            bool passwordCorrect =
                _passwordHelper.VerifyPassword(
                    customer,
                    customer.PasswordHash,
                    loginDto.Password
                );

            if (!passwordCorrect)
            {
                return ServiceResult<AuthResponseDto>
                    .Fail(
                        "Invalid email or password."
                    );
            }

            if (!string.Equals(
                    customer.Status,
                    "Active",
                    StringComparison.OrdinalIgnoreCase))
            {
                return ServiceResult<AuthResponseDto>
                    .Fail(
                        "This account is deactivated."
                    );
            }

            if (!customer.EmailVerified)
            {
                return ServiceResult<AuthResponseDto>
                    .Fail(
                        "Verify your email before logging in."
                    );
            }

            var generatedToken =
                _jwtHelper.GenerateToken(customer);

            var authResponse =
                new AuthResponseDto
                {
                    Token =
                        generatedToken.Token,

                    ExpiresAt =
                        generatedToken.ExpiresAt,

                    CustomerId =
                        customer.CustomerId,

                    FullName =
                        customer.FullName,

                    Email =
                        customer.Email,

                    Role =
                        customer.Role
                };

            return ServiceResult<AuthResponseDto>
                .Ok(
                    authResponse,
                    "Login successful."
                );
        }

        // ======================================
        // VERIFY EMAIL
        // ======================================

        public async Task<ServiceResult<object>>
            VerifyEmailAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return ServiceResult<object>.Fail(
                    "Verification token is required."
                );
            }

            string tokenHash =
                TokenHelper.HashToken(token);

            Customer? customer =
                await _customerRepository
                    .GetByVerificationTokenHashAsync(
                        tokenHash
                    );

            if (customer == null)
            {
                return ServiceResult<object>.Fail(
                    "Verification token is invalid."
                );
            }

            if (
                customer
                    .EmailVerificationTokenExpiresAt
                    == null
                ||
                customer
                    .EmailVerificationTokenExpiresAt
                    <= DateTime.UtcNow
            )
            {
                return ServiceResult<object>.Fail(
                    "Verification token has expired."
                );
            }

            if (customer.EmailVerified)
            {
                return ServiceResult<object>.Fail(
                    "Email address is already verified."
                );
            }

            customer.EmailVerified = true;

            customer.EmailVerificationTokenHash =
                null;

            customer.EmailVerificationTokenExpiresAt =
                null;

            customer.UpdatedAt = DateTime.UtcNow;

            await _customerRepository
                .SaveChangesAsync();

            return ServiceResult<object>.Ok(
                null,
                "Email verified successfully."
            );
        }

        // ======================================
        // RESEND EMAIL VERIFICATION
        // ======================================

        public async Task<ServiceResult<object>>
            ResendVerificationAsync(
                ResendVerificationDto resendDto)
        {
            string normalizedEmail =
                resendDto.Email
                    .Trim()
                    .ToLower();

            Customer? customer =
                await _customerRepository
                    .GetByEmailAsync(normalizedEmail);

            // Generic response prevents email discovery.
            if (
                customer == null
                ||
                customer.EmailVerified
            )
            {
                return ServiceResult<object>.Ok(
                    null,
                    "If the account requires verification, " +
                    "a new email has been sent."
                );
            }

            string verificationToken =
                TokenHelper.CreateToken();

            customer.EmailVerificationTokenHash =
                TokenHelper.HashToken(
                    verificationToken
                );

            customer.EmailVerificationTokenExpiresAt =
                DateTime.UtcNow.AddHours(24);

            customer.UpdatedAt = DateTime.UtcNow;

            await _customerRepository
                .SaveChangesAsync();

            string encodedToken =
                WebUtility.UrlEncode(
                    verificationToken
                );

            string verificationLink =
                $"{_emailSettings.FrontendBaseUrl
                    .TrimEnd('/')}" +
                $"/verify-email.html?token={encodedToken}";

            string emailBody = $"""
                <h2>Verify your email address</h2>

                <p>
                    Click the link below to verify
                    your Event Parking account:
                </p>

                <p>
                    <a href="{verificationLink}">
                        Verify Email Address
                    </a>
                </p>

                <p>
                    This verification link expires
                    in 24 hours.
                </p>
                """;

            await _emailService.SendEmailAsync(
                customer.Email,
                "Verify your Event Parking account",
                emailBody
            );

            return ServiceResult<object>.Ok(
                null,
                "If the account requires verification, " +
                "a new email has been sent."
            );
        }

        // ======================================
        // FORGOT PASSWORD
        // ======================================

        public async Task<ServiceResult<object>>
            ForgotPasswordAsync(
                ForgotPasswordDto forgotPasswordDto)
        {
            string normalizedEmail =
                forgotPasswordDto.Email
                    .Trim()
                    .ToLower();

            Customer? customer =
                await _customerRepository
                    .GetByEmailAsync(normalizedEmail);

            /*
             Always return the same message.

             This prevents another person from
             checking whether an email exists.
            */

            if (customer != null)
            {
                string passwordResetToken =
                    TokenHelper.CreateToken();

                customer.PasswordResetTokenHash =
                    TokenHelper.HashToken(
                        passwordResetToken
                    );

                customer.PasswordResetTokenExpiresAt =
                    DateTime.UtcNow.AddMinutes(60);

                customer.UpdatedAt =
                    DateTime.UtcNow;

                await _customerRepository
                    .SaveChangesAsync();

                string encodedToken =
                    WebUtility.UrlEncode(
                        passwordResetToken
                    );

                string resetPasswordLink =
                    $"{_emailSettings.FrontendBaseUrl
                        .TrimEnd('/')}" +
                    "/reset-password.html" +
                    $"?token={encodedToken}";

                string emailBody = $"""
                    <h2>Reset your password</h2>

                    <p>
                        A password reset was requested
                        for your Event Parking account.
                    </p>

                    <p>
                        Click the link below:
                    </p>

                    <p>
                        <a href="{resetPasswordLink}">
                            Reset Password
                        </a>
                    </p>

                    <p>
                        This link expires in 60 minutes.
                    </p>

                    <p>
                        If you did not request this,
                        you can ignore this email.
                    </p>
                    """;

                await _emailService.SendEmailAsync(
                    customer.Email,
                    "Reset your Event Parking password",
                    emailBody
                );
            }

            return ServiceResult<object>.Ok(
                null,
                "If the email is registered, " +
                "a password reset link has been sent."
            );
        }

        // ======================================
        // RESET PASSWORD
        // ======================================

        public async Task<ServiceResult<object>>
            ResetPasswordAsync(
                ResetPasswordDto resetPasswordDto)
        {
            if (string.IsNullOrWhiteSpace(
                    resetPasswordDto.Token))
            {
                return ServiceResult<object>.Fail(
                    "Password reset token is required."
                );
            }

            string tokenHash =
                TokenHelper.HashToken(
                    resetPasswordDto.Token
                );

            Customer? customer =
                await _customerRepository
                    .GetByResetTokenHashAsync(
                        tokenHash
                    );

            if (customer == null)
            {
                return ServiceResult<object>.Fail(
                    "Password reset token is invalid."
                );
            }

            if (
                customer.PasswordResetTokenExpiresAt
                    == null
                ||
                customer.PasswordResetTokenExpiresAt
                    <= DateTime.UtcNow
            )
            {
                return ServiceResult<object>.Fail(
                    "Password reset token has expired."
                );
            }

            customer.PasswordHash =
                _passwordHelper.HashPassword(
                    customer,
                    resetPasswordDto.NewPassword
                );

            // Token becomes single-use after reset.
            customer.PasswordResetTokenHash = null;

            customer.PasswordResetTokenExpiresAt = null;

            customer.UpdatedAt = DateTime.UtcNow;

            await _customerRepository
                .SaveChangesAsync();

            return ServiceResult<object>.Ok(
                null,
                "Password reset successfully."
            );
        }
    }
}