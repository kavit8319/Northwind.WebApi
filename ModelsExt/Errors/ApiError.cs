namespace Northwind.WebApi.ModelsExt.Errors
{
    public class ApiException : Exception
    {
        public int StatusCode { get; set; }
        public ApiException(string message,
            int statusCode = 500) :
            base(message)
        {
            StatusCode = statusCode;
        }
        public ApiException(Exception ex, int statusCode = 500) : base(ex.Message,ex.InnerException)
        {
            StatusCode = statusCode;
        }
    }

    public class ApiError
    {
        public string Message { get; set; }
        public bool IsError { get; set; }
        public string? Detail { get; set; }

        public ApiError(string message,string? detail=null)
        {
            Message = message;
            Detail = detail;
            IsError = true;
        }
    }
}
