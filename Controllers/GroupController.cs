using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockCore.Aop.Mon;
using StockCore.Business.Builder;
using StockCore.DomainEntity;
using StockCore.Factory;
using StockWebApi.Extension;
using StockWebApi.Entity;
using static StockCore.DomainEntity.Enum.TraceSource;

namespace StockWebApi.Controllers
{
    [Route("api/[controller]")]
    public class GroupController : BaseController
    {
        private const int PROCESSID = 2003100;
        private readonly IFactory<string, IBuilder<string, IEnumerable<QuoteGroup>>> allGroupBuilderFactory;
        private readonly IFactory<string, IBuilder<string, IEnumerable<Stock>>> stockByGroupBuilderFactory;
        public GroupController(
            ILogger logger,
            IFactory<string, IBuilder<string, IEnumerable<QuoteGroup>>> allGroupBuilderFactory,
            IFactory<string, IBuilder<string, IEnumerable<Stock>>> stockByGroupBuilderFactory):base(PROCESSID,logger)
        {
            this.allGroupBuilderFactory = allGroupBuilderFactory;
            this.stockByGroupBuilderFactory = stockByGroupBuilderFactory;
        }
        [HttpGet()]
        [EnableCors("SiteCorsPolicy")]
        public async Task<Data<Groups>> GetAllGroupAsync(string requestID=null)
        {
            return await baseControllerBuildAsync(
                buildAsync:async ()=> await buildGetAllGroupAsync(),
                requestID:requestID);
        }
        [HttpGet("{qroup}")]
        [EnableCors("SiteCorsPolicy")]
        public async Task<Data<StockContent>> GetStockByGroupAsync(string qroup,string requestID=null)
        {
            return await baseControllerBuildAsync(
                buildAsync:async ()=> await buildGetStockByGroupAsync(qroup),
                requestID:requestID);
        }
        private async Task<Groups> buildGetAllGroupAsync([CallerMemberName]string methodName="")
        {
            Groups item = null;
            using(var factory = allGroupBuilderFactory)
            {
                var tracer = new Tracer().Load(PROCESSID,null,$"{this.GetType().Name}.{methodName}",TraceSourceName.WebApi);
                var allGroupBuilder = allGroupBuilderFactory.Build(tracer);
                var deCollection = await allGroupBuilder.BuildAsync();
                item= new Groups().Load(deCollection);
            }
            return item;
        }
        private async Task<StockContent> buildGetStockByGroupAsync(string groupName,[CallerMemberName]string methodName="")
        {
            StockContent item = null;
            using(var factory = stockByGroupBuilderFactory)
            {
                var tracer = new Tracer().Load(PROCESSID,null,$"{this.GetType().Name}.{methodName}",TraceSourceName.WebApi);
                var stockByGroupBuilder = stockByGroupBuilderFactory.Build(tracer);
                var deCollection = await stockByGroupBuilder.BuildAsync(groupName);
                item = new StockContent().Load(deCollection);
            }
            return item;  
        }
    }
}
