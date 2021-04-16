using System;
using System.Diagnostics;
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
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using Vysotsky.API.Infrastructure;
using Vysotsky.Data;
using Vysotsky.Data.Entities;
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
            services.AddHealthChecks();
            services
                .AddSingleton<ICurrentUserProvider, CurrentUserProvider>()
                .AddSingleton<UnhandledExceptionMiddleware>()
                .AddScoped<RevokableAuthenticationMiddleware>()
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
                .AddScoped(s =>
                {
                    var logger = s.GetRequiredService<ILogger<VysotskyDataConnection>>();
                    var connectionString =
                        s.GetRequiredService<IConfiguration>().GetValue<string>("PG_CONNECTION_STRING");
                    var options = new LinqToDbConnectionOptionsBuilder()
                        .UsePostgreSQL(connectionString)
                        .WithTraceLevel(TraceLevel.Info)
                        .WriteTraceWith((text, _, l) =>
                        {
                            switch (l)
                            {
                                case TraceLevel.Off:
                                    break;
                                case TraceLevel.Error:
                                    logger.InterpolatedError($"{text}");
                                    break;
                                case TraceLevel.Warning:
                                    logger.InterpolatedWarning($"{text}");
                                    break;
                                case TraceLevel.Info:
                                    logger.InterpolatedInformation($"{text}");
                                    break;
                                case TraceLevel.Verbose:
                                    logger.InterpolatedTrace($"{text}");
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(l), l, null);
                            }
                        })
                        .Build<VysotskyDataConnection>();
                    return new VysotskyDataConnection(options);
                })
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
            services.AddHttpContextAccessor();
            services.AddAuthorizationCore();
            services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.Scan(t =>
                t.FromAssemblyOf<IStringHasher>()
                    .AddClasses(f =>
                        f.InExactNamespaces("Vysotsky.Service.Impl")
                            .Where(x => x.IsPublic))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime());
            services.AddSingleton(s => new SecureHasherOptions
            {
                Salt = s.GetRequiredService<IConfiguration>().GetValue<string>("SALT")
            });
            services.AddSingleton(s => new AuthenticationServiceOptions
            {
                Secret = s.GetRequiredService<IConfiguration>().GetValue<string>("SECRET")
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) =>
            ConfigureWebApp(app, env);

        public static void ConfigureWebApp(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHealthChecks("/api/health");
            app.UseSwaggerUi3();
            app.UseOpenApi();
            app.UseMiddleware<UnhandledExceptionMiddleware>();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<RevokableAuthenticationMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                if (env.IsDevelopment())
                {
                    endpoints.MapGet("/test", async ctx =>
                    {
                        var hasher = ctx.RequestServices.GetRequiredService<IStringHasher>();
                        var database = ctx.RequestServices.GetRequiredService<VysotskyDataConnection>();
                        var authService = ctx.RequestServices.GetRequiredService<IAuthenticationService>();
                        if (!await database.Users.AnyAsync(i => i.Username == "test_superuser"))
                        {
                            await using var transaction = await database.BeginTransactionAsync();
                            var hash = hasher.Hash("test");
                            await database.Users.InsertAsync(() => new UserRecord
                            {
                                Username = "test_superuser",
                                PasswordHash = hash,
                                Role = UserRole.SuperUser,
                                FirstName = "Админ",
                                LastName = "Админович",
                                Contacts = Array.Empty<UserContact>(),
                                LastPasswordChange = DateTimeOffset.MinValue
                            });
                            await database.Users.InsertAsync(() => new UserRecord
                            {
                                Username = "test_supervisor",
                                PasswordHash = hash,
                                Role = UserRole.Supervisor,
                                FirstName = "Диспетчер",
                                LastName = "Админович",
                                Contacts = Array.Empty<UserContact>(),
                                LastPasswordChange = DateTimeOffset.MinValue
                            });
                            await database.Users.InsertAsync(() => new UserRecord
                            {
                                Username = "test_worker",
                                PasswordHash = hash,
                                Role = UserRole.Worker,
                                FirstName = "Работник",
                                LastName = "Админович",
                                Contacts = Array.Empty<UserContact>(),
                                LastPasswordChange = DateTimeOffset.MinValue
                            });
                            await transaction.CommitAsync();
                        }

                        await ctx.Response.WriteAsJsonAsync(new
                        {
                            superuser = new
                            {
                                username = "test_superuser",
                                token = await authService.TryIssueTokenByUserCredentialsAsync("test_superuser", "test",
                                    true),
                            },
                            supervisor = new
                            {
                                username = "test_supervisor",
                                token = await authService.TryIssueTokenByUserCredentialsAsync("test_supervisor",
                                    "test",
                                    true),
                            },
                            worker = new
                            {
                                username = "test_worker",
                                token = await authService.TryIssueTokenByUserCredentialsAsync("test_worker",
                                    "test",
                                    true),
                            }
                        }, new JsonSerializerOptions {WriteIndented = true});
                    });
                    endpoints.MapPost("/api/users/admin", async ctx =>
                    {
                        var hasher = ctx.RequestServices.GetRequiredService<IStringHasher>();
                        var database = ctx.RequestServices.GetRequiredService<VysotskyDataConnection>();
                        var hash = hasher.Hash("admin");
                        await database.Users.InsertAsync(() => new UserRecord
                        {
                            Username = "admin",
                            PasswordHash = hash,
                            Role = UserRole.SuperUser,
                            FirstName = "Админ",
                            LastName = "Админович",
                            Contacts = Array.Empty<UserContact>(),
                            LastPasswordChange = DateTimeOffset.Now
                        });
                        await ctx.Response.WriteAsync("OK");
                    });
                    endpoints.MapGet("/api/currentUser", async ctx =>
                    {
                        var currentUser = ctx.RequestServices.GetRequiredService<ICurrentUserProvider>().CurrentUser;
                        await ctx.Response.WriteAsJsonAsync(currentUser,
                            new JsonSerializerOptions {Converters = {new JsonStringEnumConverter()}});
                    });
                }
            });
        }
    }
}
