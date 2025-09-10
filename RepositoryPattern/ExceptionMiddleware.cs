using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace RepositoryPattern
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // pass to next middleware
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync($"🔥 Something went wrong: {ex.Message}");
            }
        }
    }
}
