using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using tl.ess.providersearch.poc;

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    })
    .Build()
    .Run();
