using ANG_Assessment.Controllers;
using ANG_Assessment.DB;
using ANG_Assessment.DB.DBUtil;
using ANG_Assessment.DB.Models;
using ANG_Assessment.Services;

namespace ANG_Assessment.Tasks
{
    public class WeatherUpdaterTask(IServiceProvider serviceProvider, ILogger<WeatherController> logger) : BackgroundService
    {
        private readonly ILogger<WeatherController> _logger = logger;
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly TimeSpan _delay = TimeSpan.FromMinutes(30);

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation($"Running {nameof(WeatherUpdaterTask)}");

                    using var scope = _serviceProvider.CreateScope();
                    var dataGovSGServices = scope.ServiceProvider.GetRequiredService<DataGovSGServices>();
                    var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();
                    var weatherDBUtil = new WeatherDBUtil(db);

                    List<DateTime> processDTs = [DateTime.Now.AddDays(-4), DateTime.Now];

                    foreach (var processDT in processDTs)
                    {
                        var data = await dataGovSGServices.GetFourDayWeatherAsync(processDT);
                        if (data != null && data.Data != null)
                        {
                            foreach (var record in data.Data.Records)
                            {
                                foreach (var forecast in record.Forecasts)
                                {
                                    var newRecord = new WeatherRecord
                                    {
                                        Location = "Singapore",
                                        TemperatureLow = forecast.Temperature.Low,
                                        TemperatureHigh = forecast.Temperature.High,
                                        HumidityLow = forecast.RelativeHumidity.Low,
                                        HumidityHigh = forecast.RelativeHumidity.High,
                                        WindSpeedLow = forecast.Wind.Speed.Low,
                                        WindSpeedHigh = forecast.Wind.Speed.High,
                                        WindDirection = forecast.Wind.Direction,
                                        WeatherDesc = forecast.Forecast.Text,
                                        RecordDate = forecast.Timestamp.DateTime,
                                    };
                                    await weatherDBUtil.UpsertWeatherRecordAsync(newRecord, cancellationToken);
                                }
                            }
                        }
                    }

                    _logger.LogInformation($"Completed {nameof(WeatherUpdaterTask)}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"{nameof(WeatherUpdaterTask)} Failed");
                }

                await Task.Delay(_delay, cancellationToken);
            }
        }
    }
}
