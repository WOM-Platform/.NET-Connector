using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace WomPlatform.Connector {

    internal static class RestClientExtensions {

        public static RestRequest CreateJsonPostRequest(this RestClient client, string urlPath, object jsonBody) {
            var request = new RestRequest(urlPath, Method.POST) {
                RequestFormat = DataFormat.Json
            };
            request.AddHeader("Accept", "application/json");
            if(jsonBody != null) {
                request.AddJsonBody(jsonBody);
            }
            return request;
        }

        public static async Task<T> PerformRequest<T>(this RestClient client, RestRequest request) {
            if(client is null) {
                throw new ArgumentNullException(nameof(client));
            }

            var response = await client.ExecutePostTaskAsync(request).ConfigureAwait(false);
            if(response.StatusCode != System.Net.HttpStatusCode.OK) {
                throw new InvalidOperationException(string.Format("API status code {0}", response.StatusCode));
            }

            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        public static async Task PerformRequest(this RestClient client, RestRequest request) {
            if(client is null) {
                throw new ArgumentNullException(nameof(client));
            }

            var response = await client.ExecutePostTaskAsync(request).ConfigureAwait(false);
            if(response.StatusCode != System.Net.HttpStatusCode.OK) {
                throw new InvalidOperationException(string.Format("API status code {0}", response.StatusCode));
            }
        }

    }

}
