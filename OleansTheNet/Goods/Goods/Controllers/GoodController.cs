using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Orleans;

namespace Goods.Controllers
{
    [Route("[controller]/[action]")]
    public class GoodController : Controller
    {
        IGoods.IGoods goods;

        public GoodController()
        {
            this.goods = this.GetServer<IGoods.IGoods>();
        }
        [HttpGet]
        public IActionResult GetGoods()
        {

            return new JsonResult(goods.GetGoodsDesc(1).Result);
        }
    }
}