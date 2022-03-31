using System.Globalization;

namespace xCore.Middlewares
{
    public class RequestCulture
    {
        private readonly RequestDelegate _next;

        public RequestCulture(RequestDelegate next)
        {
            _next = next;
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
            }
            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }
    }
}
