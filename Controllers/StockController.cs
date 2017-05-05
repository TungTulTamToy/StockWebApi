using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockCore.Business.Builder;
using StockCore.DomainEntity;
using StockCore.Factory;
using StockWebApi.Entity;

namespace StockWebApi.Controllers
{
    [Route("api/[controller]")]
    public class StockController : BaseController
    {
        private const int PROCESSID = 2000100;
        private readonly IFactory<string, IBuilder<string, Stock>> stockBuilderFactory;
        public StockController(
            ILogger logger,
            IFactory<string, IBuilder<string, Stock>> stockBuilderFactory):base(PROCESSID,logger)
        {
            this.stockBuilderFactory = stockBuilderFactory;
        }
        [HttpGet("{quote}")]
        [Produces("application/json")]
        public async Task<Data<Stock>> GetStockAsync(string quote,string requestID=null)
        {
            return await baseControllerBuildAsync(
                buildAsync:async ()=> await buildAsync(quote),
                requestID:requestID);
        }
        private async Task<Stock> buildAsync(string quote)
        {
            Stock stock = null;
            using(var factory = stockBuilderFactory)
            {
                var stockBuilder = stockBuilderFactory.Build(null);
                stock = await stockBuilder.BuildAsync(quote);
            }
            return stock;
        }
    }
}
