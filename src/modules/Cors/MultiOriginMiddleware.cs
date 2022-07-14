using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Ws.Core.Extensions.Cors;

internal class MultiOriginMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ICorsService _corsService;
    private readonly ICorsPolicyProvider _corsPolicyProvider;
    private readonly IDictionary<string, string> _policies;

    public MultiOriginMiddleware(
        RequestDelegate next,
        ICorsService corsService,
        ICorsPolicyProvider policyProvider,
        IDictionary<string, string> policies)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _corsService = corsService ?? throw new ArgumentNullException(nameof(corsService));
        _corsPolicyProvider = policyProvider ?? throw new ArgumentNullException(nameof(policyProvider));
        _policies = policies;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Headers.ContainsKey(CorsConstants.Origin))
        {
            bool hasPolicy = _policies.TryGetValue(context.Request.Headers[CorsConstants.Origin], out string? policyName);            
            if (!string.IsNullOrEmpty(policyName) && hasPolicy)
            {
                CorsPolicy? corsPolicy = await _corsPolicyProvider.GetPolicyAsync(context, policyName);
                if (corsPolicy != null)
                {
                    var corsResult = _corsService.EvaluatePolicy(context, corsPolicy);
                    _corsService.ApplyResult(corsResult, context.Response);

                    var accessControlRequestMethod = context.Request.Headers[CorsConstants.AccessControlRequestMethod];
                    if (string.Equals(
                            context.Request.Method,
                            CorsConstants.PreflightHttpMethod,
                            StringComparison.OrdinalIgnoreCase) &&
                            !StringValues.IsNullOrEmpty(accessControlRequestMethod))
                    {
                        // Since there is a policy which was identified,
                        // always respond to preflight requests.
                        context.Response.StatusCode = StatusCodes.Status204NoContent;
                        return;
                    }
                }
            }
        }

        await _next(context);
    }
}
