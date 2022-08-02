using Ws.Core.Extensions.Data;
using Ws.Core.Shared.Serialization;

namespace x.core.Models;

public class Post: IEntity<int>, IApp
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? Title { get; set; }
    [SensitiveData]
    public string? Body { get; set; }
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}

public class Comment: IEntity<int>, IApp
{
    public int Id { get; set; }
    public int PostId { get; set; }
    [SensitiveData]
    public string? Name { get; set; }
    [SensitiveData]
    public string? Email { get; set; }
    public string? Body { get; set; }
}
