using Orleans;
using System;
using System.Threading.Tasks;

namespace GoodsServer
{
    public class GoodsServer : Grain, IGoods.IGoods
    {
        public Task<string> GetGoodsDescribe()
        {
            return Task.FromResult("商品服务调用成功");
        }
    }
}
