using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Vysotsky.API;

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
    .Build().Run();