using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Serialization;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Goods
{
    class Program
    {
        static void Main(string[] args)
        {
            //本服务开放端口
            int silePort = 11113;
            //主简仓网关端口
            int gatewayPort = 30000;
            //主简仓开放端口    
            int mainSiloPort = 11111;

            ///定时器对象
            IGrainReminder grainReminder=null;
            //由于向网关添加一个服务处理需要多一些时间
            //所以在程序运行后马上获取服务可能会抛出获取不到的异常
            //详情请看5、常见问题
            var host =  StartHost(silePort, gatewayPort, mainSiloPort);
            var client = StartClient(gatewayPort);
            while (true)
            {
                string ReadLine = Console.ReadLine();
                if (ReadLine=="Exit")
                {
                    host.Result.StopAsync().Wait();
                    client.Result.Close();
                    break;
                }
                else if (ReadLine=="Goods")
                {
                    try
                    {
                     
                        IGoods.IGoods goods = client.Result.GetGrain<IGoods.IGoods>(0);
                        Console.WriteLine(goods.GetGoodsDescribe().Result);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("服务暂时还没有启动完成 请稍后再试"+e.Message);
                    }
                }
                else if (ReadLine == "GoodsList")
                {
                    try
                    {

                        IGoods.IGoods goods = client.Result.GetGrain<IGoods.IGoods>(0);
                        List<Entity.GoodsEntity> arrayList = goods.GetGoodsList().Result;
                        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(arrayList));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("服务暂时还没有启动完成 请稍后再试" + e.Message);
                    }
                }
                else if (ReadLine == "GoodsStartTimer")
                {
                    try
                    {

                        IGoods.IGoods goods = client.Result.GetGrain<IGoods.IGoods>(0);
                        //grainReminder = goods.StartTimerTest().Result;
                        //goods.StartTimerTestString("Test");
                        goods.StartTimer();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("服务暂时还没有启动完成 请稍后再试" + e.Message);
                    }
                }
                else if (ReadLine == "GoodsStopTimer")
                {
                    try
                    {

                        IGoods.IGoods goods = client.Result.GetGrain<IGoods.IGoods>(0);
                        //goods.StopTimerTest(grainReminder).Wait();
                        goods.StopTimerTest("Test");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("服务暂时还没有启动完成 请稍后再试" + e.Message);
                    }
                }
                else if (ReadLine == "ShoppingRecord")
                {
                    try
                    {
                        IShoppingRecord.IShoppingRecord shoppingRecord = client.Result.GetGrain<IShoppingRecord.IShoppingRecord>(0);
                        Console.WriteLine(shoppingRecord.GetShoppingRecordDescribe().Result);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("服务暂时还没有启动完成 请稍后再试" + e.Message);
                    }
                }
            }

        }

        /// <summary>
        /// 在本地启动一个Host
        /// </summary>
        /// <returns></returns>
        static async Task<ISiloHost> StartHost(int silePort, int gatewayPort, int mainSiloPort)
        {
            var builder = new SiloHostBuilder()
                    //启动一个在本地的Host 若primarySiloEndpoint属性为空 则启动主简仓 否则进行连接
                    //IPAddress.Loopback为127.0.0.1
                    //.UseLocalhostClustering()
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
                     //注入打印消息的入口
                     .ConfigureLogging(logging => logging.AddConsole())
                     .UseInMemoryReminderService()

            .AddStartupTask<CallGrainStartupTask>();

            
            //进行构建 
            var host = builder.Build();
            //启动服务
            await host.StartAsync();
            return host;
        }

        /// <summary>
        /// 连接Orleans仓库
        /// </summary>
        /// <param name="GatewayPort"></param>
        /// <returns></returns>
        static  async Task<IClusterClient> StartClient(int gatewayPort) {
            IClusterClient client = new ClientBuilder()
                  .Configure<SerializationProviderOptions>(d => { d.SerializationProviders.Add(typeof(ProtobufSerializer).GetTypeInfo()); d.FallbackSerializationProvider = typeof(ProtobufSerializer).GetTypeInfo(); })
                  //与主简仓进行连接
                  .UseStaticClustering(new IPEndPoint[] { new IPEndPoint(IPAddress.Loopback, gatewayPort) })
                .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "dev";
                options.ServiceId = "GoodsClient";
            })
            //配置刷新简仓的时间 一般来说不会这么短
            .Configure<GatewayOptions>(d => d.GatewayListRefreshPeriod = TimeSpan.FromSeconds(10))
            .ConfigureLogging(logging => logging.AddConsole())
            .Build();
          await client.Connect();
            Console.WriteLine("已经成功连上网关");
            return client;
        }
    }
}
