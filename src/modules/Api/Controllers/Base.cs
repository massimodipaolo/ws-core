using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using Ws.Core.Extensions.Data;
using Ws.Core.Extensions.Data.Cache;

namespace Ws.Core.Extensions.Api.Controllers
{
    public class BaseController : ControllerBase
    {
        protected IHttpContextAccessor _ctx;
        public BaseController(IHttpContextAccessor ctx)
        {
            _ctx = ctx;
        }
        public static ObjectResult Error(Exception ex, ILogger logger = null, HttpStatusCode statusCode = HttpStatusCode.InternalServerError, string message = null) => ExceptionResponse.Result(ex, logger, statusCode, message);
    }

    public class BaseController<T, TKey> : EntityController<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        protected IHttpContextAccessor _ctx;
        public BaseController(IRepository<T, TKey> repository, IHttpContextAccessor ctx) : base(repository)
        {
            _ctx = ctx;
        }
        public static ObjectResult Error(Exception ex, ILogger logger = null, HttpStatusCode statusCode = HttpStatusCode.InternalServerError, string message = null) => ExceptionResponse.Result(ex, logger, statusCode, message);
    }

    public class BaseCachedController<T, TKey> : EntityCachedController<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        protected IHttpContextAccessor _ctx;
        public BaseCachedController(IRepository<T, TKey> repository, ICacheRepository<T, TKey> cached, IHttpContextAccessor ctx) : base(repository, cached)
        {
            _ctx = ctx;
        }
        public static ObjectResult Error(Exception ex, ILogger logger = null, HttpStatusCode statusCode = HttpStatusCode.InternalServerError, string message = null) => ExceptionResponse.Result(ex, logger, statusCode, message);
    }
}
