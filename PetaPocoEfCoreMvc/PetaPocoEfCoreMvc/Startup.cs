using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PetaPocoEfCoreMvc
{
    using System.Reflection;

    using AutoMapper;

    using Microsoft.EntityFrameworkCore;

    using MQTTnet.AspNetCore;

    using PetaPoco;
    using PetaPoco.Providers;

    using PetaPocoEfCoreMvc.BogusResp;
    using PetaPocoEfCoreMvc.DBContext;
    using PetaPocoEfCoreMvc.Profiles;
    using PetaPocoEfCoreMvc.Service;

    using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
    using MQTTnet.Protocol;
    using MQTTnet.Server;


    public class Startup
    {
        private IServiceProvider _serviceProvider;

        public Startup(IConfiguration configuration,IServiceProvider serviceProvider)
        {
            Configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddDbContext<EfCoreDBContext>(
                options =>
                    {
                        var connectionString = Configuration["ConnectionStrings:MySQL:MvcMySQL"];
                        options.UseMySql(connectionString,
                            myopttion => { myopttion.ServerVersion(new Version(10,2), ServerType.MariaDb); });
                        
                    });

            //services.AddScoped<IDatabase>(
            //    x =>
            //        {
            //            var connectionStrnig = Configuration["ConnectionStrings:MySQL:MvcMySQL"];
                        
            //            var configuration = DatabaseConfiguration.Build().UsingConnectionString(connectionStrnig)
            //                .UsingProvider<MariaDbDatabaseProvider>();
            //            return new PetaPocoMvcDBContext(configuration);
            //        });

            services.AddScoped<IDatabase, PetaPocoMvcDBContext>(
                (x) =>
                    {
                        var connectionStrnig = Configuration["ConnectionStrings:MySQL:MvcMySQL"];

                        var configuration = DatabaseConfiguration.Build().UsingConnectionString(connectionStrnig)
                            .UsingProvider<MariaDbDatabaseProvider>();
                        return new PetaPocoMvcDBContext(configuration);
                    });

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISampleCustomerRepository, SampleCustomerRepository>();


            //Mqtt
            services.AddHostedMqttServerWithServices(
                builder =>
                    {
                        builder.WithDefaultEndpoint();
                        builder.WithConnectionValidator(
                            c =>
                                {
                                    //从IServiceCollection中构建     ServiceProvider, 用以使用注入访问数据库的服务
                                    var serprovider = services.BuildServiceProvider();
                                    var us = serprovider.GetService(typeof(IUserService)) as IUserService;
                                    var x = us.GetAll();
                                    if (c.ClientId.Length < 5)
                                    {
                                        c.ReturnCode = MqttConnectReturnCode.ConnectionRefusedIdentifierRejected;
                                        return;
                                    }

                                    if (c.Username != "admin")
                                    {
                                        c.ReturnCode = MqttConnectReturnCode.ConnectionRefusedBadUsernameOrPassword;
                                        return;
                                    }

                                    if (c.Password != "admin")
                                    {
                                        c.ReturnCode = MqttConnectReturnCode.ConnectionRefusedBadUsernameOrPassword;
                                        return;
                                    }
                                    
                                    c.ReturnCode = MqttConnectReturnCode.ConnectionAccepted;
                                });
                    });
            //this adds tcp server support based on System.Net.Socket
            services.AddMqttTcpServerAdapter();
            //this adds tcp server support based on Microsoft.AspNetCore.Connections.Abstractions
            services.AddMqttConnectionHandler();
            //this adds websocket support
            services.AddMqttWebSocketServerAdapter();

            //
            var ass = Assembly.GetEntryAssembly().GetReferencedAssemblies().Select(Assembly.Load).SelectMany(y => y.DefinedTypes)
                .Where(type => typeof(IProfile).GetTypeInfo().IsAssignableFrom(type.AsType()));

            services.AddAutoMapper(cfg => { },new List<Assembly>(){ Assembly.GetEntryAssembly() });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseMqttEndpoint();

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
