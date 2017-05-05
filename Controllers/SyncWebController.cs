using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockCore.Factory;
using StockCore.DomainEntity.Enum;
using StockCore.Aop.Mon;
using StockWebApi.Entity;
using StockCore.Business.Operation;

namespace StockWebApi.Controllers
{
    [Route("api/[controller]")]
    public class SyncWebController : BaseController
    {
        private const int PROCESSID = 2001100;
        private readonly IFactory<string,IOperation<string>> syncQuoteFactory;
        private readonly IFactory<SyncAllFactoryCondition, IOperation<IEnumerable<string>>> syncAllFactory;
        public SyncWebController(
            ILogger logger,
            IFactory<string, IOperation<string>> syncQuoteFactory,
            IFactory<SyncAllFactoryCondition, IOperation<IEnumerable<string>>> syncAllFactory):base(PROCESSID,logger)
        {
            this.syncQuoteFactory = syncQuoteFactory;
            this.syncAllFactory = syncAllFactory;
        }
        [HttpGet()]
        public async Task<Data<string>> SyncAllAsync(string requestID=null)
        {
            return await baseControllerBuildAsync<string>(
                operateAsync:async ()=> await syncAllAsync(),
                requestID:requestID);
        }
        [HttpGet("{quote}")]
        public async Task<Data<string>> SyncWebAsync(string quote,string requestID=null)
        {
            return await baseControllerBuildAsync<string>(
                operateAsync:async ()=> await syncQuoteAsync(quote),
                requestID:requestID);
        }
        private async Task syncQuoteAsync(string quote)
        {
            using(var factory = syncQuoteFactory)
            {
                var tracer=new Tracer().Load(0,null,"Start Sync Web from WebApi",TraceSource.TraceSourceName.WebApi);
                var syncQuote = syncQuoteFactory.Build(null);
                await syncQuote.OperateAsync(quote);
            }
        }
        private async Task syncAllAsync()
        {
            using(var factory = syncAllFactory)
            {
                var tracer=new Tracer().Load(0,null,"Start Sync All Web from WebApi",TraceSource.TraceSourceName.WebApi);
                var syncAll = syncAllFactory.Build(null);
                var quotes = Enum.GetNames(typeof(Quotes.QuotesSample2));
                await syncAll.OperateAsync(quotes);
            }
        }
    }
}
