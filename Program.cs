using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MTC.WebApp.BackOffice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());

            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            //CreateWebHostBuilder(args).Build().Run();
            CreateHostBuilder(args).Build().Run();

        }

        /*public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel(c => c.AddServerHeader = false)
                .UseStartup<Startup>();*/

        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder.ConfigureKestrel(options => {
                       var http2 = options.Limits.Http2;
                       http2.InitialConnectionWindowSize = 10485760; //Byte To 10 MB
                       http2.InitialStreamWindowSize = 15242880; //Byte To 5 MB

                   });
                   webBuilder.UseIISIntegration();
                   webBuilder.UseStartup<Startup>();
               });
    }
}
