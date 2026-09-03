using Event_parking.Repositories.Implementations;
using Event_parking.Repositories.Interfaces;
using Event_parking.Services.Implementations;
using Event_parking.Services.Interfaces;
namespace Event_parking;

   

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            // ======================================
            // MEMBER 3 - REPOSITORIES
            // ======================================

            builder.Services.AddScoped<ISeatRepository, SeatRepository>();
            builder.Services.AddScoped<IParkingRepository, ParkingRepository>();

            // ======================================
            // MEMBER 3 - SERVICES
            // ======================================

            builder.Services.AddScoped<ISeatService, SeatService>();
            builder.Services.AddScoped<IParkingService, ParkingService>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }

