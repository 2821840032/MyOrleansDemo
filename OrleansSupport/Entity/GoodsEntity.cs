using Bond;
using System;

namespace Entity
{
    /// <summary>
    /// 商品实体
    /// </summary>
    [Schema]
    [Serializable]
    public class GoodsEntity
    {
        public int ID { get; set; }

        public string GoodsName { get; set; }
    }
}
 