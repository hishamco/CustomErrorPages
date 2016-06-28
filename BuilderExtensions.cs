using Microsoft.AspNetCore.Builder;

namespace CustomErrorPages
{
    public static class BuilderExtensions
    {
        public static IApplicationBuilder UseCustomErrorPages(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CustomErrorPagesMiddleware>();
        }
    }
}
