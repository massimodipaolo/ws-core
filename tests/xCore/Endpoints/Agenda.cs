using Carter;
using Ws.Core.Extensions.Data;

namespace xCore.Endpoints;

public class Agenda : xCore.Endpoints.CrudOp, ICarterModule, IEntity<string>
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Title { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public bool IsComplete { get; set; } = false;
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"/api/{nameof(Agenda)}", GetAll<Agenda,string>).WithTags(nameof(Agenda));
        app.MapGet($"/api/{nameof(Agenda)}/{{id}}", GetById<Agenda, string>).WithTags(nameof(Agenda));
        app.MapPost($"/api/{nameof(Agenda)}", Create<Agenda, string>).WithTags(nameof(Agenda));
        app.MapPut($"/api/{nameof(Agenda)}/{{id}}", Update<Agenda, string>).WithTags(nameof(Agenda));
        app.MapDelete($"/api/{nameof(Agenda)}/{{id}}", Delete<Agenda, string>).WithTags(nameof(Agenda));
    }
}