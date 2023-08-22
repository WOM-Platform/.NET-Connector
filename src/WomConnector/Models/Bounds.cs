using System.Text.Json.Serialization;

namespace WomPlatform.Connector.Models {

    public class Bounds {

        [JsonRequired]
        [JsonPropertyName("leftTop")]
        public double[] LeftTop { get; set; }

        [JsonRequired]
        [JsonPropertyName("rightBottom")]
        public double[] RightBottom { get; set; }

    }

}
