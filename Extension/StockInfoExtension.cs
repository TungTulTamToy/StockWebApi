using System.Collections.Generic;
using System.Linq;
using StockCore.DomainEntity;
using StockWebApi.Entity;

namespace StockWebApi.Extension
{
    public static class StockInfoExtension
    {
        public static StockInfo Load(this StockInfo item,IEnumerable<Stock> items)
        {
            if(items!=null && items.Any())  
            {
                item.StockCollection = items;
                var peYear = items.SelectMany(i=>i.Pe?.Select(pe=>pe.Year));
                var nYear = items.SelectMany(i=>i.NetProfit?.Select(n=>n.Year));
                var gYear = items.SelectMany(i=>i.Growth?.Select(g=>g.Year));
                var pegYear = items.SelectMany(i=>i.Peg?.Select(peg=>peg.Year));
                var peDiffYear = items.SelectMany(i=>i.PeDiffPercent?.Select(ped=>ped.Year));

                item.StockSummary = new StockSummary().Load(
                    new AgregatorSummary().Load(peYear),
                    new AgregatorSummary().Load(nYear),
                    new AgregatorSummary().Load(gYear),
                    new AgregatorSummary().Load(pegYear),
                    new AgregatorSummary().Load(peDiffYear));           
            }
            return item;
        }
    }
}