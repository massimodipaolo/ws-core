using Carter;
using NLog.Filters;
using Ws.Core.Extensions.Data;

namespace ws.bom.oven.web.endpoints;

public class PayloadCms : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var _prefix = "/api/bowl";
        app.MapGet($"{_prefix}", services.PayloadCms.Store).WithTags(nameof(PayloadCms));
        app.MapGet($"{_prefix}/route", services.PayloadCms.Route).WithTags(nameof(PayloadCms));
        app.MapGet($"{_prefix}/route/{{*id}}", services.PayloadCms.RouteById).WithTags(nameof(PayloadCms));
        app.MapGet($"{_prefix}/{{slug}}", services.PayloadCms.Collection).WithTags(nameof(PayloadCms));
        app.MapGet($"{_prefix}/{{slug}}/{{id}}", services.PayloadCms.CollectionById).WithTags(nameof(PayloadCms));
        app.MapGet($"{_prefix}/globals/{{slug}}", services.PayloadCms.Global).WithTags(nameof(PayloadCms));
    }
}