using System;
using Newtonsoft.Json;

namespace WomPlatform.Connector.Models {

    /// <summary>
    /// Request payload for payment verification.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class PaymentVerifyPayload {

        /// <summary>
        /// Encrypted payload (instance of <see cref="Content" />).
        /// </summary>
        [JsonProperty("payload", Required = Required.Always)]
        public string Payload { get; set; }

        /// <summary>
        /// Inner payload encrypted by source.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        public class Content {

            [JsonProperty("otc", Required = Required.Always)]
            public Guid Otc { get; set; }

        }

    }

}
