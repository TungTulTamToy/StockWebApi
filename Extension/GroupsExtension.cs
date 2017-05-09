using System.Collections.Generic;
using System.Linq;
using StockCore.DomainEntity;
using StockWebApi.Entity;

namespace StockWebApi.Extension
{
    public static class GroupsExtension
    {
        public static Groups Load(this Groups item,IEnumerable<IQuoteGroup> items)
        {
            if(items!=null && items.Any())
            {
                item.Name = items.Select(g=>g.Name).ToList();
            }
            return item;
        }
    }
}