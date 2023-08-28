using System;
using Newtonsoft.Json;

namespace WomPlatform.Connector.Models {

    /// <summary>
    /// Response payload for voucher transfer between users.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class VoucherTransferResponse {

        /// <summary>
        /// Encrypted payload (instance of <see cref="Content" />).
        /// </summary>
        [JsonProperty("payload", Required = Required.Always)]
        public string Payload { get; set; }

        /// <summary>
        /// Inner payload.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        public class Content {

            /// <summary>
            /// Identifies the registry.
            /// </summary>
            [JsonProperty("registryUrl", Required = Required.Always)]
            public string RegistryUrl { get; set; }

            /// <summary>
            /// Returns the nonce originally sent by the source.
            /// </summary>
            [JsonProperty("nonce", Required = Required.Always)]
            public string Nonce { get; set; }

            /// <summary>
            /// Voucher redemption OTC.
            /// </summary>
            [JsonProperty("otc", Required = Required.Always)]
            public Guid Otc { get; set; }

            /// <summary>
            /// Voucher redemption password.
            /// </summary>
            [JsonProperty("password", Required = Required.Always)]
            public string Password { get; set; }

            /// <summary>
            /// Voucher redemption link for clients.
            /// </summary>
            [JsonProperty("link", Required = Required.DisallowNull)]
            public string Link { get; set; }

            /// <summary>
            /// Voucher count.
            /// </summary>
            [JsonProperty("count", Required = Required.Default)]
            public int Count { get; set; }

        }

    }
}
