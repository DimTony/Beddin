using Beddin.API.Middleware;

namespace Beddin.API.Extensions
{
    public static class ApplicationExtensions
    {
        public static WebApplication UseApiMiddleware(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Beddin API v1");
                    options.RoutePrefix = "swagger";
                });
            }

            app.UseCors(app.Environment.IsDevelopment() ? "Development" : "Production");

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            if (!app.Environment.IsDevelopment())
                app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<SessionValidationMiddleware>();
            app.UseMiddleware<PasswordPolicyMiddleware>();

            app.MapControllers();

            return app;
        }

    }
}
