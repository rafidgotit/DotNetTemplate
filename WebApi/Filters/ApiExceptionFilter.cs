using Sugary.Framework.Exceptions;
using Sugary.WepApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;
using System;
using System.Net;
using Microsoft.Identity.Client;
using Logger = NLog.Logger;

namespace Sugary.WebApi.Filters
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        protected static Logger Logger = LogManager.GetCurrentClassLogger();
        public override void OnException(ExceptionContext context)
        {
            Logger.Error(context.Exception);

            ApiError apiError = null;
            if (context.Exception is ApiException)
            {
                // handle explicit 'known' API errors
                var ex = context.Exception as ApiException;
                context.Exception = null;
                apiError = new ApiError(ex.HttpStatusCode, ex.Title, details: ex.Detail);

                context.HttpContext.Response.StatusCode = (int)ex.HttpStatusCode;
            }
            else if (context.Exception is UnauthorizedAccessException)
            {
                apiError = new ApiError(HttpStatusCode.Unauthorized, "Unauthorized Access");
                context.HttpContext.Response.StatusCode = 401;
            }
            else
            {
                // Unhandled errors
#if !DEBUG
                var msg = context.Exception.GetBaseException().Message;
                string stack = null;
#else
                var msg = context.Exception.GetBaseException().Message;
                string stack = context.Exception.StackTrace;
#endif

                apiError = new ApiError(HttpStatusCode.InternalServerError, msg);
                context.HttpContext.Response.StatusCode = 500;
                // handle logging here
            }

            // always return a JSON result
            context.Result = new JsonResult(apiError);

            base.OnException(context);
        }
    }
}
