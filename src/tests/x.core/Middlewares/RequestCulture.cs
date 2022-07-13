using System.Globalization;

namespace x.core.Middlewares
{
    public class RequestCulture
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RequestCulture(RequestDelegate next, ILogger<RequestCulture> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var cultureQuery = context.Request.Query["culture"];
            if (!string.IsNullOrWhiteSpace(cultureQuery))
            {
                var culture = new CultureInfo(cultureQuery);
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
                context.Response.Headers.Add(new KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues>("X-culture", cultureQuery));
                _logger.LogInformation($"Invoked {nameof(RequestCulture)} middleware with [{cultureQuery}]");
            }
            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }
    }
}
