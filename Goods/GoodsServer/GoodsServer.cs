using Entity;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoodsServer
{
    public class GoodsServer : Grain, IGoods.IGoods, IRemindable
    {
        ILogger<GoodsServer> logger;
        public GoodsServer(ILogger<GoodsServer> logger) {
            this.logger = logger;
        }
        /// <summary>
        /// 用于测试相同的谷物ID是否会调用相同的谷物
        /// </summary>
        int result = 0;
        public Task<string> GetGoodsDescribe()
        {
            result += 1;
            return Task.FromResult("商品服务调用成功"+ result);
        }
       

        public Task ReceiveReminder(string reminderName, TickStatus status)
        {
            logger.Info("通知调用一次" + reminderName);
            return Task.FromResult(0);
        }

        public Task<IGrainReminder> StartTimerTest()
        {
            return RegisterOrUpdateReminder("Test", TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(1));
        }

        public Task StopTimerTest(IGrainReminder grainReminder)
        {
            return UnregisterReminder(grainReminder);
        }

        public Task StartTimerTestString(string TimerName)
        {
           return Task.FromResult(RegisterOrUpdateReminder(TimerName, TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(1)));
        }

        public Task StopTimerTest(string TimerName)
        {
            return UnregisterReminder(GetReminder(TimerName).Result);
        }

        public Task StartTimer()
        {
           return Task.FromResult(RegisterTimer(TimerPerform, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(3)));
        }
         Task TimerPerform(Object input)
        {
            logger.Info("调用定时器一次");
            return Task.FromResult(0);

        }

        public Task<List<GoodsEntity>> GetGoodsList()
        {
            return Task.FromResult(new List<GoodsEntity>() { new GoodsEntity() { ID = 0, GoodsName = "牙刷" }, new GoodsEntity() { ID = 1, GoodsName = "牙膏" } });
        }
    }
}
