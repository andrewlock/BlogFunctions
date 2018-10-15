using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BlogFunctions
{
    public static class CallBuildWebhook
    {
        private static HttpClient _client = new HttpClient();

        [FunctionName("CallBuildWebhook")]
        public static async Task RunAsync([TimerTrigger("0 15 10 * * 2")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Tiggering Blog rebuild at: {DateTime.Now}");

            var buildHookUrl = Environment.GetEnvironmentVariable("BuildHookUrl");

            await _client.PostAsJsonAsync(buildHookUrl, "{}");

            log.LogInformation($"Blog rebuild triggered successfully at: {DateTime.Now}");
        }
    }
}
