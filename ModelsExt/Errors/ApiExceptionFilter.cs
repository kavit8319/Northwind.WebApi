using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Northwind.WebApi.ModelsExt.Errors
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            ApiError apiError;
            if (context.Exception is ApiException ex)
            {
                // handle explicit 'known' API errors
                context.Exception=null!;
                apiError = new ApiError(ex.Message,  ex.InnerException is null?null: ex.InnerException.Message); 

                context.HttpContext.Response.StatusCode = ex.StatusCode;
            }
            else if (context.Exception is UnauthorizedAccessException)
            {
                apiError = new ApiError("Unauthorized Access");
                context.HttpContext.Response.StatusCode = 401;

                // handle logging here
            }
            else
            {
                // Unhandled errors
#if !DEBUG
                var msg = "An unhandled error occurred.";                
                string stack = null;
#else
                var msg = context.Exception.GetBaseException().Message;
                string? stack = context.Exception.StackTrace;
#endif

                apiError = new ApiError(msg);
                apiError.Detail = stack;

                context.HttpContext.Response.StatusCode = 500;

                // handle logging here
            }

            // always return a JSON result
            context.Result = new JsonResult(apiError);

            base.OnException(context);
        }
    }
}
