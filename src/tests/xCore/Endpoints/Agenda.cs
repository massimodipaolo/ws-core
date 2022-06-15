using Carter;
using Ws.Core.Extensions.Data;
using xCore.Models;

namespace xCore.Endpoints;

public class AgendaModule : CrudOp, ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var _prefix = $"/api/{nameof(App).ToLower()}/{nameof(Agenda)}";
        app.MapGet($"{_prefix}", GetAll<Agenda, string>).WithTags(nameof(Agenda));
        app.MapGet($"{_prefix}/{{id}}", GetById<Agenda, string>).WithTags(nameof(Agenda));
        app.MapPost($"{_prefix}", Create<Agenda, string>).WithTags(nameof(Agenda));
        app.MapPost($"{_prefix}/range", CreateMany<Agenda, string>).WithTags(nameof(Agenda));
        app.MapPut($"{_prefix}/{{id}}", Update<Agenda, string>).WithTags(nameof(Agenda));
        app.MapPut($"{_prefix}", UpdateMany<Agenda, string>).WithTags(nameof(Agenda));
        app.MapDelete($"{_prefix}/{{id}}", Delete<Agenda, string>).WithTags(nameof(Agenda));
        app.MapDelete($"{_prefix}", DeleteMany<Agenda, string>).WithTags(nameof(Agenda));
        app.MapPost($"{_prefix}/merge/{{operation}}", Merge<Agenda, string>).WithTags(nameof(Agenda));
    }
}