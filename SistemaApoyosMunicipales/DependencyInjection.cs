using Microsoft.Extensions.DependencyInjection;

namespace SistemaApoyosMunicipales.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(
            this IServiceCollection services
        )
        {
            services.AddControllers();

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen();



            return services;
        }
    }
}
