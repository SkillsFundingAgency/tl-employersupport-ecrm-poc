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


## Twitter

We want to be able to call the twitter search API from a Zendesk page. 
The target API call is https://api.twitter.com/2/tweets/search/recent?query=from:TLevels_govuk

A bearer token is required and can be generated in the twitter developer account, or in code using the API Key and API Secret. 
The bearer token shouldn't expire, so it should be safe to store it as an application secret.

Problems with calling twitter from a web page are
 - the bearer token needs to be sent to twitter, so it would need to be exposed on the page for use in javascript
 - twitter will reject cross-origin calls and give a CORS error

A possible approach is to use APIM to proxy the call. 
 - create an APIM resource
 - added an API using OpenAPI - the spec is at https://api.twitter.com/2/openapi.json
 - (Note - this might give an error that NonCompliantRulesProblem couldn't be processed. If so, download and edit the file to remove all references, then create an API from the edited file)
 - add an inbound CORS policy to allow 
 - add a set-header policy that creates an authorization header and passes it to twitter
 - add a product to APIM and include the API
 - create a subscription and note the key for use in the web page

Set header policy can be set on the operation or for all APIs and should be (with the actual token instead of XXX)
```
<set-header name="Authorization" exists-action="override">
    <value>Bearer XXX</value>
</set-header>
```

TODO: Consider deleting the Ocp-Apim-Subscription-Key header since twitter won't use it

CORS policy can be set on the `Recent search` operation should be similar to (with more clearly defined origins)
```
<cors>
    <allowed-origins>
        <origin>*</origin>
    </allowed-origins>
    <allowed-methods preflight-result-max-age="300">
        <method>GET</method>
    </allowed-methods>
    <allowed-headers>
        <header>*</header>
    </allowed-headers>
</cors>
```

On the Zendesk page, the subscription key from APIM needs to be added in the `Ocp-Apim-Subscription-Key` header.
```
 xhr.setRequestHeader("Ocp-Apim-Subscription-Key", `${$('#twitter_apim_api_key').val()}`);
```

The APIM url should be used as the base for calling, e.g.
```
https://your-apim-name-apim-test.azure-api.net/2/tweets/search/recent?query=from:TLevels_govuk&max_results=10
```

It should be possible to cache calls to twitter - this will help avoid hitting rate limits. 
See https://github.com/toddkitta/azure-content/blob/master/articles/api-management/api-management-howto-cache.md

Additional query parameters still need to be explored including
- `max_results=20` - to show more results
- `expansions` and `media.fields` - these should allow us to pull down images from twitter

An more expanded url is
https://your-apim-name-apim-test.azure-api.net/2/tweets/search/recent?query=from:TLevels_govuk&max_results=10&expansions=attachments.media_keys&media.fields=preview_image_url

A working APIM instance template can be seen in the `ARM` folder.
