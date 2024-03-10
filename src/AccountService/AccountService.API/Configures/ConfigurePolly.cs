using Polly.Extensions.Http;
using Polly;
using Polly.CircuitBreaker;
using AccountService.Application.Interfaces;
using AccountService.Application.Services;

namespace AccountService.API.Configures
{
    public static class ConfigurePolly
    {
        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        public static IServiceCollection CircuitBreaker(this IServiceCollection services)
        {
            var retryPolicy = GetRetryPolicy();
            var circuitBreakerPolicy = GetCircuitBreakerPolicy();

            services.AddHttpClient<IIdentityService, IdentityService>()
                    .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                    .AddPolicyHandler(retryPolicy)
                    .AddPolicyHandler(circuitBreakerPolicy);

            return services;
        }
    }
}
