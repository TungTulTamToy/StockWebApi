using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using StockCore.Wrapper;
using StockCore.DomainEntity;
using StockCore.Provider;
using StockCore.Business.Repo;
using StockCore.DomainEntity.Enum;
using StockCore.Business.Repo.AppSetting;
using StockCore.Business.Repo.MongoDB;
using StockCore.Business.Builder;
using StockCore.Factory;
using StockCore.Factory.Builder;
using StockCore.Factory.DB;
using StockCore.Factory.Html;
using StockCore.Factory.Sync;
using StockCore.Aop.Mon;
using MongoDB.Driver;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using System.Text;
using StockWebApi.Entity;
using Microsoft.AspNetCore.Cors.Infrastructure;
using StockCore.Business.Operation;
using StockWebApi.Extension;

namespace StockWebApi
{
    public class Startup
    {
        private const int PROCESSID = 2002100;
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("module.json", optional: false, reloadOnChange: true)
                .AddJsonFile("dynamicgroup.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            //services.AddLogging();
            services.AddSingleton<ILogger>(ctx=>ctx.GetService<ILogger<Program>>());
            services.AddSingleton<IConfigProvider, StockCore.Provider.ConfigurationProvider>()
                    .AddSingleton<IConfigReader<IModule>, ModuleConfigReader>()
                    .AddSingleton<IConfigReader<IDynamicGroup>,DynamicGroupReader>()
                    .AddSingleton<IConfigurationRoot>(_=>Configuration)

                    .AddScoped<IFactory<SyncAllFactoryCondition, IOperation<IEnumerable<string>>>, SyncAllFactory>()
                    .AddScoped<IFactory<SyncQuoteFactoryCondition, IOperation<string>>, SyncQuoteFactory>()
                    .AddScoped<IFactory<string, IOperation<IEnumerable<QuoteGroup>>>, SyncQuoteGroupFactory>()
                    .AddScoped<IFactory<string, IOperation<IEnumerable<Price>>>, SyncPriceFactory>()
                    .AddScoped<IFactory<string, IOperation<IEnumerable<Consensus>>>, SyncConsensusFactory>()
                    .AddScoped<IFactory<string, IOperation<IEnumerable<Share>>>, SyncShareFactory>()
                    .AddScoped<IFactory<string, IOperation<IEnumerable<Statistic>>>, SyncStatisticFactory>()
                    .AddScoped<IFactory<string, IOperation<IEnumerable<SetIndex>>>, SyncSetIndexFactory>()
                    .AddScoped<IFactory<string, IOperation<IEnumerable<QuoteMovement>>>, SyncQuoteMovementFactory>()
                    .AddScoped<IFactory<string, IGetByKey<IEnumerable<Consensus>,string>>, ConsensusHtmlReaderFactory>()
                    .AddScoped<IFactory<string, IGetByKey<IEnumerable<Price>,string>>, PriceHtmlReaderFactory>()
                    .AddScoped<IFactory<string, IGetByKey<IEnumerable<SetIndex>,string>>, SetIndexHtmlReaderFactory>()
                    .AddScoped<IFactory<string, IGetByKey<IEnumerable<Share>,string>>, ShareHtmlReaderFactory>()
                    .AddScoped<IFactory<string, IGetByKey<IEnumerable<Statistic>,string>>, StatisticHtmlReaderFactory>()
                    .AddScoped<IFactory<string, IGetByKeyRepo<OperationState,string>>, DBOperationStateRepoFactory>()                    
                    .AddScoped<IFactory<string, IGetByKeyRepo<Consensus,string>>, DBConsensusRepoFactory>()
                    .AddScoped<IFactory<string, IGetByKeyRepo<Share,string>>, DBShareRepoFactory>()
                    .AddScoped<IFactory<string, IGetByKeyRepo<Statistic,string>>, DBStatisticRepoFactory>()
                    .AddScoped<IFactory<string, IGetByKeyRepo<Price,string>>, DBPriceRepoFactory>()
                    .AddScoped<IFactory<string, IGetByKeyRepo<QuoteGroup,string>>, DBQuoteGroupRepoFactory>()
                    .AddScoped<IFactory<string, IGetByKeyRepo<QuoteMovement,string>>, DBQuoteMovementRepoFactory>()            
                    .AddScoped<IFactory<string, IGetByFuncRepo<string,StockCoreCache<Stock>>>, DBCacheRepoFactory<Stock>>()
                    .AddScoped<IFactory<string, IGetByFuncRepo<string,StockCoreCache<IEnumerable<QuoteGroup>>>>, DBCacheRepoFactory<IEnumerable<QuoteGroup>>>()
                    .AddScoped<IFactory<string, IGetByFuncRepo<string,StockCoreCache<IEnumerable<Stock>>>>, DBCacheRepoFactory<IEnumerable<Stock>>>()
                    .AddScoped<IFactory<string, IRepo<SetIndex>>, DBSetIndexRepoFactory>()
                    .AddScoped<IFactory<string, IBuilder<string, Stock>>, StockBuilderFactory>()
                    .AddScoped<IFactory<string, IBuilder<string, IEnumerable<QuoteGroup>>>, AllQuoteGroupBuilderFactory>()
                    .AddScoped<IFactory<string, IBuilder<string, IEnumerable<Stock>>>, StockByGroupBuilderFactory>()
                    .AddScoped<IFactory<string, IBuilder<IEnumerable<Price>, IEnumerable<PriceCal>>>, PriceCalBuilderFactory>()

                    .AddScoped<IMongoDatabaseWrapper,MongoDatabaseWrapper>()
                    .AddScoped<IMongoClient>(ctx => new MongoClient(ctx.GetService<IConfigurationRoot>().GetSection("MongoConnection:ConnectionString").Value))    

                    .AddTransient<IDeleteOneModelBuilder,DeleteOneModelBuilder>()
                    .AddTransient<IReplaceOneModelBuilder,ReplaceOneModelBuilder>()
                    .AddTransient<IFilterDefinitionBuilderWrapper,FilterDefinitionBuilderWrapper>()
                    .AddTransient<IHtmlDocumentWrapper,HtmlDocumentWrapper>()
                    .AddTransient<IHttpClientWrapper>(_=> new HttpClientWrapper(new HttpClient()));

            var corsBuilder = new CorsPolicyBuilder();
            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowAnyMethod();
            //corsBuilder.AllowAnyOrigin(); // For anyone access.
            corsBuilder.WithOrigins("http://localhost:5100","http://localhost:4200"); // for a specific url. Don't add a forward slash on the end!
            corsBuilder.AllowCredentials();

            services.AddCors(options =>
            {
                options.AddPolicy("SiteCorsPolicy", corsBuilder.Build());
            });

                //var serviceProvider = services.BuildServiceProvider();

                //var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                //var logger = loggerFactory.CreateLogger<Program>();
                //logger.LogDebug("Start application");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.ContentType = "application/json";
                        var ex = context.Features.Get<IExceptionHandlerFeature>();
                        if (ex != null)
                        {
                            StockCoreLightweightException newEx=null;
                            if(ex.Error is StockCoreException)
                            {
                                newEx = new StockCoreLightweightException().Load((StockCoreException)ex.Error);
                            }
                            else
                            {
                                newEx = new StockCoreLightweightException().Load(new StockCoreException(PROCESSID,"",ex.Error,info:"Web Api Global Error catch"));                                
                            }
                            var info = new Info()
                            {
                                RequestID = Guid.NewGuid().ToString(),
                                Code = (int)HttpStatusCode.InternalServerError,
                                HasError = true,
                                Exception = newEx
                            };
                            var data = new Data<string>()
                            {
                                Content = "",
                                Info = info
                            };
                            var err = JsonConvert.SerializeObject(data);
                            await context.Response.Body.WriteAsync(Encoding.ASCII.GetBytes(err),0,err.Length).ConfigureAwait(false);
                        }
                    });
                }
            );

            app.UseMvc();            
        }
    }
}
