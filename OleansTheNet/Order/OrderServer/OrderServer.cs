using Entity;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrderServer
{
    public class OrderServer : Grain, IShoppingRecord.IShoppingRecord
    {
        List<OrderEntity> Orders;
        public OrderServer() {
            Orders = new List<OrderEntity>();
            Orders.Add(new OrderEntity() { ID = 1, GoodsID = 1, Number = 10 });
            Orders.Add(new OrderEntity() { ID = 2, GoodsID = 2, Number = 10 });
            Orders.Add(new OrderEntity() { ID = 3, GoodsID = 3, Number = 10 });
            Orders.Add(new OrderEntity() { ID = 4, GoodsID = 4, Number = 10 });
        }

        public Task<OrderEntity> GetOrderDescribe(int orderID)
        {
            var order = Orders.Where(d => d.ID == orderID).FirstOrDefault();
            return Task.FromResult(order);
          
        }
    }
}
