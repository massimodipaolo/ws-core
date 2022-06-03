using Ws.Core.Extensions.Data;

namespace xCore.Models;

public class Geo: IAppJsonSerializable
{
    public string Lat { get; set; }
    public string Lng { get; set; }
}

public class Address: IAppJsonSerializable
{
    public string Street { get; set; }
    public string Suite { get; set; }
    public string City { get; set; }
    public string Zipcode { get; set; }
    public Geo Geo { get; set; }
}

public class Company: IAppJsonSerializable
{
    public string Name { get; set; }
    public string CatchPhrase { get; set; }
    public string Bs { get; set; }
}

public class User: IEntity<int>, IApp
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public Address Address { get; set; }
    public string Phone { get; set; }
    public string Website { get; set; }
    public Company Company { get; set; }
    public virtual ICollection<Post> Posts { get; set; }
    public virtual ICollection<Album> Albums { get; set; }
    public virtual ICollection<Todo> Todos { get; set; }
}
public class User2 : User, IEntity<int>, IApp { }
