using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HostFiltering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Vysotsky.API;

Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(_ => { })
    .UseSerilog((_, _, l) =>
        l
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
            {
                AutoRegisterTemplate = true, AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6
            }).MinimumLevel.Information()
    )
    .ConfigureWebHost(builder => builder
        // Defaults
        .UseKestrel((builderContext, options) =>
            options.Configure(builderContext.Configuration.GetSection("Kestrel"), true))
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
