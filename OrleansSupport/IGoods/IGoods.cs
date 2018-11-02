using Orleans;
using System;
using System.Threading.Tasks;

namespace IGoods
{
    public interface IGoods: IGrainWithIntegerKey
    {
        /// <summary>
        /// 商品服务测试接口
        /// </summary>
        /// <returns></returns>
        Task<string> GetGoodsDescribe();

    }
}
