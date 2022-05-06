using Carter;
using System.Linq;
using Ws.Core.Extensions.Data;
using xCore.Models;

namespace xCore.Endpoints;

public class App : CrudOp, ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var _prefix = "/api/app";
        app.MapGet($"{_prefix}/{nameof(User)}", GetAll<User,int>).WithTags(nameof(App));
        app.MapGet($"{_prefix}/{nameof(User)}/{{id}}", GetById<User, int>).WithTags(nameof(App));
        app.MapGet($"{_prefix}/{nameof(User)}/ext/{{id}}",
            (int id, IRepository<User, int> _repo, IRepository<Comment, int> _repoComment, IRepository<Photo, int> _repoPhoto) =>
            {
                if (_repo.Find(id) is User item)
                {
                    item.Posts = item.Posts.Join(_repoComment.List.Where(_ => item.Posts.Select(__ => __.Id).Contains(_.PostId)), p => p.Id, c => c.PostId, (p, c) => (p, c))
                    .GroupBy(_ => _.p.Id)
                    .Select(_ => {
                        var post = _.Select(g => g.p).First(); var comments = _.Select(g => g.c);
                        return new Post() { Id = post.Id, Title = post.Title, Body = post.Body, Comments = comments.ToList() };
                    })
                    .ToList();
                    item.Albums = item.Albums.Join(_repoPhoto.List.Where(_ => item.Albums.Select(__ => __.Id).Contains(_.AlbumId)), a => a.Id, p => p.AlbumId, (a, p) => (a, p))
                    .GroupBy(_ => _.a.Id)
                    .Select(_ => {
                        var album = _.Select(g => g.a).First(); var photos = _.Select(g => g.p);
                        return new Album() { Id = album.Id, Title = album.Title, Photos = photos.ToList() };
                    })
                    .ToList();
                    return Results.Ok(item);
                }
                else
                    return Results.NotFound();
            })
            .WithTags(nameof(App));
        app.MapPost($"{_prefix}/{nameof(User)}", Create<User,int>).WithTags(nameof(App));
        app.MapPut($"{_prefix}/{nameof(User)}/{{id}}", Update<User,int>).WithTags(nameof(App));
        app.MapDelete($"{_prefix}/{nameof(User)}/{{id}}", Delete<User,int>).WithTags(nameof(App));

        app.MapGet($"{_prefix}/{nameof(Post)}", GetAll<Post, int>).WithTags(nameof(App));
        app.MapGet($"{_prefix}/{nameof(Post)}/{{id}}", GetById<Post, int>).WithTags(nameof(App));
        app.MapPost($"{_prefix}/{nameof(Post)}", Create<Post, int>).WithTags(nameof(App));
        app.MapPut($"{_prefix}/{nameof(Post)}/{{id}}", Update<Post, int>).WithTags(nameof(App));
        app.MapDelete($"{_prefix}/{nameof(Post)}/{{id}}", Delete<Post, int>).WithTags(nameof(App));

        app.MapGet($"{_prefix}/{nameof(Comment)}", GetAll<Comment, int>).WithTags(nameof(App));
        app.MapGet($"{_prefix}/{nameof(Comment)}/{{id}}", GetById<Comment, int>).WithTags(nameof(App));
        app.MapPost($"{_prefix}/{nameof(Comment)}", Create<Comment, int>).WithTags(nameof(App));
        app.MapPut($"{_prefix}/{nameof(Comment)}/{{id}}", Update<Comment, int>).WithTags(nameof(App));
        app.MapDelete($"{_prefix}/{nameof(Comment)}/{{id}}", Delete<Comment, int>).WithTags(nameof(App));

        app.MapGet($"{_prefix}/{nameof(Album)}", GetAll<Album, int>).WithTags(nameof(App));
        app.MapGet($"{_prefix}/{nameof(Album)}/{{id}}", GetById<Album, int>).WithTags(nameof(App));
        app.MapPost($"{_prefix}/{nameof(Album)}", Create<Album, int>).WithTags(nameof(App));
        app.MapPut($"{_prefix}/{nameof(Album)}/{{id}}", Update<Album, int>).WithTags(nameof(App));
        app.MapDelete($"{_prefix}/{nameof(Album)}/{{id}}", Delete<Album, int>).WithTags(nameof(App));

        app.MapGet($"{_prefix}/{nameof(Photo)}", GetAll<Photo, int>).WithTags(nameof(App));
        app.MapGet($"{_prefix}/{nameof(Photo)}/{{id}}", GetById<Photo, int>).WithTags(nameof(App));
        app.MapPost($"{_prefix}/{nameof(Photo)}", Create<Photo, int>).WithTags(nameof(App));
        app.MapPut($"{_prefix}/{nameof(Photo)}/{{id}}", Update<Photo, int>).WithTags(nameof(App));
        app.MapDelete($"{_prefix}/{nameof(Photo)}/{{id}}", Delete<Photo, int>).WithTags(nameof(App));

        app.MapGet($"{_prefix}/{nameof(xCore.Models.Todo)}", GetAll<xCore.Models.Todo, int>).WithTags(nameof(App));
        app.MapGet($"{_prefix}/{nameof(xCore.Models.Todo)}/{{id}}", GetById<xCore.Models.Todo, int>).WithTags(nameof(App));
        app.MapPost($"{_prefix}/{nameof(xCore.Models.Todo)}", Create<xCore.Models.Todo, int>).WithTags(nameof(App));
        app.MapPut($"{_prefix}/{nameof(xCore.Models.Todo)}/{{id}}", Update<xCore.Models.Todo, int>).WithTags(nameof(App));
        app.MapDelete($"{_prefix}/{nameof(xCore.Models.Todo)}/{{id}}", Delete<xCore.Models.Todo, int>).WithTags(nameof(App));
    }
}
