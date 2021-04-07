using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Vysotsky.API.Infrastructure;

namespace Vysotsky.API
{
    internal class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            services.AddSingleton<UnhandledExceptionMiddleware>();
            // Add OpenAPI/Swagger document
            services.AddOpenApiDocument(c => { c.DocumentName = "Vysotsky"; });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Add OpenAPI/Swagger middlewares
            app.UseOpenApi(); // Serves the registered OpenAPI/Swagger documents by default on `/swagger/{documentName}/swagger.json`
            app.UseSwaggerUi3(); // Serves the Swagger UI 3 web ui to view the OpenAPI/Swagger documents by default on `/swagger`

            app.UseMiddleware<UnhandledExceptionMiddleware>();
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
