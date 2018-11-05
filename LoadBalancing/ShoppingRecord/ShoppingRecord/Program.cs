using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Serialization;
using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingRecord
{
    class Program
    {
        static void Main(string[] args)
        {
            //主简仓网关端口
            int gatewayPort = 30001;
            //主简仓开放端口    
            int mainSiloPort = 11112;

            var host = StartGatewayHost(gatewayPort, mainSiloPort);
            var gatewayHost = StartHost(mainSiloPort, gatewayPort);
            while (true)
            {
                string ReadLine = Console.ReadLine();
                if (ReadLine == "Exit")
                {
                    gatewayHost.Result.StopAsync().Wait();
                    host.Result.StopAsync().Wait();
                   
                    break;
                }
            }

        }

        /// <summary>
        /// 启动网关的简仓
        /// </summary>
        /// <returns></returns>
        static async Task<ISiloHost> StartHost(int silePort, int gatewayPort)
        {
            var builder = new SiloHostBuilder()
                   .Configure<SerializationProviderOptions>(d => { d.SerializationProviders.Add(typeof(ProtobufSerializer).GetTypeInfo()); d.FallbackSerializationProvider = typeof(ProtobufSerializer).GetTypeInfo(); })
                    .UseDevelopmentClustering(new IPEndPoint(IPAddress.Loopback, silePort))
                    .ConfigureEndpoints(GetInternalIp(), siloPort: silePort, gatewayPort: gatewayPort)

                    .Configure<ClusterOptions>(options =>
                    {
                        //ClusterId为集群名称 相同的名称才能被分配到一个集群中
                        options.ClusterId = "dev";
                        //当前服务的名称  
                        options.ServiceId = "GoodsServer";
                    })
                     //注入打印消息的入口
                     .ConfigureLogging(logging => logging.AddConsole());

            //进行构建 
            var host = builder.Build();
            //启动服务
            await host.StartAsync();
            Console.WriteLine("简仓服务启动成功");
            return host;
        }

        /// <summary>
        /// 启动本地网关 
        /// </summary>
        /// <param name="gatewayPort"></param>
        /// <param name="mainSiloPort"></param>
        /// <returns></returns>
        static async Task<ISiloHost> StartGatewayHost(int gatewayPort, int silePort)
        {
            var builder = new SiloHostBuilder()
                   .Configure<SerializationProviderOptions>(d => { d.SerializationProviders.Add(typeof(ProtobufSerializer).GetTypeInfo()); d.FallbackSerializationProvider = typeof(ProtobufSerializer).GetTypeInfo(); })
                   .UseLocalhostClustering(gatewayPort: gatewayPort, siloPort: silePort)
                    .Configure<ClusterOptions>(options =>
                    {
                        //ClusterId为集群名称 相同的名称才能被分配到一个集群中
                        options.ClusterId = "dev";
                        //当前服务的名称  
                        options.ServiceId = "GoodsServer";
                    })
                     //注入打印消息的入口
                     .ConfigureLogging(logging => logging.AddConsole());
            //进行构建 
            var host = builder.Build();
            //启动服务
            await host.StartAsync();
            Console.WriteLine("网关服务启动成功");
            return host;
        }

        public static IPAddress GetInternalIp()
        {
            IPHostEntry myEntry = Dns.GetHostEntry(Dns.GetHostName());
            return myEntry.AddressList.FirstOrDefault(e => e.AddressFamily.ToString().Equals("InterNetwork"));
        }
    }
}
