using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace Ws.Core.Extensions.Api.Controllers
{
    public class ExceptionResponse
    {
        public static ObjectResult Result(Exception ex, ILogger logger, HttpStatusCode statusCode, string Message = null)
        {
            if (logger != null)
                switch (statusCode)
                {
                    case HttpStatusCode.InternalServerError:
                        logger.LogError(ex, ex.Message);
                        break;
                    case HttpStatusCode.BadRequest:
                        logger.LogWarning(ex, ex.Message);
                        break;
                    default:
                        logger.LogInformation(ex, ex.Message);
                        break;
                }

            return new ObjectResult(new ExceptionType(Message ?? ex.Message)) { StatusCode = (int)statusCode };
        }

        public static ObjectResult SessionExpired(ILogger logger = null) => Result(new Exception("Session expired"), logger, HttpStatusCode.BadRequest);
    }

    public class ExceptionType
    {
        public ExceptionType(string message) => Message = message;

        public string Message { get; set; }
    }
}
