using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using tl.employersupport.ecrm.poc.application.functions.Extensions;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((_, services) =>
        {
            //Required order - add configuration first, then http clients, then other services
            services
                .AddEmailConfiguration()
                .AddZendeskConfiguration()
                .AddZendeskHttpClient()
                .AddTransient<IDateTimeService, DateTimeService>()
                .AddEmailServices()
                .AddTransient<ITicketService, TicketService>()
                ;
        })
    .Build();

host.Run();
