using DataMatrix.Web.HttpClients.Clients.Api;
using DataMatrix.Web.HttpClients.Interfaces;
using DataMatrix.Web.Models.Settings;
using Polly;
using Polly.Extensions.Http;

namespace DataMatrix.Web.Extensions
{
    public static class AddHttpsExtension
    {
        public static IServiceCollection AddHttps(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            ApiHttpClientSettings settings = provider.GetService<ApiHttpClientSettings>() 
                ?? throw new ArgumentNullException(nameof(ApiHttpClientSettings));

            services.AddHttpClient(
                nameof(ApiHttpClient),
                client =>
                {
                    client.BaseAddress = new Uri(settings.BaseAddress);
                    client.Timeout = settings.MaxTimeout;
                })
                .AddPolicyHandler(GetRetryPolicy<ApiHttpClient>(provider));

            services.AddScoped<IApiHttpClient, ApiHttpClient>();

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy<T>(IServiceProvider provider)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(6,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: async (message, time, count, context) =>
                    {
                        var msg = $"{count} попытка выполнения http запроса. Context {context}. Message {(message.Exception is not null ? message.Exception.Message : (message.Result.StatusCode.ToString() + " " + (await message.Result.Content.ReadAsStringAsync())))}";
                        var logger = provider.GetService<ILogger<T>>() ?? throw new ArgumentNullException(nameof(ILogger<T>));
                        logger.LogInformation(msg);
                    });
        }
    }
}
