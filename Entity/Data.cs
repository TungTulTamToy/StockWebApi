using System;

namespace StockWebApi.Entity
{
    public class Data<T> where T:class
    {
        public T Content{get;set;}
        public Info Info{get;set;}
    }
}