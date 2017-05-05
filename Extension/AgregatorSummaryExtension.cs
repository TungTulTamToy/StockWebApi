using System.Collections.Generic;
using System.Linq;
using StockWebApi.Entity;

namespace StockWebApi.Extension
{
    public static class AgregatorSummaryExtension
    {
        public static AgregatorSummary Load(this AgregatorSummary item,IEnumerable<int> items)
        {
            if(items != null && items.Any())
            {
                item.Min = items.Min();
                item.Max = items.Max();
            }
            return item;
        }
    }
}