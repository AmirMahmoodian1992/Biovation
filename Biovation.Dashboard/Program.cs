using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Biovation.Dashboard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            //PerformanceCounter cpuCounter;
            //PerformanceCounter ramCounter;

            //cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            //ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).UseSerilog()
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();
                    });
        }
    }
}
