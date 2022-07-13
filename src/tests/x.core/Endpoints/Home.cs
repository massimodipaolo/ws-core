using Carter;

namespace x.core.Endpoints;

public class Home : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", () => GetHtmlHome);
    }

    private static IResult GetHtmlHome => Results.Extensions.Html(@$"<!doctype html>
<html lang=""en"">
    <head>
        <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
        <title>ws-core</title>
        <link href=""https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css"" rel=""stylesheet"" integrity=""sha384-1BmE4kWBq78iYhFldvKuhfTAU6auU8tT94WrHftjDbrCEXSU1oBoqyl2QvZ6jIW3"" crossorigin=""anonymous"">
    </head>
    <body>
        <div class=""container-md"">
        <h1>©𝘄𝘀 + extCore + injectors + api = 🆆🆂-🅲🅾🆁🅴</h1>
        <hr>
        <ul class=""list-group list-group-flush"">
          <li class=""list-group-item""><samp>/foo</samp> Route on Startup <a href=""/foo"" target=""_blank""> => </a></li>
          <li class=""list-group-item""><samp>/bing/foo</samp> Gateway map <a href=""/bing/foo"" target=""_blank""> => </a></li>
          <li class=""list-group-item""><samp>/api/{nameof(x.core.Endpoints.App).ToLower()}/{nameof(x.core.Models.Agenda).ToLower()}</samp> Api module <a href=""/api/{nameof(x.core.Endpoints.App).ToLower()}/{nameof(x.core.Models.Agenda)}"" target=""_blank""> => </a></li>
          <li class=""list-group-item""><samp>/{nameof(Extensions).ToLower()}/{typeof(Ws.Core.Extensions.Diagnostic.AppRuntime).Namespace.ToLower()}</samp> Api on ref module <a href=""/{nameof(Extensions).ToLower()}/{typeof(Ws.Core.Extensions.Diagnostic.AppRuntime).Namespace.ToLower()}"" target=""_blank""> => </a></li>
          <li class=""list-group-item""><samp>/api/diagnostic</samp> Legacy controller <a href=""/api/diagnostic"" target=""_blank""> => </a></li>          
          <li class=""list-group-item""><samp>/healthchecks-ui</samp> NuGet extension <a href=""/healthchecks-ui"" target=""_blank""> => </a> | <a href=""/healthz/checks"" target=""_blank""> Checks </a></li>
          <li class=""list-group-item""><samp>/hangfire</samp> App extension <a href=""/hangfire"" target=""_blank""> => </a></li>
          <li class=""list-group-item""><samp>/image.processor.demo</samp> Dll extension <a href=""/media/index.html"" target=""_blank""> => </a></li>
          <li class=""list-group-item""><samp>/branch</samp> Injector middleware <a href=""/branch?text=Welcome to branch middleware"" target=""_blank""> => </a></li>
          <li class=""list-group-item""><samp>/message/send</samp> Injector decorators <a href=""/message/send"" target=""_blank""> => </a></li>
          <li class=""list-group-item""><samp>/swagger</samp> Api discover <a href=""/swagger"" target=""_blank""> => </a></li>
          <li class=""list-group-item""><samp>/profiler</samp> Mini profiler <a href=""/mini-profiler-resources/results-index"" target=""_blank""> => </a> | <a href=""/mini-profiler-resources/results-list"" target=""_blank""> List </a> | <a href=""/mini-profiler-resources/results"" target=""_blank""> Last </a></li>
        </ul>
        </div>
    </body>
</html>");


}

class HtmlResult : IResult
{
    private readonly string _htmlContent;
    public HtmlResult(string htmlContent)
    {
        _htmlContent = htmlContent;
    }
    public async Task ExecuteAsync(HttpContext ctx)
    {
        ctx.Response.ContentType = $"{System.Net.Mime.MediaTypeNames.Text.Html};charset=utf-8;";
        ctx.Response.ContentLength = System.Text.Encoding.UTF8.GetByteCount(_htmlContent);        
        await ctx.Response.WriteAsync(_htmlContent);
    }
}

static class ResultExtensions
{
    public static IResult Html(this IResultExtensions extensions, string html)
    => new HtmlResult(html);
}