using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WomPlatform.Connector.Models {

    /// <summary>
    /// Request payload for voucher verification.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class VoucherVerifyPayload {

        /// <summary>
        /// Encrypted payload (represents a <see cref="Content" /> instance).
        /// </summary>
        [JsonProperty("payload", Required = Required.Always)]
        public string Payload { get; set; }

        /// <summary>
        /// Inner payload encrypted by source.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        public class Content {

            [JsonProperty("otc", Required = Required.Always)]
            public Guid Otc { get; set; }

        }

    }

}
