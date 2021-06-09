using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace tl.employersupport.ecrm.poc.application.tests.TestHelpers
{
    // ReSharper disable UnusedMember.Global
    public class TestHttpClientFactory
    {
        public HttpClient CreateHttpClient(Uri uri, object response, string responseContentType = "application/json", HttpStatusCode responseCode = HttpStatusCode.OK)
        {
            var serializedResponse = JsonSerializer.Serialize(response);

            return CreateHttpClient(uri, serializedResponse, responseContentType, responseCode);
        }

        public HttpClient CreateHttpClient(Uri uri, string response, string responseContentType = "application/json", HttpStatusCode responseCode = HttpStatusCode.OK)
        {
            var httpResponseMessage = CreateFakeResponse(response, responseContentType, responseCode);
           
            var fakeMessageHandler = new FakeHttpMessageHandler();
            fakeMessageHandler.AddFakeResponse(uri, httpResponseMessage);

            var httpClient = new HttpClient(fakeMessageHandler);

            return httpClient;
        }

        public HttpClient CreateHttpClientWithBaseUri(Uri baseUri, string relativeUri, string response, string responseContentType = "application/json", HttpStatusCode responseCode = HttpStatusCode.OK)
        {
            var httpResponseMessage = CreateFakeResponse(response, responseContentType, responseCode);

            var uri = new Uri(baseUri, relativeUri);
            var fakeMessageHandler = new FakeHttpMessageHandler();
            fakeMessageHandler.AddFakeResponse(uri, httpResponseMessage);

            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = baseUri
            };

            return httpClient;
        }

        public HttpClient CreateHttpClient(Uri uri, Stream response, string responseContentType = "application/json", HttpStatusCode responseCode = HttpStatusCode.OK)
        {
            var httpResponseMessage = CreateFakeResponse(response, responseContentType, responseCode);

            var fakeMessageHandler = new FakeHttpMessageHandler();
            fakeMessageHandler.AddFakeResponse(uri, httpResponseMessage);

            var httpClient = new HttpClient(fakeMessageHandler);

            return httpClient;
        }

        public HttpClient CreateHttpClient(Uri baseUri, string relativeUri, Stream response, string responseContentType = "application/json", HttpStatusCode responseCode = HttpStatusCode.OK)
        {
            var httpResponseMessage = CreateFakeResponse(response, responseContentType, responseCode);

            var uri = new Uri(baseUri, relativeUri);
            var fakeMessageHandler = new FakeHttpMessageHandler();
            fakeMessageHandler.AddFakeResponse(uri, httpResponseMessage);

            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = baseUri
            };

            return httpClient;
        }

        public HttpClient CreateHttpClient(IDictionary<Uri, HttpResponseMessage> httpResponseMessages)
        {
            var fakeMessageHandler = new FakeHttpMessageHandler();

            foreach (var (key, value) in httpResponseMessages)
            {
                fakeMessageHandler.AddFakeResponse(key, value);
            }

            var httpClient = new HttpClient(fakeMessageHandler);

            return httpClient;
        }

        public HttpClient CreateHttpClient(Uri baseUri, IDictionary<Uri, string> responseMessages, string responseContentType = "application/json", HttpStatusCode responseCode = HttpStatusCode.OK)
        {
            var fakeMessageHandler = new FakeHttpMessageHandler();
            foreach (var (key, value) in responseMessages)
            {
                var httpResponseMessage = CreateFakeResponse(value, responseContentType, responseCode);
                fakeMessageHandler.AddFakeResponse(key, httpResponseMessage);
            }

            var httpClient = new HttpClient(fakeMessageHandler)
            {
                BaseAddress = baseUri
            };

            return httpClient;
        }

        public HttpResponseMessage CreateFakeResponse(string response, string responseContentType = "application/json", HttpStatusCode responseCode = HttpStatusCode.OK)
        {
            var httpResponseMessage = new HttpResponseMessage(responseCode)
            {
                Content = new StringContent(response)
            };

            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(responseContentType);

            return httpResponseMessage;
        }

        public HttpResponseMessage CreateFakeResponse(Stream response, string responseContentType = "application/json", HttpStatusCode responseCode = HttpStatusCode.OK)
        {
            var httpResponseMessage = new HttpResponseMessage(responseCode)
            {
                Content = new StreamContent(response)
            };
            
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(responseContentType);

            return httpResponseMessage;
        }
    }
}
