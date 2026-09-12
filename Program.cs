using System.Text;
using Event_parking.BackgroundServices;
using Event_parking.Configurations;
using Event_parking.Data;
using Event_parking.Helpers;
using Event_parking.Repositories.Implementations;
using Event_parking.Repositories.Interfaces;
using Event_parking.Services.Implementations;
using Event_parking.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ======================================
        // CONTROLLERS
        // ======================================

        builder.Services.AddControllers();

        // ======================================
        // SWAGGER
        // ======================================

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter your JWT token.",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                }
            );

            options.AddSecurityRequirement(
                document =>
                    new OpenApiSecurityRequirement
                    {
                        [
                            new OpenApiSecuritySchemeReference(
                                "Bearer",
                                document
                            )
                        ] = []
                    }
            );
        });

        // ======================================
        // DATABASE CONNECTION
        // ======================================

        string? connectionString =
            builder.Configuration.GetConnectionString(
                "DefaultConnection"
            );

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "DefaultConnection is missing."
            );
        }

        builder.Services.AddDbContext<ApplicationDbContext>(
            options =>
            {
                options.UseSqlServer(connectionString);
            }
        );

        // ======================================
        // CONFIGURATION CLASSES
        // ======================================

        builder.Services.Configure<JwtSettings>(
            builder.Configuration.GetSection(
                JwtSettings.SectionName
            )
        );

        builder.Services.Configure<EmailSettings>(
            builder.Configuration.GetSection(
                EmailSettings.SectionName
            )
        );

        builder.Services.Configure<BookingSettings>(
            builder.Configuration.GetSection(
                BookingSettings.SectionName
            )
        );

        // ======================================
        // READ JWT SETTINGS
        // ======================================

        JwtSettings jwtSettings =
            builder.Configuration
                .GetSection(JwtSettings.SectionName)
                .Get<JwtSettings>()
            ??
            throw new InvalidOperationException(
                "JwtSettings configuration is missing."
            );

        if (string.IsNullOrWhiteSpace(jwtSettings.Key))
        {
            throw new InvalidOperationException(
                "JWT Key is missing."
            );
        }

        if (jwtSettings.Key.Length < 32)
        {
            throw new InvalidOperationException(
                "JWT Key must contain at least 32 characters."
            );
        }

        // ======================================
        // JWT AUTHENTICATION
        // ======================================

        builder.Services
            .AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme =
                        JwtBearerDefaults.AuthenticationScheme;

                    options.DefaultChallengeScheme =
                        JwtBearerDefaults.AuthenticationScheme;

                    options.DefaultScheme =
                        JwtBearerDefaults.AuthenticationScheme;
                }
            )
            .AddJwtBearer(
                options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;

                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,

                            ValidIssuer =
                                jwtSettings.Issuer,

                            ValidAudience =
                                jwtSettings.Audience,

                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(
                                        jwtSettings.Key
                                    )
                                ),

                            ClockSkew =
                                TimeSpan.Zero
                        };

                    options.Events =
                        new JwtBearerEvents
                        {
                            OnAuthenticationFailed =
                                context =>
                                {
                                    Console.WriteLine(
                                        "================================"
                                    );

                                    Console.WriteLine(
                                        "JWT AUTHENTICATION FAILED"
                                    );

                                    Console.WriteLine(
                                        "ERROR TYPE: " +
                                        context.Exception
                                            .GetType()
                                            .Name
                                    );

                                    Console.WriteLine(
                                        "ERROR MESSAGE: " +
                                        context.Exception
                                            .Message
                                    );

                                    Console.WriteLine(
                                        "================================"
                                    );

                                    return Task.CompletedTask;
                                }
                        };
                }
            );

        // ======================================
        // AUTHORIZATION
        // ======================================

        builder.Services.AddAuthorization();

        // ======================================
        // REPOSITORIES
        // ======================================

        builder.Services.AddScoped<
            ICustomerRepository,
            CustomerRepository
        >();

        builder.Services.AddScoped<
            IVehicleRepository,
            VehicleRepository
        >();

        builder.Services.AddScoped<
            ICategoryRepository,
            CategoryRepository
        >();

        builder.Services.AddScoped<
            IVenueRepository,
            VenueRepository
        >();

        builder.Services.AddScoped<
            IEventRepository,
            EventRepository
        >();

        builder.Services.AddScoped<
            ISeatRepository,
            SeatRepository
        >();

        builder.Services.AddScoped<
            ISeatSectionRepository,
            SeatSectionRepository
        >();

        builder.Services.AddScoped<
            IParkingRepository,
            ParkingRepository
        >();

        builder.Services.AddScoped<
            IBookingRepository,
            BookingRepository
        >();

        builder.Services.AddScoped<
            IPaymentRepository,
            PaymentRepository
        >();

        builder.Services.AddScoped<
            INotificationRepository,
            NotificationRepository
        >();

        builder.Services.AddScoped<
            IAdminDashboardRepository,
            AdminDashboardRepository
        >();

        // ======================================
        // SERVICES
        // ======================================

        builder.Services.AddScoped<
            IAuthService,
            AuthService
        >();

        builder.Services.AddScoped<
            ICustomerService,
            CustomerService
        >();

        builder.Services.AddScoped<
            IVehicleService,
            VehicleService
        >();

        builder.Services.AddScoped<
            IEmailService,
            EmailService
        >();

        builder.Services.AddScoped<
            ICategoryService,
            CategoryService
        >();

        builder.Services.AddScoped<
            IVenueService,
            VenueService
        >();

        builder.Services.AddScoped<
            IEventService,
            EventService
        >();

        builder.Services.AddScoped<
            ISeatService,
            SeatService
        >();

        builder.Services.AddScoped<
            ISeatSectionService,
            SeatSectionService
        >();

        builder.Services.AddScoped<
            IParkingService,
            ParkingService
        >();

        builder.Services.AddScoped<
            IBookingService,
            BookingService
        >();

        builder.Services.AddScoped<
            IPaymentService,
            PaymentService
        >();

        builder.Services.AddScoped<
            INotificationService,
            NotificationService
        >();

        builder.Services.AddScoped<
            IAdminDashboardService,
            AdminDashboardService
        >();

        // ======================================
        // HELPERS
        // ======================================

        builder.Services.AddScoped<
            PasswordHelper
        >();

        builder.Services.AddScoped<
            JwtHelper
        >();

        builder.Services.AddScoped<
            BookingNumberGenerator
        >();

        // ======================================
        // BOOKING EXPIRY BACKGROUND SERVICE
        // ======================================

        builder.Services.AddHostedService<
            BookingExpiryService
        >();

        // ======================================
        // CORS
        // ======================================

        builder.Services.AddCors(
            options =>
            {
                options.AddPolicy(
                    "FrontendPolicy",
                    policy =>
                    {
                        policy
                            .WithOrigins(
                                "http://localhost:4200",
                                "https://localhost:4200",
                                "http://localhost:5500",
                                "https://localhost:5500"
                            )
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    }
                );
            }
        );

        // ======================================
        // BUILD APPLICATION
        // ======================================

        var app = builder.Build();

        // ======================================
        // DEVELOPMENT CONFIGURATION
        // ======================================

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();

            app.UseSwaggerUI(
                options =>
                {
                    options.SwaggerEndpoint(
                        "/swagger/v1/swagger.json",
                        "Event Parking API V1"
                    );

                    options.RoutePrefix =
                        "swagger";
                }
            );
        }

        // ======================================
        // HTTP PIPELINE
        // ======================================

        app.UseHttpsRedirection();

        // Allows files inside wwwroot to be opened
        // Example:
        // https://localhost:7168/uploads/events/image.jpg
        app.UseStaticFiles();

        app.UseCors(
            "FrontendPolicy"
        );

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}