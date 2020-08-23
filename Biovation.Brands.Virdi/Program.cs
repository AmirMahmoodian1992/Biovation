using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Biovation.CommonClasses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Biovation.Brands.Virdi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Log.Logger = new LoggerConfiguration()
            //    .Enrich.FromLogContext()
            //    .MinimumLevel.Verbose()
            //    .Enrich.With(new ThreadIdEnricher())
            //    .Enrich.WithProperty("Version", Assembly.GetExecutingAssembly().GetName().Version)
            //    .WriteTo.Console(
            //        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}, {ThreadId}] {Message}{NewLine}{Exception}"
            //        /*,restrictedToMinimumLevel: minimumConsoleLogLevel*/)
            //    .CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
