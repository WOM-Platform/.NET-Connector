using System;
using Newtonsoft.Json;

namespace WomPlatform.Connector.Models {

    /// <summary>
    /// Response payload for payment registration.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class PaymentRegisterResponse {

        /// <summary>
        /// Encrypted paylod (instance of <see cref="Content" />).
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
            [JsonProperty("registryUrl")]
            public string RegistryUrl { get; set; }

            /// <summary>
            /// Returns the nonce originally sent by the source.
            /// </summary>
            [JsonProperty("nonce", Required = Required.Always)]
            public string Nonce { get; set; }

            /// <summary>
            /// Payment processing OTC.
            /// </summary>
            [JsonProperty("otc", Required = Required.Always)]
            public Guid Otc { get; set; }

            /// <summary>
            /// Payment processing password.
            /// </summary>
            [JsonProperty("password", Required = Required.Always)]
            public string Password { get; set; }

            /// <summary>
            /// Payment link for clients.
            /// </summary>
            [JsonProperty("link", Required = Required.DisallowNull)]
            public string Link { get; set; }

        }

    }

}
