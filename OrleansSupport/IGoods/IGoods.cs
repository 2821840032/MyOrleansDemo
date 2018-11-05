using Entity;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// 启动定时器测试
        /// </summary>
        /// <returns></returns>
        Task<IGrainReminder> StartTimerTest();

        /// <summary>
        /// 启动定时器测试
        /// </summary>
        /// <returns></returns>
        Task StartTimerTestString(string TimerName);



        /// <summary>
        /// 关闭定时器测试
        /// </summary>
        /// <returns></returns>
        Task StopTimerTest(IGrainReminder grainReminder);

        /// <summary>
        /// 关闭定时器测试
        /// </summary>
        /// <returns></returns>
        Task StopTimerTest(string TimerName);

        Task StartTimer();
        /// <summary>
        /// 获取商品列表
        /// </summary>
        /// <returns></returns>
        Task<List<GoodsEntity>> GetGoodsList();
    }
}
