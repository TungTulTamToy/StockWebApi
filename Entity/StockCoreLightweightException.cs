using System;
using StockCore.Aop.Mon;

namespace StockWebApi.Entity
{
    public class StockCoreLightweightException
    {
        public Guid ID { get; set; }
        public string ModuleName { get; set; }
        public string Info { get; set; }
        public int ErrorID { get; set; }
    }
}