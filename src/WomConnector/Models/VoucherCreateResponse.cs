using System;

namespace WomPlatform.Connector.Models {

    public class VoucherCreateResponse {

        /// <summary>
        /// Encrypted payload (instance of <see cref="Content" />).
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
            /// Voucher redemption OTC.
            /// </summary>
            public Guid Otc { get; set; }

            /// <summary>
            /// Voucher redemption password.
            /// </summary>
            public string Password { get; set; }

        }

    }

}
