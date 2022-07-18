using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace WomPlatform.Connector.Models {

    /// <summary>
    /// Output payload for payment information query performed by POS.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class PaymentStatusResponse {

        /// <summary>
        /// Unique ID of the POS.
        /// </summary>
        [JsonProperty("posId", Required = Required.Always)]
        [JsonConverter(typeof(IdentifierConverter))]
        public Identifier PosId { get; set; }

        /// <summary>
        /// Encrypted payload (instance of <see cref="Content" />).
        /// </summary>
        [JsonProperty("payload", Required = Required.Always)]
        public string Payload { get; set; }

        /// <summary>
        /// Payload encrypted with POS public key.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        public class Content {

            /// <summary>
            /// Gets whether the payment is persistent.
            /// </summary>
            [DefaultValue(false)]
            [JsonProperty("persistent", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
            public bool Persistent { get; set; } = false;

            [JsonProperty("hasBeenPerformed")]
            public bool HasBeenPerformed { get; set; }

            [JsonProperty("confirmations")]
            public List<Confirmation> Confirmations { get; set; }

        }

        /// <summary>
        /// Payment confirmations.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        public class Confirmation {

            /// <summary>
            /// Timestamp of payment.
            /// </summary>
            [JsonProperty("performedAt", Required = Required.Always)]
            public DateTime PerformedAt { get; set; }

        }

    }

}
