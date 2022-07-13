using Ws.Core.Extensions.Data;

namespace x.core.Models;

public class Album: IEntity<int>, IApp
{
    public int Id { get; set; }    
    public int UserId { get; set; }
    public string Title { get; set; }
    public virtual ICollection<Photo> Photos { get; set; }
}

public class Photo : IEntity<int>, IApp
{
    public int Id { get; set; }    
    public int AlbumId { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public string ThumbnailUrl { get; set; }
}