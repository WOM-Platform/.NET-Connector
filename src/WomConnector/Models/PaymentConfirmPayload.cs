using System;
using Newtonsoft.Json;

namespace WomPlatform.Connector.Models {

    /// <summary>
    /// Input payload for payment confirmation.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class PaymentConfirmPayload {

        /// <summary>
        /// Encrypted payload (instance of <see cref="Content" />).
        /// </summary>
        [JsonProperty("payload")]
        public string Payload { get; set; }

        /// <summary>
        /// Encrypted payload of <see cref="PaymentConfirmPayload"/>.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        public class Content {

            [JsonProperty("otc", Required = Required.Always)]
            public Guid Otc { get; set; }

            [JsonProperty("password", Required = Required.Always)]
            public string Password { get; set; }

            /// <summary>
            /// Base64-encoded session key to be used in response.
            /// </summary>
            [JsonProperty("sessionKey", Required = Required.Always)]
            public string SessionKey { get; set; }

            [JsonProperty("vouchers", Required = Required.Always)]
            public VoucherInfo[] Vouchers { get; set; }

        }

        /// <summary>
        /// Single voucher identification data.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        public class VoucherInfo {

            [JsonProperty("id", Required = Required.Always)]
            public Identifier Id { get; set; }

            [JsonProperty("secret", Required = Required.Always)]
            public string Secret { get; set; }

        }

    }

}
