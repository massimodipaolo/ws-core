using Ws.Core.Extensions.Data;

namespace x.core.Models;

public record CrudBase : IRecord, IAppTracked, IEntity<string>
{
    public string? Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public record CrudBase1 : IRecord, IAppTracked, IEntity<Guid>
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public record CrudBase2 : IRecord, IAppTracked, IEntity<Guid>
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public record CrudBase3 : IRecord, IAppTracked, IEntity<Guid>
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public record CrudBase4 : IRecord, IAppTracked, IEntity<Guid>
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}