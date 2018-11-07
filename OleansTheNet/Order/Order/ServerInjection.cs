using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order
{
    /// <summary>
    /// 注入本地服务
    /// </summary>
    public static class ServerInjection
    {
        /// <summary>
        /// 服务注入  在最后使用
        /// </summary>
        /// <param name="services"></param>
        public static void ServerInjectionADD(this IServiceCollection services) {
           


            services.AddSingleton<IShoppingRecord.IShoppingRecord, OrderServer.OrderServer>();




            //引用对象留在最后
            serviceProvider = services.BuildServiceProvider();
        }

        public static ServiceProvider serviceProvider { get; set; }

        public static T GetServer<T>(this Controller controller) where T:class, IGrainWithIntegerKey
        {
           var TService = serviceProvider.GetService<T>();
            if (TService==null)
            {
                var client = serviceProvider.GetService<IClusterClient>();
                if (client==null)
                {
                    throw new Exception("客户端没有启动 或者没有被注入");
                }
                TService = client.GetGrain<T>(0);
                if (TService==null)
                {
                    throw new Exception("没有找到该类型" + TService.GetType());
                }
                return TService;
            }
            return TService;
        }
    }
    
}
