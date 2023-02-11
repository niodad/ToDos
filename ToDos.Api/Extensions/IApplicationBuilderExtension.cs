using ToDos.Api.Middleware;

namespace ToDos.Api.Extensions
{
    public static class Extensions
    {

        public static void AddToDosMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
