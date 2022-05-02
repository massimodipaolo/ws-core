using Carter;
using Ws.Core.Extensions.Data;
using xCore.Models;

namespace xCore.Endpoints;

public class App : CrudOp, ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var _prefix = "/api/app";
        app.MapGet($"{_prefix}/{nameof(User)}", GetAll<User,int>).WithTags(nameof(App));
        app.MapGet($"{_prefix}/{nameof(User)}/{{id}}", GetById<User,int>).WithTags(nameof(App));
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
