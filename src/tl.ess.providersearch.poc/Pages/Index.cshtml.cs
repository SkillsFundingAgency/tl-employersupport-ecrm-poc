using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace tl.ess.providersearch.poc.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public string BearerToken { get; init; }
    public string SearchApiUrl { get; init; }
    
    public IndexModel(ILogger<IndexModel> logger, IConfiguration config)
    {
        _logger = logger;
        BearerToken = config["BearerToken"];
        SearchApiUrl = config["SearchApiUrl"];
    }

    public void OnGet()
    {

    }
}
