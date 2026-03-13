using ANG_Assessment.DB;
using Microsoft.EntityFrameworkCore;

namespace ANG_Assessment.Services
{
    public class DatabaseMigrationService(AppDBContext db, ILogger<DatabaseMigrationService> logger)
    {
        private readonly AppDBContext _db = db;
        private readonly ILogger<DatabaseMigrationService> _logger = logger;

        public bool EnsureDatabaseUpToDate()
        {
            if (_db.Database.HasPendingModelChanges())
            {
                _logger.LogCritical("EF Core model has pending changes!");
                return false;
            }

            if (_db.Database.GetPendingMigrations().Any())
            {
                _logger.LogInformation("Pending migrations found. Applying migrations...");
                _db.Database.Migrate();
                _logger.LogInformation("Migrations applied successfully.");
            }

            return true;
        }
    }
}
