using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace BlogFunctions
{
    public static class ClearCloudflareCache
    {
        private static HttpClient _client = new HttpClient
        {
            BaseAddress = new Uri("https://api.cloudflare.com"),
        };

        [FunctionName("ClearCloudflareCache")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("Clear Cloudflare cache triggered");

            var zoneId = Environment.GetEnvironmentVariable("ZoneId");
            var xAuthEmail = Environment.GetEnvironmentVariable("X-Auth-Email");
            var xAuthKey = Environment.GetEnvironmentVariable("X-Auth-Key");

            var path = $"/client/v4/zones/{zoneId}/purge_cache";
            var body = new { purge_everything = true };

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Add("X-Auth-Email", xAuthEmail);
            _client.DefaultRequestHeaders.Add("X-Auth-Key", xAuthKey);

            var response = await _client.PostAsJsonAsync(path, body);

            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                log.LogInformation("Cloudflare response: " + content);
                return new OkObjectResult("Cloudflare CDN Cleared");
            }

            log.LogError("Cloudflare rejected clear-cache request {StatusCode}: {Reason}", response.StatusCode, content);
            return new BadRequestObjectResult("Cloudflare rejected clear-cache request");
        }
    }
}
