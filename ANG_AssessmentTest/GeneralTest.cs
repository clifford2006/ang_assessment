using ANG_Assessment.DB;
using ANG_Assessment.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace ANG_AssessmentTest
{
    public class GeneralTest
    {
        [Fact]
        public void DBMigrationTest()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseSqlite(connection)
                .Options;

            var db = new AppDBContext(options);

            var loggerMock = new Mock<ILogger<DatabaseMigrationService>>();
            var service = new DatabaseMigrationService(db, loggerMock.Object);

            Assert.True(service.EnsureDatabaseUpToDate());
        }
    }
}