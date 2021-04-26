using System;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using LinqToDB.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using Vysotsky.API.Hubs;
using Vysotsky.API.Infrastructure;
using Vysotsky.Data;
using Vysotsky.Service.Impl;
using Vysotsky.Service.Interfaces;

namespace Vysotsky.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration) =>
            Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) => ConfigureServices(services, Configuration);

        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            ConfigureTelemetry(services);
            ConfigureDocumentation(services);
            ConfigureDatabase(services);
            ConfigureInfrastructure(services, configuration);
            ConfigureDomainServices(services);
            ConfigureSecretServices(services);
        }

        private static void ConfigureDocumentation(IServiceCollection services) =>
            services
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
                });

        private static void ConfigureTelemetry(IServiceCollection services) =>
            services.AddHealthChecks();

        private static void ConfigureDatabase(IServiceCollection services) =>
            services
                .AddScoped(s =>
                {
                    var logger = s.GetRequiredService<ILogger<VysotskyDataConnection>>();
                    var connectionString =
                        s.GetRequiredService<IConfiguration>().GetValue<string>("PG_CONNECTION_STRING");
                    var options = new LinqToDbConnectionOptionsBuilder()
                        .UsePostgreSQL(connectionString)
                        .WithTraceLevel(TraceLevel.Info)
                        .WriteTraceWith((text, category, l) =>
                        {
                            if (category != null && text != null && l != TraceLevel.Off)
                            {
                                FormattableString str = $"{category:LinqToDbCategory}: {text:LinqToDbText}";
                                switch (l)
                                {
                                    case TraceLevel.Error:
                                        logger.InterpolatedError(str);
                                        break;
                                    case TraceLevel.Warning:
                                        logger.InterpolatedWarning(str);
                                        break;
                                    case TraceLevel.Info:
                                        logger.InterpolatedInformation(str);
                                        break;
                                    case TraceLevel.Verbose:
                                        logger.InterpolatedDebug(str);
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException(nameof(l), l, null);
                                }
                            }
                        })
                        .Build<VysotskyDataConnection>();
                    var connection = new VysotskyDataConnection(options);
                    return connection;
                });

        private static void ConfigureInfrastructure(IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.SecurityTokenValidators.Clear();
                    options.SecurityTokenValidators.Add(new RevokableJwtSecurityTokenHandler());
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateTokenReplay = false,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(configuration.GetValue<string>("SECRET"))),
                    };
                });
            services
                .AddHttpContextAccessor()
                .AddAuthorizationCore()
                .AddSingleton<ICurrentUserProvider, CurrentUserProvider>()
                .AddSingleton<UnhandledExceptionMiddleware>()
                .AddScoped<RevokableAuthenticationMiddleware>()
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.Converters.Add(new DateTimeOffsetConverter());
                    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
                });
            services.AddSignalR();
        }

        private static void ConfigureSecretServices(IServiceCollection services)
        {
            services.AddSingleton(s => new SecureHasherOptions
            {
                Salt = s.GetRequiredService<IConfiguration>().GetValue<string>("SALT")
            });

            services.AddSingleton(s => new AuthenticationServiceOptions
            {
                Secret = s.GetRequiredService<IConfiguration>().GetValue<string>("SECRET")
            });
        }

        private static void ConfigureDomainServices(IServiceCollection services)
        {
            services.AddScoped<IEventBus, SignalREventBus>();
            services.Scan(t =>
                t.FromAssemblyOf<IStringHasher>()
                    .AddClasses(f =>
                        f.InExactNamespaces("Vysotsky.Service.Impl")
                            .Where(x => x.IsPublic))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) =>
            ConfigureWebApp(app, env);

        public static void ConfigureWebApp(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(c => c
                .AllowAnyHeader()
                .AllowAnyOrigin()
                .AllowAnyMethod());

            app.UseHealthChecks("/api/health");
            app.UseSwaggerUi3(c => c.Path = "/swagger");
            app.UseReDoc(c => c.Path = "/redoc");
            app.UseOpenApi();

            app.UseMiddleware<UnhandledExceptionMiddleware>();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<RevokableAuthenticationMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotificationHub>("/notification-hub");
                endpoints.MapHub<ChatHub>("/chat-hub");
                endpoints.MapControllers();
            });
        }
    }
}
