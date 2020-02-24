using Newtonsoft.Json;

namespace WomPlatform.Connector.Models {

    [JsonObject(MemberSerialization.OptIn)]
    public class Bounds {

        [JsonProperty("leftTop", Required = Required.Always)]
        public double[] LeftTop { get; set; }

        [JsonProperty("rightBottom", Required = Required.Always)]
        public double[] RightBottom { get; set; }

    }

}
