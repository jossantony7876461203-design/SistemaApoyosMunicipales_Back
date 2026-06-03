using Microsoft.AspNetCore.Builder;

namespace SistemaApoyosMunicipales.API
{
    public static class ApplicationBuilderExtensions
    {
        public static WebApplication UsePresentation(
            this WebApplication app
        )
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();

                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            return app;
        }
    }
}