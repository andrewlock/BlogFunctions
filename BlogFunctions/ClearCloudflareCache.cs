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
        [FunctionName("ClearCloudflareCache")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("Clear Cloudflare cache triggered");

            return await CallCloudflareToClearCache(req, log);
        }

        static async Task<IActionResult> CallCloudflareToClearCache(HttpRequest req, ILogger log)
        {
            var zoneId = Environment.GetEnvironmentVariable("ZoneId");
            var xAuthEmail = Environment.GetEnvironmentVariable("X-Auth-Email");
            var xAuthKey = Environment.GetEnvironmentVariable("X-Auth-Key");

            var path = $"/client/v4/zones/{zoneId}/purge_cache";
            var body = new { purge_everything = true };
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.cloudflare.com");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("X-Auth-Email", xAuthEmail);
                client.DefaultRequestHeaders.Add("X-Auth-Key", xAuthKey);

                var response = await client.PostAsJsonAsync(path, body);

                if (response.IsSuccessStatusCode)
                {
                    log.LogInformation("Cloudflare response: " + await response.Content.ReadAsStringAsync());
                    return new OkObjectResult("Cloudflare CDN Cleared");
                }

                return new BadRequestObjectResult("Cloudflare rejected clear-cache request");
            }
        }
    }
}