using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WomPlatform.Connector.Models {

    public class PaymentConfirmPayload {

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

            public VoucherInfo[] Vouchers { get; set; }

        }

        public class VoucherInfo {

            public VoucherId Id { get; set; }

            public string Secret { get; set; }

        }

    }

}
