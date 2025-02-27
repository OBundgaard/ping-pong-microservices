
using Core.Helpers;
using Core.Interfaces.Repositories;
using Core.Models.PingService;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Trace;
using PingService.Repositories;
using Serilog;

namespace PingService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Repositories
            builder.Services.AddScoped<IRepositoryAll<Credentials>, CredentialRepository>();
            builder.Services.AddScoped<IRepository<Permissions>, PermissionRepository>();

            // DB Context
            builder.Services.AddDbContext<PingDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("PingServiceConnection"), sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null
                );
            }));

            // Base Services
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
