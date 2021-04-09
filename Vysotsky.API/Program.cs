using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Vysotsky.API;

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.ConfigureKestrel(k =>
        {
            k.Listen(IPAddress.Any, 5001);
        });
        webBuilder.UseStartup<Startup>();
    })
    .Build().Run();