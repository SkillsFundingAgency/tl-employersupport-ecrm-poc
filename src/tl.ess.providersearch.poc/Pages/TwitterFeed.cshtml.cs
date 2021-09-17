using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace tl.ess.providersearch.poc.Pages
{
    public class TwitterFeedModel : PageModel
    {
        private readonly ILogger<TwitterFeedModel> _logger;

        public string TwitterAccountName { get; }
        public string TwitterApiKey { get;  }
        public string TwitterApimApiKey { get;  }
        public string TwitterApiSecret { get;  }
        public string TwitterBearerToken { get; }
        public string TwitterApimBaseUri { get; }

        public TwitterFeedModel(ILogger<TwitterFeedModel> logger, IConfiguration config)
        {
            _logger = logger;

            TwitterAccountName = config["TwitterAccountName"];
            TwitterApiKey = config["TwitterApiKey"];
            TwitterApimApiKey = config["TwitterApimApiKey"];
            TwitterApiSecret = config["TwitterApiSecret"];
            TwitterBearerToken = config["TwitterBearerToken"];
            TwitterApimBaseUri = config["TwitterApimBaseUri"];
        }

        public void OnGet()
        {
        }
    }
}
