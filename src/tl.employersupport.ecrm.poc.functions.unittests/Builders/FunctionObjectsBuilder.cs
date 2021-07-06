using System;
using System.IO;
using System.Net.Http;
using System.Text;
using Azure.Core.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using tl.employersupport.ecrm.poc.application.Extensions;

// ReSharper disable MemberCanBePrivate.Global

namespace tl.employersupport.ecrm.poc.functions.unittests.Builders
{
    public static class FunctionObjectsBuilder
    {
        public static FunctionContext BuildFunctionContext(ILogger logger = null)
        {
            logger ??= Substitute.For<ILogger>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            loggerFactory.CreateLogger(Arg.Any<string>())
                .Returns(logger);

            var workerOptions = Options.Create(new WorkerOptions
            {
                Serializer = new JsonObjectSerializer(
                    JsonExtensions.CamelCaseJsonSerializerOptions)
            });

            var serviceProvider = new ServiceCollection()
                .AddScoped(_ => loggerFactory)
                .AddScoped(_ => workerOptions)
                .BuildServiceProvider();

            var functionContext = Substitute.For<FunctionContext>();

            functionContext.InstanceServices
                .Returns(serviceProvider);

            return functionContext;
        }

        public static HttpRequestData BuildHttpRequestData(
            HttpMethod method,
            string url = null,
            string body = null,
            FunctionContext functionContext = null)
        {
            return BuildHttpRequestData(
                method,
                url is not null ? new Uri(url) : null,
                body,
                functionContext);
        }

        public static HttpRequestData BuildHttpRequestData(
            HttpMethod method,
            Uri url = null,
            string body = null,
            FunctionContext functionContext = null)
        {
            functionContext ??= BuildFunctionContext();

            var request = Substitute.For<HttpRequestData>(functionContext);
            request.Method.Returns(method.ToString());

            if (body != null)
            {
                var requestBody = new MemoryStream(Encoding.ASCII.GetBytes(body));
                requestBody.Flush();
                requestBody.Position = 0;
                request.Body.Returns(requestBody);
            }
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
