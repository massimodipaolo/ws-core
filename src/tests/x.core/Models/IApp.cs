using Ws.Core.Extensions.Data;

namespace x.core.Models;

public interface IApp { }
public interface IAppTracked
{
    DateTime CreatedAt { get; set; }
}
public interface IAppJsonSerializable { }

public record IRecord { }