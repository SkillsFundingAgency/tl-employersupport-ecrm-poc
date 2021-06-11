using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model;
using tl.employersupport.ecrm.poc.application.Services;
using tl.employersupport.ecrm.poc.console;

await Host.CreateDefaultBuilder(args)
    .UseContentRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
    .ConfigureLogging(_ =>
    {
        // Add any 3rd party loggers - replace _ above with logging parameter
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
                    client.BaseAddress = 
                        zendeskOptions.ApiBaseUri.EndsWith("/") 
                            ? new Uri(zendeskOptions.ApiBaseUri) 
                            : new Uri(zendeskOptions.ApiBaseUri + "/");

                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    if (zendeskOptions.AuthenticationMethod == AuthenticationScheme.BasicWithUserPassword)
                    {
                        //Basic auth with user/password - email_address:password and this must be base64-encoded
                        var userAuthenticationString = $"{zendeskOptions.User}:{zendeskOptions.Password}";
                        var encodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(userAuthenticationString));
                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Basic", encodedAuthenticationString);
                    }
                    else if (zendeskOptions.AuthenticationMethod == AuthenticationScheme.BasicWithApiToken)
                    {
                        //Basic auth with token - email_address/token:api_token and this must be base64-encoded
                        var tokenAuthenticationString = $"{zendeskOptions.User}/token:{zendeskOptions.ApiToken}";
                        var encodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(tokenAuthenticationString));
                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Basic", encodedAuthenticationString);
                    }
                    else
                    {
                        throw new NotSupportedException("Invalid Zendesk authentication scheme");
                    }

                    if (zendeskOptions.CompressApiResponse)
                    {
                        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                    }
                }
            )
            .ConfigurePrimaryHttpMessageHandler(_ =>
            {
                var handler = new HttpClientHandler();

                if (zendeskOptions.CompressApiResponse && handler.SupportsAutomaticDecompression)
                {
                    handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                }
                return handler;
            })
            //TODO:
            //.AddTypedClient<ZendeskApi>()
            ;

        services
            .AddHostedService<ConsoleHostedService>()
            .AddTransient<ITicketService, TicketService>()
            ;
    })
    .RunConsoleAsync();
