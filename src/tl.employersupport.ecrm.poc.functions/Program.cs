using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Services;
using tl.employersupport.ecrm.poc.functions.Extensions;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        if (context.HostingEnvironment.IsDevelopment())
        {
            //https://stackoverflow.com/questions/66500195/net-5-grpc-client-call-throws-exception-requesting-http-version-2-0-with-versi
            HttpClient.DefaultProxy = new WebProxy();
        }

        //Required order - add configuration first, then http clients, then other services
        services
            .AddEmailConfiguration()
            .AddEcrmConfiguration()
            .AddZendeskConfiguration()
            .AddEcrmHttpClient()
            .AddEcrmODataHttpClient()
            .AddZendeskHttpClient()
            .AddAuthenticationService()
            .AddTransient<IDateTimeService, DateTimeService>()
            .AddEmailServices()
            .AddXrmOrganizationServices()
            .AddTransient<ITicketService, TicketService>()
            .AddTransient<IEcrmService, EcrmService>()
            ;
    })
    .Build();

host.Run();
