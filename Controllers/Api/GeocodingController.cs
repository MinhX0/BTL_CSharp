using System.Globalization;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers.Api
{
    [ApiController]
    [Route("api/geocode")]
    public class GeocodingController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<GeocodingController> _logger;

        public GeocodingController(IHttpClientFactory httpClientFactory, ILogger<GeocodingController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // GET /api/geocode/reverse?lat=...&lon=...&lang=vi
        [HttpGet("reverse")]
        public async Task<IActionResult> Reverse([FromQuery] double lat, [FromQuery] double lon, [FromQuery] string? lang = "vi")
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var url =
                    $"https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat={lat.ToString(CultureInfo.InvariantCulture)}&lon={lon.ToString(CultureInfo.InvariantCulture)}&addressdetails=1&accept-language={Uri.EscapeDataString(lang ?? "vi")}";

                var req = new HttpRequestMessage(HttpMethod.Get, url);
                // Identify your app per Nominatim policy: https://operations.osmfoundation.org/policies/nominatim/
                req.Headers.UserAgent.Add(new ProductInfoHeaderValue("BTL_CSharp-ASPNETCORE", "1.0"));
                // Replace with a real contact address (policy requirement)
                req.Headers.From = "vlam21176@gmail.com";

                var res = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
                var body = await res.Content.ReadAsStringAsync();

                if (!res.IsSuccessStatusCode)
                    return StatusCode((int)res.StatusCode, body);

                return Content(body, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reverse geocoding failed for lat={Lat}, lon={Lon}", lat, lon);
                return Problem("Reverse geocoding failed.");
            }
        }
    }
}