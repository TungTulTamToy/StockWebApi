using System;
using StockCore.Aop.Mon;

namespace StockWebApi.Entity
{
    public class Info
    {
        public string RequestID{get;set;}
        public int Code{get;set;}
        public bool HasError{get;set;}
        public StockCoreLightweightException Exception{get;set;}
    }
}