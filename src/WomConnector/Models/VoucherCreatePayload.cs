using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WomPlatform.Connector.Models {

    /// <summary>
    /// Request payload for voucher creation.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class VoucherCreatePayload {

        /// <summary>
        /// Unique ID of the source.
        /// </summary>
        [JsonProperty("sourceId", Required = Required.Always)]
        [JsonConverter(typeof(IdentifierConverter))]
        public Identifier SourceId { get; set; }

        /// <summary>
        /// Nonce to prevent repetition (base64-encoded).
        /// </summary>
        [JsonProperty("nonce", Required = Required.Always)]
        public string Nonce { get; set; }

        /// <summary>
        /// Payload signed and encrypted by source (encoded as <see cref="Content" />).
        /// </summary>
        [JsonProperty("payload", Required = Required.Always)]
        public string Payload { get; set; }

        /// <summary>
        /// Inner payload signed and encrypted by source.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        public class Content {

            /// <summary>
            /// Unique ID of the source.
            /// </summary>
            [JsonProperty("sourceId", Required = Required.Always)]
            [JsonConverter(typeof(IdentifierConverter))]
            public Identifier SourceId { get; set; }

            /// <summary>
            /// Nonce to prevent repetition (base64-encoded).
            /// </summary>
            [JsonProperty("nonce", Required = Required.Always)]
            public string Nonce { get; set; }

            /// <summary>
            /// Password specified by user.
            /// </summary>
            [JsonProperty("password")]
            public string Password { get; set; }

            /// <summary>
            /// Details of the vouchers to create.
            /// </summary>
            [JsonProperty("vouchers", Required = Required.Always)]
            public VoucherInfo[] Vouchers { get; set; }

        }

        /// <summary>
        /// Encapsulates information about voucher instances to generate.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        public class VoucherInfo {

            [JsonProperty("aim", Required = Required.Always)]
            public string Aim { get; set; }

            [JsonProperty("latitude", Required = Required.Default)]
            public double Latitude { get; set; }

            [JsonProperty("longitude", Required = Required.Default)]
            public double Longitude { get; set; }

            [JsonProperty("timestamp", Required = Required.Always)]
            public DateTime Timestamp { get; set; }

            [DefaultValue(1)]
            [JsonProperty("count", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
            public int Count { get; set; } = 1;

            [DefaultValue(VoucherCreationMode.Standard)]
            [JsonProperty("creationMode", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
            [JsonConverter(typeof(StringEnumConverter))]
            public VoucherCreationMode CreationMode { get; set; } = VoucherCreationMode.Standard;

        }

        /// <summary>
        /// Determines the mode of voucher creation.
        /// </summary>
        public enum VoucherCreationMode {
            Standard,
            SetLocationOnRedeem
        }

    }

}
