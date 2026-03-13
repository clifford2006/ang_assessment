using Microsoft.EntityFrameworkCore;

namespace ANG_Assessment.DB.Models
{
    public class WeatherRecord : IModelConfiguration
    {
        public int Id { get; set; }
        public required string Location { get; set; }
        public double TemperatureLow { get; set; }
        public double TemperatureHigh { get; set; }
        public double HumidityLow { get; set; }
        public double HumidityHigh { get; set; }
        public double WindSpeedLow { get; set; }
        public double WindSpeedHigh { get; set; }
        public string WindDirection { get; set; } = string.Empty;
        public string WeatherDesc { get; set; } = string.Empty;
        public DateTime RecordDate { get; set; }
        public DateTime LastUpdateTime { get; set; }

        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherRecord>(entity =>
            {
            });
        }
    }
}
