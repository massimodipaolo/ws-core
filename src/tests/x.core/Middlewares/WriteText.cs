namespace x.core.Middlewares
{
    public class WriteText
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        public WriteText(RequestDelegate next, ILogger<WriteText> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var text = context.Request.Query["text"];
            if (!string.IsNullOrWhiteSpace(text))
            {
                await context.Response.WriteAsync($"{text}");
                _logger?.LogInformation($"Invoked {nameof(WriteText)} middleware with [{text}]");
            }
            else
                await _next(context);
        }
    }
}
