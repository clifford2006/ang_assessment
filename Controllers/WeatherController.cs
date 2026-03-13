using ANG_Assessment.DB;
using ANG_Assessment.DB.Models;
using ANG_Assessment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;

namespace ANG_Assessment.Controllers
{
    [ApiController]
    [Route("weather")]
    public class WeatherController(IServiceProvider serviceProvider, ILogger<WeatherController> logger) : ControllerBase
    {
        private readonly ILogger<WeatherController> _logger = logger;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        [HttpGet("retrieve/{location}")]
        public async Task<ActionResult<WeatherRecord>> GetWeatherAsync([FromRoute, DefaultValue("Singapore")] string location, [FromQuery] string? date)
        {
            DateTime queryDate;

            if (!string.IsNullOrEmpty(date))
            {
                if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out queryDate))
                {
                    return BadRequest("Invalid date format. Use yyyy-MM-dd.");
                }
            }
            else
            {
                queryDate = DateTime.Now;
            }

            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var data = await db.WeatherRecords
                .Where(x => x.Location == location && x.RecordDate.Date == queryDate.Date)
                .FirstOrDefaultAsync();
            if (data != null)
            {
                return Ok(data);
            }
            else
            {
                return NotFound($"No data found for {queryDate:yyyy-MM-dd} in {location}");
            }
        }

        [HttpGet("export/{location}")]
        public async Task<IActionResult> ExportCsv([FromRoute, DefaultValue("Singapore")] string location, [FromQuery] string from, [FromQuery] string to)
        {
            if (!DateTime.TryParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime queryFromDate))
            {
                return BadRequest("Invalid date format. Use yyyy-MM-dd.");
            }
            if (!DateTime.TryParseExact(to, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime queryToDate))
            {
                return BadRequest("Invalid date format. Use yyyy-MM-dd.");
            }

            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();

            var records = await db.WeatherRecords
                .Where(x => x.Location == location && x.RecordDate.Date >= queryFromDate.Date && x.RecordDate.Date <= queryToDate.Date)
                .OrderBy(x => x.RecordDate)
                .ToListAsync();

            if (records.Count > 0)
            {
                var csv = new StringBuilder();

                csv.AppendLine("Location,Date,Weather Desc,Temp Low,Temp High,Humidity Low,Humidity High,Wind Low,Wind High,Wind Direction");
                foreach (var r in records)
                {
                    csv.AppendLine($"{r.Location},{r.RecordDate:yyyy-MM-dd},{r.WeatherDesc},{r.TemperatureLow},{r.TemperatureHigh},{r.HumidityLow},{r.HumidityHigh},{r.WindSpeedLow},{r.WindSpeedHigh},{r.WindDirection}");
                }

                var bytes = Encoding.UTF8.GetBytes(csv.ToString());

                return File(bytes, "text/csv", $"Weather_{location}.csv");
            }
            else
            {
                return NotFound($"No data found between {from:yyyy-MM-dd} and {to:yyyy-MM-dd} in {location}");
            }
        }

        [HttpPost("subscribe/{location}")]
        public async Task<IActionResult> Subscribe([FromRoute, DefaultValue("Singapore")] string location, [FromBody] SubscribeRequest subscribeRequest)
        {
            if (string.IsNullOrWhiteSpace(subscribeRequest.Email))
            {
                return BadRequest("Email is required.");
            }
            var emailValidator = new EmailAddressAttribute();
            if (!emailValidator.IsValid(subscribeRequest.Email))
            {
                return BadRequest("Invalid email format.");
            }

            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();

            var existing = await db.Set<AlertSubscribe>()
                .FirstOrDefaultAsync(x => x.SubscribeEmail == subscribeRequest.Email && x.Location == location);

            if (existing != null)
            {
                return Conflict("Subscription already exists.");
            }

            var subscribe = new AlertSubscribe
            {
                SubscribeEmail = subscribeRequest.Email,
                Location = location,
                SubscribeDate = DateTime.UtcNow
            };

            await db.AddAsync(subscribe);
            await db.SaveChangesAsync();

            return Ok($"Subscribed {subscribeRequest.Email} to {location} successfully.");
        }

        [HttpPost("unsubscribe/{location}")]
        public async Task<IActionResult> Unsubscribe([FromRoute, DefaultValue("Singapore")] string location, [FromBody] SubscribeRequest subscribeRequest)
        {
            if (string.IsNullOrWhiteSpace(subscribeRequest.Email))
            {
                return BadRequest("Email is required.");
            }
            var emailValidator = new EmailAddressAttribute();
            if (!emailValidator.IsValid(subscribeRequest.Email))
            {
                return BadRequest("Invalid email format.");
            }

            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();

            var existing = await db.Set<AlertSubscribe>()
                .FirstOrDefaultAsync(x => x.SubscribeEmail == subscribeRequest.Email && x.Location == location);

            if (existing == null)
            {
                return NotFound("Subscription not found.");
            }

            db.Remove(existing);
            await db.SaveChangesAsync();

            return Ok($"Unsubscribed {subscribeRequest.Email} from {location} successfully.");
        }
    }
}
