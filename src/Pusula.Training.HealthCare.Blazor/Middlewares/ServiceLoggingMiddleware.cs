using Microsoft.AspNetCore.Http;
using Serilog;
using System.IO;
using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.Blazor.Middlewares
{
    public class ServiceLoggingMiddleware : IServiceLoggingMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            Log.ForContext("Method", context.Request.Method)
                .ForContext("QueryString", context.Request.QueryString)
                .ForContext("Referer", context.Request.Headers.Referer)
                .ForContext("RequestBody", await RequestBodyVisualizer(context))
                .ForContext("Status", context.Response.StatusCode)
                .ForContext("SourceContext", "HealthCareServiceRequest")
                .Information($"{context.Request.Path}");

            await next(context);
        }

        private async Task<string> RequestBodyVisualizer(HttpContext context)
        {
            context.Request.EnableBuffering();
            var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;
            return requestBody;
        }
    }
}
