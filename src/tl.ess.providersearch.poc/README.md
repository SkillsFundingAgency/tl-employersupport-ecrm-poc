# ESS Provider Search POC

This sample uses javascript to call an API on the T Levels campaign site and populate the search results.

To use this in Zendesk, update home_page.hbs and document_head.hbs with the contents of 
the equivalent files in the `Zendesk files` here and upload site.js as an asset to the Zendesk site. 
This will place two tabs on the home page, one containing the js populated results from 
this project, and the other hosting an iframe.

The campaign site must have an API endpoint visible for the js sample. 
This requires the following in `Startup.ConfigureServices`:

```
          var corsOrigins = Configuration["AllowedOrigins"] ?? "*";
            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithOrigins(corsOrigins)));
```

and in `Startup.Configure`
```
            app.UseCors("CorsPolicy");
```

An alternative approach would be to have CORS controlled through a firewall or application gateway.
DevOps have suggested Barracuda Web Application Firewall. The advantage of doing it this way is that the 
web API doesn't need to deal with security.

If a large number of queries are generated from Zendesk, there could be an impact on the campaign site. 
This can be mitigated by creating a new service for the employer provider search.
Pro
    - no impact on campaign site
    - if a database is used, this would allow for growing the service into something more general
	- A simple service could be written like this:
		https://www.strathweb.com/2020/10/beautiful-and-compact-web-apis-with-c-9-net-5-0-and-asp-net-core/
    - low cost if hosted in Azure function (but might be a performance impact)
Con
    - needs it's own data source/database 
    - might need to discuss usage of NCS API since this would need to pull data nightly as well as the campaign site
    - higher costs for hosting the service if done as a web api

The campaign site must allow iframes for the second sample.
This requires the following in `Startup.Configure`
```
app.UseCsp(options => options.
        ...
        .FrameAncestors(x =>
            x.CustomSources("https://<site>.zendesk.com/"))
```

If this work is taken forwards, the custom sources for frames or sites CORS should be read from configuration.


## TODO

- Get postcode from form
- Get remote url from config via injected model
- fill in this readme

