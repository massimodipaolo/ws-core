using Ws.Core.Extensions.Data;

namespace x.core.Models;

public class Geo: IAppJsonSerializable
{
    public string Lat { get; set; } = "";
    public string Lng { get; set; } = "";
}

public class Address: IAppJsonSerializable
{
    public string? Street { get; set; } = "";
    public string? Suite { get; set; } = "";
    public string? City { get; set; } = "";
    public string? Zipcode { get; set; } = "";
    public Geo? Geo { get; set; } = new();
}

public class Company: IAppJsonSerializable
{
    public string? Name { get; set; } = "";
    public string? CatchPhrase { get; set; } = "";
    public string? Bs { get; set; } = "";
}

public class User: IEntity<int>, IApp, IAppTracked
{
    public int Id { get; set; }
    public string? Name { get; set; } = "";
    public string? Username { get; set; } = "";
    public string? Email { get; set; } = "";
    public Address? Address { get; set; } = new();
    public string? Phone { get; set; } = "";
    public string? Website { get; set; } = "";
    public Company? Company { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    /// <summary>
    /// Don't init to fixed-size collections like Array.Empty<![CDATA[<Post>]]>() => https://github.com/dotnet/efcore/issues/24497
    /// </summary>
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public virtual ICollection<Album>? Albums { get; set; } 
    public virtual ICollection<Todo>? Todos { get; set; }
}

/// <summary>
/// Note: EF map User to User2 view!
/// </summary>
public record User2 : IRecord, IEntity<int>, IApp, IAppTracked
{
    public int Id { get; set; }
    public string? Name { get; set; } = "";
    public string? Username { get; set; } = "";
    public string? Email { get; set; } = "";
    public Address? Address { get; set; } = new();
    public string? Phone { get; set; } = "";
    public string? Website { get; set; } = "";
    public Company? Company { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
