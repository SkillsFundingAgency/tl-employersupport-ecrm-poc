using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace tl.ess.providersearch.poc.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public string ApiAppId { get; }
    public string ApiKey { get; }
    public string QualificationArticleMap { get; }
    public string ArticleBaseUrl { get; }
    public string SearchApiUrl { get; init; }
    
    public IndexModel(ILogger<IndexModel> logger, IConfiguration config)
    {
        _logger = logger;
        ApiAppId = config["ApiAppId"];
        ApiKey = config["ApiKey"];
        SearchApiUrl = config["SearchApiUrl"];
        ArticleBaseUrl = config["ArticleBaseUrl"];

        var map = config
            .GetSection("QualificationArticleMap").GetChildren()
            .ToDictionary(x => x.Key, x => x.Value);
       QualificationArticleMap = JsonSerializer.Serialize(map);
    }

    public void OnGet()
    {

    }
}
