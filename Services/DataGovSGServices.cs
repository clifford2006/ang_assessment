using ANG_Assessment.Models;
using System.Text.Json;

namespace ANG_Assessment.Services
{
    public class DataGovSGServices(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public async Task<DataGovSGFourDayWeatherData?> GetFourDayWeatherAsync(DateTime? dateTime = null)
        {
            var client = _httpClientFactory.CreateClient();
            var baseUrl = configuration["DataGovSGAPI:FourDayWeatherUrl"] ?? throw new NullReferenceException("Unconfigured DataGovSGAPI:FourDayWeatherUrl");
            var uriBuilder = new UriBuilder(baseUrl);
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
            query["date"] = $"{(dateTime == null ? DateTime.Now : dateTime):yyyy-MM-dd}";
            uriBuilder.Query = query.ToString();

            var response = await client.GetAsync(uriBuilder.Uri);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<DataGovSGFourDayWeatherData>(json, _jsonOptions);
        }
    }
}
