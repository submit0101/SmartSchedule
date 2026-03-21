using EduRoomLoad.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.IO;

var pathToContentRoot = Directory.GetCurrentDirectory();

var configuration = new ConfigurationBuilder()
    .SetBasePath(pathToContentRoot)
    .AddJsonFile("/usr/share/SmartSchedule/appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseContentRoot(pathToContentRoot);
        webBuilder.UseConfiguration(configuration);
        webBuilder.UseStartup<Startup>();
    })
    .UseSerilog(logger);

host.Build().Run();
