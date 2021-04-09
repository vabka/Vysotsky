using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using LinqToDB;
using LinqToDB.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using Vysotsky.API.Infrastructure;
using Vysotsky.Data;
using Vysotsky.Data.Entities;
using Vysotsky.Service;

namespace Vysotsky.API
{
    internal class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<UnhandledExceptionMiddleware>()
                .AddOpenApiDocument(document =>
                {
                    // Add an authenticate button to Swagger for JWT tokens
                    document.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT"));
                    document.DocumentProcessors.Add(new SecurityDefinitionAppender("JWT", new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Description =
                            "Type into the textbox: Bearer {your JWT token}. You can get a JWT token from /Authorization/Authenticate."
                    }));
                })
                .AddAuthenticationService()
                .AddSecureHasher()
                .AddScoped(s =>
                {
                    var connectionString =
                        s.GetRequiredService<IConfiguration>().GetValue<string>("PG_CONNECTION_STRING");
                    var options = new LinqToDbConnectionOptionsBuilder()
                        .UsePostgreSQL(connectionString)
                        .Build<VysotskyDataConnection>();
                    return new VysotskyDataConnection(options);
                })
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateActor = false,
                        ValidateTokenReplay = false,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(Configuration.GetValue<string>("SECRET")))
                    };
                });
            services.AddScoped<BlockedTokenMiddleware>();
            services.AddHttpContextAccessor();
            services.AddAuthorizationCore();
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
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHsts();
            // Add OpenAPI/Swagger middlewares
            app.UseSwaggerUi3(); // Serves the Swagger UI 3 web ui to view the OpenAPI/Swagger documents by default on `/swagger`
            app.UseOpenApi(); // Serves the registered OpenAPI/Swagger documents by default on `/swagger/{documentName}/swagger.json`
            app.UseMiddleware<UnhandledExceptionMiddleware>();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<BlockedTokenMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                if (env.IsDevelopment())
                    endpoints.MapPost("/users/admin/create", async ctx =>
                    {
                        var hasher = ctx.RequestServices.GetRequiredService<IStringHasher>();
                        var database = ctx.RequestServices.GetRequiredService<VysotskyDataConnection>();
                        var hash = hasher.Hash("admin");
                        await database.Users.InsertAsync(() => new User
                        {
                            Username = "admin",
                            PasswordHash = hash,
                            Role = UserRole.SuperUser,
                            FirstName = "Админ",
                            LastName = "Админович",
                            Contacts = Array.Empty<UserContact>()
                        });
                        await ctx.Response.WriteAsync("OK");
                    });
            });
        }
    }
}