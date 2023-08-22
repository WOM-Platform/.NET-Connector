using System.Text.Json.Serialization;

namespace WomPlatform.Connector.Models {
    /// <summary>
    /// Encapsulates information about a location.
    /// </summary>
    public class Location {

        [JsonRequired]
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonRequired]
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

    }
}
