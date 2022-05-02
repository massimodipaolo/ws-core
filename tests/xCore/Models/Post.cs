using Ws.Core.Extensions.Data;

namespace xCore.Models;

public class Post: IEntity<int>, IApp
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public virtual ICollection<Comment> Comments { get; set; }
}

public class Comment: IEntity<int>, IApp
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Body { get; set; }
}
