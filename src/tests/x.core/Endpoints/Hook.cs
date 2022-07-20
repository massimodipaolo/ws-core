using Carter;

namespace x.core.Endpoints;
public class Hook : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("hook/healthz", Healthz);
    }

    public async Task<IResult> Healthz(object data, Ws.Core.Extensions.Message.IMessage notify)
    {
        if (data != null)
        {
            var payload = Newtonsoft.Json.JsonConvert.DeserializeObject<HealthCheckHookMessage>(data.ToString() ?? "");
            if (payload != null)
            {
                var message = new Ws.Core.Extensions.Message.Message()
                {
                    Subject = payload.Title,
                    Content = payload.Text,
                    Arguments = payload.IsFailure ? new { Importance = "High" } : null,
                    Sender = new Ws.Core.Extensions.Message.Message.Actor() { Address = $"no-reply@{nameof(Ws.Core)}" },
                    Recipients = new Ws.Core.Extensions.Message.Message.Actor[]
                    {
                                    new Ws.Core.Extensions.Message.Message.Actor() {
                                        Address = $"operator@{nameof(Ws.Core)}",
                                        Name="DTC",
                                        Type= Ws.Core.Extensions.Message.Message.ActorType.Primary}
                    }
                };
                await notify.SendAsync(message);
                return Results.Ok();
            }
        }
        return Results.BadRequest();
    }

    public class HealthCheckHookMessage
    {
        public string Title { get; set; } = "";
        public string Text { get; set; } = "";
        public bool IsFailure { get; set; } = false;
    }
}
