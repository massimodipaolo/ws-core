using Ws.Core.Extensions.Data;

namespace xCore.Models;

public record Agenda : IRecord, IAppTracked, IEntity<string>
{
    public string? Id { get; set; } = Guid.NewGuid().ToString();
    public string? Title { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsComplete { get; set; } = false;
}
