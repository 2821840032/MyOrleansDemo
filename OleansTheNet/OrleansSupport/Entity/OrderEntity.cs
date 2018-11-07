using Bond;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    /// <summary>
    /// 订单实体
    /// </summary>
    [Schema]
    [Serializable]
    public class OrderEntity
    {
        public GoodsEntity CommodiItynformation { get; set; }

        public int ID { get; set; }

        public int Number { get; set; }
        public int GoodsID { get; set; }
    }
}
