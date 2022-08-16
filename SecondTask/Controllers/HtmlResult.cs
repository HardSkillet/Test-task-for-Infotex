using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SecondTask.Controllers {
    public class HtmlResult : IActionResult
    {
        string htmlCode;
        public HtmlResult(string html)
        {
            htmlCode = html;
        }
        public async Task ExecuteResultAsync(ActionContext context)
        {
            await context.HttpContext.Response.WriteAsync(htmlCode);

        }
    }
}