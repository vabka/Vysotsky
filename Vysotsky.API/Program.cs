using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HostFiltering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vysotsky.API;

Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(_ => { })
    .ConfigureLogging((ctx, builder) =>
    {
        builder.ClearProviders();
        if (ctx.HostingEnvironment.IsProduction() || ctx.HostingEnvironment.IsStaging())
        {
            builder.AddJsonConsole(c =>
            {
                c.UseUtcTimestamp = true;
                c.IncludeScopes = true;
                c.JsonWriterOptions = new JsonWriterOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    Indented = false
                };
            });
        }
        else
        {
            builder.AddConsole(c => c.FormatterName = "simple");
        }
    })
    .ConfigureWebHost(builder =>
    {
        builder
     // Defaults
     .UseKestrel((builderContext, options) =>
         options.Configure(builderContext.Configuration.GetSection("Kestrel"), true))
     .ConfigureServices((hostingContext, services) =>
     {
         services.PostConfigure<HostFilteringOptions>(options =>
         {
             if (options.AllowedHosts is null or { Count: 0 })
             {
                 var hosts = hostingContext
                     .Configuration["AllowedHosts"]
                     ?.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                 options.AllowedHosts = hosts switch
                 {
                     { Length: > 0 } => hosts,
                     _ => new[] { "*" }
                 };
             }
         });
         services.AddSingleton<IOptionsChangeTokenSource<HostFilteringOptions>>(
             new ConfigurationChangeTokenSource<HostFilteringOptions>(hostingContext.Configuration));
         services.AddRouting();
     })
     .ConfigureServices((ctx, services) => Startup.ConfigureServices(services, ctx.Configuration))
     .Configure((_, app) => app.UseHostFiltering())
     .Configure((ctx, app) => Startup.ConfigureWebApp(app, ctx.HostingEnvironment));
    })
    .Build()
    .Run();
