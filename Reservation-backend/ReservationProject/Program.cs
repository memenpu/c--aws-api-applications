using Amazon.Extensions.NETCore.Setup;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationProject
{
    public class Program
    {
#pragma warning disable CS1591
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureAppConfiguration(builder =>
                {
                    builder.AddSystemsManager("/project/", awsOptions: new AWSOptions {Region=Amazon.RegionEndpoint.USEast1 });
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
#pragma warning restore CS1591
    }
}
