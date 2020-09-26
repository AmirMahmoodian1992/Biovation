using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using App.Metrics.AspNetCore;

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

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog().UseMetrics()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
