namespace ANG_Assessment.Models
{
    public class DataGovSGFourDayWeatherData
    {
        public int Code { get; set; }
        public string? Name { get; set; }
        public MainData? Data { get; set; }
        public string? ErrorMsg { get; set; }

        public class MainData
        {
            public required List<MainRecord> Records { get; set; }
        }

        public class MainRecord
        {
            public required List<ForecastData> Forecasts { get; set; }
        }

        public class ForecastData
        {
            public DateTimeOffset Timestamp { get; set; }
            public required TemperatureData Temperature { get; set; }
            public required HumidityData RelativeHumidity { get; set; }
            public required WeatherData Forecast { get; set; }
            public required WindData Wind { get; set; }
        }

        public class TemperatureData
        {
            public double Low { get; set; }
            public double High { get; set; }
            public required string Unit { get; set; }
        }

        public class HumidityData
        {
            public double Low { get; set; }
            public double High { get; set; }
            public required string Unit { get; set; }
        }

        public class WeatherData
        {
            public required string Summary { get; set; }
            public required string Code { get; set; }
            public required string Text { get; set; }
        }

        public class WindData
        {
            public required WindSpeedData Speed { get; set; }
            public required string Direction { get; set; }

            public class WindSpeedData
            {
                public double Low { get; set; }
                public double High { get; set; }
            }
        }
    }
}
