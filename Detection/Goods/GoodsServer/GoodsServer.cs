using Entity;
using Orleans;
using System;
using System.Threading.Tasks;

namespace GoodsServer
{
    public class GoodsServer : Grain, IGoods.IGoods
    {
        public Task<GoodsEntity> GetGoodsDesc()
        {
            return Task.FromResult(new GoodsEntity() { ID = 1, GoodsName = "商品名称" });
        }

        public Task<string> GetGoodsDescribe()
        {
            return Task.FromResult("商品服务调用成功");
        }
    }
}
