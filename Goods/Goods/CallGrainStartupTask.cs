using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Goods
{
    /// <summary>
    /// 这是简仓启动后执行代码的类
    /// 由于简仓的启动时异步的 主线程不知道什么时候完成 所以需要这样一个入口来通知已经完成
    /// </summary>
    public class CallGrainStartupTask : IStartupTask
    {
        /// <summary>
        /// 简仓服务工厂
        /// </summary>
        private readonly IGrainFactory grainFactory;

        public CallGrainStartupTask(IGrainFactory grainFactory)
        {
            this.grainFactory = grainFactory;
        }
        public async Task Execute(CancellationToken cancellationToken)
        {
            var grain = this.grainFactory.GetGrain<IGoods.IGoods>(0);
            var goodsdesc = await grain.GetGoodsList();
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(goodsdesc));
            Console.WriteLine("服务已经启动");
            //在这里 我们很清楚的看到简仓启动要远比客户端启动慢的多
        }
    }
}
