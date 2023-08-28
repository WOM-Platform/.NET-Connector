using Newtonsoft.Json;

namespace WomPlatform.Connector.Models {
    /// <summary>
    /// Request payload for voucher transfer between users.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class VoucherTransferPayload {

        /// <summary>
        /// Unique ID of the source.
        /// </summary>
        /// <remarks>
        /// This must match the set source ID for voucher transfers between users. Other IDs will be rejected.
        /// </remarks>
        [JsonProperty("sourceId", Required = Required.Always)]
        [JsonConverter(typeof(IdentifierConverter))]
        public Identifier SourceId { get; set; }

        /// <summary>
        /// Nonce to prevent message repetition (base64-encoded).
        /// </summary>
        [JsonProperty("nonce", Required = Required.Always)]
        public string Nonce { get; set; }

        /// <summary>
        /// Encrypted payload (instance of <see cref="Content" />).
        /// </summary>
        [JsonProperty("payload", Required = Required.Always)]
        public string Payload { get; set; }

        /// <summary>
        /// Encrypted payload of <see cref="PaymentConfirmPayload"/>.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        public class Content {

            [JsonProperty("password")]
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
            [JsonConverter(typeof(IdentifierConverter))]
            public Identifier Id { get; set; }

            [JsonProperty("secret", Required = Required.Always)]
            public string Secret { get; set; }

        }

    }
}
