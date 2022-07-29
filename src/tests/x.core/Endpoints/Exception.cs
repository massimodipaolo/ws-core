using Carter;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;

namespace x.core.Endpoints;

public class Exception : CrudOp, ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var _prefix = $"/api/minimal/{nameof(x.core.Endpoints.Exception).ToLower()}";
        app.MapGet($"{_prefix}/400", GetProblemDetails400).WithTags(nameof(x.core.Endpoints.Exception));
        app.MapGet($"{_prefix}/404", GetProblemDetails404).WithTags(nameof(x.core.Endpoints.Exception));
        app.MapGet($"{_prefix}/500", GetProblemDetails500).WithTags(nameof(x.core.Endpoints.Exception));
        app.MapGet($"{_prefix}/500/ex", () => Results.Ok(GetArgumentOutOfRangeException)).WithTags(nameof(x.core.Endpoints.Exception));
        app.MapGet($"{_prefix}/500/throw", ThrowProblemDetails500).WithTags(nameof(x.core.Endpoints.Exception));
    }

    public System.Exception GetArgumentOutOfRangeException(string name, string value)
    {
        System.Exception ex;
        try
        {
            throw new ArgumentOutOfRangeException(name, value, "Some out of range error occured");
        }
        catch (System.Exception inner)
        {
            try
            {
                throw new System.Exception("Oooopss! Holy 500 error, Batman 🦇", inner);
            }
            catch (System.Exception outer)
            {
                ex = outer;
            }
        }
        return ex;
    }

    #region problemDetails
    public IResult GetProblemDetails400(HttpContext ctx, ProblemDetailsFactory problemDetails)
    => _getProblemDetail(ctx, problemDetails);

    public IResult GetProblemDetails404(HttpContext ctx, ProblemDetailsFactory problemDetails)
    => _getProblemDetail(ctx, problemDetails, 404);

    public IResult GetProblemDetails500(HttpContext ctx, ProblemDetailsFactory problemDetails)
    => _getProblemDetail(ctx, problemDetails, 500, ex: GetArgumentOutOfRangeException("foo", "bar"));

    public IResult ThrowProblemDetails500()
    => throw GetArgumentOutOfRangeException("foo", "bar");

    private class ErrorDetails
    {
        public string? Message { get; set; }
        public string? Type { get; set; }
        public string? Raw { get; set; }
    }
    private IResult _getProblemDetail(HttpContext ctx, ProblemDetailsFactory problemDetails, int? statusCode = 400, string? type = null, System.Exception? ex = null)
    {
        Func<System.Exception, ErrorDetails> _getErrorDetail = (_) => new() { Message = _.Message, Type = _.GetType().ToString(), Raw = _.ToString() };

        var problem = problemDetails.CreateProblemDetails(ctx, statusCode, type:type, instance: ctx.Request.Path, detail: ex?.Message);
        if (ex != null)
        {
            var errors = new List<ErrorDetails>
            {
                _getErrorDetail(ex)
            };
            if (ex.InnerException != null)
                errors.Add(_getErrorDetail(ex.InnerException));
            problem.Extensions.Add(Hellang.Middleware.ProblemDetails.ProblemDetailsOptions.DefaultExceptionDetailsPropertyName, errors);
        }
        return Results.Problem(problem);
    }
    #endregion
}
