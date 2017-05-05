using System.Collections.Generic;
using System.Linq;
using StockWebApi.Entity;

namespace StockWebApi.Extension
{
    public static class StockSummaryExtension
    {
        public static StockSummary Load(this StockSummary item,
            AgregatorSummary peYear,
            AgregatorSummary netProfitYear,
            AgregatorSummary growthYear,
            AgregatorSummary pegYear,
            AgregatorSummary peDiffYear)
        {
            item.PeYear = peYear;
            item.NetProfitYear = netProfitYear;
            item.GrowthYear = growthYear;
            item.PegYear = pegYear;
            item.PeDiffYear = peDiffYear;
            return item;
        }
    }
}