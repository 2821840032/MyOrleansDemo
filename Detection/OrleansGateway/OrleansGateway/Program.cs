using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Serialization;
using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace OrleansGateway
{
    class Program
    {
        static void Main(string[] args)
        {
           var Host = StartHost();
         
            bool IsExit = true;
            while (IsExit)
            {
                string read = Console.ReadLine();
                if (read == "Exit")
                {
                    IsExit = false;
                    Host.Result.StopAsync();
                }
            }
        }

        /// <summary>
        /// 在本地启动一个Host
        /// </summary>
        /// <returns></returns>
        static async  Task<ISiloHost> StartHost() {
            var builder = new SiloHostBuilder()
                 .UseDashboard(options => {
                     options.Username = "ABC";
                     options.Password = "123";
                     options.Host = "*";
                     options.Port = 8080;
                     options.HostSelf = true;
                     options.CounterUpdateIntervalMs = 1000;
                 })
                    .UseLocalhostClustering()
                    .Configure<SerializationProviderOptions>(d => { d.SerializationProviders.Add(typeof(ProtobufSerializer).GetTypeInfo()); d.FallbackSerializationProvider = typeof(ProtobufSerializer).GetTypeInfo(); })
                .Configure<ClusterOptions>(options =>
                {
                    //ClusterId为集群名称 相同的名称才能被分配到一个集群中
                    options.ClusterId = "dev";
                    //当前服务的名称  
                    options.ServiceId = "Gateway";
                })
                .ConfigureLogging(logging => logging.AddConsole());
            
            var host = builder.Build();
            await host.StartAsync();
            Console.WriteLine("网关启动成功");
            await StartAgentHost();
            return host;
        }

        /// <summary>
        /// 在本地启动配置外部代理网关
        /// </summary>
        /// <returns></returns>
        static async Task<ISiloHost> StartAgentHost()
        {
            var builder = new SiloHostBuilder()
                  .UseDevelopmentClustering(new IPEndPoint(IPAddress.Loopback, 11111))
                    .Configure<SerializationProviderOptions>(d => { d.SerializationProviders.Add(typeof(ProtobufSerializer).GetTypeInfo()); d.FallbackSerializationProvider = typeof(ProtobufSerializer).GetTypeInfo(); })
                     .ConfigureEndpoints(GetInternalIp(), siloPort: 11111, gatewayPort: 30000)
                .Configure<ClusterOptions>(options =>
                {
                    //ClusterId为集群名称 相同的名称才能被分配到一个集群中
                    options.ClusterId = "dev";
                    //当前服务的名称  
                    options.ServiceId = "Gateway";
                })
                .ConfigureLogging(logging => logging.AddConsole());

            var host = builder.Build();
            await host.StartAsync();
            Console.WriteLine("代理网关启动成功");
            return host;
        }
        public static IPAddress GetInternalIp()
        {
            IPHostEntry myEntry = Dns.GetHostEntry(Dns.GetHostName());
            return myEntry.AddressList.FirstOrDefault(e => e.AddressFamily.ToString().Equals("InterNetwork"));

        }
    }
}
