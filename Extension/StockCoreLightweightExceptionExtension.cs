using System;
using StockCore.Aop.Mon;
using StockWebApi.Entity;

namespace StockWebApi.Extension
{
    public static class StockCoreLightweightExceptionExtension
    {
        public static StockCoreLightweightException Load(this StockCoreLightweightException item, StockCoreException ex)
        {
            if(ex != null)
            {
                item.ID = ex.ID;
                item.ModuleName=ex.ModuleName;
                item.Info=ex.Info;
                item.ErrorID=ex.ErrorID;
            }
            return item;
        }
    }
}