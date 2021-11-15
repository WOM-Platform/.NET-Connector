using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace WomPlatform.Connector.Models {

    /// <summary>
    /// Request payload for voucher redemption.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class VoucherRedeemPayload {

        /// <summary>
        /// Encrypted payload (instance of <see cref="Content" />).
        /// </summary>
        [JsonProperty("payload", Required = Required.Always)]
        public string Payload { get; set; }

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

            /// <summary>
            /// Optional location of the user when redeeming vouchers.
            /// </summary>
            [DefaultValue(null)]
            [JsonProperty("redeemLocation", Required = Required.Default, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
            public Location RedeemLocation { get; set; }

        }

    }

}
