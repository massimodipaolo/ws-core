using Carter;

namespace x.core.Endpoints;

public class Store : CrudOp, ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var _prefix = "/api/store";
        app.MapGet($"{_prefix}/{nameof(Models.Store.Brand)}", GetAll<Models.Store.Brand, int>).WithTags(nameof(Store));
        app.MapGet($"{_prefix}/{nameof(Models.Store.Brand)}/{{id}}", GetById<Models.Store.Brand, int>).WithTags(nameof(Store));

        app.MapGet($"{_prefix}/{nameof(Models.Store.Category)}", GetAll<Models.Store.Category, int>).WithTags(nameof(Store));
        app.MapGet($"{_prefix}/{nameof(Models.Store.Category)}/{{id}}", GetById<Models.Store.Category, int>).WithTags(nameof(Store));

        app.MapGet($"{_prefix}/{nameof(Models.Store.Product)}", GetAll<Models.Store.Product, int>).WithTags(nameof(Store));
        app.MapGet($"{_prefix}/{nameof(Models.Store.Product)}/{{id}}", GetById<Models.Store.Product, int>).WithTags(nameof(Store));

    }
}
