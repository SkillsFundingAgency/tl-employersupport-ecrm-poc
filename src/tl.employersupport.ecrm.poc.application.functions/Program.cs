using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Notify.Client;
using Notify.Interfaces;
using tl.employersupport.ecrm.poc.application;
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
            //var storage = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            // https://marcroussy.com/2021/05/03/using-ioptions-in-azure-functions/
            services
                 .AddOptions<EmailConfiguration>()
                 .Configure<IConfiguration>((settings, configuration) =>
                 {
                     configuration.GetSection(nameof(EmailConfiguration)).Bind(settings);
                 })
                 //.Services.AddOptions() //....
                 ;

            services
                .AddTransient<IDateTimeService, DateTimeService>()
                .AddTransient<IEmailService, EmailService>()
                .AddTransient<IAsyncNotificationClient, NotificationClient>(x =>
                    {
                        var emailOptions = x.GetRequiredService<IOptions<EmailConfiguration>>();
                        return new NotificationClient(emailOptions.Value.GovNotifyApiKey);
                    }
                );
        })
    .Build();

host.Run();
