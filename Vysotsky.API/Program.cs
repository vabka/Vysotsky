using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HostFiltering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Vysotsky.API;

Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(_ => { })
    .ConfigureWebHost(builder =>
    {
        builder
            // Defaults
            .UseKestrel((builderContext, options) =>
            {
                options.Configure(builderContext.Configuration.GetSection("Kestrel"), reloadOnChange: true);
            })
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
                            {Length: >0} => hosts,
                            _ => new[] {"*"}
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