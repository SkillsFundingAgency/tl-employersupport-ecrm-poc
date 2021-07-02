using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Notify.Client;
using Notify.Interfaces;
using Polly;
using tl.employersupport.ecrm.poc.application.ApiClients;
using tl.employersupport.ecrm.poc.application.HttpHandlers;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model.Configuration;
using tl.employersupport.ecrm.poc.application.Services;

namespace tl.employersupport.ecrm.poc.functions.Extensions
{
    internal static class StartupExtensions
    {
        //Notes:
        // https://marcroussy.com/2021/05/03/using-ioptions-in-azure-functions/
        
        public static IServiceCollection AddEmailConfiguration(this IServiceCollection services)
        {
            services
                .AddOptions<EmailConfiguration>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection(nameof(EmailConfiguration)).Bind(settings);
                });

            return services;
        }

        public static IServiceCollection AddEcrmConfiguration(this IServiceCollection services)
        {
            services
                .AddOptions<EcrmConfiguration>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection(nameof(EcrmConfiguration)).Bind(settings);
                });
            return services;
        }

        public static IServiceCollection AddZendeskConfiguration(this IServiceCollection services)
        {
            services
                .AddOptions<ZendeskConfiguration>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection(nameof(ZendeskConfiguration)).Bind(settings);
                });
            return services;
        }

        public static IServiceCollection AddEmailServices(this IServiceCollection services)
        {
            services
                .AddTransient<IEmailService, EmailService>()
                .AddScoped<IAsyncNotificationClient, NotificationClient>(serviceProvider =>
                {
                    var emailOptions = serviceProvider
                        .GetRequiredService<IOptions<EmailConfiguration>>()
                        .Value;
                    return new NotificationClient(emailOptions.GovNotifyApiKey);
                });

            return services;
        }

        public static IServiceCollection AddEcrmHttpClient(this IServiceCollection services)
        {
            services
                .AddHttpClient<IEcrmApiClient, EcrmApiClient>(
                    (serviceProvider, client) =>
                    {
                        var ecrmOptions = serviceProvider
                            .GetRequiredService<IOptions<EcrmConfiguration>>()
                            .Value;

                        client.BaseAddress =
                            ecrmOptions.ApiBaseUri.EndsWith("/")
                                ? new Uri(ecrmOptions.ApiBaseUri)
                                : new Uri(ecrmOptions.ApiBaseUri + "/");

                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ecrmOptions.ApiKey);
                        //client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                        //client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                    }
                )
                .ConfigurePrimaryHttpMessageHandler(_ =>
                {
                    var handler = new HttpClientHandler();
                    if (handler.SupportsAutomaticDecompression)
                    {
                        //handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                    }
                    return handler;
                })
                .AddTransientHttpErrorPolicy(policy =>
                    policy.WaitAndRetryAsync(new[] {
                        TimeSpan.FromMilliseconds(200),
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(10),
                    }))
                ;

            return services;
        }

        public static IServiceCollection AddZendeskHttpClient(this IServiceCollection services)
        {
            services
                .AddScoped<ZendeskApiTokenMessageHandler>()
                .AddHttpClient<IZendeskApiClient, ZendeskApiClient>(
                    (serviceProvider, client) =>
                    {
                        var zendeskOptions = serviceProvider
                            .GetRequiredService<IOptions<ZendeskConfiguration>>()
                            .Value;
                        client.BaseAddress =
                            zendeskOptions.ApiBaseUri.EndsWith("/")
                                ? new Uri(zendeskOptions.ApiBaseUri)
                                : new Uri(zendeskOptions.ApiBaseUri + "/");

                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                    }
                )
                .ConfigurePrimaryHttpMessageHandler(_ =>
                {
                    var handler = new HttpClientHandler();
                    if (handler.SupportsAutomaticDecompression)
                    {
                        handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                    }
                    return handler;
                })
                .AddHttpMessageHandler<ZendeskApiTokenMessageHandler>()
                .AddTransientHttpErrorPolicy(policy =>
                    policy.WaitAndRetryAsync(new[] {
                        TimeSpan.FromMilliseconds(200),
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(10),
                    }))
                ;

            return services;
        }
    }
}
