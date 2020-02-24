using System.ComponentModel;
using Newtonsoft.Json;

namespace WomPlatform.Connector.Models {

    /// <summary>
    /// Output payload for payment information query.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class PaymentInfoResponse {

        /// <summary>
        /// Encrypted payload (instance of <see cref="Content" />).
        /// </summary>
        [JsonProperty("payload", Required = Required.Always)]
        public string Payload { get; set; }

        /// <summary>
        /// Payload encrypted with session key.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        public class Content {

            /// <summary>
            /// Unique ID of the POS.
            /// </summary>
            [JsonProperty("posId", Required = Required.Always)]
            public Identifier PosId { get; set; }

            /// <summary>
            /// Name of the POS.
            /// </summary>
            [JsonProperty("posName", Required = Required.Always)]
            public string PosName { get; set; }

            /// <summary>
            /// Amount of vouchers to consume for payment.
            /// </summary>
            [JsonProperty("amount", Required = Required.Always)]
            public int Amount { get; set; }

            /// <summary>
            /// Simple filter conditions that vouchers must satisfy. May be null.
            /// </summary>
            [JsonProperty("simpleFilter")]
            public SimpleFilter SimpleFilter { get; set; }

            /// <summary>
            /// Gets whether the payment is persistent.
            /// </summary>
            [DefaultValue(false)]
            [JsonProperty("persistent", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
            public bool Persistent { get; set; } = false;

        }

    }

}
