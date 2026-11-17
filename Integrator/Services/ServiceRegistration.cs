using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Polly;
using Microsoft.IdentityModel.Tokens;

namespace Integrator.Services;

public static class ServiceRegistration
{
    public static void AddMembershipHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient("MembershipApi", client =>
        {
            var baseUrl = configuration["Membership:BaseUrl"]
                          ?? throw new InvalidOperationException("Membership:BaseUrl is not configured.");
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = Timeout.InfiniteTimeSpan; // resilience timeouts control it
        })
        .AddResilienceHandler("membership-pipeline", (pipeline, context) =>
        {
            var logger = context.ServiceProvider
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("Startup");
            
            pipeline.AddTimeout(TimeSpan.FromSeconds(30)); // total across all attempts

            pipeline.AddRetry(new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(1),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                OnRetry = args =>
                {
                    logger.LogWarning("Retry {Attempt} after {Delay}ms. Status: {Status}, Error: {Error}",
                    args.AttemptNumber + 1,
                    args.RetryDelay.TotalMilliseconds,
                    args.Outcome.Result?.StatusCode,
                    args.Outcome.Exception?.Message);
                    return default;
                }
            });
            pipeline.AddTimeout(TimeSpan.FromSeconds(10)); // per attempt
        });
    }
    
    
   
    public static IServiceCollection AddIntegratorAuthentication(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication()
            .AddJwtBearer("integrator-scheme", jwtOptions =>
            {
                jwtOptions.Authority = configuration["Api:Authority"];
                jwtOptions.Audience = configuration["Api:Audience"];
                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudiences = configuration.GetSection("Api:ValidAudiences").Get<string[]>(),
                    ValidIssuers = configuration.GetSection("Api:ValidIssuers").Get<string[]>()
                };
                jwtOptions.MapInboundClaims = false;
            });
        return services;
    }
}