using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Ws.Core.Extensions.Data;
using Ws.Core.Extensions.Data.Cache;

namespace Ws.Core.Extensions.Api.Controllers
{
    public class LoggerController : BaseController
    {
        protected ILogger _logger;
        public LoggerController(IHttpContextAccessor ctx, ILogger logger) : base(ctx)
        {
            _logger = logger;
        }
    }

    public class LoggerController<T, TKey> : BaseController<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        protected ILogger _logger;
        public LoggerController(IRepository<T, TKey> repository, IHttpContextAccessor ctx, ILogger logger) : base(repository, ctx)
        {
            _logger = logger;
        }
    }

    public class LoggerCachedController<T, TKey> : BaseCachedController<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        protected ILogger _logger;
        public LoggerCachedController(IRepository<T, TKey> repository, ICacheRepository<T, TKey> cached, IHttpContextAccessor ctx, ILogger logger) : base(repository, cached, ctx)
        {
            _logger = logger;
        }
    }
}
