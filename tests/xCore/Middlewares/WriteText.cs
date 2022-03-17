using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace xCore.Middlewares
{
    public class WriteText
    {
        private readonly RequestDelegate _next;

        public WriteText(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var text = context.Request.Query["text"];
            if (!string.IsNullOrWhiteSpace(text))
                await context.Response.WriteAsync($"{text}");
            else
                await _next(context);
        }
    }
}
