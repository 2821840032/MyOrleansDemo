using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace Order.Controllers
{
    [Route("[controller]/[action]")]
    public class OrderController : Controller
    {
        IShoppingRecord.IShoppingRecord shoppingRecord;
        IGoods.IGoods goods;
        public OrderController()
        {
            this.shoppingRecord = this.GetServer<IShoppingRecord.IShoppingRecord>();
            this.goods = this.GetServer<IGoods.IGoods>();
        }
        [HttpGet]
        public IActionResult GetOrderInformation(int OrderID)
        {
            var result = shoppingRecord.GetOrderDescribe(OrderID).Result;
            if (result==null)
            {
                throw new Exception("没有找到这个账单");
            }
            result.CommodiItynformation = goods.GetGoodsDesc(result.GoodsID).Result;
            return new JsonResult(result);
        }
    }
}