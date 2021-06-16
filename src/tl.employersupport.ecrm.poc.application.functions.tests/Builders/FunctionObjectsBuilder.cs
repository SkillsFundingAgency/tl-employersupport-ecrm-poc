using System;
using System.IO;
using System.Net.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace tl.employersupport.ecrm.poc.application.functions.tests.Builders
{
    public static class FunctionObjectsBuilder
    {
        public static FunctionContext BuildFunctionContext(ILogger logger = null)
        {
            logger ??= Substitute.For<ILogger>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            loggerFactory.CreateLogger(Arg.Any<string>())
                .Returns(logger);

            var functionContext = Substitute.For<FunctionContext>();
            functionContext.InstanceServices.GetService(Arg.Any<Type>())
                .Returns(loggerFactory);

            return functionContext;
        }

        public static HttpRequestData BuildHttpRequestData(
            HttpMethod method,
            string url = null,
            FunctionContext functionContext = null)
        {
            return BuildHttpRequestData(
                method,
                url is not null ? new Uri(url) : null,
                functionContext);
        }

        public static HttpRequestData BuildHttpRequestData(
            HttpMethod method,
            Uri url = null,
            FunctionContext functionContext = null)
        {
            functionContext ??= BuildFunctionContext();

            var request = Substitute.For<HttpRequestData>(functionContext);
            request.Method.Returns(method.ToString());
            request.Url.Returns(url);

            var responseData = BuildHttpResponseData(functionContext);
            request.CreateResponse().Returns(responseData);

            return request;
        }

        public static HttpResponseData BuildHttpResponseData(
            FunctionContext functionContext = null)
        {
            functionContext ??= BuildFunctionContext();

            var responseData = Substitute.For<HttpResponseData>(functionContext);

            var responseHeaders = new HttpHeadersCollection();
            responseData.Headers.Returns(responseHeaders);

            var responseBody = new MemoryStream();
            responseData.Body.Returns(responseBody);

            return responseData;
        }
    }
}
