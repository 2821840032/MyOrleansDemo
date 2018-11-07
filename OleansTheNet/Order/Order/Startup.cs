using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Entity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Serialization;

namespace Order
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            int silePort = 11113; int gatewayPort = 30000; int mainSiloPort = 11111;

           
            //启动Host
            StartAsyncHost(silePort, gatewayPort, mainSiloPort, services);

            //启动本地服务
            StartAsyncClient(mainSiloPort, gatewayPort, silePort, services).Wait();


            services.ServerInjectionADD();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        /// <summary>
        /// 在本地启动一个Host
        /// </summary>
        /// <returns></returns>
        async Task<ISiloHost> StartAsyncHost(int silePort, int gatewayPort, int mainSiloPort, IServiceCollection servicesCollection)
        {
            var builder = new SiloHostBuilder()

                   .Configure<SerializationProviderOptions>(d => { d.SerializationProviders.Add(typeof(ProtobufSerializer).GetTypeInfo()); d.FallbackSerializationProvider = typeof(ProtobufSerializer).GetTypeInfo(); })
                    .UseDevelopmentClustering(new IPEndPoint(IPAddress.Loopback, mainSiloPort))
                    .ConfigureEndpoints(siloPort: silePort, gatewayPort: gatewayPort)

                    .Configure<ClusterOptions>(options =>
                    {
                        //ClusterId为集群名称 相同的名称才能被分配到一个集群中
                        options.ClusterId = "dev";
                        //当前服务的名称  
                        options.ServiceId = "GoodsServer";
                    })
                    .AddStartupTask(
                      async (IServiceProvider services, CancellationToken cancellation) =>
                      {
                          var grainFactory = services.GetRequiredService<IGrainFactory>();

                          //var grain = grainFactory.GetGrain<IMyGrain>(0);
                          //注册本机服务

                      })
                     //注入打印消息的入口
                     .ConfigureLogging(logging => logging.AddConsole());

            //进行构建 
            var host = builder.Build();
            //启动服务
            await host.StartAsync();
            Console.WriteLine("服务启动成功");
            return host;
        }
        async Task StartAsyncClient(int mainSiloProt, int gatewayProt, int siloProt, IServiceCollection servicesCollection)
        {
            IClusterClient client = new ClientBuilder()
                 .Configure<SerializationProviderOptions>(d => { d.SerializationProviders.Add(typeof(ProtobufSerializer).GetTypeInfo()); d.FallbackSerializationProvider = typeof(ProtobufSerializer).GetTypeInfo(); })
                 //与主简仓进行连接
                 .UseStaticClustering(new IPEndPoint[] { new IPEndPoint(GetInternalIp(), gatewayProt) })
               .Configure<ClusterOptions>(options =>
               {
                   options.ClusterId = "dev";
                   options.ServiceId = "GoodsClient";
               })

           //配置刷新简仓的时间 一般来说不会这么短
           //.Configure<GatewayOptions>(d => d.GatewayListRefreshPeriod = TimeSpan.FromSeconds(5))
           .ConfigureLogging(logging => logging.AddConsole()).Build();
           await client.Connect();
            Console.WriteLine("Orleans客户端已经启动");
            servicesCollection.AddSingleton(client);

        }

        public static IPAddress GetInternalIp()
        {
            IPHostEntry myEntry = Dns.GetHostEntry(Dns.GetHostName());
            return myEntry.AddressList.FirstOrDefault(e => e.AddressFamily.ToString().Equals("InterNetwork"));

        }
    }
}
