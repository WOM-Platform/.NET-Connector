using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serialization;

namespace WomPlatform.Connector {

    class JsonRestSerializer : IRestSerializer {

        private readonly JsonSerializer _serializer;

        public JsonRestSerializer() {
            _serializer = JsonSerializer.Create(Client.JsonSettingsCache);
        }

        public string[] SupportedContentTypes => new string[] {
            "application/json",
            "text/json",
            "text/x-json",
            "text/javascript",
            "*+json"
        };

        public DataFormat DataFormat => DataFormat.Json;

        public string ContentType { get; set; } = "application/json";

        public T Deserialize<T>(IRestResponse response) {
            using(var reader = new JsonTextReader(new StringReader(response.Content)) {
                CloseInput = true
            }) {
                return _serializer.Deserialize<T>(reader);
            }
        }

        public string Serialize(Parameter parameter) {
            return Serialize(parameter.Value);
        }

        public string Serialize(object obj) {
            using(var stringWriter = new StringWriter(new StringBuilder(256), CultureInfo.InvariantCulture)) {
                using(var jsonTextWriter = new JsonTextWriter(stringWriter) {
                    Formatting = _serializer.Formatting,
                    CloseOutput = true
                }) {
                    _serializer.Serialize(jsonTextWriter, obj, obj.GetType());
                    jsonTextWriter.Flush();
                }

                return stringWriter.ToString();
            }
        }

    }

}
