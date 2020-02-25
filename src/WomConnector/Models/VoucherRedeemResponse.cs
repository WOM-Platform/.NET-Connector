using System;
using Newtonsoft.Json;

namespace WomPlatform.Connector.Models {

    /// <summary>
    /// Response payload for voucher redemption.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class VoucherRedeemResponse {

        /// <summary>
        /// Encrypted payload, encoded as an <see cref="Content" /> instance.
        /// </summary>
        [JsonProperty("payload", Required = Required.Always)]
        public string Payload { get; set; }

        /// <summary>
        /// Payload encrypted with session key.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        public class Content {

            /// <summary>
            /// ID of the source.
            /// </summary>
            [JsonProperty("sourceId", Required = Required.Always)]
            [JsonConverter(typeof(IdentifierConverter))]
            public Identifier SourceId { get; set; }

            /// <summary>
            /// Name of the source.
            /// </summary>
            [JsonProperty("sourceName", Required = Required.Always)]
            public string SourceName { get; set; }

            /// <summary>
            /// List of redeemed vouchers.
            /// </summary>
            [JsonProperty("vouchers", Required = Required.Always)]
            public VoucherInfo[] Vouchers { get; set; }

        }

        /// <summary>
        /// Encapsulates info about a single voucher.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        public class VoucherInfo {

            /// <summary>
            /// Unique voucher ID.
            /// </summary>
            [JsonProperty("id", Required = Required.Always)]
            [JsonConverter(typeof(IdentifierConverter))]
            public Identifier Id { get; set; }

            /// <summary>
            /// Voucher secret for usage.
            /// </summary>
            [JsonProperty("secret", Required = Required.Always)]
            public string Secret { get; set; }

            /// <summary>
            /// Aim identificator.
            /// </summary>
            [JsonProperty("aim", Required = Required.Always)]
            public string Aim { get; set; }

            [JsonProperty("latitude", Required = Required.Always)]
            public double Latitude { get; set; }

            [JsonProperty("longitude", Required = Required.Always)]
            public double Longitude { get; set; }

            [JsonProperty("timestamp", Required = Required.Always)]
            public DateTime Timestamp { get; set; }

        }

    }

}
