using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Notify.Client;
using Notify.Interfaces;
using Polly;
using tl.employersupport.ecrm.poc.application.ApiClients;
using tl.employersupport.ecrm.poc.application.HttpHandlers;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model.Configuration;
using tl.employersupport.ecrm.poc.application.Services;
using tl.employersupport.ecrm.poc.console;

await Host.CreateDefaultBuilder(args)
    .UseContentRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
    .ConfigureLogging(_ =>
    {
        // Add any 3rd party loggers - replace _ above with "logging" parameter
    })
    .ConfigureServices((hostContext, services) =>
    {
        var emailOptions = new EmailConfiguration();
        var emailConfiguration = hostContext.Configuration.GetSection(nameof(EmailConfiguration));
        emailConfiguration.Bind(emailOptions);

        var ecrmOptions = new EcrmConfiguration();
        var crmConfiguration = hostContext.Configuration.GetSection(nameof(EcrmConfiguration));
        crmConfiguration.Bind(ecrmOptions);

        var zendeskOptions = new ZendeskConfiguration();
        var zendeskConfiguration = hostContext.Configuration.GetSection(nameof(ZendeskConfiguration));
        zendeskConfiguration.Bind(zendeskOptions);

        services
            .AddOptions<MonitorConfiguration>()
            .Bind(hostContext.Configuration.GetSection(nameof(MonitorConfiguration)));
        services
            .AddOptions<EmailConfiguration>()
            .Bind(emailConfiguration);

        services
            .Configure<ZendeskConfiguration>(zendeskConfiguration)
            .AddTransient<ZendeskApiTokenMessageHandler>();

        //Add http clients before creating services
        services.AddHttpClient<IZendeskApiClient, ZendeskApiClient>(
                client =>
                {
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
                }));

        services.AddHttpClient<IFunctionsApiClient, FunctionsApiClient>()
            .AddTransientHttpErrorPolicy(policy =>
                policy.WaitAndRetryAsync(new[] {
                    TimeSpan.FromMilliseconds(200),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5)
                })
            );

        services
            .AddHttpClient<IEcrmApiClient, EcrmApiClient>((_, client) =>
                {
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

        services
            .AddHttpClient<IEcrmODataApiClient, EcrmODataApiClient>((_, client) =>
                {
                    var uri = ecrmOptions.ODataApiUri.EndsWith("/")
                        ? ecrmOptions.ODataApiUri
                        : $"{ecrmOptions.ODataApiUri}/";

                    client.BaseAddress = 
                        new Uri($"{uri}api/data/v{ecrmOptions.ODataApiVersion}/");

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
            .AddTransientHttpErrorPolicy(policy =>
                policy.WaitAndRetryAsync(new[] {
                    TimeSpan.FromMilliseconds(200),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
                }))
            ;

        services
            .AddHostedService<ConsoleHostedService>()
            .AddSingleton<IAuthenticationService, AuthenticationService>(_
                => new AuthenticationService(new AuthenticationConfiguration
                {
                    Audience = ecrmOptions.ODataApiUri,
                    ClientId = ecrmOptions.ClientId,
                    ClientSecret = ecrmOptions.ClientSecret,
                    Tenant = ecrmOptions.Tenant
                }))
            .AddTransient<IOrganizationService>(_ =>
                new ServiceClient(
                        "AuthType=ClientSecret;" +
                        $"url={ecrmOptions.ODataApiUri};" +
                        $"ClientId={ecrmOptions.ClientId};" +
                        $"ClientSecret={ecrmOptions.ClientSecret}"))
            .AddSingleton<IDateTimeService, DateTimeService>()
            .AddTransient<IEmailService, EmailService>()
            .AddTransient<ITicketService, TicketService>()
            .AddTransient<IMonitorService, MonitorService>()
            .AddTransient<IEcrmService, EcrmService>()
            .AddTransient<IEcrmXrmClient, EcrmXrmClient>()
            .AddTransient<IAsyncNotificationClient, NotificationClient>(
                _ => new NotificationClient(emailOptions.GovNotifyApiKey));
    })
    .RunConsoleAsync();