using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Net;
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
                    .UseLocalhostClustering()
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
            Console.WriteLine("启动成功");
            return host;
        }
    }
}
