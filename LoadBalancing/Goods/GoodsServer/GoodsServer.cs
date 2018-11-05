using Entity;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Concurrency;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoodsServer
{
    [StatelessWorker]
    public class GoodsServer : Grain, IGoods.IGoods
    {
        ILogger<GoodsServer> logger;
        public GoodsServer(ILogger<GoodsServer> logger) {
            this.logger = logger;
        }
        public Task<GoodsEntity> GetGoodsDesc()
        {
            Thread.Sleep(5000);
            logger.LogInformation("商品服务调用一次");
            return Task.FromResult(new GoodsEntity() { ID = 1, GoodsName = "商品名称" });
        }

        public Task<string> GetGoodsDescribe()
        {
            return Task.FromResult("商品服务调用成功");
        }
    }
}
