using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Notify.Client;
using Notify.Interfaces;
using Polly;
using tl.employersupport.ecrm.poc.application.HttpHandlers;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model.Configuration;
using tl.employersupport.ecrm.poc.application.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    //.ConfigureAppConfiguration(c =>
    //    {
    //        c.AddCommandLine(args)
    //            .SetBasePath(Environment.CurrentDirectory)
    //            .AddJsonFile("local.settings.json", true, true)
    //            .AddEnvironmentVariables();
    //    }
    //)
    .ConfigureServices((_, services) =>
        {
            // https://marcroussy.com/2021/05/03/using-ioptions-in-azure-functions/

            services
                 .AddOptions<EmailConfiguration>()
                 .Configure<IConfiguration>((settings, configuration) =>
                 {
                     configuration.GetSection(nameof(EmailConfiguration)).Bind(settings);
                 })
                 .Services
                 .AddOptions<ZendeskConfiguration>()
                 .Configure<IConfiguration>((settings, configuration) =>
                 {
                     configuration.GetSection(nameof(ZendeskConfiguration)).Bind(settings);
                 });

            services
                .AddTransient<ZendeskApiTokenMessageHandler>();

            services.AddHttpClient<ITicketService, TicketService>(
                    nameof(TicketService),
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
                        if (zendeskOptions.CompressApiResponse)
                        {
                            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                        }
                    }
                )
                .ConfigurePrimaryHttpMessageHandler(serviceProvider =>
                {
                    var zendeskOptions = serviceProvider
                        .GetRequiredService<IOptions<ZendeskConfiguration>>()
                        .Value; var handler = new HttpClientHandler();
                    if (zendeskOptions.CompressApiResponse && handler.SupportsAutomaticDecompression)
                    {
                        handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                    }
                    return handler;
                })
                //TODO:
                //.AddTypedClient<ZendeskApi>()
                .AddHttpMessageHandler<ZendeskApiTokenMessageHandler>()
                .AddTransientHttpErrorPolicy(policy =>
                    policy.WaitAndRetryAsync(new[] {
                        TimeSpan.FromMilliseconds(200),
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(10),
                    }))
                ;

            services
                .AddTransient<IDateTimeService, DateTimeService>()
                .AddTransient<IEmailService, EmailService>()
                .AddTransient<ITicketService, TicketService>()
                .AddTransient<IAsyncNotificationClient, NotificationClient>(serviceProvider =>
                    {
                        var emailOptions = serviceProvider
                            .GetRequiredService<IOptions<EmailConfiguration>>()
                            .Value;
                        return new NotificationClient(emailOptions.GovNotifyApiKey);
                    }
                );
        })
    .Build();

host.Run();
