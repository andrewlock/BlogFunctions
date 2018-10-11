using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BlogFunctions
{
    public static class CallBuildWebhook
    {
        [FunctionName("CallBuildWebhook")]
        public static async Task RunAsync([TimerTrigger("0 15 10 * * 2")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Tiggering Blog rebuild at: {DateTime.Now}");

            await TriggerBuild();

            log.LogInformation($"Blog rebuild triggered successfully at: {DateTime.Now}");
        }

        static async Task TriggerBuild()
        {
            var buildHookBaseUrl = Environment.GetEnvironmentVariable("BuildHookBaseUrl");
            var path = Environment.GetEnvironmentVariable("BuildHookPath");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(buildHookBaseUrl);

                await client.PostAsJsonAsync(path, "{}");
            }
        }
    }
}
