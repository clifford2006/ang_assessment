using ANG_Assessment.Controllers;
using ANG_Assessment.DB;
using ANG_Assessment.DB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace ANG_AssessmentTest
{
    public class WeatherControllerTest
    {
        [Fact]
        public async Task GetWeatherAsync_InvalidDateTest()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            var logger = new Mock<ILogger<WeatherController>>();

            var controller = new WeatherController(serviceProvider.Object, logger.Object);

            var result = await controller.GetWeatherAsync("Singapore", "invalid-date");

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invalid date format. Use yyyy-MM-dd.", badRequest.Value);
        }

        [Fact]
        public async Task GetWeatherAsync_RecordNotExistTest()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseSqlite(connection)
                .Options;

            using var context = new AppDBContext(options);
            context.Database.EnsureCreated();

            var date = DateTime.Today;
            var location = "Singapore";

            var scope = new Mock<IServiceScope>();
            var scopeFactory = new Mock<IServiceScopeFactory>();
            var provider = new Mock<IServiceProvider>();

            scope.Setup(x => x.ServiceProvider).Returns(new ServiceCollection()
                .AddSingleton(context)
                .BuildServiceProvider());

            scopeFactory.Setup(x => x.CreateScope()).Returns(scope.Object);

            provider.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(scopeFactory.Object);

            var logger = new Mock<ILogger<WeatherController>>();

            var controller = new WeatherController(provider.Object, logger.Object);

            var result = await controller.GetWeatherAsync(location, date.ToString("yyyy-MM-dd"));

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal($"No data found for {date:yyyy-MM-dd} in {location}", notFound.Value);
        }

        [Fact]
        public async Task GetWeatherAsync_RecordExistTest()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseSqlite(connection)
                .Options;

            using var context = new AppDBContext(options);
            context.Database.EnsureCreated();

            var date = DateTime.Today;
            var location = "Singapore";

            context.WeatherRecords.Add(new WeatherRecord
            {
                Location = location,
                RecordDate = date
            });

            context.SaveChanges();

            var scope = new Mock<IServiceScope>();
            var scopeFactory = new Mock<IServiceScopeFactory>();
            var provider = new Mock<IServiceProvider>();

            scope.Setup(x => x.ServiceProvider).Returns(new ServiceCollection()
                .AddSingleton(context)
                .BuildServiceProvider());

            scopeFactory.Setup(x => x.CreateScope()).Returns(scope.Object);

            provider.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(scopeFactory.Object);

            var logger = new Mock<ILogger<WeatherController>>();

            var controller = new WeatherController(provider.Object, logger.Object);

            var result = await controller.GetWeatherAsync(location, date.ToString("yyyy-MM-dd"));

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsType<WeatherRecord>(ok.Value);

            Assert.Equal(location, data.Location);
        }
    }
}
