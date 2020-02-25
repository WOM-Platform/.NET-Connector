using System.ComponentModel;
using Newtonsoft.Json;

namespace WomPlatform.Connector.Models {

    /// <summary>
    /// Request payload for payment registration.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class PaymentRegisterPayload {

        /// <summary>
        /// Unique ID of the POS.
        /// </summary>
        [JsonProperty("posId", Required = Required.Always)]
        [JsonConverter(typeof(IdentifierConverter))]
        public Identifier PosId { get; set; }

        /// <summary>
        /// Nonce to prevent repetition.
        /// </summary>
        [JsonProperty("nonce", Required = Required.Always)]
        public string Nonce { get; set; }

        /// <summary>
        /// Encrypted payload (encoded as <see cref="Content" /> instance).
        /// </summary>
        [JsonProperty("payload", Required = Required.Always)]
        public string Payload { get; set; }

        [JsonObject(MemberSerialization.OptIn)]
        public class Content {

            /// <summary>
            /// Unique ID of the POS.
            /// </summary>
            [JsonProperty("posId", Required = Required.Always)]
            [JsonConverter(typeof(IdentifierConverter))]
            public Identifier PosId { get; set; }

            /// <summary>
            /// Nonce to prevent repetition.
            /// </summary>
            [JsonProperty("nonce", Required = Required.Always)]
            public string Nonce { get; set; }

            /// <summary>
            /// Password specified by user.
            /// </summary>
            [JsonProperty("password")]
            public string Password { get; set; }

            /// <summary>
            /// Amount of vouchers to consume for payment.
            /// </summary>
            [JsonProperty("amount", Required = Required.Always)]
            public int Amount { get; set; }

            /// <summary>
            /// Simple filter conditions that vouchers must satisfy.
            /// May be null.
            /// </summary>
            [JsonProperty("simpleFilter")]
            public SimpleFilter SimpleFilter { get; set; }

            /// <summary>
            /// Required URL for the acknowledgment to the Pocket.
            /// </summary>
            [JsonProperty("pocketAckUrl", Required = Required.Always)]
            public string PocketAckUrl { get; set; }

            /// <summary>
            /// Optional URL for the acknowledgment to the POS.
            /// </summary>
            [JsonProperty("posAckUrl")]
            public string PosAckUrl { get; set; }

            /// <summary>
            /// Optional flag for persistent payments.
            /// </summary>
            [DefaultValue(false)]
            [JsonProperty("persistent", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
            public bool Persistent { get; set; } = false;

        }

    }

}
