using Bond;
using System;

namespace Entity
{
    [Schema]
    [Serializable]
    public class GoodsEntity
    {
        public int ID { get; set; }
        public string GoodsName { get; set; }
    }
}
 