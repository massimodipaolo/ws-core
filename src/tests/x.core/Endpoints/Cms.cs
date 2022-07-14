using Carter;
using Ws.Core.Extensions.Data;
using x.core.Models;

namespace x.core.Endpoints;

public class Cms : CrudOp, ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var _prefix = "/api/cms";
        app.MapGet($"{_prefix}/{nameof(Models.Cms.Admin_User)}", GetAll<Models.Cms.Admin_User, int>).WithTags(nameof(Cms));
        app.MapGet($"{_prefix}/{nameof(Models.Cms.Admin_User)}/{{id}}", GetById<Models.Cms.Admin_User, int>).WithTags(nameof(Cms));

        app.MapGet($"{_prefix}/{nameof(Models.Cms.Admin_Role)}", GetAll<Models.Cms.Admin_Role, int>).WithTags(nameof(Cms));
        app.MapGet($"{_prefix}/{nameof(Models.Cms.Admin_Role)}/{{id}}", GetById<Models.Cms.Admin_Role, int>).WithTags(nameof(Cms));

        app.MapGet($"{_prefix}/{nameof(Models.Cms.Admin_Permission)}", GetAll<Models.Cms.Admin_Permission, int>).WithTags(nameof(Cms));
        app.MapGet($"{_prefix}/{nameof(Models.Cms.Admin_Permission)}/{{id}}", GetById<Models.Cms.Admin_Permission, int>).WithTags(nameof(Cms));

    }
}
