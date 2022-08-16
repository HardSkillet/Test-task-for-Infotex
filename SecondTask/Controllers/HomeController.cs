using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace SecondTask.Controllers
{
    public class HomeController : Controller
    {
        private ConfigRss _config;
        public HomeController(IOptions<ConfigRss> options) {
            _config = options.Value;
        }
        public IActionResult Tape(ContextHtml ch) {
            return new HtmlResult(ch.Rss(_config, false));
        }
        [HttpGet]
        public IActionResult Set()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Set(ConfigRss config, ContextHtml ch)
        {
            if (!(config.Url is null)) _config.Url = config.Url;
            if (!(config.Time is null)) _config.Time = config.Time;

            return new HtmlResult(ch.Rss(_config, true));
        }
    }
}
