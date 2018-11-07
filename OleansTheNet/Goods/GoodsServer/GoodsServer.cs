using Entity;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodsServer
{
    public class GoodsServer : Grain, IGoods.IGoods
    {
        List<GoodsEntity> goods;
        public GoodsServer(){
            goods = new List<GoodsEntity>();
            goods.Add(new GoodsEntity() { ID = 1, GoodsName = "牙膏" });
            goods.Add(new GoodsEntity() { ID = 2, GoodsName = "牙刷" });
            goods.Add(new GoodsEntity() { ID = 3, GoodsName = "杯子" });
            goods.Add(new GoodsEntity() { ID = 4, GoodsName = "脸盆" });
        }
        public Task<GoodsEntity> GetGoodsDesc(int goodsID)
        {
            return Task.FromResult(goods.Where(d=>d.ID==goodsID).FirstOrDefault());
        }

        public Task<string> GetGoodsDescribe()
        {
            return Task.FromResult("商品服务调用成功");
        }

        public Task<GoodsEntity1> GetGoodsDescTest(int goodsID)
        {
            return Task.FromResult(new GoodsEntity1() { ID = 3, GoodsName = "杯子" });
        }
    }
}
