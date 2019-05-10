using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PetaPocoEfCoreMvc
{
    using MQTTnet.AspNetCore;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).UseKestrel(
                    host =>
                        {
                            //host.ListenAnyIP(1883,lis=>lis.UseMqtt());
                            host.ListenAnyIP(5000);
                        })
                .UseStartup<Startup>();
    }
}
