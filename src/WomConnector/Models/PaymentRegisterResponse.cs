using System;

namespace WomPlatform.Connector.Models {

    public class PaymentRegisterResponse {

        /// <summary>
        /// Encrypted paylod (instance of <see cref="Content" />).
        /// </summary>
        public string Payload { get; set; }

        /// <summary>
        /// Inner payload.
        /// </summary>
        public class Content {

            /// <summary>
            /// Identifies the registry.
            /// </summary>
            public string RegistryUrl { get; set; }

            /// <summary>
            /// Returns the nonce originally sent by the source.
            /// </summary>
            public string Nonce { get; set; }

            /// <summary>
            /// Payment processing OTC.
            /// </summary>
            public Guid Otc { get; set; }

            /// <summary>
            /// Payment processing password.
            /// </summary>
            public string Password { get; set; }

        }

    }

}
