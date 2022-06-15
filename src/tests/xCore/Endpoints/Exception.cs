using Carter;
using Ws.Core.Extensions.Data;
using xCore.Models;

namespace xCore.Endpoints;

public class Exception : CrudOp, ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var _prefix = $"/api/minimal/{nameof(xCore.Endpoints.Exception).ToLower()}";
        app.MapGet($"{_prefix}/argumentOutOfRange/{{name}}/{{value}}", GetArgumentOutOfRangeException).WithTags(nameof(xCore.Endpoints.Exception));
    }

    public System.Exception GetArgumentOutOfRangeException(string name, string value)
    {
        System.Exception ex;
        try
        {
            throw new ArgumentOutOfRangeException(name, value, "Some out of range error occured");
        }
        catch (System.Exception inny)
        {
            try
            {
                throw new System.Exception("I AM ERROR", inny);
            }
            catch (System.Exception outer)
            {
                ex = outer;
            }
        }
        return ex;
    }
}
