using Carter;
using System.Linq;
using Ws.Core.Extensions.Data;
using x.core.Models;
using Ws.Core.Extensions.Data.Repository.EF;

namespace x.core.Endpoints;

public class App : CrudOp, ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var _prefix = $"/api/{nameof(App).ToLower()}";

        // Data.EF.SqlServer
        app.MapGet($"{_prefix}/{nameof(User)}", GetAll<User, int>).WithTags(nameof(App));
        app.MapGet($"{_prefix}/{nameof(User)}/{{id}}", GetById<User, int>).WithTags(nameof(App));
        app.MapGet($"{_prefix}/{nameof(User)}/ext/{{id}}",
            (int id, IRepository<User, int> _repo) =>
            {
                User user = _repo.List.Where(_ => _.Id.Equals(id))
                    .IncludeJoin(_ => _.Posts).ThenIncludeJoin(_ => _.Comments)
                    .FirstOrDefault()
                    ;
                return user != null ? Results.Ok(user) : Results.NotFound();
            })
            .WithTags(nameof(App));
        app.MapPost($"{_prefix}/{nameof(User)}", Create<User, int>).WithTags(nameof(App));
        app.MapPost($"{_prefix}/{nameof(User)}/range", CreateMany<User, int>).WithTags(nameof(App));
        app.MapPut($"{_prefix}/{nameof(User)}/{{id}}", Update<User, int>).WithTags(nameof(App));
        app.MapPut($"{_prefix}/{nameof(User)}", UpdateMany<User, int>).WithTags(nameof(App));
        app.MapDelete($"{_prefix}/{nameof(User)}/{{id}}", Delete<User, int>).WithTags(nameof(App));
        app.MapDelete($"{_prefix}/{nameof(User)}", DeleteMany<User, int>).WithTags(nameof(App));
        app.MapPost($"{_prefix}/{nameof(User)}/merge/{{operation}}", Merge<User, int>).WithTags(nameof(App));

        // Data.InMemory
        app.MapGet($"{_prefix}/{nameof(CrudBase)}", GetAll<CrudBase, string>).WithTags(nameof(App));
        app.MapGet($"{_prefix}/{nameof(CrudBase)}/{{id}}", GetById<CrudBase, string>).WithTags(nameof(App));
        app.MapPost($"{_prefix}/{nameof(CrudBase)}", Create<CrudBase, string>).WithTags(nameof(App));
        app.MapPost($"{_prefix}/{nameof(CrudBase)}/range", CreateMany<CrudBase, string>).WithTags(nameof(App));
        app.MapPut($"{_prefix}/{nameof(CrudBase)}/{{id}}", Update<CrudBase, string>).WithTags(nameof(App));
        app.MapPut($"{_prefix}/{nameof(CrudBase)}", UpdateMany<CrudBase, string>).WithTags(nameof(App));
        app.MapDelete($"{_prefix}/{nameof(CrudBase)}/{{id}}", Delete<CrudBase, string>).WithTags(nameof(App));
        app.MapDelete($"{_prefix}/{nameof(CrudBase)}", DeleteMany<CrudBase, string>).WithTags(nameof(App));
        app.MapPost($"{_prefix}/{nameof(CrudBase)}/merge/{{operation}}", Merge<CrudBase, string>).WithTags(nameof(App));

        // Data.EF.SqlServer
        app.MapGet($"{_prefix}/{nameof(CrudBase1)}", GetAll<CrudBase1, Guid>).WithTags(nameof(App));
        app.MapGet($"{_prefix}/{nameof(CrudBase1)}/{{id}}", GetById<CrudBase1, Guid>).WithTags(nameof(App));
        app.MapPost($"{_prefix}/{nameof(CrudBase1)}", Create<CrudBase1, Guid>).WithTags(nameof(App));
        app.MapPost($"{_prefix}/{nameof(CrudBase1)}/range", CreateMany<CrudBase1, Guid>).WithTags(nameof(App));
        app.MapPut($"{_prefix}/{nameof(CrudBase1)}/{{id}}", Update<CrudBase1, Guid>).WithTags(nameof(App));
        app.MapPut($"{_prefix}/{nameof(CrudBase1)}", UpdateMany<CrudBase1, Guid>).WithTags(nameof(App));
        app.MapDelete($"{_prefix}/{nameof(CrudBase1)}/{{id}}", Delete<CrudBase1, Guid>).WithTags(nameof(App));
        app.MapDelete($"{_prefix}/{nameof(CrudBase1)}", DeleteMany<CrudBase1, Guid>).WithTags(nameof(App));
        app.MapPost($"{_prefix}/{nameof(CrudBase1)}/merge/{{operation}}", Merge<CrudBase1, Guid>).WithTags(nameof(App));

        // Data.EF.SqlServer sp
        app.MapGet($"{_prefix}/{nameof(CrudBase2)}", GetAll<CrudBase2, Guid>).WithTags(nameof(App));
        app.MapGet($"{_prefix}/{nameof(CrudBase2)}/{{id}}", GetById<CrudBase2, Guid>).WithTags(nameof(App));
        app.MapPost($"{_prefix}/{nameof(CrudBase2)}", Create<CrudBase2, Guid>).WithTags(nameof(App));
        app.MapPost($"{_prefix}/{nameof(CrudBase2)}/range", CreateMany<CrudBase2, Guid>).WithTags(nameof(App));
        app.MapPut($"{_prefix}/{nameof(CrudBase2)}/{{id}}", Update<CrudBase2, Guid>).WithTags(nameof(App));
        app.MapPut($"{_prefix}/{nameof(CrudBase2)}", UpdateMany<CrudBase2, Guid>).WithTags(nameof(App));
        app.MapDelete($"{_prefix}/{nameof(CrudBase2)}/{{id}}", Delete<CrudBase2, Guid>).WithTags(nameof(App));
        app.MapDelete($"{_prefix}/{nameof(CrudBase2)}", DeleteMany<CrudBase2, Guid>).WithTags(nameof(App));
        app.MapPost($"{_prefix}/{nameof(CrudBase2)}/merge/{{operation}}", Merge<CrudBase2, Guid>).WithTags(nameof(App));

        // Data.Mongo
        app.MapGet($"{_prefix}/{nameof(CrudBase3)}", GetAll<CrudBase3, Guid>).WithTags(nameof(App));
        app.MapGet($"{_prefix}/{nameof(CrudBase3)}/{{id}}", GetById<CrudBase3, Guid>).WithTags(nameof(App));
        app.MapPost($"{_prefix}/{nameof(CrudBase3)}", Create<CrudBase3, Guid>).WithTags(nameof(App));
        app.MapPost($"{_prefix}/{nameof(CrudBase3)}/range", CreateMany<CrudBase3, Guid>).WithTags(nameof(App));
        app.MapPut($"{_prefix}/{nameof(CrudBase3)}/{{id}}", Update<CrudBase3, Guid>).WithTags(nameof(App));
        app.MapPut($"{_prefix}/{nameof(CrudBase3)}", UpdateMany<CrudBase3, Guid>).WithTags(nameof(App));
        app.MapDelete($"{_prefix}/{nameof(CrudBase3)}/{{id}}", Delete<CrudBase3, Guid>).WithTags(nameof(App));
        app.MapDelete($"{_prefix}/{nameof(CrudBase3)}", DeleteMany<CrudBase3, Guid>).WithTags(nameof(App));
        app.MapPost($"{_prefix}/{nameof(CrudBase3)}/merge/{{operation}}", Merge<CrudBase3, Guid>).WithTags(nameof(App));

        // Data.FileSystem
        app.MapGet($"{_prefix}/{nameof(User2)}", GetAll<User2, int>).WithTags(nameof(App));
        app.MapGet($"{_prefix}/{nameof(User2)}/{{id}}", GetById<User2, int>).WithTags(nameof(App));
        app.MapPost($"{_prefix}/{nameof(User2)}", Create<User2, int>).WithTags(nameof(App));
        app.MapPost($"{_prefix}/{nameof(User2)}/range", CreateMany<User2, int>).WithTags(nameof(App));
        app.MapPut($"{_prefix}/{nameof(User2)}/{{id}}", Update<User2, int>).WithTags(nameof(App));
        app.MapPut($"{_prefix}/{nameof(User2)}", UpdateMany<User2, int>).WithTags(nameof(App));
        app.MapDelete($"{_prefix}/{nameof(User2)}/{{id}}", Delete<User2, int>).WithTags(nameof(App));
        app.MapDelete($"{_prefix}/{nameof(User2)}", DeleteMany<User2, int>).WithTags(nameof(App));
        app.MapPost($"{_prefix}/{nameof(User2)}/merge/{{operation}}", Merge<User2, int>).WithTags(nameof(App));

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

        app.MapGet($"{_prefix}/{nameof(x.core.Models.Todo)}", GetAll<x.core.Models.Todo, int>).WithTags(nameof(App));
        app.MapGet($"{_prefix}/{nameof(x.core.Models.Todo)}/{{id}}", GetById<x.core.Models.Todo, int>).WithTags(nameof(App));
        app.MapPost($"{_prefix}/{nameof(x.core.Models.Todo)}", Create<x.core.Models.Todo, int>).WithTags(nameof(App));
        app.MapPut($"{_prefix}/{nameof(x.core.Models.Todo)}/{{id}}", Update<x.core.Models.Todo, int>).WithTags(nameof(App));
        app.MapDelete($"{_prefix}/{nameof(x.core.Models.Todo)}/{{id}}", Delete<x.core.Models.Todo, int>).WithTags(nameof(App));
    }
}
