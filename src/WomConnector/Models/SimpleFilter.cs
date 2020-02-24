using Newtonsoft.Json;

namespace WomPlatform.Connector.Models {

    [JsonObject(MemberSerialization.OptIn)]
    public class SimpleFilter {

        [JsonProperty("aim")]
        public string Aim { get; set; }

        [JsonProperty("bounds")]
        public Bounds Bounds { get; set; }

        [JsonProperty("maxAge")]
        public long? MaxAge { get; set; }

    }

}
