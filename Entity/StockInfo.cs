using System.Collections.Generic;
using StockCore.DomainEntity;

namespace StockWebApi.Entity
{
    public class StockInfo
    {
        public StockSummary StockSummary{get;set;}
        public IEnumerable<Stock> StockCollection{get;set;}
    }
}