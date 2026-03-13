using ANG_Assessment.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace ANG_Assessment.DB.DBUtil
{
    public class WeatherDBUtil(AppDBContext db)
    {
        private readonly AppDBContext _db = db;

        public async Task UpsertWeatherRecordAsync(WeatherRecord weatherRecord, CancellationToken cancellationToken = default)
        {
            var existing = await _db.WeatherRecords
                .FirstOrDefaultAsync(w => w.Location == weatherRecord.Location
                                       && w.RecordDate.Date == weatherRecord.RecordDate.Date, cancellationToken);

            if (existing != null)
            {
                existing.TemperatureLow = weatherRecord.TemperatureLow;
                existing.TemperatureHigh = weatherRecord.TemperatureHigh;
                existing.HumidityLow = weatherRecord.HumidityLow;
                existing.HumidityHigh = weatherRecord.HumidityHigh;
                existing.WindSpeedLow = weatherRecord.WindSpeedLow;
                existing.WindSpeedHigh = weatherRecord.WindSpeedHigh;
                existing.WindDirection = weatherRecord.WindDirection;
                existing.WeatherDesc = weatherRecord.WeatherDesc;
                existing.LastUpdateTime = DateTime.Now;
            }
            else
            {
                weatherRecord.LastUpdateTime = DateTime.Now;
                await _db.WeatherRecords.AddAsync(weatherRecord, cancellationToken);
            }

            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
