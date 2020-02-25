using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;

namespace WomPlatform.Connector {

    internal static class ClientExtensions {

        public static RestRequest CreateJsonPostRequest(this Client _, string urlPath, object jsonBody) {
            var request = new RestRequest(urlPath, Method.POST) {
                RequestFormat = DataFormat.Json
            };
            request.AddHeader("Accept", "application/json");
            if(jsonBody != null) {
                request.AddJsonBody(jsonBody);
            }
            return request;
        }

        public static async Task<T> PerformRequest<T>(this Client client, RestRequest request) {
            if(client is null) {
                throw new ArgumentNullException(nameof(client));
            }

            client.Logger.LogTrace(LoggingEvents.Communication,
                "HTTP request {0} {1}",
                request.Method.ToString(),
                client.RestClient.BuildUri(request));

            var response = await client.RestClient.ExecutePostAsync(request).ConfigureAwait(false);

            client.Logger.LogTrace(LoggingEvents.Communication,
                "Request body ({0}): {1}",
                request.Body.ContentType,
                request.Body.Value);

            client.Logger.LogTrace(LoggingEvents.Communication,
                "HTTP response {0}",
                response.StatusCode);

            client.Logger.LogTrace(LoggingEvents.Communication,
                "Response body ({0}): {1}",
                response.ContentType,
                response.Content);

            if(response.StatusCode != System.Net.HttpStatusCode.OK) {
                throw new InvalidOperationException(string.Format("API status code {0}", response.StatusCode));
            }

            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        public static async Task PerformRequest(this RestClient client, RestRequest request) {
            if(client is null) {
                throw new ArgumentNullException(nameof(client));
            }

            var response = await client.ExecutePostAsync(request).ConfigureAwait(false);
            if(response.StatusCode != System.Net.HttpStatusCode.OK) {
                throw new InvalidOperationException(string.Format("API status code {0}", response.StatusCode));
            }
        }

    }

}
