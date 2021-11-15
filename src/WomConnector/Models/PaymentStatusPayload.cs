using System;
using Newtonsoft.Json;

namespace WomPlatform.Connector.Models {

    /// <summary>
    /// Input payload for payment information query performed by POS.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class PaymentStatusPayload {

        /// <summary>
        /// Encrypted payload (instance of <see cref="Content" />).
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
            /// Unique one-time code of the payment.
            /// </summary>
            [JsonProperty("otc", Required = Required.Always)]
            public Guid Otc { get; set; }

        }

    }

}
