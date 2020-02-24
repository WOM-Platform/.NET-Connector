using Newtonsoft.Json;

namespace WomPlatform.Connector.Models {

    /// <summary>
    /// Output payload for payment confirmation.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class PaymentConfirmResponse {

        /// <summary>
        /// Encrypted payload (instance of <see cref="Content" />).
        /// </summary>
        [JsonProperty("payload", Required = Required.Always)]
        public string Payload { get; set; }

        /// <summary>
        /// Payload encrypted with session key.
        /// </summary>
        public class Content {

            /// <summary>
            /// Acknowledgment URL that may be used by Pocket.
            /// </summary>
            [JsonProperty("ackUrl", Required = Required.Always)]
            public string AckUrl { get; set; }

        }

    }

}
