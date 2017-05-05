using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockCore.Aop.Mon;
using StockCore.DomainEntity;
using StockCore.Extension;
using StockWebApi.Entity;
using System.Runtime.CompilerServices;
using System.Net;
using StockWebApi.Extension;

namespace StockWebApi.Controllers
{
    public class BaseController : Controller
    {
        private readonly int processID;
        private readonly ILogger logger;
        public BaseController(int processID,ILogger logger)
        {
            this.processID = processID;
            this.logger = logger;
        }
        protected async Task<Data<T>> baseControllerBuildAsync<T>(
            Func<Task> operateAsync=null,
            Func<Task<T>> buildAsync=null,
            string requestID=null,
            [CallerMemberName]string methodName="") where T:class
        {
            Data<T> result = null;
            requestID = checkRequestId(requestID);
            try
            {
                result = await process(operateAsync, buildAsync, requestID, methodName);
            }
            catch (Exception ex)
            {
                var info = composeInfo(requestID,methodName,ex);
                result = composeData(default(T), info);
            }
            return result;
        }
        private async Task<Data<T>> process<T>(Func<Task> operateAsync, Func<Task<T>> buildAsync, string requestID, string methodName) where T : class
        {
            Data<T> data = null;
            var info = composeInfo(requestID,methodName);
            if (operateAsync != null)
            {
                await operateAsync();
                data = composeData(default(T),info);
            }
            else if (buildAsync != null)
            {
                var item = await buildAsync();
                data = composeData(item,info);
            }
            return data;
        }
        private static string checkRequestId(string requestID)
        {
            if (string.IsNullOrEmpty(requestID))
            {
                requestID = Guid.NewGuid().ToString();
            }
            return requestID;
        }
        private Data<T> composeData<T>(T result,Info info) where T:class
        {
            var data = new Data<T>()
            {
                Info = info,
                Content = result
            };
            return data;
        }
        private Info composeInfo(string requestID,string methodName,Exception ex=null)
        {
            var info = new Info()
            {
                RequestID = requestID,
                Code = getReturnCode(ex),
                HasError = ex!=null,
                Exception = getException(methodName,ex)
            };
            return info;
        }
        private int getReturnCode(Exception ex)
        {
            var id = (int)HttpStatusCode.OK;
            if(ex!=null)
            {
                id = (int)HttpStatusCode.InternalServerError;
            }
            return id;
        }
        private StockCoreLightweightException getException(string methodName,Exception ex)
        {
            StockCoreLightweightException exception = null;
            if(ex!=null)
            {
                StockCoreException e=null;
                if (ex is StockCoreException)
                {
                    e = (StockCoreException)ex;
                    exception = new StockCoreLightweightException().Load(e);
                }
                else
                {
                    e = new StockCoreException(processID, methodName, ex, info: "Capture at Web API level");
                    exception = new StockCoreLightweightException().Load(e);
                }
                determineError(e);
            }
            return exception;
        }
        private void determineError(StockCoreException e)
        {
            if (e!=null && !e.IsLogged)
            {
                logger.TraceError(e);
                e.IsLogged = true;
            }
        }
    }
}