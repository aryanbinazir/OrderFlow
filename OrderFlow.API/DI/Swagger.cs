using Microsoft.OpenApi;

namespace OrderFlow.API.DI
{
    public static class Swagger
    {
        public static void RegisterSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "OrderFlow API",
                    Version = "v1",
                    Description = "OrderFlow REST API"
                });
            });
        }
    }
}