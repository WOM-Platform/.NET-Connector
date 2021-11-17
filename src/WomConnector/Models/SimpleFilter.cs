using Newtonsoft.Json;

namespace WomPlatform.Connector.Models {

    [JsonObject(MemberSerialization.OptIn)]
    public class SimpleFilter {

        /// <summary>
        /// Optional aim filter, expressed as an aim code (prefix).
        /// </summary>
        [JsonProperty("aim")]
        public string Aim { get; set; }

        /// <summary>
        /// Optional geographical bounds filter.
        /// </summary>
        [JsonProperty("bounds")]
        public Bounds Bounds { get; set; }

        /// <summary>
        /// Optional voucher freshness filter, expressed in days of age.
        /// </summary>
        [JsonProperty("maxAge")]
        public long? MaxAge { get; set; }

    }

}
