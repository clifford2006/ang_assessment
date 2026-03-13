using ANG_Assessment.DB;
using ANG_Assessment.DB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Globalization;

namespace ANG_Assessment.Controllers
{
    [ApiController]
    [Route("weather")]
    public class WeatherController(IServiceProvider serviceProvider, ILogger<WeatherController> logger) : ControllerBase
    {
        private readonly ILogger<WeatherController> _logger = logger;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        [HttpGet("forecast/{location}")]
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
    }
}
