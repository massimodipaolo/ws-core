using Carter;
using Ws.Core.Extensions.Data;

namespace xCore.Endpoints;

public class Todo : ICarterModule, IEntity<string>
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Title { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public bool IsComplete { get; set; } = false;
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/todo", GetAll);
        app.MapGet("/api/todo/{id}", GetById);
        app.MapPost("/api/todo", Create);
        app.MapPut("/api/todo/{id}", Update);
        app.MapDelete("/api/todo/{id}", Delete);
    }

    private IEnumerable<Todo> GetAll(IRepository<Todo, string> _repo) => _repo.List;
    private IResult GetById(string id, IRepository<Todo, string> _repo) =>
        _repo.Find(id) is Todo todo
                ? Results.Ok(todo)
                : Results.NotFound();
    private IResult Create(Todo todo, IRepository<Todo, string> _repo)
    {
        todo.IsComplete = false;
        _repo.Add(todo);
        return Results.Created($"/todo/{todo.Id}", todo);
    }
    private IResult Delete(string id, IRepository<Todo, string> _repo) {
        if (_repo.Find(id) is Todo todo) {
            _repo.Delete(todo);
            return Results.NoContent();
        } 
        else
            return Results.NotFound(); 
    }
    private IResult Update(string id, IRepository<Todo, string> _repo)
    {
        if (_repo.Find(id) is Todo todo)
        {
            _repo.Update(todo);
            return Results.NoContent();
        }
        else
            return Results.NotFound();
    }
}

/*
public class CrudEntity<T,TKey> : ICarterModule where T: class, IEntity<TKey> where TKey : IEquatable<TKey>
{    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"/api/{typeof(T)}", GetAll);
        app.MapGet($"/api/{typeof(T)}/{{id}}", GetById);
        app.MapPost($"/api/{typeof(T)}", Create);
        app.MapPut($"/api/{typeof(T)}/{{id}}", Update);
        app.MapDelete($"/api/{typeof(T)}/{{id}}", Delete);
    }

    private IEnumerable<T> GetAll(IRepository<T, TKey> _repo) => _repo.List;
    private IResult GetById(TKey id, IRepository<T, TKey> _repo) =>
        _repo.Find(id) is T item
                ? Results.Ok(item)
                : Results.NotFound();
    private IResult Create(T item, IRepository<T, TKey> _repo)
    {
        _repo.Add(item);
        return Results.Created($"/{typeof(T)}/{item.Id}", item);
    }
    private IResult Delete(TKey id, IRepository<T, TKey> _repo)
    {
        if (_repo.Find(id) is T item)
        {
            _repo.Delete(item);
            return Results.NoContent();
        }
        else
            return Results.NotFound();
    }
    private IResult Update(TKey id, IRepository<T, TKey> _repo)
    {
        if (_repo.Find(id) is T item)
        {
            _repo.Update(item);
            return Results.NoContent();
        }
        else
            return Results.NotFound();
    }
}
*/