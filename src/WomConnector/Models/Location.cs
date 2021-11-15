using System;
using Newtonsoft.Json;

namespace WomPlatform.Connector.Models {
    /// <summary>
    /// Encapsulates information about a location.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Location {

        [JsonProperty("latitude", Required = Required.Always)]
        public double Latitude { get; set; }

        [JsonProperty("longitude", Required = Required.Always)]
        public double Longitude { get; set; }

    }
}
