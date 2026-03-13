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
                var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();

                if (db.Database.HasPendingModelChanges())
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogCritical("EF Core model has pending changes!");

                    return;
                }
                else if (db.Database.GetPendingMigrations().Any())
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("Pending migrations found. Applying migrations...");
                    db.Database.Migrate();
                    logger.LogInformation("Migrations applied successfully.");
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
