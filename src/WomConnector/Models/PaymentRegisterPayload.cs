using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WomPlatform.Connector.Models {

    /// <summary>
    /// Request payload for payment registration.
    /// </summary>
    public class PaymentRegisterPayload {

        /// <summary>
        /// Unique ID of the POS.
        /// </summary>
        public long PosId { get; set; }

        /// <summary>
        /// Nonce to prevent repetition.
        /// </summary>
        public string Nonce { get; set; }

        /// <summary>
        /// Encrypted payload (encoded as <see cref="Content" /> instance).
        /// </summary>
        public string Payload { get; set; }

        public class Content {

            /// <summary>
            /// Unique ID of the POS.
            /// </summary>
            public long PosId { get; set; }

            /// <summary>
            /// Nonce to prevent repetition.
            /// </summary>
            public string Nonce { get; set; }

            /// <summary>
            /// Password specified by user.
            /// </summary>
            public string Password { get; set; }

            /// <summary>
            /// Amount of vouchers to consume for payment.
            /// </summary>
            public int Amount { get; set; }

            /// <summary>
            /// Simple filter conditions that vouchers must satisfy. May be null.
            /// </summary>
            public SimpleFilter SimpleFilter { get; set; }

            /// <summary>
            /// Required URL for the acknowledgment to the Pocket.
            /// </summary>
            public string PocketAckUrl { get; set; }

            /// <summary>
            /// Optional URL for the acknowledgment to the POS.
            /// </summary>
            public string PosAckUrl { get; set; }

            /// <summary>
            /// Optional flag for persistent payments.
            /// </summary>
            [DefaultValue(false)]
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
            public bool Persistent { get; set; } = false;

        }

    }

}
