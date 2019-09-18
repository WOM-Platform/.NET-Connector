using System;

namespace WomPlatform.Connector.Models {

    public class VoucherRedeemPayload {

        /// <summary>
        /// Encrypted payload (instance of <see cref="Content" />).
        /// </summary>
        public string Payload { get; set; }

        public class Content {

            public Guid Otc { get; set; }

            public string Password { get; set; }

            /// <summary>
            /// Base64-encoded session key to be used in response.
            /// </summary>
            public string SessionKey { get; set; }

        }

    }

}
