using Orleans;
using System;
using IShoppingRecord;
using System.Threading.Tasks;

namespace ShoppingRecordServer
{
    public class ShoppingRecordServer : Grain, IShoppingRecord.IShoppingRecord
    {
        public Task<string> GetShoppingRecordDescribe()
        {
            return Task.FromResult("账单服务调用成功");
        }
    }
}
