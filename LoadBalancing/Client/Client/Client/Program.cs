using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
          var client = StartClient(new IPEndPoint(IPAddress.Loopback, 30001), new IPEndPoint(IPAddress.Loopback, 30000));
            while (true)
            {
                var readline = Console.ReadLine();
                if (readline == "Exit")
                {
                    client.Result.Close();
                    break;
                }
                else {
                    perform(readline, client.Result);
                }


              
            }
        }
        static int Index = 0;
        static async void perform(string readline, IClusterClient client) {
            try
            {
                if (readline == "Goods")
                {
                    var session = client.GetGrain<IGoods.IGoods>(0);
                    Console.WriteLine((await session.GetGoodsDesc()).GoodsName);
                }
                else if (readline == "ShoppingRecord")
                {
                    var session = client.GetGrain<IShoppingRecord.IShoppingRecord>(Index++);
                    Console.WriteLine((await session.GetShoppingRecordDescribe()));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("错误" + e.Message);
            }
        }

        /// <summary>
        /// 连接Orleans仓库
        /// </summary>
        /// <param name="gatewayPort"></param>
        /// <returns></returns>
        static async Task<IClusterClient> StartClient(params IPEndPoint[] iPEndPoints)
        {
            IClusterClient client = new ClientBuilder()
                .UseStaticClustering(iPEndPoints)
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "Client";
                })
                //配置刷新简仓的时间 一般来说不会这么短
                .Configure<GatewayOptions>(d => d.GatewayListRefreshPeriod = TimeSpan.FromSeconds(5))
            .ConfigureLogging(logging => logging.AddConsole())
            .Build();
            await client.Connect();
            Console.WriteLine("已经成功连上网关");
            return client;
        }
        public static IPAddress GetInternalIp()
        {
            IPHostEntry myEntry = Dns.GetHostEntry(Dns.GetHostName());
            return myEntry.AddressList.FirstOrDefault(e => e.AddressFamily.ToString().Equals("InterNetwork"));
        }
    }
}
