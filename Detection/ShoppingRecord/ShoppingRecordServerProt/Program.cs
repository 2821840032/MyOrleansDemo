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

namespace ShoppingRecordServerProt
{
    class Program
    {
        static void Main(string[] args)
        {
            //本服务开放端口
            int silePort = 11114;
            //主简仓网关端口
            int gatewayPort = 30000;
            //主简仓开放端口    
            int mainSiloPort = 11111;

            StartHost(silePort,gatewayPort,mainSiloPort);
            while (Console.ReadLine()!="Exit")
            {

            }
            //由于向网关添加一个服务处理需要多一些时间
            //所以在程序运行后马上获取服务可能会抛出获取不到的异常
            //详情请看5、常见问题
        }


        /// <summary>
        /// 在本地启动一个Host
        /// </summary>
        /// <returns></returns>
        static async Task<ISiloHost> StartHost(int silePort, int gatewayPort, int mainSiloPort)
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
                        options.ServiceId = "ShoppingRecordServer";
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
    }
}
