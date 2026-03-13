using ANG_Assessment.DB;
using ANG_Assessment.Services;
using ANG_Assessment.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ANG_Assessment
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHttpClient();
            builder.Services.AddScoped<DatabaseMigrationService>();
            builder.Services.AddSingleton<DataGovSGServices>();
            builder.Services.AddHostedService<WeatherUpdaterTask>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDBContext>(options =>
            {
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
                options.EnableSensitiveDataLogging(false);
            });
            builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var migrationService = scope.ServiceProvider.GetRequiredService<DatabaseMigrationService>();
                if (!migrationService.EnsureDatabaseUpToDate())
                {
                    return;
                }
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
