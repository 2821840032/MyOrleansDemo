using Entity;
using Orleans;
using System;
using System.Threading.Tasks;

namespace IShoppingRecord
{
    public interface IShoppingRecord: IGrainWithIntegerKey
    {
        /// <summary>
        /// 购物记录服务测试接口
        /// </summary>
        /// <returns></returns>
        Task<OrderEntity> GetOrderDescribe(int orderID);
    }
}
