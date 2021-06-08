using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model;
using tl.employersupport.ecrm.poc.application.Services;
using tl.employersupport.ecrm.poc.console;

await Host.CreateDefaultBuilder(args)
    .UseContentRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
    .ConfigureLogging(logging =>
    {
        // Add any 3rd party loggers
    })
    .ConfigureServices((hostContext, services) =>
    {
        //services
        //    .AddOptions<ZendeskConfiguration>()
        //    .Bind(hostContext.Configuration.GetSection("ZendeskConfiguration"));

        var zendeskOptions = new ZendeskConfiguration();
        var zendeskConfiguration = hostContext.Configuration.GetSection("ZendeskConfiguration");
        zendeskConfiguration.Bind(zendeskOptions);

        services
            .Configure<ZendeskConfiguration>(zendeskConfiguration);

        //Add http clients before creating services
        services.AddHttpClient<ITicketService, TicketService>(
                nameof(TicketService),
                client =>
                {
                    client.BaseAddress = new Uri(zendeskOptions.ApiBaseUri);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    //client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiConfiguration.ApiKey);
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
            });

        services
            .AddHostedService<ConsoleHostedService>()
            .AddTransient<ITicketService, TicketService>()
            ;
    })
    .RunConsoleAsync();
