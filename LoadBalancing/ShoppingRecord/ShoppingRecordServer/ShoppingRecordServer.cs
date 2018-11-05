using Orleans;
using System;
using IShoppingRecord;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Threading;
using Orleans.Concurrency;

namespace ShoppingRecordServer
{
    //[StatelessWorker]
    public class ShoppingRecordServer : Grain, IShoppingRecord.IShoppingRecord
    {
        ILogger<ShoppingRecordServer> logger;
        public ShoppingRecordServer(ILogger<ShoppingRecordServer> logger)
        {
            this.logger = logger;
        }
        public Task<string> GetShoppingRecordDescribe()
        {
            Thread.Sleep(5000);
            logger.LogInformation("账单服务调用一次");
            return Task.FromResult("账单服务调用成功");
        }
    }
}
