using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HostFiltering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Vysotsky.API;

Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(_ => { })
    .UseSerilog((_, _, l) =>
        l
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .MinimumLevel.Information()
    )
    .ConfigureWebHost(builder => builder
        .UseKestrel((builderContext, options) =>
            options.Configure(builderContext.Configuration.GetSection("Kestrel"), true)
                .AnyIPEndpoint(3001))
        .ConfigureServices((hostingContext, services) =>
        {
            services.PostConfigure<HostFilteringOptions>(options =>
            {
                if (options.AllowedHosts is null or {Count: 0})
                {
                    var hosts = hostingContext
                        .Configuration["AllowedHosts"]
                        ?.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
                    options.AllowedHosts = hosts switch
                    {
                        {Length: > 0} => hosts,
                        _             => new[] {"*"}
                    };
                }
            });
            services.AddSingleton<IOptionsChangeTokenSource<HostFilteringOptions>>(
                new ConfigurationChangeTokenSource<HostFilteringOptions>(hostingContext.Configuration));
            services.AddRouting();
        })
        .ConfigureServices((ctx, services) => Startup.ConfigureServices(services, ctx.Configuration))
        .Configure(app => app.UseHostFiltering())
        .Configure(app =>
        {
            app.UseSerilogRequestLogging();
        })
        .Configure((ctx, app) => Startup.ConfigureWebApp(app, ctx.HostingEnvironment)))
    .Build()
    .Run();
