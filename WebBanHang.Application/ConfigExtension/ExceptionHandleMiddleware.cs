using System.Net;
using System.Text.Json;

namespace WebBanHang.Application.ConfigExtension
{
    public class ExceptionHandleMiddleware
    {
        private readonly RequestDelegate next;

        public ExceptionHandleMiddleware(RequestDelegate next) { this.next = next; }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            } catch (Exception ex)
            {
                var respone = context.Response;
                respone.ContentType = "application/json";

                switch (ex)
                {
                    case AggregateException e:
                        respone.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case InvalidDataException e:
                        respone.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    case InvalidOperationException e:
                        respone.StatusCode = (int)HttpStatusCode.NotExtended;
                        break;
                    case ArgumentOutOfRangeException e:
                        respone.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    default:
                        respone.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var result = JsonSerializer.Serialize(new { message = ex?.Message });
                await respone.WriteAsync(result);
            }
        }
    }

    public static class ExceptionMiddleware
    {
        public static IApplicationBuilder DetectExceptionMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandleMiddleware>();
        }
    }
}
